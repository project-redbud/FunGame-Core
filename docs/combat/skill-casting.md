# 技能施放

> 上级：[战斗结算](README.md) ｜ 源码：`GamingQueue.cs` 的 `ProcessTurnCore` 决策分支、`GamingQueue.ActionExecution.cs`
> 相关测试：`SkillCastTests`、`ChainActionRegressionTests.NormalSkillAndMagicRegression`

## 一句话结论

**只有魔法需要吟唱**：战技与爆发技在「预释放」决策的**同一回合内直接结算**；魔法先进 `Casting` 状态，
下一个回合自动以 `CastSkill` 结算。

## 三条施放路径

| 类型 | 决策动作 | 实际结算 | 关键行 |
| --- | --- | --- | --- |
| 战技 / 爆发技 / 物品 | `PreCastSkill` | **同回合**执行，记为 `CastSkill` / `CastSuperSkill` | `GamingQueue.cs` PreCastSkill 分支 → `ExecuteSkillEffects` |
| 魔法 | `PreCastSkill` | 进入 `CharacterState.Casting`，硬直被替换为 `RealCastTime` | 同分支的吟唱子分支 |
| 魔法（结算） | 自动 | 下一回合状态为 `Casting` → 强制 `CastSkill`，扣 MP、开始冷却 | `GamingQueue.cs:1492` 附近的状态判定 |

> `CharacterActionType.CastSkill` 的语义是「**结算一个已进入吟唱的技能**」（它从 `_castingSkills` 取上下文），
> 不是「瞬时释放一个战技」。直接返回 `CastSkill` 而没先 `PreCastSkill` 会取不到上下文。

## 资源与冷却的时机

| 项目 | 时机 |
| --- | --- |
| MP（魔法） | 开始吟唱**不**扣；**释放时**按 `RealMPCost` 扣 |
| EP（战技/爆发技） | 同回合释放时扣 `RealEPCost`；爆发技默认 `EPCost = 100` |
| 决策点 | 决策阶段扣（战技按 `GetActionPointCost`；协同另有口径） |
| 冷却 | 释放成功后 `skill.CurrentCD = skill.RealCD`（`GamingQueue.ActionExecution.cs`） |

## 冷却与资源：门禁在哪里

这是最容易误判的一处：

| 机制 | 实现位置 | 行为 |
| --- | --- | --- |
| 🚫 冷却 / 资源不足 / 被动 / 已生效 / 本回合已用 | **候选技能列表过滤** | 组装 `TurnContext.Skills` 时就被排除，决策层看不到它 |
| ✅ 资源是否够 | `CheckCanCast(caster, skill, out cost)` | **只**比较 MP/EP，**不校验冷却** |

候选列表的完整过滤条件（`GamingQueue.cs:3337` 一带）：

```csharp
s.Level > 0 && s.SkillType != SkillType.Passive && s.Enable && !s.IsInEffect && s.CurrentCD == 0
  && ((s.SkillType is SuperSkill or Skill) ? s.RealEPCost <= EP : s.RealMPCost <= MP)
```

注意其中两项：

- **`s.Enable`**：技能释放成功后会被置 `false`，**但只在 `RealCD > 0` 时**——
  `ExecuteSkillEffects` 写的是 `if (skill.RealCD > 0) skill.Enable = false;`。
  复位**只发生在该技能冷却归零的那一刻**——`TimeLapse` 里 `if (skill.CurrentCD > 0)` 递减到 `<= 0` 时才
  `skill.Enable = true`（`GamingQueue.cs:970-981`）。所以 `Enable = false` 与「冷却没走完」是绑定的：
  没有冷却的技能不会被置 `false`，也就不会被 `s.Enable` 拦下。
- **装备栏的主动技能会被合并进候选列表**（`AddCharacterEquipSlotSkills`），
  所以「技能列表」不只来自 `Character.Skills`。

> 「本回合已用过」与「冷却还没走完」在候选层面是**同一件事**：两者都由 `Enable` 与 `CurrentCD` 共同拦截。
> 想让一个技能可以连放，就把它的 `RealCD` 做成 0（`CD = 0` 或 `InstantReset = true`），
> 而不是去动 `Enable`——`Enable` 是这套联合门禁的产物，不是可以单独拨的开关。

因此：如果某条链路把技能**直接塞进**施放流程（绕过候选列表），冷却与 `Enable` 都不会拦住它。
写测试或写模组时，**要验证「冷却生效」应断言候选列表里没有该技能**，而不是断言释放失败。

## 目标选择

| 事件 | 用途 |
| --- | --- |
| `SelectSkillEvent` | 选出技能 |
| `SelectSkillTargetsEvent` | 指向性目标（`List<Character>`） |
| `SelectNonDirectionalSkillTargetsEvent` | 非指向性目标（`List<Grid>`，需要地图） |
| `SelectTargetGridEvent` | 单个目标格子 |

非指向性技能还需要 `IsNonDirectional = true`；要允许点在**没有角色的格子**上，
再打开 `AllowSelectNoCharacterGrid`。战棋模式下射程会在确认时**重验**。

### 全选与射程契约

