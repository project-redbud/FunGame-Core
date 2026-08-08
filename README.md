<p align="center">
  <img src="https://project-redbud.github.io/hero.svg" width="140" alt="FunGame.Core" />
</p>

<h1 align="center">FunGame.Core</h1>

<p align="center">
  <b>简体中文</b> · <a href="README_EN.md">English</a>
</p>

<p align="center">
  <b>基于 C#.NET 设计的轻量、可扩展回合制战斗系统类库</b><br>
  <i>行动顺序表 · 决策点 · 六乘区伤害 · 技能与特效系统，开箱即用</i>
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

## 📖 目录

- [项目简介](#introduction)
- [功能特性](#features)
- [快速开始](#quick-start)
- [安装](#installation)
- [使用与文档](#usage)
- [许可证](#license)

---

<a id="introduction"></a>
## 📌 项目简介

`FunGame.Core` 是一套基于 `C#.NET` 设计的轻量、可扩展的回合制战斗系统类库，旨在打造充满策略趣味的战棋回合制游戏。

- **轻量零依赖**：纯 BCL 实现，不依赖任何第三方库，`dotnet add package FunGame.Core` 即可接入任何 .NET 项目。
- **策略深度**：行动顺序表、决策点、六乘区伤害、技能/特效/驱散/免疫等机制开箱即用。
- **接口驱动、极易扩展**：`IGamingQueue` 定义全部契约，继承 `GamingQueue` 即可定制游戏模式；31 个事件覆盖整个游戏循环，UI、AI、网络层可独立介入。
- **实体可动态化**：Factory 工厂 + JSON 配置文件支持免编码动态创建角色、技能、特效与物品，模组化分发游戏内容。
- **数据可观测**：回合记录以不可变快照沉淀，支持即时外发到专用服务器，用于观战、回放与战斗状态重建。

---

<a id="features"></a>
## ✨ 功能特性

| 🎯 完整回合制系统 | 📊 伤害乘区概念 | ⚔️ 灵活技能系统 |
| :---: | :---: | :---: |
| 时间流逝机制的行动顺序表<br>回合内多次行动的决策点 | 六大伤害乘区 · 物理/魔法独立计算<br>暴击/闪避/穿透/护盾完整链路 | 普攻/战技/被动/爆发技/魔法五大类型<br>回合外爆发插队 · 魔法吟唱 |

| ✨ 特效与驱散 | 🧙 角色属性系统 | 🔌 模组与扩展 |
| :---: | :---: | :---: |
| 50+ 特效类型 · 强/弱/临时/特殊驱散<br>免疫与豁免检定 | 力量/敏捷/智力 · 18 项能力值<br>五大角色定位 · 装备物品体系 | 继承 GamingQueue 定制模式<br>Factory + JSON 动态实体 · 数据外发回放 |

---

<a id="quick-start"></a>
## 🚀 快速开始

以下示例创建一个混战队列（`MixGamingQueue`），让两名角色在 AI 控制下自动对战：

```csharp
using FunGame.Core.Entity;
using FunGame.Core.Model.Queue;

// 1. 创建角色
Character player = new()
{
    Name = "角色1",
    InitialHP = 80,
    InitialATK = 20,
    InitialSPD = 120
};

Character enemy = new()
{
    Name = "角色2",
    InitialHP = 60,
    InitialATK = 25,
    InitialSPD = 100
};

// 2. 创建混战队列并初始化行动顺序表
MixGamingQueue queue = new([player, enemy], Console.WriteLine);
queue.InitActionQueue();
queue.SetCharactersToAIControl(cancel: false, [player, enemy]);

// 3. 游戏循环
while (queue.NextCharacter() is Character actor)
{
    if (queue.ProcessTurn(actor)) break;
    queue.TimeLapse();
}
```

更完整的示例（团队模式、自定义角色/技能/特效/物品、事件绑定、即时外发等）请参阅 [使用与文档](#usage) 章节。

---

<a id="installation"></a>
## 📦 安装

### NuGet 包

- [NuGet](https://www.nuget.org/packages/FunGame.Core/)

```
dotnet add package FunGame.Core
```

### 发布版本源码

在 [Release](https://github.com/project-redbud/FunGame-Core/releases) 页面中下载指定发布版本的源代码，并编译为 DLL。

### 克隆仓库

```powershell
git clone https://github.com/project-redbud/FunGame-Core.git
```

### 开发版本（latest 分支）

克隆本仓库的 `latest` 分支，此分支为开发版本的最新编译 DLL。

```powershell
git clone -b latest https://github.com/project-redbud/FunGame-Core.git
```

---

<a id="usage"></a>
## 📚 使用与文档

引用 `FunGame.Core.dll` 或者直接引用整个 `FunGame.Core` 项目到你的项目中。

- 📖 **API 文档**：[FunGame 开发文档](https://project-redbud.github.io/)
  - 文档内容会随着本项目的更改而变化，但是我们不保证能够及时更新文档。
- 🤖 **AI 生成文档**：[DeepWiki 文档](https://deepwiki.com/project-redbud/FunGame-Core)
  - 此 AI 工具从头到尾分析了整个项目的代码并组织为 Wiki 形式，方便开发者结合源代码来理解整个项目。
- 🐛 **问题反馈**：在使用本项目的过程中遇到任何问题，欢迎提交 [issues](https://github.com/project-redbud/FunGame-Core/issues)，我们会积极解决你的问题。

---

<a id="license"></a>
## 📄 许可证

本项目采用 GNU Lesser General Public License v3.0 许可证。详细信息请参考 [LICENSE](LICENSE) 文件。

<details>
<summary><b>📜 许可证声明</b></summary>

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

使用本项目时，你需要在你的程序或文档中声明你使用了 `FunGame.Core`，并说明它使用 LGPL 许可证。

例如：

```
This project uses the FunGame-Core library, which is licensed under the GNU Lesser General Public License version 3.0. More information can be found at https://github.com/project-redbud/FunGame-Core.
```

### 二次开发和衍生项目

GNU Lesser General Public License (LGPL) v3.0 许可证允许你：

- **自由使用**：在任何类型的项目中使用 `FunGame.Core`，包括商业项目和开源项目。
- **自由修改**：根据自己的需求修改 `FunGame.Core` 的代码。
- **自由分发**：分发 `FunGame.Core` 的副本或修改后的版本。

> **重要：** 如果你修改或者重新分发了代码，你需要公开你对 `FunGame.Core` 的修改部分（开放源代码），并继续使用 LGPL 许可证。

---

<p align="center">
  <i>由 <a href="https://github.com/project-redbud">Project Redbud</a> 与贡献者共同维护。</i>
</p>
