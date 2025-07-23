#!/bin/bash

# MacUtil GUI macOS Deployment Script
# Creates proper .app bundles for macOS distribution

echo "🚀 Starting MacUtil GUI macOS deployment process..."
echo

# Configuration
APP_NAME="MacUtilGUI"
BUNDLE_NAME="MacUtil"
BUNDLE_IDENTIFIER="com.macutil.gui"
VERSION="1.0.0"
COPYRIGHT="Copyright © 2025 MacUtil. All rights reserved."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Clean previous builds
print_status "Cleaning previous builds..."
dotnet clean -c Release
rm -rf ./bin/Release/net9.0/publish/
rm -rf ./dist/
print_success "Cleanup complete"
echo

# Restore packages
print_status "Restoring NuGet packages..."
dotnet restore
print_success "Package restoration complete"
echo

# Create output directories
mkdir -p ./dist/

# Function to create app bundle
create_app_bundle() {
    local runtime=$1
    local arch_name=$2
    
    print_status "Building for $arch_name ($runtime)..."
    
    # Publish the application
    dotnet publish -c Release -r $runtime -p:UseAppHost=true --self-contained true -o ./bin/Release/net9.0/publish/$runtime/
    
    if [ $? -ne 0 ]; then
        print_error "$arch_name build failed"
        return 1
    fi
    
    print_success "$arch_name build successful"
    
    # Create .app bundle structure
    local app_bundle="./dist/${BUNDLE_NAME}-${arch_name}.app"
    print_status "Creating $arch_name app bundle..."
    
    mkdir -p "$app_bundle/Contents/MacOS"
    mkdir -p "$app_bundle/Contents/Resources"
    
    # Copy the executable and dependencies
    cp -R "./bin/Release/net9.0/publish/$runtime/"* "$app_bundle/Contents/MacOS/"
    
    # Create Info.plist
    cat > "$app_bundle/Contents/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleIconFile</key>
    <string>MacUtilGUI.icns</string>
    <key>CFBundleIdentifier</key>
    <string>$BUNDLE_IDENTIFIER</string>
    <key>CFBundleName</key>
    <string>$BUNDLE_NAME</string>
    <key>CFBundleDisplayName</key>
    <string>MacUtil GUI</string>
    <key>CFBundleVersion</key>
    <string>$VERSION</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
    <key>CFBundleExecutable</key>
    <string>$APP_NAME</string>
    <key>CFBundleInfoDictionaryVersion</key>
    <string>6.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleSignature</key>
    <string>????</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>NSPrincipalClass</key>
    <string>NSApplication</string>
    <key>LSApplicationCategoryType</key>
    <string>public.app-category.utilities</string>
    <key>NSRequiresAquaSystemAppearance</key>
    <false/>
    <key>NSHumanReadableCopyright</key>
    <string>$COPYRIGHT</string>
</dict>
</plist>
EOF
    
    # Copy icon file
    if [ -f "MacUtilGUI.icns" ]; then
        cp "MacUtilGUI.icns" "$app_bundle/Contents/Resources/"
    else
        print_warning "Icon file MacUtilGUI.icns not found"
    fi
    
    # Make the executable file executable
    chmod +x "$app_bundle/Contents/MacOS/$APP_NAME"
    
    print_success "$arch_name app bundle created: $app_bundle"
    
    # Show bundle size
    local bundle_size
    bundle_size=$(du -sh "$app_bundle" | cut -f1)
    print_status "Bundle size: $bundle_size"
    
    return 0
}

# Build for Intel x64 Macs
if ! create_app_bundle "osx-x64" "Intel"; then
    exit 1
fi
echo

# Build for Apple Silicon ARM64 Macs
if ! create_app_bundle "osx-arm64" "AppleSilicon"; then
    exit 1
fi
echo

# Create universal app bundle using dotnet-bundle (if available)
print_status "Attempting to create universal app bundle using dotnet-bundle..."

# First, try with Intel build
dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64 -p:Configuration=Release -p:UseAppHost=true 2>/dev/null

if [ $? -eq 0 ] && [ -d "./bin/Release/net9.0/osx-x64/publish/${BUNDLE_NAME}.app" ]; then
    print_success "Universal app bundle created via dotnet-bundle"
    cp -R "./bin/Release/net9.0/osx-x64/publish/${BUNDLE_NAME}.app" "./dist/${BUNDLE_NAME}-Universal.app"
    
    # Update the universal bundle with proper icon
    if [ -f "MacUtilGUI.icns" ]; then
        cp "MacUtilGUI.icns" "./dist/${BUNDLE_NAME}-Universal.app/Contents/Resources/"
    fi
    
    universal_size=$(du -sh "./dist/${BUNDLE_NAME}-Universal.app" | cut -f1)
    print_status "Universal bundle size: $universal_size"
else
    print_warning "dotnet-bundle not available or failed, using separate bundles"
fi

echo
print_success "🎉 macOS deployment complete!"
echo
print_status "📦 Created app bundles:"
echo "─────────────────────────────────────────────────────────"
ls -la ./dist/*.app 2>/dev/null || echo "No .app bundles found in ./dist/"
echo
print_status "🎯 Usage Instructions:"
echo "─────────────────────────────────────────────────────────"
echo "• Intel Mac users: Use MacUtil-Intel.app"
echo "• Apple Silicon Mac users: Use MacUtil-AppleSilicon.app"
if [ -d "./dist/${BUNDLE_NAME}-Universal.app" ]; then
    echo "• Universal (any Mac): Use MacUtil-Universal.app"
fi
echo
print_status "📝 Next Steps for Distribution:"
echo "─────────────────────────────────────────────────────────"
echo "1. Test the .app bundles on target machines"
echo "2. For distribution outside App Store:"
echo "   - Code sign the apps with a Developer ID certificate"
echo "   - Notarize the apps with Apple"
echo "   - Create a .dmg installer (optional)"
echo "3. For App Store distribution:"
echo "   - Use App Store certificates and provisioning profiles"
echo "   - Package as .pkg for submission"
echo
print_success "All builds completed successfully! 🚀"
