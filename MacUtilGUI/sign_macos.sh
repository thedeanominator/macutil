#!/bin/bash

# macOS Code Signing Script for MacUtil GUI
# This script signs your app bundles for distribution outside the App Store

# Configuration - UPDATE THESE VALUES
DEVELOPER_ID="Developer ID Application: CT Tech Group LLC (8ZHX2A9ALF)"
APP_BUNDLE_PATH="./dist/MacUtil-Universal.app"  # Universal app bundle (recommended)
ENTITLEMENTS_FILE="./MacUtilGUI.entitlements"

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

echo "🔐 MacUtil GUI Code Signing Script"
echo "=================================="
echo

# Check if app bundle exists
if [ ! -d "$APP_BUNDLE_PATH" ]; then
    print_error "App bundle not found at: $APP_BUNDLE_PATH"
    print_status "Please run ./deploy_macos.sh first to create the app bundle"
    exit 1
fi

# Check if entitlements file exists
if [ ! -f "$ENTITLEMENTS_FILE" ]; then
    print_error "Entitlements file not found at: $ENTITLEMENTS_FILE"
    exit 1
fi

# Check if codesign is available
if ! command -v codesign &> /dev/null; then
    print_error "codesign command not found. Please install Xcode Command Line Tools."
    exit 1
fi

print_status "App bundle: $APP_BUNDLE_PATH"
print_status "Entitlements: $ENTITLEMENTS_FILE"
print_status "Developer ID: $DEVELOPER_ID"
echo

# List available signing identities
print_status "Available signing identities:"
security find-identity -v -p codesigning
echo

print_warning "IMPORTANT: Make sure you have:"
print_warning "1. A valid Developer ID Application certificate in your Keychain"
print_warning "2. Updated the DEVELOPER_ID variable in this script"
print_warning "3. An Apple Developer account for notarization"
echo

read -p "Do you want to proceed with code signing? (y/N): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    print_status "Code signing cancelled"
    exit 0
fi

# Sign all files in the MacOS directory
print_status "Signing all binaries in the app bundle..."
find "$APP_BUNDLE_PATH/Contents/MacOS/" -type f | while read fname; do
    print_status "Signing: $fname"
    codesign --force --timestamp --options=runtime --entitlements "$ENTITLEMENTS_FILE" --sign "$DEVELOPER_ID" "$fname"
    if [ $? -ne 0 ]; then
        print_error "Failed to sign: $fname"
        exit 1
    fi
done

# Sign the app bundle itself
print_status "Signing the app bundle..."
codesign --force --timestamp --options=runtime --entitlements "$ENTITLEMENTS_FILE" --sign "$DEVELOPER_ID" "$APP_BUNDLE_PATH"

if [ $? -eq 0 ]; then
    print_success "App bundle signed successfully!"
    
    # Verify the signature
    print_status "Verifying signature..."
    codesign --verify --verbose "$APP_BUNDLE_PATH"
    
    if [ $? -eq 0 ]; then
        print_success "Signature verification passed!"
        echo
        print_status "🎯 Next Steps for Distribution:"
        echo "1. Test the signed app on a different Mac"
        echo "2. For notarization (required for macOS 10.15+):"
        echo "   a. Create a zip: ditto -c -k --sequesterRsrc --keepParent '$APP_BUNDLE_PATH' MacUtil.zip"
        echo "   b. Submit for notarization: xcrun altool --notarize-app -f MacUtil.zip --primary-bundle-id com.macutil.gui -u your@apple.id -p @keychain:AC_PASSWORD"
        echo "   c. Wait for notarization to complete"
        echo "   d. Staple the notarization: xcrun stapler staple '$APP_BUNDLE_PATH'"
        echo
        print_success "Code signing complete! 🔐"
    else
        print_error "Signature verification failed!"
        exit 1
    fi
else
    print_error "Failed to sign app bundle!"
    exit 1
fi
