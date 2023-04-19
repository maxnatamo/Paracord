<h1 align="center">
  🪢 Paracord
</h1>

> A homebrew HTTP framework, written in .NET Core 7

<div align="center">
  <a href="https://github.com/maxnatamo/Paracord/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/maxnatamo/paracord?style=for-the-badge" />
  </a>
  <a href="https://github.com/maxnatamo/Paracord/blob/main/CONTRIBUTING.md">
    <img src="https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=for-the-badge" />
  </a>
  <br />
  <a href="https://github.com/maxnatamo/Paracord/actions">
    <img src="https://img.shields.io/github/actions/workflow/status/maxnatamo/paracord/continuous.yml?branch=main&label=Build&style=for-the-badge" />
  </a>
  <a href="https://www.nuget.org/packages/Paracord/">
    <img src="https://img.shields.io/nuget/v/Paracord?label=Dev&style=for-the-badge" />
  </a>
  <a href="https://www.nuget.org/packages/Paracord/">
    <img src="https://img.shields.io/nuget/v/Paracord?label=PROD&style=for-the-badge" />
  </a>
</div>

# 🪢 Paracord

**Paracord** is an **open-source** web framework for .NET Core / C#.

Because it's made with .NET Core, it is...
- Cross platform,
- easy to containerize,
- easy to extend,
- and more.

As it stands currently, Paracord is still in very *pre-alpha*, so there's no way, nor would we ever recommend, to use the framework in it's current state.

## 🧯 Installation

To use Paracord in a new project, you can use a pre-built template:

```sh
# Download template
dotnet new install Paracord.Templates

# Create new API project
dotnet new paracord-api -o Paracord.Api
```

To use Paracord in an existing project, you can install it using Nuget:

```sh
dotnet add package Paracord --prerelease
```

## 📝 Contributing

If you want to contribute, great! We'd love your help!

For more in-depth information on contributing to the project and how to get started, see [CONTRIBUTING](CONTRIBUTING.md).