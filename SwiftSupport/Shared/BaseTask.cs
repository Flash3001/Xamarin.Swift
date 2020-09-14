using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using SwiftSupport.Internals;

namespace SwiftSupport.Shared
{
    public abstract class BaseTask : Task
    {
        public string SessionId { get; set; }
       
        [Required]
        public string OutputPath { get; set; }

        [Required]
        public string XcodePath { get; set; }

        [Required]
        public string SdkPlatform { get; set; }

        [Required]
        public string SdkVersion { get; set; }

        protected string GetOutputPath(string path = null) => string.IsNullOrWhiteSpace(path)
            ? Path.Combine(OutputPath, "Frameworks")
            : Path.Combine(OutputPath, "Frameworks", path);

        protected string GetRuntimePath()
        {
            //Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/lib/swift/iphoneos/libswiftAccelerate.dylib

            return Path.Combine(XcodePath, "Toolchains", "XcodeDefault.xctoolchain", "usr", "lib", GetSwiftFolder(), GetPlatformName());
        }

        protected string GetToolsPath()
        {
            ///Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/bin/otool

            return Path.Combine(XcodePath, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin");
        }

        public string GetSwiftFolder()
        {
            // HACK
            // I got the feeling this not how we should be doing it.
            // But I dont have access to AppleSdkSettings.XcodeVersion.Major
            // Or have any idea why Apple changed "swift" to "swift-5"
            // can we have "swift-6" or even "swift-4" installed later? 

            var version = new Version(SdkVersion);

            Version useSwift5 = new Version(99, 0);

            switch (SdkPlatform) // C# 8...
            {
                case "iPhoneSimulator": 
                case "iPhoneOS": useSwift5 = new Version(6, 0); break;
                case "AppleTVSimulator": 
                case "AppleTVOS": useSwift5 = new Version(13,0); break;
                case "WatchSimulator":
                case "WatchOS": useSwift5 = new Version(6, 0); break;
                case "MacOSX": useSwift5 = new Version(10, 15); break;
            }

            return version >= useSwift5 ? "swift-5.0" : "swift";
        }

        protected string GetPlatformName()
        {
            var platform = "iphoneos";


            switch (SdkPlatform) // C# 8...
            {
                case "AppleTVSimulator": platform = "appletvsimulator"; break;
                case "AppleTVOS": platform = "appletvos"; break;
                case "iPhoneSimulator": platform = "iphonesimulator"; break;
                case "WatchSimulator": platform = "watchsimulator"; break;
                case "WatchOS": platform = "watchos"; break;
                case "MacOSX": platform = "macosx"; break;
                case "iPhoneOS": platform = "iphoneos"; break;
            }

            return platform;
        }

        protected string[] GetKnownArchs()
        {
            switch (SdkPlatform) // C# 8...
            {
                case "AppleTVSimulator": return new string[] { "x86_64" };
                case "AppleTVOS": return new string[] { "arm64" };
                case "iPhoneSimulator": return new string[] { "i386", "x86_64" };
                case "WatchSimulator": return new string[] { "i386" };
                case "WatchOS": return new string[] { "armv7k", "arm64_32" };
                case "MacOSX": return new string[] { "x86_64" };
                case "iPhoneOS": return new string[] { "armv7", "armv7s", "arm64", "arm64e" };
                default: return new string[] { };
            }
        }


        public string RunLipo(string args)
        {
            Log.LogMessage(MessageImportance.Normal, $"{GetToolsPath()}/lipo {args}");
            
            return this.Run(GetToolsPath(), "lipo", args);
        }

        public IEnumerable<string> RunOtool(string args)
        {
            Log.LogMessage(MessageImportance.Normal, $"{GetToolsPath()}/otool {args}");

            return this.RunAndReadLines(GetToolsPath(), "otool", args);
        }
    }
}
