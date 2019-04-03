#TO BE UPDATED

# Xamarin.Swift4.Support
The packages for Swift 4 and 3.2 aren't uploaded to Nuget as an update to the old ones, but as entire new set of packages - I guess the initial name for the packages was not very good.

The new libraries (8 in total) added in Xcode 9 are:
- libswiftAccelerate
- libswiftARKit
- libswiftCoreFoundation
- libswiftMediaPlayer
- libswiftMetal
- libswiftMetalKit
- libswiftModelIO
- libswiftVision

If you are using Swift 4 or 3.2 you should be using this packages:

https://www.nuget.org/packages/Xamarin.Swift4/

<h4>Libraries:</h4>

- **https://www.nuget.org/packages/Xamarin.Swift4.Accelerate/**
- **https://www.nuget.org/packages/Xamarin.Swift4.ARKit/**
- https://www.nuget.org/packages/Xamarin.Swift4.AssetsLibrary/
- https://www.nuget.org/packages/Xamarin.Swift4.AVFoundation/
- https://www.nuget.org/packages/Xamarin.Swift4.CallKit/
- https://www.nuget.org/packages/Xamarin.Swift4.CloudKit/
- https://www.nuget.org/packages/Xamarin.Swift4.Contacts/
- https://www.nuget.org/packages/Xamarin.Swift4.Core/
- https://www.nuget.org/packages/Xamarin.Swift4.CoreAudio/
- https://www.nuget.org/packages/Xamarin.Swift4.CoreData/
- **https://www.nuget.org/packages/Xamarin.Swift4.CoreFoundation/**
- https://www.nuget.org/packages/Xamarin.Swift4.CoreGraphics/
- https://www.nuget.org/packages/Xamarin.Swift4.CoreImage/
- https://www.nuget.org/packages/Xamarin.Swift4.CoreLocation/
- https://www.nuget.org/packages/Xamarin.Swift4.CoreMedia/
- https://www.nuget.org/packages/Xamarin.Swift4.Darwin/
- https://www.nuget.org/packages/Xamarin.Swift4.Dispatch/
- https://www.nuget.org/packages/Xamarin.Swift4.Foundation/
- https://www.nuget.org/packages/Xamarin.Swift4.GameplayKit/
- https://www.nuget.org/packages/Xamarin.Swift4.GLKit/
- https://www.nuget.org/packages/Xamarin.Swift4.HomeKit/
- https://www.nuget.org/packages/Xamarin.Swift4.MapKit/
- **https://www.nuget.org/packages/Xamarin.Swift4.MediaPlayer/**
- **https://www.nuget.org/packages/Xamarin.Swift4.Metal/**
- **https://www.nuget.org/packages/Xamarin.Swift4.MetalKit/**
- **https://www.nuget.org/packages/Xamarin.Swift4.ModelIO/**
- https://www.nuget.org/packages/Xamarin.Swift4.Intents/
- https://www.nuget.org/packages/Xamarin.Swift4.ObjectiveC/
- https://www.nuget.org/packages/Xamarin.Swift4.OS/
- https://www.nuget.org/packages/Xamarin.Swift4.Photos/
- https://www.nuget.org/packages/Xamarin.Swift4.QuartzCore/
- https://www.nuget.org/packages/Xamarin.Swift4.RemoteMirror/
- https://www.nuget.org/packages/Xamarin.Swift4.SceneKit/
- https://www.nuget.org/packages/Xamarin.Swift4.SIMD/
- https://www.nuget.org/packages/Xamarin.Swift4.SpriteKit/
- https://www.nuget.org/packages/Xamarin.Swift4.SwiftOnoneSupport/
- https://www.nuget.org/packages/Xamarin.Swift4.UIKit/
- **https://www.nuget.org/packages/Xamarin.Swift4.Vision/**
- https://www.nuget.org/packages/Xamarin.Swift4.WatchKit/
- https://www.nuget.org/packages/Xamarin.Swift4.XCTest/


