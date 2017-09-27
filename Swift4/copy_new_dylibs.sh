#!/bin/bash
DEVELOPER_DIR=`xcode-select --print-path`
if [ ! -d "${DEVELOPER_DIR}" ]; then
	echo "No developer directory found!"
	exit 1
fi

for PROJECT in $(ls -1 -d Xamarin.Swift4.*/); do
    for SWIFT_LIB in $(ls -1 "${PROJECT}/Frameworks/"); do
        echo "Copying ${SWIFT_LIB}"
        cp "${DEVELOPER_DIR}/Toolchains/XcodeDefault.xctoolchain/usr/lib/swift/iphoneos/${SWIFT_LIB}" "${PROJECT}/Frameworks"
        cp "${DEVELOPER_DIR}/Toolchains/XcodeDefault.xctoolchain/usr/lib/swift/iphonesimulator/${SWIFT_LIB}" "${PROJECT}/SwiftFrameworksSimulator"
    done
done
