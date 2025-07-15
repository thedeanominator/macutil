#!/bin/sh -e

. ../common-script.sh

cleanup_system() {
    printf "%b\n" "${YELLOW}Performing system cleanup...${RC}"
    # Fix Missions control to NEVER rearrange spaces
    printf "%b\n" "${CYAN}Fixing Mission Control to never rearrange spaces...${RC}"
    $ESCALATION_TOOL defaults write com.apple.dock mru-spaces -bool false

    # Apple Intelligence Crap
    $ESCALATION_TOOL defaults write com.apple.CloudSubscriptionFeatures.optIn "545129924" -bool "false"

    # Empty Trash
    printf "%b\n" "${CYAN}Emptying Trash...${RC}"
    $ESCALATION_TOOL rm -rf ~/.Trash/*

    # Remove old log files
    printf "%b\n" "${CYAN}Removing old log files...${RC}"
    find /var/log -type f -name "*.log" -mtime +30 -exec $ESCALATION_TOOL rm -f {} \;
    find /var/log -type f -name "*.old" -mtime +30 -exec $ESCALATION_TOOL rm -f {} \;
    find /var/log -type f -name "*.err" -mtime +30 -exec $ESCALATION_TOOL rm -f {} \;
    
}

checkEnv
cleanup_system