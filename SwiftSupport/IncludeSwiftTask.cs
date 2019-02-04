using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Parallel = System.Threading.Tasks.Parallel;

namespace SwiftSupport
{
    public class IncludeSwiftTask : BaseTask
    {
        [Required]
        public string MtouchArch { get; set; }

        [Required]
        public ITaskItem[] Resources { get; set; }

        private string[] AvailableArchs()
        {
            var firstItemPath = Resources?.FirstOrDefault()?.ItemSpec;
            if (string.IsNullOrWhiteSpace(firstItemPath))
            {
                return new string[0];
            }

            var archs = RunLipo($"{Path.Combine(GetRuntimePath(), firstItemPath)} -archs");

            return archs.Split(' ').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c)).ToArray();
        }

        public override bool Execute()
        {
            var arcs = MtouchArch.Split(',').Select(c => c.Trim().ToLower()).ToList(); // lipo uses lower case
            var availableArchs = AvailableArchs();

            Log.LogMessage(MessageImportance.Normal, $"Copying: {string.Join(", ", Resources.Select(c => c.ItemSpec))}");
            Log.LogMessage(MessageImportance.Normal, $"Swift Arcs Needed: {MtouchArch}");
            Log.LogMessage(MessageImportance.Normal, $"Swift Arcs Available: {string.Join(", ", availableArchs)}");

            var archsToRemove = availableArchs.Except(arcs).Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();

            StringBuilder argsBuilder = new StringBuilder();

            if (archsToRemove.Length == 0)
            {
                argsBuilder.Append(" -create -output");
            }
            else
            {
                foreach (var arch in archsToRemove)
                {
                    argsBuilder.Append($" -remove {arch}");
                }

                argsBuilder.Append(" -output");
            }

            var args = argsBuilder.ToString();
            var xcodePath = GetRuntimePath();

            Parallel.ForEach(Resources.Select(c => c.ItemSpec), (dylib) =>
            {
                Log.LogMessage(MessageImportance.Normal, $"Copying: {dylib}");
                RunLipo($"{Path.Combine(xcodePath, dylib)} {args} {GetOutputPath(dylib)}");
            });

            return true;
        }
    }
}