`SelectAllEnemies` / `SelectAllTeammates` 表示全图选取相应阵营的全部合法目标；两者同时启用时取并集。
`CanSelectEnemy`、`CanSelectTeammate`、免疫、可选状态和技能目标谓词仍然生效；自身只在全选友方且
`CanSelectSelf` 为真时纳入。全选优先于目标数量、范围中心与目标周围扩散，不会因扩散带入其他阵营。

`Skill.CastAnywhere` 在魔法或全选声明成立时恒为真，其他技能使用配置值；`IsNonDirectional` 在全选时恒为假。
两个属性保留 setter，子类在构造函数中赋值；全选声明可以按等级或状态动态求值。
`NormalAttack.AttackAnywhere` 也由全选声明推导；`AttackRange` 默认随 `Character.ATR` 变化，可以显式配置。
规划、手动选择、普通行动和协同重验均读取这些有效射程。

全选规划保持完整目标集合，目标绑定只指定规则条件的观察对象；普通与协同决策在执行前重验集合，
遗漏、额外目标或资格变化都会使原方案失效。宿主手动选择返回非空列表表示确认全选，框架按当前合法集合执行；
返回空列表表示取消。JSON、技能复制、角色复制与模板恢复保存目标声明和配置射程，避免把全图派生值固化为配置。

默认位置偏好下，全图指向性技能可复用当前站位的目标方案；安全偏好、风险上限、距离条件或强制移动仍可触发站位评估。
相关回归见 `AllTargetSelectionTests`。

## 吟唱被打断 / 失败

- 中断（`InterruptCasting`）、目标失效、资源不足：**保留原硬直并加 3**（例：原硬直 0.5 → 3.5），
  状态回到 `Actionable`，已付的决策点不返还；协同吟唱另见[协同行动](../queue/chain-action.md)。
- 施法者死亡或离队时会清理吟唱上下文，不会在复活后补放。

## 坑

1. **技能等级必须 > 0** 才会进入候选列表（过滤条件含 `s.Level > 0`）；`SkillType.Passive` 永远不进主动候选。
2. `IsInEffect` 为真的技能不能再次使用（持续型技能靠这个自锁）。
3. **判断「有没有冷却」一律用 `RealCD`，不用 `CD`**：`RealCD` 已经吃过 `InstantReset` 与 `CDR` 两个修正
   （`RealCD => InstantReset ? 0 : Math.Max(0, CD * (1 - CDR))`），而 `Enable` 的复位分支又要求
   `CurrentCD > 0`（`GamingQueue.cs:970-981`）——两者都活在 `RealCD` 的坐标系里。
   释放时的守卫因此写成 `if (skill.RealCD > 0) skill.Enable = false;`，语义是：

   - `RealCD > 0`：`Enable` 置 `false`，冷却归零那一刻复位；
   - `RealCD == 0`：`Enable` 保持 `true`，技能**每回合都能连放**，代价只剩资源。

   `RealCD == 0` 的三种来源是 `CD == 0`（`Skill.CD` 的默认值）、`InstantReset = true`、`CDR` 达到 100%。
   写模组时若既要「无冷却」又要「每回合至多一次」，靠 `CD = 0` 做不到（那是连放），
   得靠 `IsInEffect` 或抬高资源成本。
4. 改 `GamingQueue.GameplayEquilibriumConstant` 会同时影响配额与成本——测试里换实例比改字段安全。
5. 魔法吟唱期间角色的硬直**就是吟唱时间**，不是原硬直；释放后才变回「原硬直 + 动作硬直」。
6. 物品作为技能使用时（`SkillType.Item`）走的是另一条路径：要求 `item.IsActive`、`item.Enable`、`item.IsInGameItem`，
   且消耗的是 `item.Skills.Active` 的成本；使用次数与移除见[物品与用户](../entities/items-and-users.md)。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `InstantSkill_PaysEPAndStartsCooldownInTheSameTurn` | 一次 `PreCastSkill` 即扣 EP、起 CD、记为 `CastSkill` |
| `InstantSkill_IsNotCastWhenEPIsInsufficient` | EP 不足 → 零成本、零冷却 |
| `Magic_EntersCastingWithoutPayingMP` | 吟唱开始：MP 不变、硬直 = `RealCastTime`、状态 `Casting`、扣魔法决策点 |
| `Magic_PaysMPWhenReleasedOnTheNextTurn` | 释放时扣 MP、状态回 `Actionable` |
| `CoolingDownSkill_IsFilteredOutOfTheOfferedSkillList` | **冷却靠候选列表过滤**（决定性的用例） |
| `UnaffordableSkill_IsFilteredOutOfTheOfferedSkillList` | 资源不足同样在候选阶段被滤掉 |
| `SkillWithZeroCooldown_CanBeCastOnConsecutiveTurns` | `CD = 0` → `Enable` 保持 `true`，两回合各扣一次 EP |
| `SkillWithCooldown_StaysDisabledUntilCooldownExpires` | 有 CD → `Enable = false`，当场被候选列表滤掉 |
| `InstantResetSkill_IgnoresItsCooldownAndStaysCastable` | `CD = 100` + `InstantReset` → `RealCD = 0`，守卫必须看 `RealCD` |
| `SuperSkill_IsReleasedImmediatelyAndPaysEP` | 爆发技走 `PreCastSkill` 直接结算 |
| `ChainActionRegressionTests.ChainCastingKeepsTwoDecimalPrecision` | 吟唱时间保留两位小数（3.3333 → 3.33） |
