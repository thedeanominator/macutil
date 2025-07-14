#!/bin/sh -e

. ../common-script.sh

fixfinder () {
    printf "%b\n" "${YELLOW}Applying global theme settings for Finder...${RC}"

    # Set the default Finder view to list view
    printf "%b\n" "${CYAN}Setting default Finder view to list view...${RC}"
    $ESCALATION_TOOL defaults write com.apple.finder FXPreferredViewStyle -string "Nlsv"

    # Show all filename extensions
    printf "%b\n" "${CYAN}Showing all filename extensions in Finder...${RC}"
    $ESCALATION_TOOL defaults write NSGlobalDomain AppleShowAllExtensions -bool true

    # Set the sidebar icon size to small
    printf "%b\n" "${CYAN}Setting sidebar icon size to small...${RC}"
    $ESCALATION_TOOL defaults write NSGlobalDomain NSTableViewDefaultSizeMode -int 1

    # Show status bar in Finder
    printf "%b\n" "${CYAN}Showing status bar in Finder...${RC}"
    $ESCALATION_TOOL defaults write com.apple.finder ShowStatusBar -bool true

    # Show path bar in Finder
    printf "%b\n" "${CYAN}Showing path bar in Finder...${RC}"
    $ESCALATION_TOOL defaults write com.apple.finder ShowPathbar -bool true

    # Clean up Finder's sidebar
    printf "%b\n" "${CYAN}Cleaning up Finder's sidebar...${RC}"
    $ESCALATION_TOOL defaults write com.apple.finder SidebarDevicesSectionDisclosedState -bool true
    $ESCALATION_TOOL defaults write com.apple.finder SidebarPlacesSectionDisclosedState -bool true
    $ESCALATION_TOOL defaults write com.apple.finder SidebarShowingiCloudDesktop -bool false

    # Restart Finder to apply changes
    printf "%b\n" "${GREEN}Finder has been restarted and settings have been applied.${RC}"
    $ESCALATION_TOOL killall Finder
}

checkEnv
fixfinder