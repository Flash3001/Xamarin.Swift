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

namespace SwiftSupport.Internals
{
    internal static class ProcessRunner
    {
        private static string GetFullPathToTool(string app)
        {
            //if (!string.IsNullOrEmpty(ToolPath))
            //    return Path.Combine(ToolPath, app);

            var path = Path.Combine("/usr/bin", app);

            return File.Exists(path) ? path : app;
        }

        public static string Run(this BaseTask parentTask, string app, string args)
        {
            using (var process = new Process())
            {
                StartAndWaitProcess(app, args, process);

                using (var stream = process.StandardOutput)
                {
                    return stream.ReadToEnd();
                }
            }
        }

        public static IEnumerable<string> RunAndReadLines(this BaseTask parentTask, string app, string args)
        {
            using (var process = new Process())
            {
                StartAndWaitProcess(app, args, process);

                using (var stream = process.StandardOutput)
                {
                    string line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
            }
        }

        private static void StartAndWaitProcess(string app, string args, Process process)
        {
            process.StartInfo = GetInfo(app, args);
            process.Start();
            process.WaitForExit();
            
            // ... 
            while (!process.HasExited && process.Responding)
            {
                Thread.Sleep(10);
            }
        }

        private static ProcessStartInfo GetInfo(string app, string args)
        {
            return new ProcessStartInfo(GetFullPathToTool(app), args)
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
