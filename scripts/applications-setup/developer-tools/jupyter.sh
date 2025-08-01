#!/bin/sh -e

. ../../common-script.sh

installjupyterlab() {
    if ! brewprogram_exists pulsar; then
        printf "%b\n" "${YELLOW}Installing jupyterlab...${RC}"
        brew install --cask jupyterlab
        if [ $? -ne 0 ]; then
            printf "%b\n" "${RED}Failed to install jupyterlab. Please check your Homebrew installation or try again later.${RC}"
            exit 1
        fi
        printf "%b\n" "${GREEN}Pulsar installed successfully!${RC}"
    else
        printf "%b\n" "${GREEN}Sublime is already installed.${RC}"
    fi

}

checkEnv
installjupyterlab
