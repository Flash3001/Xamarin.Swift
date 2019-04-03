using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using SwiftSupport.Internals;
using SwiftSupport.Shared;

namespace SwiftSupport
{
    public class CheckSwiftVersionsTask : BaseVersionCheckTask
    {
        private readonly Regex _frameworkNameRegex = new Regex(@"(?<path>.*\.framework)/(?<name>.+)");
        private readonly Regex _frameworkVersionRegex = new Regex(@".*(?<version>Apple Swift version.*\)).*");
        private readonly Regex _installedVersionRegex = new Regex(@".*(?<version>Apple Swift version.*\))(.*Target.*)?");

        [Required]
        public ITaskItem[] Frameworks { get; set; }

        [Required]
        public bool ShowErrorAsWarnings { get; set; }

        public override bool Execute()
        {
            Version installedSwiftVersion = null;
            var frameworkAndVersion = new ConcurrentDictionary<string, Version>();

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

            ShouldIncludeSwiftDylibs = frameworkAndVersion.Count > 0 && NeedToIncludeSwift(installedSwiftVersion, frameworkAndVersion.Values);

            return ValidateVersions(installedSwiftVersion, frameworkAndVersion);
        }

        private void ScanAndAddFrameworkVersion(ITaskItem lib, ConcurrentDictionary<string, Version> frameworkAndVersion)
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

        private bool ValidateVersions(Version installedSwiftVersion, ConcurrentDictionary<string, Version> frameworkAndVersion)
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
            Log.LogMessage($"Looking for Swift version on: {framework.ItemSpec}");

            var frameworkMatch = _frameworkNameRegex.Match(framework.ItemSpec);
            if (frameworkMatch.Success == false)
            {
                return null;
            }

            var frameworPath = frameworkMatch.Groups["path"].Value;
            var frameworkName = frameworkMatch.Groups["name"].Value;

            var headerPath = Path.Combine(frameworPath, "Headers", $"{frameworkName}-Swift.h");

            Log.LogMessage($"Expected Swift header on: {headerPath}");

            if (File.Exists(headerPath) == false)
            {
                Log.LogMessage($"{headerPath} do not exist.");
                return null;
            }

            Log.LogMessage($"{headerPath} exists.");


            return new Tuple<string, string>(frameworkName, headerPath);
        }

        private Version GetInstalledVersion()
        {
            var installed = this.Run(GetToolsPath(), "swift", "--version");

            var installedMatch = _installedVersionRegex.Match(installed);
            if (installedMatch.Success == false)
            {
                return null;
            }

            return ParseRawSwiftVersion(installedMatch.Groups["version"].Value?.Trim());
        }

        private Version GetFrameworkVersion(string headerPath)
        {
            var frameworkVersion = this.Run(string.Empty, "grep", $"swiftlang {headerPath}");

            var installedMatch = _frameworkVersionRegex.Match(frameworkVersion);
            if (installedMatch.Success == false)
            {
                return null;
            }

            return ParseRawSwiftVersion(installedMatch.Groups["version"].Value?.Trim());
        }
    }
}
