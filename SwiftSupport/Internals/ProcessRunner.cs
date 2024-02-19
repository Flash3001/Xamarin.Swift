using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using SwiftSupport.Shared;

namespace SwiftSupport.Internals
{
    internal static class ProcessRunner
    {
        private static string GetFullPathToTool(string toolPath, string app)
        {
            if (!string.IsNullOrEmpty(toolPath))
            {
                return Path.Combine(toolPath, app);
            }
            else
            {
                var path = Path.Combine("/usr/bin", app);
                return File.Exists(path) ? path : app;
            }
        }

        public static string Run(this BaseTask parentTask, string toolPath, string app, string args)
        {
            using (var process = new Process())
            {
                StartAndWaitProcess(toolPath, app, args, process);

                using (var stream = process.StandardOutput)
                {
                    return stream.ReadToEnd();
                }
            }
        }

        public static IEnumerable<string> RunAndReadLines(this BaseTask parentTask, string toolPath, string app, string args)
        {
            using (var process = new Process())
            {
                StartAndWaitProcess(toolPath, app, args, process);

                using (var stream = process.StandardOutput)
                {
                    string line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        yield return line;
                    }
                };
            }
        }

        private static void StartAndWaitProcess(string toolPath, string app, string args, Process process)
        {
            process.StartInfo = GetInfo(toolPath, app, args);
            process.Start();
            process.WaitForExit();
            
            // ... 
            while (!process.HasExited && process.Responding)
            {
                Thread.Sleep(10);
            }
        }

        private static ProcessStartInfo GetInfo(string toolPath, string app, string args)
        {
            return new ProcessStartInfo(GetFullPathToTool(toolPath, app), args)
            {
                WorkingDirectory = Environment.CurrentDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
        }
    }
}
