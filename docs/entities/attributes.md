# 属性系统

> 上级：[实体模型](README.md) ｜ 源码：`src/Entity/Character/Character.cs`
> 相关测试：`CharacterAttributeTests`

## 一句话结论

属性是**两段式**的：`Initial*` 是初始设定（可写），`Base*` 由等级与核心属性推导（只读），
最终值是 `Base* + Ex*` 三件套（加值 / 加值2 / 百分比联动）。

```
InitialHP ─┐
Level ─────┼→ BaseHP ─┐
STR ───────┘          ├→ MaxHP = Max(1, BaseHP + ExHP + ExHP2 + ExHP3)
ExHPPercentage ───────┘        （ExHP 来自力量、ExHP3 = BaseHP × ExHPPercentage）
```

## `Ex*` 的三种形态（**记这张表就不会写错**）

| 形态 | 例子 | 可写 | 语义 |
| --- | --- | :---: | --- |
| 固定加值 | `ExATK2`、`ExDEF2`、`ExHP2`、`ExPDR`、`ExCritRate` | ✅ | 直接加到最终值上 |
| 百分比联动 | `ExATKPercentage`、`ExHPPercentage`、`ExSTRPercentage` | ✅ | 经 `Ex*3` 实时反映 `Base*` 的变化 |
| 只读派生 | `ExATK`、`ExATK3`、`ExDEF`、`ExDEF3`、`ExHP`、`ExHP3`、`ExSTR2` | ❌ | 由核心属性或 `Base*` 算出，赋值会编译失败 |

> 想让加成**随基础属性成长**，写 `Ex*Percentage`；想**在施加瞬间锁定**数值，写 `Ex*2`。
> 这个取舍在 `ExampleSuperSkillEffect` 的注释里有完整说明（`src/Library/Module/Example/ExampleSkill.cs`）。

## 常用派生属性速查

| 属性 | 公式要点 |
| --- | --- |
| `MaxHP` | `Max(1, BaseHP + ExHP + ExHP2 + ExHP3)`；`BaseHP` 含 `(Level-1)` 成长与 `STR × STRtoHPFactor` |
| `ATK` / `DEF` | `Base + Ex + Ex2 + Ex3`；`BaseDEF` 含 `STR × STRtoDEFFactor` |
| `PDR` | `Min(1, DEF / (DEF + DEFReductionFactor) + ExPDR)` |
| `SPD` | `InitialSPD + AGI × AGItoSPDMultiplier + ExSPD` |
| `ActionCoefficient` | `Clamp(SPD / SPDUpperLimit + ExActionCoefficient, 0, 0.9)` —— 直接缩小动作硬直 |
| `AccelerationCoefficient` | `Min(1, INT × … + ExAccelerationCoefficient)` —— 缩短吟唱 |
| `CDR` | 冷却缩减，`INT × INTtoCDRMultiplier + ExCDR` |
| `CritRate` / `CritDMG` / `EvadeRate` | 基准值（0.05 / 1.25 / 0.05）+ 敏捷或力量贡献 + `Ex*`，前两者经 `PercentageCheck` 钳到 `[0,1]` |
| `HR` / `MR` | `Max(0, InitialHR + STR × STRtoHRFactor + ExHR)`，同理 `MR` 用 `INT` |

系数全在 `Model/Framework/EquilibriumConstant.cs`，**改系数就是改全局平衡**。

## 钳制一览

| 属性 | 钳制 |
| --- | --- |
| `HP` / `MP` | `[0, MaxHP / MaxMP]` |
| `EP` | `[0, GameplayEquilibriumConstant.MaxEP]` |
| `PhysicalPenetration` / `MagicalPenetration` | `PercentageCheck` → `[0, 1]` |
| `ActionCoefficient` | `[0, 0.9]` |
| `MagicResistance` 分桶 | setter `Min(1, value)` |
| `Level` | setter `[1, MaxLevel]`；**getter 只保证 `>= 0`**（见[生命周期](lifecycle.md)） |

> 钳制发生在 **setter 与派生 getter** 两处：`HP` 由 setter 保证不越界，`MaxHP` 变化时不会自动同步——
> 想让 HP 跟着新上限走，得显式 `Recovery()` 或回填。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `MaxHp_EqualsInitialHpAtLevelOne` / `MaxHp_GrowsWithLevel` | 1 级等于 `InitialHP`；升级按 `LevelToHPFactor + HPGrowthFactor × InitialHP` 增长 |
| `Attack_IncludesExtraBonuses` | `ExATK2 + ExATKPercentage` 组合（100 → 180） |
| `Defense_ProducesPhysicalDamageReduction` | `DEF = DEFReductionFactor` → `PDR = 0.5` |
| `SpeedDerivedCoefficients_AreClamped` | `ActionCoefficient` 上限 0.9 |
| `PenetrationRate_IsClampedIntoPercentageRange` | 穿透越界被钳到 `[0,1]` |
| `HpMpAndEp_AreClampedIntoRange` | 三个资源池的上下界 |
| `MagicResistance_ClampsAndAverages` | 分桶 setter 钳制与 `Avg`（9 桶均值，4 位小数） |
| `Shield_TotalsGroupPhysicalMagicalAndMix` | 护盾三类桶与索引器读写 |
