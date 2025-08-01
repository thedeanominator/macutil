#!/bin/sh -e

. ../../common-script.sh

# Note that this browser is still in active development
# Previous attempts in using it led to some memory usage issues
# Ideally would not use this in its current state

installZenBrowser() {
    if ! brewprogram_exists zen; then
        printf "%b\n" "${YELLOW}Installing Zen Browser...${RC}"
        brew install --cask zen
        if [ $? -ne 0 ]; then
            printf "%b\n" "${RED}Failed to install Zen Browser. Please check your Homebrew installation or try again later.${RC}"
            exit 1
        fi
        printf "%b\n" "${GREEN}Zen Browser installed successfully!${RC}"
    else
        printf "%b\n" "${GREEN}Zen Browser is already installed.${RC}"
    fi
}

checkEnv
installZenBrowser
