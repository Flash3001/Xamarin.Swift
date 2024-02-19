using System;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using SwiftSupport.Internals;
using SwiftSupport.Shared;

namespace SwiftSupport
{
    public class ProduceIncludeSwiftSupportCommand : BaseIncludeSwiftTask
    {
        [Output]
        public ITaskItem Command { get; set; }

        [Required]
        public ITaskItem ArchiveOrIpaDir { get; set; }

        public override bool Execute()
        {
            /*
            Apple requires apps built using Swift code to send a
            copy of each libswift*.dylib in a SwiftSupport folder 
            living inside the .IPA. 

            These copies have to be exact as they come with Xcode, 
            It means it will have all the architectures and no
            changes to codesign.

            This is different from the copies inside the /Frameworks
            (somename.app/Frameworks) folder that only need to include
            the architectures that you use and must be signed with 
            your signing key - contents on this folder will be removed
            for iOS 12.2 when using Swift 5 when App Store does 
            app thinning           
            */

            StringBuilder lipoCopyArgs = new StringBuilder();

            var frameworksFolder = GetOutputPath();
            string args = GetLipoArgs(GetKnownArchs());

            var swiftSupportPath = Path.Combine(ArchiveOrIpaDir.ItemSpec, "SwiftSupport");
            var platformPath = Path.Combine(swiftSupportPath, GetPlatformName());

            string[] xcodePaths = GetRuntimePath();
            var toolsPath = GetToolsPath();

            var otool = Path.Combine(toolsPath, "otool");
            var lipo = Path.Combine(toolsPath, "lipo");

            var sb = new StringBuilder();

            sb.Append($"mkdir -p \"{swiftSupportPath}\"; ");
            sb.Append($"mkdir -p \"{platformPath}\"; ");

            sb.Append($"ls -1 \"{frameworksFolder}\"");
            sb.Append($" | grep libswift");
            sb.Append(@" | while read dylib; do ");
            var possiblePaths = xcodePaths.Select(xcodePath => $@"(ls ""{xcodePath}/$dylib"" >> /dev/null 2>&1 && ""{lipo}"" ""{xcodePath}/$dylib"" {args} ""{platformPath}/$dylib"")");
            sb.Append(string.Join(" || ", possiblePaths)); //sb.Append($@"cp {xcodePath}/$dylib ""{platformPath}/$dylib""");
            sb.Append(@";done");

            Command = new TaskItem(sb.ToString());

            return true;
        }
    }
}
