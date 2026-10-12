# 特效钩子

> 上级：[战斗结算](README.md) ｜ 源码：`src/Entity/Skill/Effect.cs`、`GamingQueue.EffectDispatch.cs`、`Api/EffectHookCache.cs`、`Model/EffectResult/`
> 相关测试：`EffectTests`、`DamageCalculationTests.EffectHooks_*`

## 一句话结论

特效 = 一组可重写的钩子 + 一个「挂到谁身上」的归属。框架在每个管线节点构造一次上下文，
依次流经**事件 → 技能分发 → 特效钩子**；钩子的返回值通过 `EffectResult` 里的记录类型聚合。

## 钩子分类（`Effect.cs`）

| 域 | 代表钩子 |
| --- | --- |
| 伤害 | `AlterDamageTypeBeforeCalculation`、`AlterExpectedDamageBeforeCalculation`、`AlterActualDamageAfterCalculation`、`OnApplyDamage`、`AfterDamageCalculation` |
| 检定 | `BeforeEvadeCheck`、`OnEvadedTriggered`、`BeforeCriticalCheck`、`OnCriticalDamageTriggered` |
| 技能施放 | `BeforeSkillCasted`、`OnSkillCasting`、`OnSkillCasted`、`BeforeSkillCastWillBeInterrupted`、`OnSkillCastInterrupted` |
| 持续与时间 | `OnTimeElapsed`、`BeforeApplyRecoveryAtTimeLapsing`、`OnTurnStart` / `OnTurnEnd` |
| 生命周期 | `OnEffectGained`、`OnEffectLost`、`OnAttributeChanged`、`OnOwnerLevelUp` |
| 驱散/免疫/豁免 | `OnDispellingEffect`、`OnEffectIsBeingDispelled`、`OnImmuneCheck`、`OnDamageImmuneCheck`、`OnExemptionCheck` |
| 护盾/偷取/能量 | `BeforeShieldCalculation`、`OnShieldBroken`、`BeforeLifesteal`、`AlterEPAfterDamage` |
| 行动与回合 | `AlterActionTypeBeforeAction`、`AlterHardnessTimeAfterNormalAttack`、`AfterCharacterMove` |
| 回合奖励 | `OnRoundRewardGained` / `Lost` / `Stolen` |

## 回读对象的聚合语义（**最容易写错的地方**）

`Model/EffectResult/` 下的 `readonly record struct` 都带聚合标注，含义固定：

| 标注 | 含义 | 例 |
| --- | --- | --- |
| **SUM** | 各特效的数值累加 | `AlterActualDamageResult.DamageDelta` |
| **OR** | 任一特效返回 `true` 即生效 | `OnEvadedTriggeredResult.IgnoreEvaded`、`OnImmuneCheckResult.IgnoreImmunity` |
| **覆盖后者胜** | 返回非 `null` 时覆盖上下文，后面的特效可再覆盖 | `AlterDamageTypeResult.DamageType`、`AlterHardnessTimeResult.OverrideCheckProtected` |

**返回 `default` 表示不干预**——这是所有结果类型的统一约定，没有副作用。

## 上下文的复用约定

`DamageCalculationOptions.Context`（`internal`）在一次结算内被 `Calculate*Damage` 与 `DamageToEnemy` 共用，
让计算阶段与应用阶段的钩子看到同一份持续更新的快照。复用有明确前提：

> 仅当既有快照的 `Trigger` / `Enemy` 与本次**引用相同**时才复用，否则重建。
> **嵌套伤害（钩子里再造伤害）由调用方把 `Context` 置回 `null`**，使两次结算互不干扰。

如果你在特效里对**另一个目标**发起伤害却复用同一个 options，钩子观察到的 `Enemy` 会是错的。

## 挂载与去重

- `Effect.AddToCharacter(target)`：加进 `Character.Effects`（`HashSet`），**只有首次加入**才触发 `OnEffectGained`；
- `Effect.Equals` 是**引用相等**（`Effect.cs` 末尾 `ReferenceEquals`），而 `Character` / `Item` / `Skill` 用的是 `Guid`。

> 结论：**同一个特效实例重复施加是幂等的；不同实例即使同名同类型也会叠加**。
> 「不可叠加」的效果要在自己的 `OnSkillCasted` 里判断并复用 `this`，参见 `ExampleSuperSkillEffect`。

## 状态与控制效果

`AddEffectTypeToCharacter(character, types)` 会顺带：
1. 写入 `CharacterEffectTypes`；
2. 按 `SkillSet.GetCharacterStateByEffectType` 映射成 `CharacterState` 写入 `CharacterEffectStates`；
3. 调用 `UpdateCharacterState()` 汇总出最终的 `CharacterState` / `IsUnselectable` / `ImmuneType`。

移除时必须**两个都清**：`RemoveEffectTypesFromCharacter` + `RemoveEffectStatesFromCharacter`。
只调前者，角色会一直停留在控制状态里——这是真实踩过的坑。

## 记录与缓存

`Api/EffectHookCache`（`internal`）用反射缓存「某特效类型重写了哪些钩子」，`ScanAllAssemblies()` 在模组加载时预填充。
它的唯一用途是决定**回合记录是否登记该钩子触发**（`RoundRecord.AddEffectTriggered`）。

> **重写与否只影响「记录」，不影响「调用」**：没重写的钩子仍会被调用，只是基类空实现、无事发生、不必入账。
> 所以「回合记录里没有某内置钩子」是正常的性能取舍，不是漏记。

## 坑

1. 把该进「加值②」的效果写成「加值①」，伤害会被减伤/暴击按不同次序放大——先确认自己要在哪个乘区。
2. `RemoveEffectTypesFromCharacter` 不清理 `CharacterEffectStates`（见上）。
3. 用 `SetMagicType(naoe)` 注册的普攻伤害类型改写，优先级由 `NormalAttackOfEffect.Priority` 决定。
4. 特效的 `Skill` 为空时 `RecordEffectTriggeredIfOverridden` 直接返回，动态特效要记得带上宿主技能。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `AddToCharacter_IgnoresDuplicatedInstance` / `KeepsDistinctInstancesApart` | 同实例幂等、不同实例叠加 |
| `EffectType_DrivesCharacterState` / `CharacterStates_FromMultipleEffectsAreMerged` | 状态汇总与两个 `Remove*` 都要调 |
| `ImmuneTypes_AreMergedIntoCharacterAndCleanedUp` | 免疫位汇总与清理后回到 `None` |
| `DispelFlags_FollowDispelType` | 弱/强/临时驱散标志的派生规则 |
| `IsInEffect_FollowsSkillLevelAndTemporaryDispel` | `Level > 0 && !IsBeingTemporaryDispelled` |
| `DamageCalculationTests.EffectHooks_*` | 钩子介入位置与 `TriggerEffects` 开关 |
