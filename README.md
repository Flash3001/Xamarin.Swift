Xamarin doesn't provide official support for Swift bindings, but it can still work by leveraging the native connection between Swift and Objective-C. We basically need to be sure the correct runtime and standard libraries are included in the final app.

This project is meant to do it for Xamarin.iOS projects with experimental support for Xamarin.tvOS and Xamarin.macOS. You can use the same package if you are using any version of Swift, including Swift 5, 4 and 3. 

You can find it at https://www.nuget.org/packages/Xamarin.Swift/

# To Swift library binders:

If you need more information on how to do the binding and how it differs from a normal Objective-C binding please read 
https://medium.com/@Flash3001/binding-swift-libraries-xamarin-ios-ff32adbc7c76

Your library binding doesn’t need to include the Swift runtime or use this project when you bind it as the final app is the one that needs to include it, but if you are publishing it through Nuget it is nice to list Xamarin.Swift as dependency so the final app can include it.

# To Swift libraries users: 

<h4>Running the app</h4>

When you build an app that depends on a Swift library you need to include everything that library depends in your app, including runtime and standard libraries. (that changed with Swift 5 for iOS 12.2, macOS 10.14.4 and tvOS 12.2)

Until Swift 5 a library built using on version of Xcode|Swift would only run on the exact runtime it was built for, if you included the wrong version in your app it would crash at start up.

<h4>Publishing</h4>

When publishing your app you need to include the same runtime and standard libraries inside your IPA on a folder called SwiftSuport. The difference to the ones included inside the APP is that the ones in your app needs to be signed using your Distribution key (done during the build process) and the ones inside the IPA will be uploaded as they came on Xcode, with Apples signature.


# What this project does

The goal is to automatically include the Swift dylibs (the runtime and standard libraries) inside your .APP during build time and to include the same dependencies during the Release build in your .IPA.

Xamarin.Swift (https://www.nuget.org/packages/Xamarin.Swift/) is the latest version of this project and will: 

  1. Check if the inclusion of those Swift dylibs in the Frameworks folder is really necessary (rules later on).

  2. Check your installed Xcode version against all your app's Swift based .frameworks - if they don't match or are not compatible the build will fail.

  3. Figure out each Swift dependency your project have.

  4. Copy those dependencies from Xcode to your project using the correct target OS and device (iOS, iOS Simulator, macOS, tvOS and tvOS Simulator). 

  4.a. - Swift dylibs support multiple architectures: armv7, armv7s, arm64 and arm64e for iOS Devices. x86 and x86_64 for iOS Simulator (and others for other devices), but a normal project on Xamarin.iOS usually uses armv7 and arm64 making the inclusion of armv7s and arm64e unnecessary. Only the archs your project uses will be included in your .APP. (it doesn’t affect your final user download on the App Store because Apple already does App Thinning and only sends what that device needs).

  5. If your Release build is checked to generate the IPA it will copy the same files over to the SwiftSupport folder (this folder is a right next to the Payload folder).


<h2>Possible issues</h2>

If you are here it probably is because you are having issues during compilation, runtime or publish. So to the main issues: 

<h4>My app crashes at start up; My app crashes on iOS 11, but not on iOS 12.2; It doesn't run on the simulator;</h4>

It is possible that this project .target file that MSBuild runs when you build your Xamarin project is not running or was broken by some update. Please check your build log and search for the Swift keyword to see if you find any of the Targets names on https://github.com/Flash3001/Xamarin.Swift/blob/master/SwiftSupport/build/SwiftSupport-ios.targets

If you see that it is not running check your .csproj to see if the Xamarin.Swift.targets is there and with a valid path. (except if you use PackageReference)

<h4>iTunes Connect rejected my binary</h4>

It can be for number of reasons, but the main one are:

"Invalid Swift Support - The SwiftSupport folder is missing."

Sending the app without the SwiftSupport folder inside the IPA. If the automatic process of Xamarin.Swift fails for some reason, or you are using a older version of this project you can do it yourself by running the script available on https://github.com/Flash3001/ipa-packager 

"Invalid Swift Support - libswiftCore.dylib ... doesn't match..."

The files on Frameworks and SwiftSupport have to be almost exactly the same, included architectures and version. The only difference is the who signed it. The ones in Frameworks are signed by your distribution key and the ones in SwiftSupport needs to be signed by Apple as they came on Xcode. If those files differ in either architecture or version it will be rejected.

Something else? Please create a new issue so we can investigate =) 

<h4>Rules for Swift dependencies inclusion</h4>

* if you project have any Swift dependency

* and you minimum target OS version is lower than OS 12.2, macOS 10.14.4 and tvOS 12.2

* or those libraries were build using a version of Swift lower than Swift 5


# Migration path from older versions of this project

If you are using any of the Xamarin.SwiftSupport, Xamarin.Swift3 or Xamarin.Swift4 packages and is migrating to Swift 5 or just want the improved version you need to remove any of the previous packages manually as Xamarin.Swift is not a Nuget upgrade to any of those, it is a new package. Be sure to edit your .csproj to remove any references if you are doing it manually. 

<h4>Wait! I am using a library built using Swift4 and I use all the Xamarin.Swift4 packages. Can I remove them and install Xamarin.Swift?</h4>

Yes, it will work. (except if you have unmatched Xcode installation and rely on your build server to have the correct version).

If you are a binder maintainer: Please do it on your dependency list on Nuget, it will help prevent a bunch of known issues caused by mismatched library and Swift|Xcode version. 

# License
Copyright 2019 Lucas Teixeira

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
