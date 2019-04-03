using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using SwiftSupport.Internals;
using SwiftSupport.Shared;
using Parallel = System.Threading.Tasks.Parallel;

namespace SwiftSupport
{
    public class ScanSwiftTask : BaseTask
    {
        [Output]
        public ITaskItem[] SwiftDependencies { get; set; }

        [Required]
        public ITaskItem[] Frameworks { get; set; }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, $"Frameworks: {string.Join(", ", Frameworks.Select(c => c.ItemSpec))}"); // debug

            var dependencies = new SafeList<Dependency>();

            void AddRange(IEnumerable<string> toAdd)
            {
                foreach (var dependency in toAdd)
                {
                    if (dependencies.AddIfNotFound(c => c.Dylib == dependency, () => new Dependency(dependency)))
                    {
                        Log.LogMessage(MessageImportance.Normal, $"Swift Dependency Found: {dependency}");
                    }
                }
            }

            Parallel.ForEach(Frameworks.Select(c => c.ItemSpec), (lib) =>
            {
                AddRange(ScanForDependenciesOnFrameworks(lib));
            });

            // we could precalculate this one and publish with a 
            // know dependencies list for Swift dylibs
            // but that means updating the list everytime a new version
            // is released and supporting multiple versions.
            // Update: Windows version is doing it.
            while (dependencies.Any(c => c.Pending))
            {
                var verifyList = from c in dependencies
                                 where c.Pending
                                 select c;

                Parallel.ForEach(verifyList.ToList(), (dependency) =>
                {
                    AddRange(ScanForDependenciesOnSwift(dependency.Dylib));

                    dependency.MarkAsScanned();
                });
            }

            SwiftDependencies = dependencies
                                    .Select(c => new TaskItem(c.Dylib))
                                    .ToArray();

            return true;
        }

        private IEnumerable<string> ScanForDependenciesOnFrameworks(string dylib)
        {
            // these frameworks depend on which Swift libraries? 
            Log.LogMessage(MessageImportance.Normal, $"Looking for Swift Dependency on: {dylib}");

            return ScanOutputForDylibs(RunOtool($"-l '{dylib}'"));
        }

        private IEnumerable<string> ScanForDependenciesOnSwift(string dylib)
        {
            // Swift libraries can depend on other Swift libraries
            Log.LogMessage(MessageImportance.Normal, $"Looking for Swift Dependency on: {dylib}");

            return ScanOutputForDylibs(RunOtool($"-l '{Path.Combine(GetRuntimePath(), dylib)}'"));
        }

        private IEnumerable<string> ScanOutputForDylibs(IEnumerable<string> lines)
        {
            // dont know to run grep on ProcessStartInfo
            foreach (var line in lines.Where(c => c.Contains("@rpath/libswift")))
            {
                yield return Regex.Match(line, "@rpath/(libswift[\\w]+.dylib)").Groups[1].Value;
            }
        }
    }
}
