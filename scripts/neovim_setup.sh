#!/bin/sh -e

# Install Neovim and dependencies with brew
printf "%b\n" "Installing Neovim and dependencies..."
brew install neovim ripgrep fzf

# Backup existing config if it exists
if [ -d "$HOME/.config/nvim" ] && [ ! -d "$HOME/.config/nvim-backup" ]; then
    printf "%b\n" "Backing up existing Neovim config..."
    cp -r "$HOME/.config/nvim" "$HOME/.config/nvim-backup"
fi

# Clear existing config
rm -rf "$HOME/.config/nvim"
mkdir -p "$HOME/.config/nvim"

# Clone Titus kickstart config directly to .config/nvim
printf "%b\n" "Applying Titus Kickstart config..."
git clone --depth 1 https://github.com/ChrisTitusTech/neovim.git /tmp/neovim
cp -r /tmp/neovim/titus-kickstart/* "$HOME/.config/nvim/"
rm -rf /tmp/neovim

printf "%b\n" "Neovim setup completed."
