<p align="center">
  <img src="https://project-redbud.github.io/hero.svg" width="140" alt="FunGame.Core" />
</p>

<h1 align="center">贡献指南</h1>

<p align="center">
  <b>简体中文</b> · <a href="CONTRIBUTING_EN.md">English</a>
</p>

<p align="center">
  <b>感谢你愿意让 FunGame.Core 变得更好 🌱</b><br>
  <i>这份指南说明本项目的开发约定、代码与测试规范，以及提交与发布流程</i>
</p>

---

## 📖 目录

- [开始之前](#before)
- [我能做什么](#what-can-i-do)
- [开发环境](#setup)
- [项目结构](#layout)
- [构建与测试](#build)
- [代码规范](#style)
- [测试规范](#testing)
- [文档](#docs)
- [提交与 Pull Request](#pr)
- [发布流程](#release)
- [许可证与署名](#license)

---

<a id="before"></a>
## 🤝 开始之前

- **尊重每一位参与者**，只针对代码与结论，不针对人。
- **技术分歧请摆证据**：给出文件路径、行号、复现命令或测试用例，而不是「我觉得」。
- **不确定就先问**：在 issue 里描述你的场景与疑问，比直接提交一个方向不对的 PR 更高效。
- **提交必须签名**：`master` 启用了 *Require signed commits*，未签名的提交会被直接拒绝。
  动手写代码之前先按[提交签名](#signing)把本地配好，免得最后被拦在门外。

---

<a id="what-can-i-do"></a>
## 💡 我能做什么

| 类型 | 说明 | 入口 |
| :---: | --- | --- |
| 🐛 报告缺陷 | 附版本号、最小复现步骤、期望行为与实际行为、异常堆栈 | [Issues](https://github.com/project-redbud/FunGame-Core/issues) |
| 💬 提出建议 | 描述使用场景与期望的 API 形态，欢迎带上伪代码 | [Issues](https://github.com/project-redbud/FunGame-Core/issues) |
| 🔧 提交代码 | 修缺陷、补测试、加机制。请先看[提交与 Pull Request](#pr) | [Pull Requests](https://github.com/project-redbud/FunGame-Core/pulls) |
| 📚 完善文档 | 修正文档错误、补充示例、编写 `docs/` 下的设计文档 | 同 PR |
| 🌍 翻译 | README / 贡献指南等文档的翻译与校对 | 同 PR |
| ⭐ 其他 | Star、在项目中使用并反馈体验，同样有价值 | — |

---

<a id="setup"></a>
## 🛠 开发环境

| 项 | 要求 |
| --- | --- |
| 运行时 | [.NET SDK 10.0.x](https://dotnet.microsoft.com/download)（`dotnet --version` 应 ≥ 10.0） |
| 编辑器 | Visual Studio / Rider / VS Code + C# Dev Kit，任选 |
| 操作系统 | Windows、Linux、macOS 均可 |
| 依赖 | **`src/` 为纯 BCL 实现，禁止引入任何第三方 NuGet 包** |

> **关于零依赖**
>
> 「零依赖」是本项目对使用者的承诺：`dotnet add package FunGame.Core` 之后不会给你的项目带进任何传递依赖。
> 如果你认为某个能力非引入第三方库不可，请先在 issue 中讨论——通常可以用 BCL 实现，或把它放到**调用方**（UI / 服务端 / 模组）里，而不是内核里。
>
> 测试项目不受此限制，可以正常使用 xUnit 等测试框架。

> **关于换行符**
>
> 仓库通过 `.gitattributes` 管理换行符：**`.cs` 文件统一为 CRLF**，其余文件交由 Git 自动归一化。
> 请勿在提交中引入整文件级的换行符变更，这会让 diff 完全不可读。

---

<a id="layout"></a>
## 📁 项目结构

```
FunGame.Core/
├── src/                        # 类库源码（纯 BCL）
│   ├── Api/                    # 对外工具与适配层
│   ├── Controller/             # AIController：AI 决策控制器
│   ├── Entity/                 # 实体模型
│   ├── Interface/              # 契约定义
│   ├── Library/                # 基础设施
│   ├── Model/                  # 引擎核心
│   └── FunGame.Core.csproj
├── tests/                      # 测试（xUnit，按功能域划分）
│   ├── FunGame.Core.Tests.Queue/
│   ├── FunGame.Core.Tests.Combat/
│   ├── FunGame.Core.Tests.Entities/
│   └── FunGame.Core.Tests.Infrastructure/
├── docs/                       # 设计文档
├── FunGame.Core.slnx           # 解决方案
├── build-release-x64.bat       # 构建脚本（仅 src，不含测试）
└── publish-release-x64.bat     # 发布脚本（仅 src，不含测试）
```

### `src/` 各模块职责

| 模块 | 职责 | 典型内容 |
| :---: | --- | --- |
| `Api` | 对外工具与适配 | `Factory`、`JsonService`、`JsonTool`、`NetworkUtility` / `Calculation` 等工具箱、回放渲染与状态推算 |
| `Controller` | AI 决策 | `AIController` |
| `Entity` | 实体模型 | `Character`、`Skill`、`Effect`、`Item`、`User`、`Inventory`、交易与探索相关实体、统计 |
| `Interface` | 契约 | `IGamingQueue`、`IFactory`、`IBaseEntity`、事件与事件处理器 |
| `Library` | 基础设施 | 常量与枚举、异常、`JsonConverter` 家族、模块加载、架构基类 |
| `Model` | 引擎核心 | `Queue`（行动顺序表与结算）、`EffectContext` / `EffectResult`（钩子契约）、`Framework`（决策点、回合记录、平衡常数等数据模型） |

> `Model/Queue/` 下的 `GamingQueue` 是 `partial class`，按职责拆为多个文件：
> `GamingQueue.cs`（回合循环与主流程）、`ChainAction.cs`（协同行动）、`EffectDispatch.cs`（特效钩子分发）、
> `ActionExecution.cs`（动作执行与资源结算）、`RoundReward.cs`（回合奖励）。**新增逻辑请放进对应的分部文件**。

### `tests/` 各项目职责

| 项目 | 覆盖范围 |
| :---: | --- |
| `FunGame.Core.Tests.Queue` | 行动顺序表、时间流逝、决策点预算与配额、协同行动、队伍与回合记录 |
| `FunGame.Core.Tests.Combat` | 六乘区伤害、免疫 / 护盾 / 吸血 / 能量链路、技能施放与资源结算、特效钩子、普攻 |
| `FunGame.Core.Tests.Entities` | 角色派生属性与钳制、等级与回复、装备、实体相等性语义、物品与用户、交易与探索 |
| `FunGame.Core.Tests.Infrastructure` | 工具方法与计算、JSON 序列化与转换器、工厂、常量映射、异常、事件中心、回合记录渲染 |

> 测试按**功能域**划分，而不是照搬 `src/` 的物理分层——一个功能域的用例应当能在一处读完。

---

<a id="build"></a>
## 🔨 构建与测试

```powershell
git clone https://github.com/project-redbud/FunGame-Core.git
cd FunGame-Core

# 构建解决方案（源码 + 全部测试项目）
dotnet build FunGame.Core.slnx -c Release

# 运行全部测试
dotnet test FunGame.Core.slnx -c Release
```

常用变体：

```powershell
# 只跑某一个功能域
dotnet test tests/FunGame.Core.Tests.Combat/FunGame.Core.Tests.Combat.csproj -c Release

# 只跑匹配名称的用例
dotnet test FunGame.Core.slnx -c Release --filter "FullyQualifiedName~DamagePipelineTests"

# 只构建源码（两个 bat 脚本等价于这一步，且不含测试项目）
dotnet build src/FunGame.Core.csproj -c Release
```

> **产物位置**
>
> `src` 项目通过 `BaseOutputPath` 把输出统一到根目录 `bin/`：
>
> - 程序集：`bin/Release/net10.0/FunGame.Core.dll`（连同 `.xml` 与 `.deps.json`，CI 用它们推 `latest` 分支）
> - NuGet 包：`bin/FunGame.Core.3.0.0-dev-<MMDD>.nupkg`
>
> 由于开启了 `GeneratePackageOnBuild`，**只要构建 `src` 就会顺带打包**——看到 `bin/` 下多出 `.nupkg` 属正常现象。

### 提交前自检

- [ ] `dotnet build FunGame.Core.slnx -c Release` —— **0 警告 0 错误**
- [ ] `dotnet test FunGame.Core.slnx -c Release` —— 全部通过
- [ ] 新增 / 修改的行为有对应用例；修缺陷时，用例能**在修复前的代码上失败**
- [ ] 未引入第三方依赖，未提交 `bin/`、`obj/`、`.vs/` 等产物

---

<a id="style"></a>
## 🎨 代码规范

### 基本约定

| 项 | 约定 |
| --- | --- |
| 目标框架 | `net10.0`，语言特性跟随当前 SDK |
| 缩进 / 换行 | 4 空格缩进；`.cs` 为 CRLF |
| 命名空间 | 以**块级**为主（既有代码），新文件也可用文件级命名空间（见 `Model/EffectResult/`） |
| 类型声明 | 优先 `sealed`；简单数据载体优先 `record` / 主构造函数 |
| 变量声明 | 使用**显式类型**（`Character character = new() { ... };`），不使用 `var` |
| 集合 | 优先集合表达式：`List<Skill> skills = [];`、`[.. source]`、`[]` |
| 控制流 | 优先模式匹配与 `switch` 表达式；早返回，避免深层嵌套 |
| 可空性 | 开启 `Nullable`；`?` 标注不可省略 |

### 文档与注释

- **公共成员必须有中文 XML 文档**（`<summary>` / `<param>` / `<returns>`），项目已开启 `GenerateDocumentationFile`。
- **注释解释「为什么」，不复述「做了什么」**。代码本身已经说明做了什么。
- 涉及机制取舍、刻意为之的反直觉写法、以及与框架约定的耦合时，请写清楚背景——这类注释是本项目最有价值的注释。

<a id="current-state"></a>
### 只描述当前状态

**文档与代码注释一律描述「现在是什么样」，不描述「以前是什么样」。**

最常见的违规写法是：保留旧逻辑的说明，再追加一段新逻辑的说明。两套说法并存，后来者无法判断哪一套
对应于代码的真实行为，只能去翻 `git log` 求证——这是维护中最昂贵的不确定性。
**变更史由 `git log` 与 PR 描述承载，不写进代码与文档。**

| ❌ 不要这样 | ✅ 写成这样 |
| --- | --- |
| `// 原来这里用 ==，判断有误，改成 != 才对` | `// 用 != 是为了让真伤与 IgnoreImmune 都能跳过免疫检定` |
| `// 冷却校验已废弃，现在改为候选列表过滤` | `// 冷却不是校验、而是候选列表过滤：CheckCanCast 只查 MP/EP` |
| `/// <summary>获取价格（新签名，返回可空）</summary>` | `/// <summary>获取价格；未设价时返回 <c>null</c></summary>` |
| 「该限制已修复」「不再是整局只能释放一次」 | 删掉那条限制，直接描述当前行为 |
| 「待定夺」「暂时这样写」「以后要改成 X」 | 写清当前行为与它在设计上的理由；计划另开 issue |

三条落地细则：

- **描述缺陷时写当前行为，不写「应该怎么改」**：尚未修掉的缺陷也是「当前状态」，如实说明它是什么、
  为什么这样，把修复方案放进 issue。文档里留一句「这里是个 BUG」而代码不动，下次读的人就无法判断
  它是已知设计还是漏改。
- **未落地的计划不进仓库**：设计意图、权衡过程、还没做的改动放 issue 或 PR 描述。
  写进 `docs/` 或注释的内容必须已经是事实。
- **确属临时的实现要带 issue 编号**：写成 `// TODO(#123): ...`。没有编号的「临时」等于永久，
  而读者无从判断它是否还成立。

### 好与坏

```csharp
// ❌ 反面示例
// 判断能不能放技能
public static bool ck(Character c, Skill s) {
    if (s.CurrentCD > 0) { return false; }   // 冷却中
    if (s.MPCost > c.MP) { return false; }   // 蓝不够
    return true;
}
```

```csharp
// ✅ 推荐写法
/// <summary>
/// 判断角色能否释放指定技能（仅校验资源与冷却，不校验目标合法性）
/// </summary>
/// <param name="caster">施法者</param>
/// <param name="skill">待释放的技能</param>
public static bool CanCast(Character caster, Skill skill)
{
    if (skill.CurrentCD > 0) return false;
    if (skill.RealMPCost > caster.MP) return false;
    return true;
}
```

### 警告策略

项目在 `csproj` 中仅放行少量无意义警告（`1701;1702;CS1591;CS1587;IDE0130`），**目标是构建保持 0 警告**。

- **禁止使用 `#pragma warning disable` / `#pragma warning restore` 压制警告**，必须解决警告本身。
- 若某类警告**涉及面较广**、逐处修复不现实，可以加入 `csproj` 的 `NoWarn`，但**必须先与项目维护者确认**：
  在 issue 或 PR 中说明警告编号、触发范围与放行理由，得到同意后再改 `csproj`。

---

<a id="testing"></a>
## 🧪 测试规范

### 项目约定

测试使用 **xUnit**，按功能域拆分为四个项目。新增用例请放进已有项目中最匹配的一个；确实需要新功能域时再新建同名前缀项目。

新建测试项目的 `csproj` 模板：

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <RootNamespace>$(MSBuildProjectName)</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../../src/FunGame.Core.csproj" />
  </ItemGroup>

  <!-- xunit 2.x 不会自动注入全局 using，必须显式声明 -->
  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

</Project>
```

### 用例命名

- 类名对应被测类型或场景组，方法名用 `被测对象_期望行为`：

```csharp
[Fact]
public void Physical_FullPenetrationRemovesDefenseReduction()

[Theory]
[InlineData("user@example.com", true)]
[InlineData("user..name@example.com", false)]
public void IsEmail_MatchesMailPattern(string value, bool expected)
```

- 涉及引擎机制的用例，请给出一眼能看懂的夹具与断言值，例如硬直公式 `max(6,3)+2 = 8`、预算 `5 → 3`。

### 全局单例

`Factory.OpenFactory`、`EventCenter.Instance`（以及 `General.GameplayEquilibriumConstant`）是全局可变状态。
触及它们的测试类所在项目需要关闭并行，并在 `finally` 中还原：

```csharp
[assembly: CollectionBehavior(DisableTestParallelization = true)]

[Fact]
public void RegisterFactory_TakesPrecedenceOverFallback()
{
    Factory.EffectFactoryDelegate custom = (id, name, owner, args) => id == 999999 ? new RegisteredEffect(owner) : null;
    Factory.OpenFactory.RegisterFactory(custom);
    try
    {
        Assert.IsType<RegisteredEffect>(Factory.OpenFactory.GetInstance(999999, "", skill, []));
    }
    finally
    {
        Factory.OpenFactory.UnRegisterFactory(custom);
    }
}
```

### 引擎夹具

`GamingQueue` 需要一个**经构造函数注册过角色**的队列（内部统计表由 `InitCharacters` 建立，缺了会在时间流逝时抛异常），
因此本仓库的测试夹具统一约定：

| 夹具 | 用途 |
| :---: | --- |
| `BattleFixture` | 团队队列 4 人（A/B/C/D，D 为敌人），覆盖行动顺序、协同行动、回合记录 |
| `Duel` | 1v1，覆盖伤害计算与伤害应用两个阶段 |
| `TurnBattle` | 可推进回合，覆盖技能施放、资源与冷却 |

构造角色时请设置 `ExActionCoefficient = -1`，以抵消 SPD 带来的行动系数，让硬直断言只反映被测逻辑。

### 修缺陷必须带回归用例

这是本项目的硬要求：**修缺陷的 PR 必须包含一条能覆盖该缺陷的用例**，并且你应当验证过它**在修复前的代码上会失败**。

```
修复前：新的用例 FAIL  →  修复后：该用例 PASS  →  其余用例不受影响
```

如果一条用例在修复前后都通过，那它没有守住任何东西。

### 断言与告警

- 浮点断言请带上精度：`Assert.Equal(8d, hardness, 3)`。
- 让断言带上意图：`Assert.Equal(baseDamage * actor.CritDMG, damage, 3)` 优于写死一个魔数。
- xUnit 分析器会拦截 `Assert.False(collection.Contains(x))`、`Assert.Equal(1, x.Count)` 这类写法，
  请改用 `Assert.DoesNotContain` / `Assert.Single`。**测试项目同样要保持 0 警告。**

---

<a id="docs"></a>
## 📚 文档

文档分两处，用途不同，**同一份说明不要两边各写一遍**：

| 位置 | 定位 | 收录什么 |
| :---: | --- | --- |
| 本仓库 `docs/` | **与实现强耦合的设计文档** | 机制的不变量与公式推导、架构取舍、接入契约、仍在演进中的特性说明 |
| [文档站](https://project-redbud.github.io/)（[仓库](https://github.com/project-redbud/project-redbud.github.io)） | **面向使用者的成品文档** | 规则书 `guide/`、开发者指南 `dev/`、API 参考 `api/`、快速开始 |

判据很简单：**说法一变、代码（或反向）就必须跟着变的，放本仓库 `docs/`** —— 这样才卡得住 PR 评审；
**读者不打开 `src/` 也该能读完的，放文档站**。

- **每篇文档只描述当前状态**，与[代码注释同理](#current-state)：不要在同一篇里并列「旧行为」与「新行为」，
  也不要出现「曾经」「已修复」「不再是」这类变更史叙述。改动机制后请把它改写为当前的正确说法，
  而不是在原句后面补一段解释——历史交给 `git log`。
- `docs/` 按**功能域分目录**（与测试项目一一对应），每个目录用 `README.md` 作为索引：

  | 目录 | 覆盖 |
  | --- | --- |
  | [`docs/queue/`](docs/queue/README.md) | 行动顺序表与时间流逝、决策点、协同行动、回合记录 |
  | [`docs/combat/`](docs/combat/README.md) | 伤害流水线、技能施放、特效钩子、免疫与护盾 |
  | [`docs/entities/`](docs/entities/README.md) | 属性系统、角色生命周期、相等性语义、物品与用户 |
  | [`docs/infrastructure/`](docs/infrastructure/README.md) | JSON 与转换器、工厂与模块、常量映射、运行时服务 |

  每篇文档讲的是**关键代码在哪、以及哪里会踩坑**，不复述规则书。新增文档放进对应模块目录，跨模块的内容放 `docs/` 根目录。
  引用代码时以「文件 + 符号名」为准——**行号会随重构漂移，只是参考**。
- 设计文档与功能同 PR 提交；大特性建议在 PR 描述中给出机制说明、不变量与验证方式，方便评审人快速判断正确性。
- **面向使用者的行为变更**（新增机制、公开 API 变化、平衡常数调整）请同步更新 `README.md` / `README_EN.md`，
  并考虑补进文档站 —— 规则书与 API 参考的补充请在文档站仓库提 PR。
- 文档站用 VitePress 构建（`npm ci` → `npm run docs:build`，推 `main` 由 Actions 发布到 Pages）；
  `README` 顶部的 hero 图取自该站 `public/hero.svg`，**移动或改名该文件会同时打断各仓库的 README**。

---

<a id="pr"></a>
## 📝 提交与 Pull Request

### 分支

| 分支 | 用途 |
| :---: | --- |
| `master` | 主分支，仅接受 PR 合并；**提交必须签名**（见[提交签名](#signing)） |
| `latest` | **由 CI 自动推送编译产物，请勿手动提交** |
| 其他 | 开发分支，命名建议 `feat/xxx`、`fix/xxx`、`refactor/xxx`、`docs/xxx` |

### 提交信息

本项目采用「英文类型前缀 + 简要描述」的写法，并在末尾标注关联的 issue / PR 编号。**描述用什么语言不限**，中文、英文或混写均可：

```
feat: 协同行动 (#192)
fix: 免疫/护盾检定条件写反了；邮件格式和 JSON 数组判定不够鲁棒
refactor: organize repository into src and tests
fix: MOV bound weapon bug
```

常用前缀：`feat` / `fix` / `refactor` / `test` / `docs` / `chore`。
一个提交只做一件事；**不要**把格式化、重命名与逻辑修改混在同一个提交里。

<a id="signing"></a>
### 提交签名

**所有提交都必须经过加密签名并通过验证。** `master` 启用了 GitHub 的 *Require signed commits* 分支保护：
未签名、签名损坏或无法验证的提交，推送与合并都会被**直接拒绝**。

> 这里的「签名」指 **GPG / SSH / Sigstore 加密签名**，不是 `Signed-off-by:` 这种声明式的 DCO 签署——
> 后者只是提交信息里的一行文本，不会让 GitHub 显示 Verified，也过不了这道门禁。

GPG 与 SSH 任选一种，GitHub 都认：

```powershell
# 方式一：GPG
gpg --full-generate-key                    # 生成密钥（RSA 4096 或 ed25519）
gpg --armor --export <KEY_ID>              # 导出公钥，粘到 GitHub → Settings → SSH and GPG keys
git config commit.gpgsign true
git config user.signingkey <KEY_ID>

# 方式二：SSH
git config gpg.format ssh
git config user.signingkey ~/.ssh/id_ed25519.pub   # 同一把公钥也要作为 Signing Key 加到 GitHub
```

容易踩的几点：

- **rebase / amend 会重写提交，签名随之丢失**：打开 `git config --global rebase.gpgSign true`；
  给已有的未签名提交补签用 `git rebase --exec 'git commit --amend --no-edit -S' <base>`。
- **密钥邮箱必须与提交的 Author 邮箱一致**，否则 GitHub 标成 `Unverified`——有签名，但不算数。
- 在 GitHub 网页上产生的提交与合并（在线改文件、解决冲突、Squash / Rebase merge）由 GitHub 用自己的
  web-flow 密钥签名，无需本地配置。
- 自检用 `git log --format='%h %G? %s'`：首列 `G` 才算通过（`N` 无签名、`E` 无法验证、`B` 签名损坏）；
  看详情用 `git log --show-signature`。
- 报 `gpg failed to sign the data` 时，先确认 `gpg --list-secret-keys` 能列出密钥、pinentry 能弹窗；
  无头环境需要预置 `GPG_TTY`，或改用 SSH 签名。

### PR 检查清单

- [ ] PR 描述写清**动机**（解决什么问题）与**做法**（关键取舍）
- [ ] 关联相关 issue（`Fixes #123` / `Refs #123`）
- [ ] 给出**验证方式**：执行的命令与结果（构建输出、测试结果），而不是「应该没问题」
- [ ] 保持单一主题，避免顺带重构无关代码
- [ ] 新增公开 API 已带 XML 文档；破坏性变更在描述中显式标注
- [ ] 文档与注释只描述当前状态——没有把旧逻辑留在原地再补一段新逻辑的写法，也没有「曾经 / 已修复」这类变更史叙述
- [ ] 无第三方依赖、无产物文件、无换行符噪声
- [ ] 所有提交都已签名并为 GitHub 验证通过；未签名的提交会被 `master` 的分支保护拒绝
- [ ] 未新增 `NoWarn`；确需放行时，已在描述中说明并取得维护者同意

> **关于 AI 辅助**
>
> 使用 AI 工具协助编写代码是可以的，但**提交者需对结果负责**：请自行确认逻辑正确、测试通过，
> 并在 PR 描述中如实说明验证方式。未经审阅即提交的生成代码会给评审带来很大负担。

---

<a id="release"></a>
## 🚀 发布流程

发布由维护者执行，贡献者了解即可：

| 工作流 | 触发条件 | 行为 |
| :---: | --- | --- |
| `dotnet.yml` | push / PR 到 `master` | 还原 → 构建解决方案 → 运行全部测试 → 将 `src` 产物推到 `latest` 分支 |
| `release.yml` | push tag `vX.Y.Z` 或 `X.Y.Z`；或手动触发并填写版本号 | 构建 → **测试门禁** → `dotnet pack` → 通过 Trusted Publishing（OIDC）发布到 NuGet |

- 版本号取自 tag；开发版本由 `VersionPrefix` + 日期后缀（`3.0.0-dev-<MMDD>`）生成。
- 非正式 tag（预发布等）会静默跳过发布，不产生失败记录。

---

<a id="license"></a>
## ⚖️ 许可证与署名

本项目采用 **GNU Lesser General Public License v3.0（LGPL-3.0-or-later）**，详见 [LICENSE](LICENSE)。

- 你可以在商业项目与开源项目中自由使用、修改、分发本项目。
- 如果你**修改并重新分发**了代码，需要公开你对 `FunGame.Core` 的修改部分，并继续使用 LGPL 许可证。
- 使用本项目时，请在你的程序或文档中声明：

```
This project uses the FunGame-Core library, which is licensed under the GNU Lesser General Public License version 3.0. More information can be found at https://github.com/project-redbud/FunGame-Core.
```

> **提交即同意**
>
> 你向本项目提交的贡献将同样以 LGPL-3.0-or-later 授权。请确保你有权提交这些代码
> （原创，或来自兼容许可证的代码），且不包含任何第三方专有内容。

---

<p align="center">
  <i>由 <a href="https://github.com/project-redbud">Project Redbud</a> 与贡献者共同维护。</i>
</p>
