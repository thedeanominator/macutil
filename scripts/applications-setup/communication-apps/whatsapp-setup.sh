#!/bin/sh -e

. ../../common-script.sh

installWhatsApp() {
    if ! brewprogram_exists whatsapp; then
        printf "%b\n" "${YELLOW}Installing WhatsApp...${RC}"
        brew install --cask whatsapp
        if [ $? -ne 0 ]; then
            printf "%b\n" "${RED}Failed to install WhatsApp. Please check your Homebrew installation or try again later.${RC}"
            exit 1
        fi
        printf "%b\n" "${GREEN}WhatsApp installed successfully!${RC}"
    else
        printf "%b\n" "${GREEN}WhatsApp is already installed.${RC}"
    fi
}

checkEnv
installWhatsApp