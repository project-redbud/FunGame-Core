# 免疫与护盾

> 上级：[战斗结算](README.md) ｜ 源码：`GamingQueue.cs` 的 `DamageToEnemy` 免疫/护盾段、`Entity/Character/Shield.cs`
> 相关测试：`DamagePipelineTests`

## 一句话结论

免疫是**四个「或」分支**的判定 + 两个「无视」出口；护盾是**按伤害类型分桶**的吸收链，
两者都在伤害的**应用阶段**、扣血之前。真实伤害整段旁路。

## 免疫判定

命中任一分支即免疫：

```
ImmuneType.All          → 免疫一切
ImmuneType.Skilled      → 免疫技能伤害（但免疫不了普通攻击）
ImmuneType.Physical     → 免疫物理伤害
ImmuneType.Magical      → 免疫魔法伤害
```

「无视免疫」有两个来源，二者取**或**：

1. `NormalAttack.IgnoreImmune`：仅对普通攻击生效，按 `All` / `Physical` / `Magical` 匹配；
2. `OnDamageImmuneCheck` 钩子：任何来源都可以无视（返回 `IgnoreImmunity = true`）。

### 跳过条件（语义）

```csharp
if (damageType != DamageType.True && !options.IgnoreImmune)   // 进入免疫检定
```

| 场景 | 是否检定 |
| --- | :---: |
| 物理/魔法伤害，默认 | ✅ |
| `options.IgnoreImmune = true` | ❌ 直接无视 |
| 真实伤害 | ❌ 直接无视 |

> 两个条件用 `&&` 连接：真伤与跳过开关是**两条独立的跳过理由**，任一成立就跳过检定。
> 护盾段同理——`damageType != DamageType.True && options.CalculateShield`（见下）。

## 护盾

`Shield` 把护盾分成三类桶：

| 桶 | 成员 |
| --- | --- |
| 物理 | `Physical` |
| 魔法（按类型） | `None` / `Starmark` / `PurityNatural` / `PurityContemporary` / `Bright` / `Shadow` / `Element` / `Aster` / `SpatioTemporal` |
| 混合 | `Mix`（不计类型，任何伤害都吃） |

外加**特效护盾**：`ShieldOfEffects`（键为特效），每个带自己的 `ShieldType` 与数值，用
`AddShieldOfEffect(soe)` 注册、`CalculateShieldOfEffect(effect, damage)` 结算（扣到 ≤ 0 时自动移除该条目）。
索引器 `Shield[isMagic, magicType]` 用于读写分桶值。

### 结算顺序与跳过

```
BeforeShieldCalculation 钩子（可改伤害、可跳过）
  → 特效护盾（ShieldOfEffects）
  → 类型护盾 Shield[isMagic, magicType]
  → 混合护盾 TotalMix
  → 溢出伤害扣血；破碎时触发 OnShieldBroken / OnShieldNeutralizeDamage
```

```csharp
if (damageType != DamageType.True && options.CalculateShield)   // 进入护盾结算
```

| 场景 | 是否结算护盾 |
| --- | :---: |
| 物理/魔法伤害，默认 | ✅ |
| `options.CalculateShield = false` | ❌ 伤害全额落到 HP |
| 真实伤害 | ❌ 全额落到 HP |

## 坑

1. **`ImmuneType` 是 `[Flags]`，`All` 是独立位**（`1 << 3`），不等于 `Physical | Magical | Skilled`；
   判定写的是 `(enemy.ImmuneType & ImmuneType.All) == ImmuneType.All`，所以只置 `ImmuneType.Physical`
   **不会**顺带免疫魔法。
2. 角色的 `ImmuneType` / `IsUnselectable` 是**汇总结果**，由 `Character.UpdateCharacterState()` 从
   特效写入的 `CharacterImmuneTypes` 等三张表算出——直接给 `character.ImmuneType` 赋值会被下一次
   `UpdateCharacterState()` 覆盖。长期免疫请走特效。
3. 护盾是**吸收**而不是减伤：它按剩余伤害依次扣，扣完为止，超出部分才进 HP。
4. 混盾（`Mix`）不分物理魔法，别拿它当「全面防护」用——它和分类型护盾是累加关系。
5. 真伤不只跳过护盾，也跳过减伤与事后加值：它只走「扣血 → 偷取/EP → 死亡」。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `PhysicalImmunity_BlocksPhysicalDamage` | `ImmuneType.Physical` 挡住物理 |
| `SkilledImmunity_DoesNotBlockNormalAttack` / `BlocksSkillDamage` | 技能免疫对普攻无效、对技能有效 |
| `TrueDamage_IgnoresImmunity` | 真伤 + `ImmuneType.All` + 500 护盾 → 全额 100 进 HP |
| `IgnoreImmuneOption_BypassesImmunity` | `IgnoreImmune = true` 直接穿透物理免疫 |
| `Shield_PartiallyAbsorbsDamage` / `FullyAbsorbsDamageBelowItsValue` | 吸收与溢出、护盾值递减 |
| `Shield_IsSeparatedByDamageAndMagicType` | 物理桶与魔法桶互不干扰 |
| `ShieldCanBeSkippedByOption` | `CalculateShield = false` → 全额伤害、护盾不变 |
| `ShieldOfEffect_IsConsumedAndRemovedWhenBroken`（实体侧） | 特效护盾扣到 0 时条目被移除 |
