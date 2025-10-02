# Set the default Finder view to list view
defaults write com.apple.finder FXPreferredViewStyle -string "Nlsv"

# Set default list view settings for new folders
defaults write com.apple.finder FK_StandardViewSettings -dict-add ListViewSettings '{ "columns" = ( { "ascending" = 1; "identifier" = "name"; "visible" = 1; "width" = 300; }, { "ascending" = 0; "identifier" = "dateModified"; "visible" = 1; "width" = 181; }, { "ascending" = 0; "identifier" = "size"; "visible" = 1; "width" = 97; } ); "iconSize" = 16; "showIconPreview" = 0; "sortColumn" = "name"; "textSize" = 12; "useRelativeDates" = 1; }'

# Clear existing folder view settings to force use of default settings
defaults delete com.apple.finder FXInfoPanesExpanded 2>/dev/null || true
defaults delete com.apple.finder FXDesktopVolumePositions 2>/dev/null || true

# Set list view for all view types
defaults write com.apple.finder FK_StandardViewSettings -dict-add ExtendedListViewSettings '{ "columns" = ( { "ascending" = 1; "identifier" = "name"; "visible" = 1; "width" = 300; }, { "ascending" = 0; "identifier" = "dateModified"; "visible" = 1; "width" = 181; }, { "ascending" = 0; "identifier" = "size"; "visible" = 1; "width" = 97; } ); "iconSize" = 16; "showIconPreview" = 0; "sortColumn" = "name"; "textSize" = 12; "useRelativeDates" = 1; }'

# Sets default search scope to the current folder
defaults write com.apple.finder FXDefaultSearchScope -string "SCcf"

# Remove trash items older than 30 days
defaults write com.apple.finder "FXRemoveOldTrashItems" -bool "true"

# Show all filename extensions
defaults write NSGlobalDomain AppleShowAllExtensions -bool true

# Set the sidebar icon size to small
# defaults write NSGlobalDomain NSTableViewDefaultSizeMode -int 1

# Show status bar in Finder
defaults write com.apple.finder ShowStatusBar -bool true

# Show path bar in Finder
defaults write com.apple.finder ShowPathbar -bool true

# Clean up Finder's sidebar
defaults write com.apple.finder SidebarDevicesSectionDisclosedState -bool true
defaults write com.apple.finder SidebarPlacesSectionDisclosedState -bool true
defaults write com.apple.finder SidebarShowingiCloudDesktop -bool false

# Restart Finder to apply changes
killall Finder
