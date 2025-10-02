#!/bin/sh -e

# ---- Install functions -------------------------------------------------------

installChrome() {
    if ! brewprogram_exists google-chrome; then
        printf "%b\n" "${YELLOW}Installing Google Chrome...${RC}"
        brew install --cask google-chrome
        if [ $? -ne 0 ]; then
            printf "%b\n" "${RED}Failed to install Google Chrome Browser. Please check your Homebrew installation or try again later.${RC}"
            exit 1
        fi
        printf "%b\n" "${GREEN}Google Chrome Browser installed successfully!${RC}"
    else
        printf "%b\n" "${GREEN}Google Chrome Browser is already installed.${RC}"
    fi
}

installFirefox() {
    if ! brewprogram_exists firefox; then
        printf "%b\n" "${YELLOW}Installing Mozilla Firefox...${RC}"
        brew install --cask firefox
        if [ $? -ne 0 ]; then
            printf "%b\n" "${RED}Failed to install Firefox Browser. Please check your Homebrew installation or try again later.${RC}"
            exit 1
        fi
        printf "%b\n" "${GREEN}Firefox Browser installed successfully!${RC}"
    else
        printf "%b\n" "${GREEN}Firefox Browser is already installed.${RC}"
    fi
}

installChromium() {
    if ! brewprogram_exists chromium; then
        printf "%b\n" "${YELLOW}Installing Chromium...${RC}"
        brew install --cask chromium
        if [ $? -ne 0 ]; then
            printf "%b\n" "${RED}Failed to install Chromium Browser. Please check your Homebrew installation or try again later.${RC}"
            exit 1
        fi
        printf "%b\n" "${GREEN}Chromium Browser installed successfully!${RC}"
    else
        printf "%b\n" "${GREEN}Chromium Browser is already installed.${RC}"
    fi
}

installVivaldi() {
    if ! brewprogram_exists vivaldi; then
        printf "%b\n" "${YELLOW}Installing Vivaldi...${RC}"
        brew install --cask vivaldi
        if [ $? -ne 0 ]; then
            printf "%b\n" "${RED}Failed to install Vivaldi Browser. Please check your Homebrew installation or try again later.${RC}"
            exit 1
        fi
        printf "%b\n" "${GREEN}Vivaldi Browser installed successfully!${RC}"
    else
        printf "%b\n" "${GREEN}Vivaldi Browser is already installed.${RC}"
    fi
}

# ---- Helper to dispatch by name ---------------------------------------------

install_by_name() {
    case "$1" in
        chrome|google-chrome)   installChrome ;;
        firefox)                installFirefox ;;
        chromium)               installChromium ;;
        vivaldi)                installVivaldi ;;
        all)
            installChrome
            installFirefox
            installChromium
            installVivaldi
            ;;
        *)
            printf "%b\n" "${RED}Unknown option: '$1'${RC}"
            return 1
            ;;
    esac
}

# ---- Interactive menu --------------------------------------------------------

show_menu() {
    printf "\n"
    printf "%b\n" "${CYAN}Select the browser to install:${RC}"
    printf "  1) Google Chrome\n"
    printf "  2) Mozilla Firefox\n"
    printf "  3) Chromium\n"
    printf "  4) Vivaldi\n"
    printf "  5) All of the above\n"
    printf "  q) Quit\n"
    printf "%b"   "${YELLOW}Enter choice: ${RC}"
}

main() {
    

    # Non-interactive mode: accept a single argument (chrome|firefox|chromium|vivaldi|all)
    if [ $# -ge 1 ]; then
        install_by_name "$1"
        exit $?
    fi

    # Interactive menu
    while :; do
        show_menu
        # shellcheck disable=SC2162
        read choice
        case "$choice" in
            1) install_by_name chrome && break ;;
            2) install_by_name firefox && break ;;
            3) install_by_name chromium && break ;;
            4) install_by_name vivaldi && break ;;
            5) install_by_name all && break ;;
            q|Q) printf "%b\n" "${YELLOW}Aborted by user.${RC}"; exit 0 ;;
            *) printf "%b\n" "${RED}Invalid choice. Please try again.${RC}";;
        esac
    done
}

main "$@"
