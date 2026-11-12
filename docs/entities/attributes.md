# 属性系统

> 上级：[实体模型](README.md) ｜ 源码：`src/Entity/Character/Character.cs`
> 相关测试：`CharacterAttributeTests`、`CharacterAggregationTests`、`CharacterPrimaryAttributeTests`；JSON 见 `CharacterAttributeSerializationTests`、`UnitAttributeSerializationTests`

## 一句话结论

HP / MP / STR / AGI / INT / ATK / DEF 七组属性使用同一额外值口径：
`Initial*` 是初始配置，`Base*` 是基础值，`Ex*` 是额外值聚合。
STR / AGI / INT / ATK / DEF 的最终值为 `Base* + Ex*`；资源上限为 `Math.Max(1, BaseHP + ExHP)`、`Math.Max(1, BaseMP + ExMP)`。
`HP` / `MP` 表示当前资源，可读写并受上限钳制。

## `Ex*` 的三种形态（**记这张表就不会写错**）

| 形态 | 例子 | 可写 | 语义 |
| --- | --- | :---: | --- |
| 固定点数 | `ExSTR2`、`ExAGI2`、`ExINT2`、`ExATK2`、`ExDEF2`、`ExHP2`、`ExMP2` | ✅ | 技能和物品提供的点数 |
| 百分比配置 | `ExSTRPercentage`、`ExATKPercentage`、`ExHPPercentage` 等 | ✅ | 相对于对应 `Base*` 的比例 |
| 只读派生与聚合 | `Ex*3`、`Ex*FromEx*`、`ExSTR`、`ExATK`、`ExHP` 等 | ❌ | 百分比派生、核心属性来源或全部额外来源之和 |

`Ex*3 = Base* × Ex*Percentage`，随基础值变化。核心属性的 `Ex* = Ex*2 + Ex*3`。战斗属性的 `Ex*` 还包含额外核心属性提供的只读来源：

| 来源属性 | 公式 |
| --- | --- |
| `ExATKFromExPrimaryAttribute` | `ExPrimaryAttributeValue` |
| `ExDEFFromExSTR` | `ExSTR × GameplayEquilibriumConstant.STRtoDEFFactor` |
| `ExHPFromExSTR` | `ExSTR × GameplayEquilibriumConstant.STRtoHPFactor` |
| `ExMPFromExINT` | `ExINT × GameplayEquilibriumConstant.INTtoMPFactor` |

例如 `ExATK = ExATKFromExPrimaryAttribute + ExATK2 + ExATK3`。
`ExPrimaryAttributeValue` 按主属性读取 `ExSTR` / `ExAGI` / `ExINT`，`None` 回退为力量。
`PrimaryAttributeValue = BasePrimaryAttributeValue + ExPrimaryAttributeValue`。

Character / Unit 的属性 JSON 保存初始配置、成长、固定点数与比例，以及当前资源。
`Base*`、聚合 `Ex*`、百分比派生 `Ex*3`、四个来源属性由配置计算，不写入属性 JSON。
读取当前 HP / MP / EP 时，转换器先暂存，待其余配置完成后回填；见 [JSON 与转换器](../infrastructure/json.md)。

## 常用派生属性速查

| 属性 | 公式要点 |
| --- | --- |
| `MaxHP` | `Max(1, BaseHP + ExHP)`；`BaseHP` 含 `(Level-1)` 成长与 `BaseSTR × STRtoHPFactor` |
| `ATK` / `DEF` | `Base + Ex`；`BaseDEF` 含 `BaseSTR × STRtoDEFFactor` |
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
| `Level` | setter `[1, GameplayEquilibriumConstant.MaxLevel]`；getter 加上 `ExLevel`，若自定义 `MaxLevel > 0` 则再限制上界，最后保证 `>= 0`（见[生命周期](lifecycle.md)） |

> 钳制发生在 **setter 与派生 getter** 两处：`HP` 由 setter 保证不越界，`MaxHP` 变化时不会自动同步——
> 想让 HP 跟着新上限走，得显式 `Recovery()` 或回填。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `MaxHp_EqualsInitialHpAtLevelOne` / `MaxHp_GrowsWithLevel` | 1 级等于 `InitialHP`；升级按 `LevelToHPFactor + HPGrowthFactor × InitialHP` 增长 |
| `Attack_IncludesExtraBonuses` | `ExATK2 + ExATKPercentage` 组合（100 → 180） |
| `CharacterAggregationTests` | 聚合、来源拆分、角色与单位面板、复制复活和魔法卡包 |
| `CharacterPrimaryAttributeTests` | 核心属性点数与正负百分比的传导、非主属性、升级成长、战斗属性加成叠加、资源边界与复制 |
| `CharacterAttributeSerializationTests` | 固定点数与比例字段的读取、派生属性与序列化往返 |
| `UnitAttributeSerializationTests` | Unit 三种 JSON 入口、共享字段口径与嵌套角色往返 |
| `Defense_ProducesPhysicalDamageReduction` | `DEF = DEFReductionFactor` → `PDR = 0.5` |
| `SpeedDerivedCoefficients_AreClamped` | `ActionCoefficient` 上限 0.9 |
| `PenetrationRate_IsClampedIntoPercentageRange` | 穿透越界被钳到 `[0,1]` |
| `HpMpAndEp_AreClampedIntoRange` | 三个资源池的上下界 |
| `MagicResistance_ClampsAndAverages` | 分桶 setter 钳制与 `Avg`（9 桶均值，4 位小数） |
| `Shield_TotalsGroupPhysicalMagicalAndMix` | 护盾三类桶与索引器读写 |
