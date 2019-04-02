using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace SwiftSupport
{
    public abstract class BaseVersionCheckTask : BaseTask
    {
        public string SdkVersion { get; set; }

        [Required]
        public ITaskItem AppManifest { get; set; }

        [Output]
        public bool ShouldIncludeSwiftDylibs { get; set; }

        protected Version GetMinOsVersion()
        {
            // <key>MinimumOSVersion</key><string>11.4</string>
            // Poor man implementation =) 
            if (File.Exists(AppManifest.ItemSpec))
            {
                string key = SdkPlatform == "MacOSX" ? "LSMinimumSystemVersion" : "MinimumOSVersion";
                string pattern = @"<key>(\s+)?" + key + @"(\s+)?<\/key>(\s+)?<string>(?<version>[\d\.]+)<\/string>";

                var filecontent = File.ReadAllText(AppManifest.ItemSpec);
                if (filecontent.Contains(key))
                {
                    var match = Regex.Match(filecontent, pattern);
                    if (match.Success)
                    {
                        return new Version(match.Groups["version"].Value);
                    }

                    return null; // dont assume anything if the os version is there but the regex fails
                }
            }

            if (string.IsNullOrWhiteSpace(SdkVersion) == false)
            {
                return new Version(SdkVersion);
            }

            return null;
        }

        protected Version SwiftEmbeddedOn()
        {
            switch (SdkPlatform) // C# 8...
            {
                case "AppleTVSimulator":
                case "AppleTVOS": return new Version(12, 2);
                case "WatchSimulator":
                case "WatchOS": return new Version(5, 2);
                case "MacOSX": return new Version(10, 14, 4);
                case "iPhoneSimulator":
                case "iPhoneOS":
                default: return new Version(12, 2);
            }
        }

        protected Version ParseRawSwiftVersion(string rawSwiftVersion)
        {
            const string pattern = @"[Ss]wift\s+(version\s+)?(?<version>[\d\.]+)";

            var match = Regex.Match(rawSwiftVersion, pattern);

            if (match.Success)
            {
                return new Version(match.Groups["version"].Value);
            }
            else
            {
                Log.LogMessage(MessageImportance.Normal, $"Failed to parse Swift version from string. File a report at https://github.com/Flash3001/Xamarin.SwiftSupport/issues including the following string: {rawSwiftVersion}");
                return null;
            }
        }

        protected bool NeedToIncludeSwift(Version sdkSwiftVersion, IEnumerable<Version> swiftVersions)
        {
            try
            {
                /*
                For now if you are running Xcode 10.2 and have a Swift 4 library it will fail because they are not compatible, so we are safe for now.
                but what will happen when Swift 6 or 10 is out. Will there be a mix os framework versions?
                Can I have a library written on Swift 6 another one on 5 and my Xcode include the runtime for 7?
                Will they work together? Should we include the runtime in this situation? We will figure out on the future.
                */

                // Based on Xcode version
                if (NeedToIncludeSwiftForLibrary(sdkSwiftVersion))
                {
                    return true;
                }

                // Based on Framework versions
                if (swiftVersions.Any(NeedToIncludeSwiftForLibrary))
                {
                    return true;
                }

                // Based on Target OS
                if (NeedToIncludeSwiftForTargetOS())
                {
                    return true;
                }

                Log.LogMessage(MessageImportance.Normal, $"Avoiding to include Swift dylibs because your target OS already includes it.");

                return false;
            }
            catch (Exception ex)
            {
                Log.LogMessage(MessageImportance.Normal, $"Including Swift dylibs because something went wrong and we dont want to prevent the build because of that. Please file a bug report with the following Warning at https://github.com/Flash3001/Xamarin.SwiftSupport/issues");
                Log.LogWarningFromException(ex);

                return false;
            }
        }

        private bool NeedToIncludeSwiftForTargetOS()
        {
            var targetVersion = GetMinOsVersion();
            if (targetVersion == null)
            {
                Log.LogMessage(MessageImportance.Normal, $"Including Swift dylibs because it was not possible to figure out your target OS version.");
                return true;
            }

            if (targetVersion < SwiftEmbeddedOn())
            {
                Log.LogMessage(MessageImportance.Normal, $"Including Swift dylibs because you are targeting an OS version that doesnt include it.");
                return true;
            }

            return false;
        }


        private bool NeedToIncludeSwiftForLibrary(Version swiftVersion)
        {
            if (swiftVersion < new Version(5, 0))
            {
                Log.LogMessage(MessageImportance.Normal, $"Including Swift dylibs because you need a version of Swift lower than 5.");
                return true;
            }

            return false;
        }
    }
}
