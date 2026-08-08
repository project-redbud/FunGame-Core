<p align="center">
  <img src="https://project-redbud.github.io/hero.svg" width="140" alt="FunGame.Core" />
</p>

<h1 align="center">FunGame.Core</h1>

<p align="center">
  <a href="README.md">简体中文</a> · <b>English</b>
</p>

<p align="center">
  <b>A lightweight, extensible turn-based battle system library for C#.NET</b><br>
  <i>Action queue · Decision points · Six-zone damage · Skills &amp; effects, out of the box</i>
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/FunGame.Core/"><img alt="NuGet version" src="https://img.shields.io/nuget/v/FunGame.Core.svg?style=flat-square" /></a>
  <a href="https://www.nuget.org/packages/FunGame.Core/"><img alt="NuGet downloads" src="https://img.shields.io/nuget/dt/FunGame.Core.svg?style=flat-square" /></a>
  <a href="LICENSE"><img alt="License" src="https://img.shields.io/github/license/project-redbud/FunGame-Core.svg?style=flat-square" /></a>
  <a href="https://dotnet.microsoft.com/"><img alt=".NET" src="https://img.shields.io/badge/.NET-net10.0-512BD4.svg?style=flat-square" /></a>
  <a href="https://github.com/project-redbud/FunGame-Core/commits/master"><img alt="Last commit" src="https://img.shields.io/github/last-commit/project-redbud/FunGame-Core.svg?style=flat-square" /></a>
  <a href="https://github.com/project-redbud/FunGame-Core/stargazers"><img alt="GitHub stars" src="https://img.shields.io/github/stars/project-redbud/FunGame-Core.svg?style=flat-square" /></a>
  <a href="https://github.com/project-redbud/FunGame-Core/graphs/contributors"><img alt="Contributors" src="https://img.shields.io/github/contributors/project-redbud/FunGame-Core.svg?style=flat-square" /></a>
</p>

---

## 📖 Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Quick Start](#quick-start)
- [Installation](#installation)
- [Usage &amp; Documentation](#usage)
- [License](#license)

---

<a id="introduction"></a>
## 📌 Introduction

`FunGame.Core` is a lightweight and extensible turn-based battle system library built with `C#.NET`, designed to create strategy-rich tactical RPG battle systems.

- **Lightweight, zero dependencies**: Pure BCL implementation with no third-party dependencies — `dotnet add package FunGame.Core` is all you need to plug it into any .NET project.
- **Strategic depth**: Action queue, decision points, six-zone damage, skills, effects, dispel and immunity mechanics — out of the box.
- **Interface-driven, highly extensible**: All contracts are defined by `IGamingQueue`; inherit `GamingQueue` to customize the game mode. 31 events cover the whole game loop, letting UI, AI and network layers plug in independently.
- **Dynamic entities**: The Factory plus JSON configuration files enable code-free creation of characters, skills, effects and items, and modular distribution of game content.
- **Observable data**: Round records are persisted as immutable snapshots and can be instantly delivered to a dedicated server for spectating, replay and battle-state reconstruction.

---

<a id="features"></a>
## ✨ Features

| 🎯 Complete Turn-Based System | 📊 Damage Zones | ⚔️ Flexible Skill System |
| :---: | :---: | :---: |
| Time-lapse action queue with decision points for multiple actions per turn | Six damage zones, independent physical/magic paths, full crit/dodge/pierce/shield pipeline | Five skill types: normal attack, active, passive, ultimate and magic — with out-of-turn ultimate insertion &amp; magic casting |

| ✨ Effects &amp; Dispel | 🧙 Character Attributes | 🔌 Modules &amp; Extensibility |
| :---: | :---: | :---: |
| 50+ effect types with dispel tiers and immunity/exemption checks | STR / AGI / INT core attributes, 18 stats, five role archetypes, equipment &amp; items | Custom modes by inheriting GamingQueue; Factory + JSON dynamic entities; record sink replay |

---

<a id="quick-start"></a>
## 🚀 Quick Start

The following example creates a mix battle queue (`MixGamingQueue`) and lets two characters fight automatically under AI control:

```csharp
using FunGame.Core.Entity;
using FunGame.Core.Model.Queue;

// 1. Create characters
Character player = new()
{
    Name = "Player 1",
    InitialHP = 80,
    InitialATK = 20,
    InitialSPD = 120
};

Character enemy = new()
{
    Name = "Player 2",
    InitialHP = 60,
    InitialATK = 25,
    InitialSPD = 100
};

// 2. Create a mix queue and initialize the action queue
MixGamingQueue queue = new([player, enemy], Console.WriteLine);
queue.InitActionQueue();
queue.SetCharactersToAIControl(cancel: false, [player, enemy]);

// 3. Run the game loop
while (queue.NextCharacter() is Character actor)
{
    if (queue.ProcessTurn(actor)) break;
    queue.TimeLapse();
}
```

For more examples (team mode, custom characters/skills/effects/items, event binding, outbound records, etc.), see the [Usage &amp; Documentation](#usage) section.

---

<a id="installation"></a>
## 📦 Installation

### NuGet Package

- [NuGet](https://www.nuget.org/packages/FunGame.Core/)

```
dotnet add package FunGame.Core
```

### Release Source Code

Download the source code of a specific release from the [Release](https://github.com/project-redbud/FunGame-Core/releases) page and compile it into a DLL.

### Clone the Repository

```powershell
git clone https://github.com/project-redbud/FunGame-Core.git
```

### Development Build (`latest` Branch)

Clone the `latest` branch of this repository — it contains the latest compiled DLL of the development version.

```powershell
git clone -b latest https://github.com/project-redbud/FunGame-Core.git
```

---

<a id="usage"></a>
## 📚 Usage &amp; Documentation

Reference `FunGame.Core.dll`, or include the whole `FunGame.Core` project in your solution.

- 📖 **API Documentation**: [FunGame Docs](https://project-redbud.github.io/)
  - The documentation changes along with the project, but we do not guarantee that it is always up to date.
- 🤖 **AI-Generated Documentation**: [DeepWiki Docs](https://deepwiki.com/project-redbud/FunGame-Core)
  - An AI-generated wiki built from a full analysis of the codebase — great for understanding the project alongside the source code.
- 🐛 **Issues**: Encountered a problem? Feel free to open an [issue](https://github.com/project-redbud/FunGame-Core/issues).

---

<a id="license"></a>
## 📄 License

This project is licensed under the GNU Lesser General Public License v3.0. See the [LICENSE](LICENSE) file for details.

<details>
<summary><b>📜 License Notice</b></summary>

```
Copyright (C) 2023-present Project Redbud and contributors.
Copyright (C) 2022-2023 Milimoe.

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as
published by the Free Software Foundation, either version 3 of the
License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
```

</details>

When using this project, you must declare in your program or documentation that you use `FunGame.Core` and that it is licensed under LGPL.

For example:

```
This project uses the FunGame-Core library, which is licensed under the GNU Lesser General Public License version 3.0. More information can be found at https://github.com/project-redbud/FunGame-Core.
```

### Derivatives and Modifications

The LGPL v3.0 license grants you:

- **Free to use**: Use `FunGame.Core` in any kind of project, including commercial and open-source projects.
- **Free to modify**: Modify the code of `FunGame.Core` to suit your needs.
- **Free to distribute**: Distribute copies or modified versions of `FunGame.Core`.

> **Important:** If you modify or redistribute the code, you must disclose your modifications to `FunGame.Core` (open source) and continue to use the LGPL license.

---

<p align="center">
  <i>Maintained by <a href="https://github.com/project-redbud">Project Redbud</a> and contributors.</i>
</p>
