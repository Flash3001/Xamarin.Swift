# Xamarin.Swift3.Support

Xamarin doesn't yet provide support for binding Swift libraries. 
This project is try to provide all Swift3 runtime libraries in a organized way. 

Each runtime dependecy will be provided through a NuGet package, all of them will depend on Xamarin.Swift3 package which only include a MSBuild target file for moving the denpendencies when using Simulator and Device. 

<h2>List of NuGet packages</h2>
TO-DO 

<h2>Finding out which packages to include</h2>
TO-DO

<h2>Walkthrough</h2>
If you need a full walkthrough on How To Bind Swift Libries, you can find it here: http://stackoverflow.com/documentation/xamarin.ios/6091/binding-swift-libraries

<h2>Takeaways</h2>
When creating the IPA to publish the final app you might get the following error:

"Invalid Swift Support - The SwiftSupport folder is missing. Rebuild your app using the current public (GM) version of Xcode and resubmit it."

Use this script to create this SwiftSupport folder: https://github.com/Flash3001/ipa-packager 


<h2>License</h2>
Copyright 2016 Lucas Teixeira, Daniel Cohen Gindi & Philipp Jahoda

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
