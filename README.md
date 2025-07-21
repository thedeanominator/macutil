# Chris Titus Tech's MacOS Utility

[![Version](https://img.shields.io/github/v/release/ChrisTitusTech/macutil?color=%230567ff&label=Latest%20Release&style=for-the-badge)](https://github.com/ChrisTitusTech/macutil/releases/latest)
![GitHub Downloads (specific asset, all releases)](https://img.shields.io/github/downloads/ChrisTitusTech/macutil/macutil?label=Total%20Downloads&style=for-the-badge)


> [!NOTE]
> Since the project is still in active development, you may encounter some issues. Please consider [submitting feedback](https://github.com/ChrisTitusTech/macutil/issues) if you do.

## 💡 Usage
To get started, pick which branch you would like to use, then run the command in your terminal:
### Stable Branch (Recommended)
```bash
curl -fsSL https://christitus.com/mac | sh
```
### Dev branch not setup
```bash

```

### CLI arguments

View available options by running:

```bash
macutil --help
```

For installer options:

```bash
curl -fsSL https://christitus.com/mac | sh -s -- --help
```

## ⬇️ Installation

## Configuration

macutil supports configuration through a TOML config file. Path to the file can be specified with `--config` (or `-c`).

Example config:
```toml
# example_config.toml

auto_execute = [
    "Fastfetch",
    "Alacritty",
    "Kitty"
]

skip_confirmation = true
size_bypass = true
```


## 💖 Support

If you find macutil helpful, please consider giving it a ⭐️ to show your support!

## 🛠 Contributing

We welcome contributions from the community! Before you start, please review our [Contributing Guidelines](.github/CONTRIBUTING.md) to understand how to make the most effective and efficient contributions.

[Official macutil Roadmap](https://chris-titus-docs.github.io/macutil-docs/roadmap/)

Docs are now [here](https://github.com/Chris-Titus-Docs/macutil-docs)

## 🏅 Thanks to All Contributors

Thank you to everyone who has contributed to the development of macutil. Your efforts are greatly appreciated, and you're helping make this tool better for everyone!

[![Contributors](https://contrib.rocks/image?repo=ChrisTitusTech/macutil)](https://github.com/ChrisTitusTech/macutil/graphs/contributors)

## 📜 Contributor Milestones

- 2025/07/21: Claude Sonnet 4 makes boilerplate code for the project.