# Xamarin.Swift3.Support 

Xamarin doesn't yet provide support for binding Swift libraries. 
This project is a try to provide all Swift3 runtime/libraries in a organized way. 

Each library dependecy is provided through a NuGet package, all of them will depend on Xamarin.Swift3 package which only include a MSBuild target file for moving the denpendencies when using Simulator and Device. 

Do not include them all as dependency in your project as it will increase the final App size. Use just what you need.

<h2>List of NuGet packages</h2>
<h4>Moves runtime files around:</h4>
https://www.nuget.org/packages/Xamarin.Swift3/

<h4>Libraries:</h4>

- https://www.nuget.org/packages/Xamarin.Swift3.AssetsLibrary/
- https://www.nuget.org/packages/Xamarin.Swift3.AVFoundation/
- https://www.nuget.org/packages/Xamarin.Swift3.CallKit/
- https://www.nuget.org/packages/Xamarin.Swift3.CloudKit/
- https://www.nuget.org/packages/Xamarin.Swift3.Contacts/
- https://www.nuget.org/packages/Xamarin.Swift3.Core/
- https://www.nuget.org/packages/Xamarin.Swift3.CoreAudio/
- https://www.nuget.org/packages/Xamarin.Swift3.CoreData/
- https://www.nuget.org/packages/Xamarin.Swift3.CoreGraphics/
- https://www.nuget.org/packages/Xamarin.Swift3.CoreImage/
- https://www.nuget.org/packages/Xamarin.Swift3.CoreLocation/
- https://www.nuget.org/packages/Xamarin.Swift3.CoreMedia/
- https://www.nuget.org/packages/Xamarin.Swift3.Darwin/
- https://www.nuget.org/packages/Xamarin.Swift3.Dispatch/
- https://www.nuget.org/packages/Xamarin.Swift3.Foundation/
- https://www.nuget.org/packages/Xamarin.Swift3.GameplayKit/
- https://www.nuget.org/packages/Xamarin.Swift3.GLKit/
- https://www.nuget.org/packages/Xamarin.Swift3.HomeKit/
- https://www.nuget.org/packages/Xamarin.Swift3.MapKit/
- https://www.nuget.org/packages/Xamarin.Swift3.Intents/
- https://www.nuget.org/packages/Xamarin.Swift3.ObjectiveC/
- https://www.nuget.org/packages/Xamarin.Swift3.OS/
- https://www.nuget.org/packages/Xamarin.Swift3.Photos/
- https://www.nuget.org/packages/Xamarin.Swift3.QuartzCore/
- https://www.nuget.org/packages/Xamarin.Swift3.RemoteMirror/
- https://www.nuget.org/packages/Xamarin.Swift3.SceneKit/
- https://www.nuget.org/packages/Xamarin.Swift3.SIMD/
- https://www.nuget.org/packages/Xamarin.Swift3.SpriteKit/
- https://www.nuget.org/packages/Xamarin.Swift3.SwiftOnoneSupport/
- https://www.nuget.org/packages/Xamarin.Swift3.UIKit/
- https://www.nuget.org/packages/Xamarin.Swift3.WatchKit/
- https://www.nuget.org/packages/Xamarin.Swift3.XCTest/

<h2>Walkthrough</h2>
If you need a full walkthrough on How To Bind Swift Libries, you can find it here: 
https://medium.com/@Flash3001/binding-swift-libraries-xamarin-ios-ff32adbc7c76

<h2>Takeaways</h2>
When creating the IPA to publish the final app you might get the following error:

"Invalid Swift Support - The SwiftSupport folder is missing. Rebuild your app using the current public (GM) version of Xcode and resubmit it."

Use this script to create this SwiftSupport folder: https://github.com/Flash3001/ipa-packager 


<h2>License</h2>
Copyright 2017 Lucas Teixeira

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
