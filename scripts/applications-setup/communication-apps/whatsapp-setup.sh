#!/bin/bash

echo "Installing WhatsApp Desktop..."

# Check if Homebrew is installed
if ! command -v brew &> /dev/null; then
    echo "Homebrew is required but not installed. Please install Homebrew first."
    exit 1
fi

# Install WhatsApp Desktop using Homebrew
if brew list --cask whatsapp &> /dev/null; then
    echo "WhatsApp Desktop is already installed."
else
    echo "Installing WhatsApp Desktop..."
    brew install --cask whatsapp
    
    if [ $? -eq 0 ]; then
        echo "WhatsApp Desktop installed successfully!"
    else
        echo "Failed to install WhatsApp Desktop."
        exit 1
    fi
fi

echo "WhatsApp Desktop setup complete!"
