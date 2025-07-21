#!/bin/bash

# MacUtil GUI Publisher Script
# Builds the F# Avalonia GUI application for both Intel and Apple Silicon Macs

echo "🚀 Starting MacUtil GUI publish process..."
echo

# Clean previous builds
echo "🧹 Cleaning previous builds..."
dotnet clean -c Release
rm -rf ./bin/Release/net9.0/publish/
echo "✅ Cleanup complete"
echo

# Create publish directory structure
mkdir -p ./bin/Release/net9.0/publish/

# Publish for Intel x64 Macs
echo "🔨 Building for Intel x64 Macs..."
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -o ./bin/Release/net9.0/publish/osx-x64/
if [ $? -eq 0 ]; then
    echo "✅ Intel x64 build successful"
else
    echo "❌ Intel x64 build failed"
    exit 1
fi
echo

# Publish for Apple Silicon ARM64 Macs
echo "🔨 Building for Apple Silicon ARM64 Macs..."
dotnet publish -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -o ./bin/Release/net9.0/publish/osx-arm64/
if [ $? -eq 0 ]; then
    echo "✅ Apple Silicon ARM64 build successful"
else
    echo "❌ Apple Silicon ARM64 build failed"
    exit 1
fi
echo

# Show build results
echo "📦 Build Results:"
echo "─────────────────────────────────────────────────────────"
echo "Intel x64 binary:"
ls -lh ./bin/Release/net9.0/publish/osx-x64/MacUtilGUI
echo
echo "Apple Silicon ARM64 binary:"
ls -lh ./bin/Release/net9.0/publish/osx-arm64/MacUtilGUI
echo

# Create distribution folder with renamed binaries
echo "📁 Creating distribution folder..."
mkdir -p ./dist/
cp ./bin/Release/net9.0/publish/osx-x64/MacUtilGUI ./dist/MacUtilGUI-intel
cp ./bin/Release/net9.0/publish/osx-arm64/MacUtilGUI ./dist/MacUtilGUI-silicon

# Make binaries executable
chmod +x ./dist/MacUtilGUI-intel
chmod +x ./dist/MacUtilGUI-silicon

echo "✅ Distribution binaries created:"
echo "   • ./dist/MacUtilGUI-intel     (for Intel Macs)"
echo "   • ./dist/MacUtilGUI-silicon   (for Apple Silicon Macs)"
echo

# Show usage instructions
echo "🎯 Usage Instructions:"
echo "─────────────────────────────────────────────────────────"
echo "Intel Mac users: Run ./dist/MacUtilGUI-intel"
echo "Apple Silicon Mac users: Run ./dist/MacUtilGUI-silicon"
echo
echo "Or you can run the appropriate binary from:"
echo "• ./bin/Release/net9.0/publish/osx-x64/MacUtilGUI"
echo "• ./bin/Release/net9.0/publish/osx-arm64/MacUtilGUI"
echo

echo "🎉 Publish process completed successfully!"
echo "📝 Note: Both binaries are self-contained and include all dependencies."
