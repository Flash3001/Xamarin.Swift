using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using SwiftSupport.Internals;

namespace SwiftSupport
{
    public class CheckSwiftVersionsTask : BaseTask
    {
        private Regex _frameworkNameRegex = new Regex(@"(?<path>.*Frameworks/.+\.framework)/(?<name>.+)");
        private Regex _frameworkVersionRegex = new Regex(@".*(?<version>Apple Swift version.*\)).*");
        private Regex _installedVersionRegex = new Regex(@".*(?<version>Apple Swift version.*\))(.*Target.*)?");

        [Required]
        public ITaskItem[] Frameworks { get; set; }

        [Required]
        public bool ShowErrorAsWarnings { get; set; }

        public override bool Execute()
        {
            string installedSwiftVersion = null;
            var frameworkAndVersion = new ConcurrentDictionary<string, string>();

            void getInstalledVersion()
            {
                installedSwiftVersion = GetInstalledVersion();
            }

            void getFrameworksVersion()
            {
                Parallel.ForEach(Frameworks, (lib) =>
                {
                    ScanAndAddFrameworkVersion(lib, frameworkAndVersion);
                });
            }

            Parallel.Invoke(getInstalledVersion, getFrameworksVersion);

            return ValidateVersions(installedSwiftVersion, frameworkAndVersion);
        }

        private void ScanAndAddFrameworkVersion(ITaskItem lib, ConcurrentDictionary<string, string> frameworkAndVersion)
        {
            var framework = ParseFramework(lib);
            if (framework == null)
            {
                return;
            }

            var libSwiftVersion = GetFrameworkVersion(framework.Item2);
            if (libSwiftVersion == null)
            {
                return;
            }

            frameworkAndVersion.TryAdd(framework.Item1, libSwiftVersion);
        }

        private bool ValidateVersions(string installedSwiftVersion, ConcurrentDictionary<string, string> frameworkAndVersion)
        {
            if (installedSwiftVersion == null)
            {
                Log.LogWarning($"Failed to get installed Swift version.");
                return true;
            }

            Log.LogMessage($"Installed: {installedSwiftVersion}");

            var allMatches = true;

            foreach (var item in frameworkAndVersion)
            {
                if (item.Value == installedSwiftVersion)
                {
                    Log.LogMessage($"{item.Key} Framework: {item.Value}");
                }
                else
                {
                    // Can we tell which version of Xcode needs to be installed to support X framework?
                    // Also, how does Swift 5 affect this? 
                    var msg = $"{item.Key} Framework do not match installed Swift version. " +
                        $"Framework: {item.Value}. " +
                        $"Installed: {installedSwiftVersion}. " +
                        "Change your Xcode or library version.";

                    if (ShowErrorAsWarnings)
                    {
                        Log.LogWarning(msg);
                    }
                    else
                    {
                        Log.LogError(msg);
                    }

                    allMatches = false;
                }
            }

            return allMatches || ShowErrorAsWarnings;
        }

        private Tuple<string, string> ParseFramework(ITaskItem framework)
        {
            var frameworkMatch = _frameworkNameRegex.Match(framework.ItemSpec);
            if (frameworkMatch.Success == false)
            {
                return null;
            }

            var frameworPath = frameworkMatch.Groups["path"].Value;
            var frameworkName = frameworkMatch.Groups["name"].Value;

            var headerPath = Path.Combine(frameworPath, "Headers", $"{frameworkName}-Swift.h");

            if (File.Exists(headerPath) == false)
            {
                return null;
            }

            return new Tuple<string, string>(frameworkName, headerPath);
        }

        private string GetInstalledVersion()
        {
            var installed = this.Run("swift", "--version");

            var installedMatch = _installedVersionRegex.Match(installed);
            if (installedMatch.Success == false)
            {
                return null;
            }

            return installedMatch.Groups["version"].Value?.Trim();
        }

        private string GetFrameworkVersion(string headerPath)
        {
            var frameworkVersion = this.Run("grep", $"swiftlang {headerPath}");

            var installedMatch = _frameworkVersionRegex.Match(frameworkVersion);
            if (installedMatch.Success == false)
            {
                return null;
            }

            return installedMatch.Groups["version"].Value?.Trim();
        }
    }
}
