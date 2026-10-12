# 伤害流水线

> 上级：[战斗结算](README.md) ｜ 源码：`GamingQueue.cs` 的 `CalculatePhysicalDamage` / `CalculateMagicalDamage` / `DamageToEnemy`
> 相关测试：`DamageCalculationTests`、`DamagePipelineTests`

## 一句话结论

伤害分**两个阶段**：`Calculate*Damage` 算出「最终伤害值」，`DamageToEnemy` 把它应用到目标身上。
特效钩子插在两者的每一段之间——**放错阶段就是放错乘区**。

## 全流程

```
① 计算阶段  CalculatePhysicalDamage / CalculateMagicalDamage
   伤害类型改写 (changeCount < 3)         AlterDamageTypeBeforeCalculation
   → 加值①（暴击/减伤之前）                AlterExpectedDamageBeforeCalculation
   → 闪避检定（仅 isNormalAttack）          BeforeEvadeCheck → OnEvadedTriggered
   → 减伤                                  CalculateReduction
   → 暴击检定                              BeforeCriticalCheck → OnCriticalDamageTriggered
   ↘ 输出：finalDamage + DamageResult
② 应用阶段  DamageToEnemy
   → 加值②（暴击/减伤之后，真伤跳过）        AlterActualDamageAfterCalculation
   → 魔法效能                              options.IsMagicSkill
   → 免疫                                  OnImmuneCheck / NormalAttack.IgnoreImmune
   → 护盾                                  BeforeShieldCalculation → OnShieldBroken
   → 扣血、生命偷取、EP 获取                BeforeLifesteal / AlterEPAfterDamage
   → 死亡判定                              DeathCalculation
```

## 计算阶段的公式

| 伤害类型 | 减伤 | 备注 |
| --- | --- | --- |
| 物理 | `penetratedDEF = (1 - actor.PhysicalPenetration) * enemy.DEF`<br>`reduction = penetratedDEF / (penetratedDEF + DEFReductionFactor)`<br>`final = expected - expected * PercentageCheck(reduction + enemy.ExPDR)` | `DEFReductionFactor` 默认 240 |
| 魔法 | `MDF = (1 - actor.MagicalPenetration) * enemy.MDF[magicType]`<br>`final = expected - expected * MDF` | 魔抗按**魔法类型分桶**，未设过的桶为 0 |

- **闪避**只对普通攻击生效：`if (isNormalAttack && options.CalculateEvade)`；
- **暴击**：`final *= actor.CritDMG`，返回 `DamageResult.Critical`；`CritRate` 为 0 时必然不暴击；
- 穿透为 1 时减伤归零（`final == expected`）。

## 应用阶段的开关

`DamageCalculationOptions` 是分步遥控器（`Model/Framework/DamageCalculationOptions.cs`）：

| 开关 | 默认 | 关掉会怎样 |
| --- | :---: | --- |
| `TriggerEffects` | `true` | 全部特效钩子不触发（含两个加值段）→ 得到「纯公式值」 |
| `CalculateEvade` / `CalculateCritical` | `true` | 不掷骰，结果确定 |
| `CalculateReduction` | `true` | 不减伤 |
| `CalculateShield` | `true` | 跳过护盾结算 |
| `IgnoreImmune` | `false` | 见[免疫与护盾](immunity-and-shield.md) |

> 需要**确定性**测试时，把 `CalculateEvade` / `CalculateCritical` 关掉，只留公式——用例就不用撒容差。

## 坑

1. **绕过计算阶段等于丢掉全部减伤/暴击**。直接调 `DamageToEnemy` 传入的是一个已经算完的数字，
   它只做应用阶段。想走完整链路请用 `Calculate*Damage` 或 `NormalAttack.Attack`。
2. **`DamageToEnemy` 对 `HP <= 0` 的目标直接返回**，且会把负数伤害归零——别指望它做「复活扣血」之类的怪招。
3. `DamageResult.Evaded` 时后续全部跳过（不加成、不扣盾）。
4. **伤害类型改写有上限 `changeCount < 3`**，防止特效互相改写导致无限递归。
5. `options.ExpectedDamage == 0` 时会被自动填成本次伤害，用于调试记录；重复使用同一个 options 实例时留意这点。
6. 真伤在流水线里是「旁路」：跳过加值②与护盾结算（免疫见下一节）。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `Physical_DefenseReducesDamageAfterPenetration` | 减伤公式与 `DEFReductionFactor` 一致 |
| `Physical_FullPenetrationRemovesDefenseReduction` | 穿透 1 → 伤害不衰减 |
| `Physical_CriticalMultipliesPostReductionDamage` | 暴击乘的是**减伤后**的值 |
| `NormalAttack_CanBeEvaded` / `EvadeCheck_OnlyAppliesToNormalAttacks` | 闪避仅普攻、闪避后伤害为 0 |
| `Magical_UsesResistanceOfTheGivenMagicType` / `Magical_OtherMagicTypesAreUnaffected` | 魔抗分桶 |
| `EffectHooks_AddBonusBeforeReduction` | 加值①先于减伤（差值 = 加值 × (1-减伤率)） |
| `EffectHooks_CanRedirectPhysicalDamageIntoMagical` | 伤害类型改写后走魔法公式（1000 → 750） |
| `EffectHooks_AreIgnoredWhenTriggerEffectsIsOff` | `TriggerEffects = false` 时加值不生效 |
