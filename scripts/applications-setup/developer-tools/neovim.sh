#!/bin/sh -e

. ../../common-script.sh

installNeovim() {
    print_info "Setting up Neovim..."
    
    # Install Neovim and dependencies with brew
    print_info "Installing Neovim and dependencies..."
    brew install neovim ripgrep fzf
    
    # Backup existing config if it exists
    if [ -d "$HOME/.config/nvim" ] && [ ! -d "$HOME/.config/nvim-backup" ]; then
        print_info "Backing up existing Neovim config..."
        cp -r "$HOME/.config/nvim" "$HOME/.config/nvim-backup"
    fi
    
    # Clear existing config
    rm -rf "$HOME/.config/nvim"
    mkdir -p "$HOME/.config/nvim"
    
    # Clone Titus kickstart config directly to .config/nvim
    print_info "Applying Titus Kickstart config..."
    git clone --depth 1 https://github.com/ChrisTitusTech/neovim.git /tmp/neovim
    cp -r /tmp/neovim/titus-kickstart/* "$HOME/.config/nvim/"
    rm -rf /tmp/neovim
    print_success "Neovim setup completed."
}

checkEnv
installNeovim