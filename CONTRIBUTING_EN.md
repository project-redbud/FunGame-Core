<p align="center">
  <img src="https://project-redbud.github.io/hero.svg" width="140" alt="FunGame.Core" />
</p>

<h1 align="center">Contributing Guide</h1>

<p align="center">
  <a href="CONTRIBUTING.md">简体中文</a> · <b>English</b>
</p>

<p align="center">
  <b>Thank you for making FunGame.Core better 🌱</b><br>
  <i>This guide covers the development conventions, code &amp; test standards, and the submission and release workflow</i>
</p>

---

## 📖 Table of Contents

- [Before You Start](#before)
- [How You Can Help](#what-can-i-do)
- [Development Setup](#setup)
- [Project Layout](#layout)
- [Build &amp; Test](#build)
- [Code Style](#style)
- [Testing Standards](#testing)
- [Documentation](#docs)
- [Commits &amp; Pull Requests](#pr)
- [Release Process](#release)
- [License &amp; Attribution](#license)

---

<a id="before"></a>
## 🤝 Before You Start

- **Respect every participant.** Criticise the code and the conclusion, never the person.
- **Back technical disagreements with evidence**: file paths, line numbers, a reproduction command, or a failing test case — not "I think".
- **Ask when unsure.** Describing your scenario and question in an issue is far cheaper than a pull request heading in the wrong direction.
- **Commits must be signed.** `master` enforces *Require signed commits*, so unsigned commits are rejected outright. Set up [commit signing](#signing) locally before you start writing code, rather than getting blocked at the end.

---

<a id="what-can-i-do"></a>
## 💡 How You Can Help

| Type | What it takes | Where |
| :---: | --- | --- |
| 🐛 Report a bug | Version, minimal reproduction steps, expected vs. actual behaviour, stack trace | [Issues](https://github.com/project-redbud/FunGame-Core/issues) |
| 💬 Suggest a feature | Describe the use case and the API shape you expect — pseudo-code is welcome | [Issues](https://github.com/project-redbud/FunGame-Core/issues) |
| 🔧 Contribute code | Bug fixes, tests, new mechanics. Read [Commits &amp; Pull Requests](#pr) first | [Pull Requests](https://github.com/project-redbud/FunGame-Core/pulls) |
| 📚 Improve docs | Fix mistakes, add examples, write design documents under `docs/` | Same as PR |
| 🌍 Translate | Translate or proofread docs such as the README and this guide | Same as PR |
| ⭐ Anything else | Starring the repo and reporting your experience using it matters too | — |

---

<a id="setup"></a>
## 🛠 Development Setup

| Item | Requirement |
| --- | --- |
| Runtime | [.NET SDK 10.0.x](https://dotnet.microsoft.com/download) (`dotnet --version` should be ≥ 10.0) |
| Editor | Visual Studio / Rider / VS Code + C# Dev Kit — your choice |
| OS | Windows, Linux and macOS all work |
| Dependencies | **`src/` is a pure BCL implementation — third-party NuGet packages are not allowed** |

> **On the zero-dependency promise**
>
> "Zero dependencies" is a commitment to consumers: after `dotnet add package FunGame.Core`, nothing is
> added to your project's transitive dependency graph.
> If you believe a capability genuinely requires a third-party library, please discuss it in an issue first —
> usually it can be done with the BCL, or it belongs in the **caller** (UI / server / mods) rather than in the core.
>
> Test projects are exempt from this rule and may use xUnit and friends freely.

> **On line endings**
>
> Endings are managed via `.gitattributes`: **`.cs` files are normalised to CRLF**, everything else is
> normalised by Git. Please do not include whole-file line-ending changes in a commit — they make the
> diff completely unreadable.

---

<a id="layout"></a>
## 📁 Project Layout

```
FunGame.Core/
├── src/                        # Library source (pure BCL)
│   ├── Api/                    # Public utilities and adapters
│   ├── Controller/             # AIController: AI decision controller
│   ├── Entity/                 # Entity models
│   ├── Interface/              # Contracts
│   ├── Library/                # Infrastructure
│   ├── Model/                  # Engine core
│   └── FunGame.Core.csproj
├── tests/                      # xUnit tests, split by functional domain
│   ├── FunGame.Core.Tests.Queue/
│   ├── FunGame.Core.Tests.Combat/
│   ├── FunGame.Core.Tests.Entities/
│   └── FunGame.Core.Tests.Infrastructure/
├── docs/                       # Design documents
├── FunGame.Core.slnx           # Solution
├── build-release-x64.bat       # Build script (src only, no test projects)
└── publish-release-x64.bat     # Publish script (src only, no test projects)
```

### Responsibilities inside `src/`

| Module | Responsibility | Typical contents |
| :---: | --- | --- |
| `Api` | Public utilities and adapters | `Factory`, `JsonService`, `JsonTool`, `NetworkUtility` / `Calculation` toolboxes, record renderer and state predictor |
| `Controller` | AI decisions | `AIController` |
| `Entity` | Entity models | `Character`, `Skill`, `Effect`, `Item`, `User`, `Inventory`, trade &amp; exploration entities, statistics |
| `Interface` | Contracts | `IGamingQueue`, `IFactory`, `IBaseEntity`, events and event handlers |
| `Library` | Infrastructure | Constants and enums, exceptions, the `JsonConverter` family, module loading, architectural base classes |
| `Model` | Engine core | `Queue` (action queue and resolution), `EffectContext` / `EffectResult` (hook contracts), `Framework` (decision points, round records, equilibrium constants, …) |

> `GamingQueue` under `Model/Queue/` is a `partial class` split by responsibility:
> `GamingQueue.cs` (turn loop and main flow), `ChainAction.cs` (chain actions), `EffectDispatch.cs` (effect hook dispatch),
> `ActionExecution.cs` (action execution and resource settlement), `RoundReward.cs` (round rewards).
> **Put new logic into the matching partial file.**

### What each test project covers

| Project | Scope |
| :---: | --- |
| `FunGame.Core.Tests.Queue` | Action queue, time lapse, decision-point budget &amp; quota, chain actions, teams and round records |
| `FunGame.Core.Tests.Combat` | Six damage zones, immunity / shield / lifesteal / EP chains, skill casting and resource settlement, effect hooks, normal attacks |
| `FunGame.Core.Tests.Entities` | Derived attributes and clamping, level &amp; recovery, equipment, entity equality semantics, items and users, trade and exploration |
| `FunGame.Core.Tests.Infrastructure` | Utility methods and calculations, JSON serialisation and converters, factory, constant mappings, exceptions, event center, round-record rendering |

> Tests are split by **functional domain**, not mirrored from the physical `src/` layout — a single domain
> should be readable in one place.

---

<a id="build"></a>
## 🔨 Build &amp; Test

```powershell
git clone https://github.com/project-redbud/FunGame-Core.git
cd FunGame-Core

# Build the solution (library + all test projects)
dotnet build FunGame.Core.slnx -c Release

# Run every test
dotnet test FunGame.Core.slnx -c Release
```

Useful variants:

```powershell
# Run a single domain
dotnet test tests/FunGame.Core.Tests.Combat/FunGame.Core.Tests.Combat.csproj -c Release

# Run tests matching a name
dotnet test FunGame.Core.slnx -c Release --filter "FullyQualifiedName~DamagePipelineTests"

# Build the library only (what the two .bat scripts do, and they exclude test projects)
dotnet build src/FunGame.Core.csproj -c Release
```

> **Where artefacts land**
>
> `src` uses `BaseOutputPath` to consolidate output into the repository-root `bin/`:
>
> - Assemblies: `bin/Release/net10.0/FunGame.Core.dll` (plus `.xml` and `.deps.json`, which CI pushes to `latest`)
> - NuGet package: `bin/FunGame.Core.3.0.0-dev-<MMDD>.nupkg`
>
> Because `GeneratePackageOnBuild` is enabled, **building `src` also packs the library** — a fresh `.nupkg`
> under `bin/` is expected, not a mistake.

### Checklist before you push

- [ ] `dotnet build FunGame.Core.slnx -c Release` — **0 warnings, 0 errors**
- [ ] `dotnet test FunGame.Core.slnx -c Release` — everything passes
- [ ] New or changed behaviour is covered by tests; a bug-fix test **fails on the pre-fix code**
- [ ] No third-party dependencies, and no `bin/`, `obj/`, `.vs/` artefacts committed

---

<a id="style"></a>
## 🎨 Code Style

### Ground rules

| Item | Convention |
| --- | --- |
| Target framework | `net10.0`; language features follow the current SDK |
| Indentation / endings | 4 spaces; `.cs` files use CRLF |
| Namespaces | Mostly **block-scoped** (existing code); file-scoped is acceptable for new files (see `Model/EffectResult/`) |
| Type declarations | Prefer `sealed`; use `record` / primary constructors for plain data carriers |
| Local declarations | Use **explicit types** (`Character character = new() { ... };`), not `var` |
| Collections | Prefer collection expressions: `List<Skill> skills = [];`, `[.. source]`, `[]` |
| Control flow | Prefer pattern matching and `switch` expressions; return early instead of nesting deeply |
| Nullability | `Nullable` is enabled; `?` annotations are not optional |

### Documentation and comments

- **Public members must carry Chinese XML documentation** (`<summary>` / `<param>` / `<returns>`);
  `GenerateDocumentationFile` is enabled.
- **Comments explain *why*, not *what*.** The code already says what it does.
- When you make a deliberate trade-off, a counter-intuitive choice, or couple to a framework convention,
  write the background down — those are the most valuable comments in this repository.

<a id="current-state"></a>
### Describe the current state only

**Documentation and code comments describe what the code does *now* — never what it used to do.**

The most common violation is keeping the description of the old logic and appending a paragraph about the new
one. With two competing accounts side by side, a reader cannot tell which one matches the code and has to dig
through `git log` to find out — that is the most expensive kind of uncertainty in maintenance.
**Change history belongs to `git log` and the pull request, not to the code or the docs.**

| ❌ Don't | ✅ Do |
| --- | --- |
| `// This used to compare with ==, which was wrong; now it uses !=` | `// != is used so that both true damage and IgnoreImmune skip the immunity check` |
| `// Cooldown validation is deprecated; it is now a candidate-list filter` | `// Cooldown is not a validation but a candidate-list filter: CheckCanCast only checks MP/EP` |
| `/// <summary>Gets the price (new signature, nullable)</summary>` | `/// <summary>Gets the price; returns <c>null</c> when no price is set</summary>` |
| "This limitation has been fixed", "no longer castable only once" | Delete the limitation and describe the current behaviour |
| "TBD", "temporary", "should become X later" | State the current behaviour and its design rationale; file an issue for the plan |

Three practical rules:

- **For a defect, describe the current behaviour — not how it should be fixed.** An unfixed defect is still
  the current state: say what it is and why it is that way, and put the fix in an issue. A stray "this is a
  bug" in the docs with the code left untouched leaves the next reader unable to tell a known design from an
  oversight.
- **Unlanded plans stay out of the repository.** Design intent, trade-off history and work not yet done
  belong in an issue or the PR body. Whatever goes into `docs/` or a comment must already be true.
- **A genuinely temporary implementation carries an issue number**: write `// TODO(#123): ...`.
  Without a number, "temporary" means permanent, and the reader cannot tell whether it still holds.

### Good vs. bad

```csharp
// ❌ Anti-pattern
// Check whether the skill can be cast
public static bool ck(Character c, Skill s) {
    if (s.CurrentCD > 0) { return false; }   // on cooldown
    if (s.MPCost > c.MP) { return false; }   // not enough MP
    return true;
}
```

```csharp
// ✅ Preferred
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

> In this repository the XML documentation itself is written in Chinese, matching the rest of the codebase.
> Keep that convention even for English-speaking contributors — the project is Chinese-first.

### Warning policy

The `csproj` only suppresses a handful of pointless warnings (`1701;1702;CS1591;CS1587;IDE0130`).
**The build must stay at zero warnings.**

- **Suppressing warnings with `#pragma warning disable` / `#pragma warning restore` is not allowed** —
  fix the warning itself.
- If a warning class is **too widespread** to fix case by case, it may be added to the `csproj` `NoWarn`
  list — but **you must confirm with the project maintainer first**. State the warning ID, the scope it
  fires in and the rationale in an issue or pull request, and only touch the `csproj` once approved.

---

<a id="testing"></a>
## 🧪 Testing Standards

### Project conventions

Tests use **xUnit**, split into four functional-domain projects. Add new cases to the closest existing
project; only create a new `FunGame.Core.Tests.*` project when a genuinely new domain appears.

Template for a new test project:

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

  <!-- xunit 2.x does not inject global usings; this line is required -->
  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

</Project>
```

### Naming

- Class names mirror the type or scenario group; method names use `Subject_ExpectedBehaviour`:

```csharp
[Fact]
public void Physical_FullPenetrationRemovesDefenseReduction()

[Theory]
[InlineData("user@example.com", true)]
[InlineData("user..name@example.com", false)]
public void IsEmail_MatchesMailPattern(string value, bool expected)
```

- For engine mechanics, pick fixtures and values a reader can verify at a glance — e.g. the hardness
  formula `max(6,3)+2 = 8`, or a budget going `5 → 3`.

### Global singletons

`Factory.OpenFactory`, `EventCenter.Instance` (and `General.GameplayEquilibriumConstant`) are global mutable
state. Projects whose tests touch them must disable parallelisation and restore state in a `finally`:

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

### Engine fixtures

`GamingQueue` needs characters registered **through the constructor** (the internal statistics table is
built by `InitCharacters`; without it, time lapse throws). The fixtures in this repository follow that rule:

| Fixture | Purpose |
| :---: | --- |
| `BattleFixture` | Four-character team queue (A/B/C/D, D as the enemy) covering the action queue, chain actions and round records |
| `Duel` | 1v1, covering the damage-calculation and damage-application stages |
| `TurnBattle` | Turn-advancing queue covering skill casting, resources and cooldowns |

Set `ExActionCoefficient = -1` when building characters: it cancels the action coefficient derived from SPD,
so hardness assertions reflect only the logic under test.

### Bug fixes must ship with a regression test

This is a hard requirement: **a bug-fix pull request must include a test covering that bug**, and you should
have verified it **fails on the pre-fix code**.

```
before the fix: the new test FAILS  →  after the fix: it PASSES  →  nothing else regresses
```

A test that passes both before and after your fix guards nothing.

### Assertions and warnings

- Always give floating-point assertions a precision: `Assert.Equal(8d, hardness, 3)`.
- Make assertions self-explanatory: `Assert.Equal(baseDamage * actor.CritDMG, damage, 3)` beats a magic number.
- The xUnit analysers reject patterns such as `Assert.False(collection.Contains(x))` and
  `Assert.Equal(1, x.Count)` — use `Assert.DoesNotContain` / `Assert.Single` instead.
  **Test projects also stay at zero warnings.**

---

<a id="docs"></a>
## 📚 Documentation

Documentation lives in two places with different purposes — **never write the same explanation in both**:

| Location | Purpose | What belongs there |
| :---: | --- | --- |
| `docs/` in this repository | **Design documents tightly coupled to the implementation** | Invariants and formula derivations, architectural trade-offs, integration contracts, features still in flux |
| [Documentation site](https://project-redbud.github.io/) ([repo](https://github.com/project-redbud/project-redbud.github.io)) | **Finished, user-facing documentation** | Rules book `guide/`, developer guide `dev/`, API reference `api/`, getting started |

The test is simple: **if changing the wording forces the code to change (or vice versa), it belongs in this
repository's `docs/`** — that is the only place pull-request review can hold it in sync. **If a reader can
finish it without opening `src/`, it belongs on the documentation site.**

- **Every document describes the current state only** — the same rule as for
  [code comments](#current-state). Do not juxtapose the old behaviour with the new one in a single document,
  and avoid "used to", "has been fixed" or "no longer" narratives. When a mechanic changes, rewrite the text
  to the current truth instead of appending an explanation after the old sentence — history goes to `git log`.
- `docs/` is organised as **one directory per functional domain** (mirroring the test projects), each with a
  `README.md` index:

  | Directory | Covers |
  | --- | --- |
  | [`docs/queue/`](docs/queue/README.md) | Action queue and time lapse, decision points, chain actions, round records |
  | [`docs/combat/`](docs/combat/README.md) | Damage pipeline, skill casting, effect hooks, immunity and shields |
  | [`docs/entities/`](docs/entities/README.md) | Attribute system, character lifecycle, equality semantics, items and users |
  | [`docs/infrastructure/`](docs/infrastructure/README.md) | JSON and converters, factory and modules, constant mappings, runtime services |

  Each document explains **where the key code lives and what will bite you** — it does not restate the rules
  book. Add new documents to the matching module directory; cross-cutting ones go in the `docs/` root.
  Cite code as **file + symbol name** — line numbers drift with refactors and are only a hint.
- Design documents ship in the same pull request as the feature; for large features, describe the mechanics,
  the invariants and the verification method in the PR body so reviewers can judge correctness quickly.
- **User-visible behaviour changes** (new mechanics, public API changes, equilibrium-constant tuning) must be
  reflected in `README.md` / `README_EN.md`, and ideally fed into the documentation site — rules-book and
  API-reference additions are pull requests against the documentation repository.
- The documentation site is built with VitePress (`npm ci` → `npm run docs:build`, pushed to `main` and
  published to Pages by Actions). The hero image at the top of every README is served from that site's
  `public/hero.svg` — **moving or renaming it breaks the READMEs of all sibling repositories**.

---

<a id="pr"></a>
## 📝 Commits &amp; Pull Requests

### Branches

| Branch | Purpose |
| :---: | --- |
| `master` | Main branch, accepts merges from pull requests only; **commits must be signed** (see [Commit signing](#signing)) |
| `latest` | **Compiled artefacts pushed by CI — never commit to it manually** |
| Others | Development branches; `feat/xxx`, `fix/xxx`, `refactor/xxx`, `docs/xxx` are good names |

### Commit messages

This project uses an English type prefix plus a short description.
**The description language is not restricted** — Chinese, English or a mix are all fine:

```
feat: 协同行动
fix: 免疫/护盾检定条件写反了；邮件格式和 JSON 数组判定不够鲁棒
refactor: organize repository into src and tests
fix: MOV bound weapon bug
```

Common prefixes: `feat` / `fix` / `refactor` / `test` / `docs` / `chore`.
Commits on a PR branch do not need an issue or PR number. By convention, this repository uses
**squash merge**, which automatically appends the PR number to the final commit title when the PR is
merged, for example `feat: 协同行动 (#192)`. When fixing an existing issue, you may include its number
in the commit message.

One commit does one thing; **never** mix formatting or renames with a logic change.

<a id="signing"></a>
### Commit signing

**Every commit must be cryptographically signed and verified.** `master` enforces GitHub's
*Require signed commits* branch protection: commits that are unsigned, have a broken signature, or cannot be
verified are **rejected outright** on push and on merge.

> "Signed" here means **GPG / SSH / Sigstore** — not a declarative `Signed-off-by:` DCO sign-off, which is
> merely a line of text in the commit message. It does not earn a *Verified* badge on GitHub and will not
> pass this gate.

GPG and SSH both work; pick one:

```powershell
# Option 1: GPG
gpg --full-generate-key                    # generate a key (RSA 4096 or ed25519)
gpg --armor --export <KEY_ID>              # export the public key to GitHub → Settings → SSH and GPG keys
git config commit.gpgsign true
git config user.signingkey <KEY_ID>

# Option 2: SSH
git config gpg.format ssh
git config user.signingkey ~/.ssh/id_ed25519.pub   # the same key must also be registered as a Signing Key on GitHub
```

Things that trip people up:

- **`rebase` / `amend` rewrite commits and lose the signature**: enable
  `git config --global rebase.gpgSign true`; to re-sign existing unsigned commits, use
  `git rebase --exec 'git commit --amend --no-edit -S' <base>`.
- **The key's email must match the commit's author email**, otherwise GitHub labels the commit `Unverified` —
  signed, but it does not count.
- Commits and merges created on GitHub itself (editing a file in the browser, resolving conflicts,
  squash / rebase merge) are signed by GitHub with its own web-flow key; no local setup needed.
- Check with `git log --format='%h %G? %s'`: only `G` in the first column passes
  (`N` unsigned, `E` cannot be verified, `B` broken signature); `git log --show-signature` shows the details.
- If you hit `gpg failed to sign the data`, first confirm `gpg --list-secret-keys` lists your key and that
  pinentry can prompt; headless environments need `GPG_TTY` preset, or switch to SSH signing.

### Pull request checklist

- [ ] The description states the **motivation** (what problem) and the **approach** (key trade-offs)
- [ ] Related issues are linked (`Fixes #123` / `Refs #123`)
- [ ] **Verification is provided**: the commands you ran and their results (build output, test results) —
      not "should be fine"
- [ ] Single topic; avoid unrelated refactors along the way
- [ ] New public APIs carry XML documentation; breaking changes are called out explicitly
- [ ] Documentation and comments describe the current state only — no old logic left in place with a new
      explanation appended, and no "used to" / "has been fixed" change-history narrative
- [ ] No third-party dependencies, no build artefacts, no line-ending noise
- [ ] Every commit is signed and verified by GitHub; unsigned commits are rejected by `master`'s branch protection
- [ ] No new `NoWarn` entries — if one is genuinely needed, it is explained in the description and approved by a maintainer

> **On AI-assisted contributions**
>
> Using AI tools to help write code is fine, but **the submitter is accountable for the result**: confirm the
> logic is correct, the tests pass, and honestly describe how you verified it in the PR description.
> Unreviewed generated code puts a heavy load on reviewers.

---

<a id="release"></a>
## 🚀 Release Process

Releases are performed by maintainers — contributors only need to be aware of the mechanism:

| Workflow | Trigger | Behaviour |
| :---: | --- | --- |
| `dotnet.yml` | push / PR to `master` | Restore → build the solution → run all tests → push `src` artefacts to the `latest` branch |
| `release.yml` | tag `vX.Y.Z` or `X.Y.Z`; or manual dispatch with a version | Build → **test gate** → `dotnet pack` → publish to NuGet via Trusted Publishing (OIDC) |

- The version comes from the tag; development builds use `VersionPrefix` plus a date suffix (`3.0.0-dev-<MMDD>`).
- Non-release tags (pre-releases and the like) are skipped silently rather than reported as failures.

---

<a id="license"></a>
## ⚖️ License &amp; Attribution

This project is licensed under the **GNU Lesser General Public License v3.0 (LGPL-3.0-or-later)** — see [LICENSE](LICENSE).

- You may freely use, modify and distribute it in commercial and open-source projects.
- If you **modify and redistribute** the code, you must publish your modifications and keep the LGPL license.
- When using this project, please state it in your program or documentation:

```
This project uses the FunGame-Core library, which is licensed under the GNU Lesser General Public License version 3.0. More information can be found at https://github.com/project-redbud/FunGame-Core.
```

> **Submitting means agreeing**
>
> Your contributions are licensed under LGPL-3.0-or-later as well. Make sure you have the right to submit the
> code (it is original, or comes from a compatible license) and that it contains no third-party proprietary content.

---

<p align="center">
  <i>Maintained by <a href="https://github.com/project-redbud">Project Redbud</a> and contributors.</i>
</p>
