#!/bin/sh -e

. ../../common-script.sh

installPulsar() {
    if ! brewprogram_exists pulsar; then
        printf "%b\n" "${YELLOW}Installing Pulsar...${RC}"
        brew install --cask pulsar
        if [ $? -ne 0 ]; then
            printf "%b\n" "${RED}Failed to install Pulsar. Please check your Homebrew installation or try again later.${RC}"
            exit 1
        fi
        printf "%b\n" "${GREEN}Pulsar installed successfully!${RC}"
    else
        printf "%b\n" "${GREEN}Sublime is already installed.${RC}"
    fi

}

checkEnv
installSublime
