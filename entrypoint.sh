#!/bin/bash
echo "Start building..."
cd /source
mkdir builddist
ARCH="--linux-x64"

if [ ! -z "$1" ]; then
    ARCH="$1"
fi

if [[ "$ARCH" != "--linux-x64" && "$ARCH" != "--osx-x64" && "$ARCH" != "--win-x64" ]]; then
    echo "Invalid arg '$ARCH'; Only --linux-x64/--osx-x64/--win-x64 is allowed."
    exit 1
fi

case "$ARCH" in
    --linux-x64)
        echo "Building for linux x64..."
        dotnet restore -r linux-x64
        dotnet publish -c Release -r linux-x64 /p:PublishSingleFile=true --self-contained true
        mv /source/bin/Release/net6.0/linux-x64/publish/* builddist
        mv ./amsat-all-frequencies.json builddist
        ;;
    --osx-x64)
        echo "Building for macos x64..."
        dotnet restore -r osx-x64
        dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64 -p:UseAppHost=true -property:Configuration=Release     
        mv /source/bin/Release/net6.0/osx-x64/publish/SenhaixFreqWriter.app builddist
        cp Asset/shx8800-icons/icon.icns builddist/SenhaixFreqWriter.app/Contents/Resources
        mv ./amsat-all-frequencies.json builddist/SenhaixFreqWriter.app/Contents/MacOS
        ;;
    --win-x64)
        echo "Building for windows x64..."
        dotnet restore -r win-x64
        dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:TargetOS=Windows --self-contained true 
        mv /source/bin/Release/net6.0-windows10.0.19041.0/win-x64/publish/* builddist
        mv ./amsat-all-frequencies.json builddist
        ;;
    *)
        echo "Invalid Args"
        exit 1
        ;;
esac

echo "Done!"