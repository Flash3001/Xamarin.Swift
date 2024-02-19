using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using SwiftSupport.Shared;

namespace SwiftSupport
{
    public class ProduceIncludeFrameworksCommand : BaseIncludeSwiftTask
    {
        [Output]
        public ITaskItem Command { get; set; }

        [Required]
        public ITaskItem[] Frameworks { get; set; }

        public override bool Execute()
        {
            /* This process does something similar to what ScanSwiftTask and IncludeSwiftTask does
             * but with caveats.           
             * 
             * The goal was to have a single code base for both macOS and Windows, but due to how 
             * iOS compilation works on Windows it was not possible.
             * 
             * When you do iOS specific compilation steps on Windows it is not actually running on 
             * Windows, it is sending a message and required files to a build server on macOS that 
             * runs and copy the results back to Windows when necessary.
             * 
             * This messaging from Windows to macOS happens through a class called TaskRunner
             * that lives on Xamarin.Messaging...
             * 
             * My first attempt to make the compilation work on Windows was to add a copy of 
             * Xamarin.Messaging (or try reflection), access the TaskRunner and send the message
             * to macOS to run IncludeSwiftTask and ScanSwiftTask, but it turns out these classes are
             * nowhere to be found on macOS build agent - are all dependencies installed when 
             * Xamarin first install the build agent or is there a way to inject code on demand?
             * I assumed the former was the case and stopped persuing this route as it was too 
             * hacky and I couldnt find the code for Xamarin.Messaging dlls, all I had was 
             * disassembled versions of those. 
             * 
             * The second attempt was to convert these Tasks into commands to be executed 
             * by Xamarin version of Microsoft.Build.Tasks.Exec that has all the plubling necessary
             * to make the communication between macOS and Windows work, as it is pre-installed on 
             * the build agent, but it didnt work. For some reason I was getting a 
             * *"Exec" task received an invalid value for the "StdOutEncoding" parameter.* 
             * while trying to read the command output. What?!
             * 
             * Tried to save the ouput to a file that was going to be synchronized between macOS and
             * Windows, but only the file existence was, not its content. 
             * 
             * End result? I can only do one-way communication and send the entire thing as a single command.
             * 
             * I need to say that even though it doenst work the same as macOS it is a huge
             * improvement over the original version - more on that on another comment.
             * 
             * There are 3 missing features:
             * 
             * - There is no way to figure out what are the architectures available on the 
             * current version of Swift on your Xcode, so I had to include a precomputed list.
             * Search for GetPlatformName method. macOS version learns this on demand so when 
             * the day comes for a new architecture to be included I dont have to do anything
             * and it will just work. Windows version will require an update.
             * 
             * - When searching for Swift dependencies on the Swift runtime the script is only 
             * doing a single check, so if one day it happens that your Framework depends on A
             * that depends on B that depends on C and B is not a direct dependency from any of 
             * your Frameworks and only B have a reference to C. C will be missing. I should 
             * draw this, because dependency explanations are a pain to understand from text.
             * macOS version continue to scan all libraries it finds until there is nothing else
             * to scan.
             * 
             * - The final .APP and .IPA files will always include the Swift runtime libraries 
             * even if the liraries are made with Swift 5 and your target is above iOS 12.2
             * While developers still give support for iOS lower than 12.2 it should be fine, 
             * but at some point I need to figure it out.             
             * 
             * Sorry Windows users, at least for now I dont know what can be done to unify things!         
            */

            string args = GetLipoArgs(GetKnownArchs());
            var xcodePath = GetRuntimePath().FirstOrDefault(); // TODO if the windows user is including anything from Swift 5.5 this wont work 
            var toolsPath = GetToolsPath();

            var otool = Path.Combine(toolsPath, "otool");
            var lipo = Path.Combine(toolsPath, "lipo");

            var sb = new StringBuilder();

            // Scan app Frameworks for Swift Dependencies 
            sb.Append("{ ");
            sb.Append(string.Join(" & ", Frameworks.Select(c => $"'{otool}' -l '{c.ItemSpec}'")));
            sb.Append("; }");

            // Clean and sort the results to get single, unique Swift dependency per line
            sb.Append(@" | grep @rpath/libswift");
            sb.Append(@" | awk -Frpath/ '{print $2}' | awk -F..offset '{print $1}'"); //sb.Append(@" | sed -E 's/^.*@rpath.(.*.dylib).*$/\1/'"); // !#$!@# path translation
            sb.Append(@" | sort | uniq");

            // Swift libraries can depend on another Swift library - this only goes 1 level deep
            // need to test if it will be an issue - macOS version doesnt work the same way.
            sb.Append(@" | while read line; do ");
            sb.Append($@"'{otool}' -l ""{xcodePath}/$line""");
            sb.Append(@" | grep @rpath/libswift");
            sb.Append(@" | awk -Frpath/ '{print $2}' | awk -F..offset '{print $1}'"); //sb.Append(@" | sed -E 's/^.*@rpath.(.*.dylib).*$/\1/'"); // !#$!@# path translation
            sb.Append(@";done");
            sb.Append(@" | sort | uniq");

            // For each dependency found copy it to the /Frameworks folder
            // Removing the architectures we dont need for the final project
            sb.Append(@" | while read line; do ");
            sb.Append($@"'{lipo}' ""{xcodePath}/$line"" {args} ""{GetOutputPath(string.Empty)}/$line"""); // TODO: how to make it parallel?
            sb.Append(@";done");

            Command = new TaskItem(sb.ToString());

            return true;
        }
    }
}
