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

        protected string[] GetRuntimePath()
        {
            //Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/lib/swift/iphoneos/libswiftAccelerate.dylib

            return new string[2]
            {
                Path.Combine(XcodePath, "Toolchains", "XcodeDefault.xctoolchain", "usr", "lib", "swift-5.0", GetPlatformName()),
                Path.Combine(XcodePath, "Toolchains", "XcodeDefault.xctoolchain", "usr", "lib", "swift-5.5", GetPlatformNameSwift55()),
            };
        }

        protected string GetToolsPath()
        {
            ///Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/bin/otool

            return Path.Combine(XcodePath, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin");
        }

        protected string GetPlatformName()
        {
            var platform = "iphoneos";

            switch (SdkPlatform) // C# 8...
            {
                case "AppleTVSimulator": platform = "appletvsimulator"; break;
                case "AppleTVOS": platform = "appletvos"; break;
                case "iPhoneSimulator": platform = "iphonesimulator"; break;
                case "WatchSimulator": platform = "watchos"; break;
                case "WatchOS": platform = "watchos"; break;
                case "MacOSX": platform = "macosx"; break;
                case "iPhoneOS": platform = "iphoneos"; break;
            }

            return platform;
        }

        protected string GetPlatformNameSwift55()
        {
            var platform = "iphoneos";

            switch (SdkPlatform) 
            {
                case "AppleTVSimulator": platform = "appletvsimulator"; break;
                case "AppleTVOS": platform = "appletvos"; break;
                case "iPhoneSimulator": platform = "iphonesimulator"; break;
                case "WatchSimulator": platform = "watchos"; break;
                case "WatchOS": platform = "watchsimulator"; break;
                case "MacOSX": platform = "macosx"; break;
                case "iPhoneOS": platform = "iphoneos"; break;
            }

            return platform;
        }

        protected string[] GetKnownArchs()
        {
            switch (SdkPlatform)
            {
                case "AppleTVSimulator": return new string[] { "x86_64" };
                case "AppleTVOS": return new string[] { "arm64" };
                case "iPhoneSimulator": return new string[] { "x86_64" };
                case "WatchSimulator": return new string[] { "arm64_32" };
                case "WatchOS": return new string[] { "arm64_32" };
                case "MacOSX": return new string[] { "x86_64" };
                case "iPhoneOS": return new string[] { "arm64" };
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
