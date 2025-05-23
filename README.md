## 项目简介

`FunGame` 是一套基于 `C#.NET` 设计的回合制游戏服务器端开发框架，旨在简化多人回合制在线游戏的开发流程。

配套解决方案：[FunGameServer](https://github.com/project-redbud/FunGame-Server)（基于 `ASP.NET Core Web API` 的跨平台高性能服务器控制台）

本仓库 `FunGame.Core` 项目是 `FunGame` 框架的核心模块，包含了框架的基础组件。
本项目不局限于服务器端开发，在支持 `.NET` 编程的客户端项目中也能使用。

## 安装

- [NuGet](https://www.nuget.org/packages/FunGame.Core/)

```
dotnet add package FunGame.Core
```

- 在 [Release](https://github.com/project-redbud/FunGame-Core/releases) 页面中下载最新发布版本。

- 克隆本仓库。

```powershell
git clone https://github.com/project-redbud/FunGame-Core.git
```

- 克隆本仓库的 `latest` 分支，此分支为开发版本的最新编译 DLL。

```powershell
git clone -b latest https://github.com/project-redbud/FunGame-Core.git
```

## 使用

引用 `FunGame.Core.dll` 或者直接引用整个 `FunGame.Core` 项目到你的项目中。

我们维护了一份 API 文档，如有需要请随时查阅：[FunGame 开发文档](https://project-redbud.github.io/)。
文档内容会随着本项目的更改而变化，但是我们不保证能够及时更新文档。

也欢迎查阅由 `DeepWiki` 提供的 AI 生成的文档：[DeepWiki 文档](https://deepwiki.com/project-redbud/FunGame-Core)，此 AI 工具从头到尾分析了整个项目的代码并组织为 Wiki 形式，方便开发者结合源代码来理解整个项目。

在使用本项目的过程中遇到任何问题，欢迎提交 [issues](https://github.com/project-redbud/FunGame-Core/issues)，我们会积极解决你的问题。

## 许可证

本项目采用 GNU Lesser General Public License v3.0 许可证。 详细信息请参考 [LICENSE](LICENSE) 文件。

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

使用本项目时，你需要在你的程序或文档中声明你使用了 `FunGame.Core`，并说明它使用 LGPL 许可证。

例如：
```
This project uses the FunGame-Core library, which is licensed under the GNU Lesser General Public License version 3.0. More information can be found at https://github.com/project-redbud/FunGame-Core.
```

### 二次开发和衍生项目

GNU Lesser General Public License (LGPL) v3.0 许可证允许你：

- **自由使用**： 在任何类型的项目中使用 `FunGame.Core`，包括商业项目和开源项目。
- **自由修改**： 根据自己的需求修改 `FunGame.Core` 的代码。
- **自由分发**： 分发 `FunGame.Core` 的副本或修改后的版本。

**重要：** 如果你修改或者重新分发了代码，你需要公开你对 `FunGame.Core` 的修改部分（开放源代码），并继续使用 LGPL 许可证。
