printf "%b\n" "${YELLOW}Reducing motion and animations on macOS...${RC}"

# Reduce motion in Accessibility settings (most effective)
defaults write com.apple.universalaccess reduceMotion -bool true

# Disable window animations
defaults write NSGlobalDomain NSAutomaticWindowAnimationsEnabled -bool false

# Speed up window resize animations
defaults write NSGlobalDomain NSWindowResizeTime -float 0.001

# Disable smooth scrolling
defaults write NSGlobalDomain NSScrollAnimationEnabled -bool false

# Disable animation when opening and closing windows
defaults write NSGlobalDomain NSAutomaticWindowAnimationsEnabled -bool false

# Disable animation when opening a Quick Look window
defaults write -g QLPanelAnimationDuration -float 0

# Disable animation when opening the Info window in Finder
defaults write com.apple.finder DisableAllAnimations -bool true

# Speed up Mission Control animations
defaults write com.apple.dock expose-animation-duration -float 0.1

# Speed up Launchpad animations
defaults write com.apple.dock springboard-show-duration -float 0.1
defaults write com.apple.dock springboard-hide-duration -float 0.1

# Disable dock hiding animation
defaults write com.apple.dock autohide-time-modifier -float 0
defaults write com.apple.dock autohide-delay -float 0

# Disable animations in Mail.app
defaults write com.apple.mail DisableReplyAnimations -bool true
defaults write com.apple.mail DisableSendAnimations -bool true

# Disable zoom animation when focusing on text input fields
defaults write NSGlobalDomain NSTextShowsControlCharacters -bool true

# Restart Dock to apply changes
killall Dock
