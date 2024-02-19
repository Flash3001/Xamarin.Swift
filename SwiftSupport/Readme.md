Still trying to figure out a name for this project! 
SwiftSupport? UniversalSwiftSupport? Xamarin.Swift? 

The goal is to provide support for any version of Swift for any 
supported plaform (macOS, tvOS, watchOS) - more on that when it gets ready to be published.

**Running an App with Swift Code**

To include Swift support for an App we only to include all libswift*.dylib 
referenced from any Framework in your app.

This is how it is done: 

1 - Scan your Frameworks for references to Swift runtime libraries - ScanSwiftTask.cs
2 - Scan these Swift libraries for internal dependencies (available on your Xcode installation) - ScanSwiftTask.cs
3 - Copy all required dependencies from Xcode to name.app/Frameworks - IncludeSwiftTask.cs

On step 3 we only copy the architectures needed for your app.

Due to some limitations on the Windows version all 3 steps happens at once as a script. 
See ProduceIncludeFrameworksCommand.cs for comments - yes, 2 versions.

**Publishing an with Swift Code**

We need to include a copy of each Swift dependency on your .IPA, not the same as before
this time it needs to be outside of your .app. 

For each dependency on name.IPA/Playload/name.app/Frameworks/libswift*.dylib we need a copy on 
name.IPA/SwiftSupport/iphoneos/libswift*.dylib - iphoneos name changes for tvOS, macOS, watchOS.

This new copy needs to the original version that came with Xcode, without any changes to the 
file signature (dont use codesign on it) and all the architectures included.

For more information read ProduceIncludeSwiftSupportCommand.cs (same for Windows and macOS).

This process can happen on 2 different situations on Xamarin:

1 - when you have *Build iTunes Package Archiver (IPA)* checked
2 - when you click *Archive for Publishing* 

The later one doesnt work as expected on Xamarin as it created a archive file containing the required SwiftSupport 
folder and its file, but when you click Sign and Distribute it doesnt copy all the files, just the .app file.
For more information read SwiftSupport.targets

**Changes on Swift 5 - iOS 12.2**

-- TO BE ADDED -- 