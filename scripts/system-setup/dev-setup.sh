#!/bin/sh -e
# shellcheck disable=SC2086

. ../common-script.sh

installDepend() {
    ## Check for dependencies.
    DEPENDENCIES='tree multitail tealdeer unzip cmake make jq fd ripgrep automake autoconf rustup python pipx'
    printf "%b\n" "${YELLOW}Installing dependencies...${RC}"
    brew install $DEPENDENCIES
}

checkEnv
installDepend