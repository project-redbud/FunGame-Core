# 角色生命周期

> 上级：[实体模型](README.md) ｜ 源码：`src/Entity/Character/Character.cs`、`CharacterBuilder.cs`
> 相关测试：`CharacterProgressionTests`

## 构造与 0 级陷阱

`Character()` 无参构造会把 `InitialHP / InitialMP / InitialATK / InitialDEF` 填成**平衡常数里的默认值**，
并初始化 `MDF` / `Shield` / `NormalAttack` / `Class` / `Profile`。

```csharp
Character character = new() { InitialHP = 100 };
character.MaxHP;   // 15      ← 不是 100
character.ATK;     // 94.55   ← 不是 100
character.Level;   // 0
```

**原因**：`Level` 的 getter 只做 `Math.Max(0, field + ExLevel)`，**不会**把 0 抬到 1；
而 `BaseHP = InitialHP + (Level-1) × (LevelToHPFactor + HPGrowthFactor × InitialHP) + …`，
在 0 级时 `(Level-1)` 为负，于是 1 级时是 100、0 级时只有 15。
`BaseATK` 同理（`(Level-1) × (LevelToATKFactor + ATKGrowthFactor × InitialATK)` → 减少 5.45）。

> **规则**：手写构造角色后**一定显式设定等级**（`character.Level = 1;`），
> 否则所有派生属性都会按 0 级算。setter 会把值钳进 `[1, MaxLevel]`。

## 等级变更的副作用

```csharp
public int Level
{
    set
    {
        int past = field;
        field = Math.Min(Math.Max(1, value), GameplayEquilibriumConstant.MaxLevel);
        if (past != field + ExLevel)
        {
            OnAttributeChanged();   // 广播特效钩子 + 重置普攻伤害类型
            Recovery();             // ← 回满 HP/MP
        }
    }
}
```

所以**升级会回满血**。要保留当前血量：`SetLevel(level, recovery: false)`，或先记下比例、设置后回填。

## 三种 `Recovery`

| 重载 | 行为 | 典型用途 |
| --- | --- | --- |
| `Recovery(double EP = -1)` | `HP = MaxHP; MP = MaxMP`；传了 `EP` 才覆盖能量 | 出生、升级、复活 |
| `Recovery(int time, double EP = -1)` | `HP += HR × time`、`MP += MR × time`（`time > 0` 才生效） | 时间流逝按回复力回血 |
| `Recovery(pastHP, pastMP, pastMaxHP, pastMaxMP)` | 按**旧比例**回填到新上限 | 上限变化时保持血量百分比 |

> `InitialHR` / `InitialMR` 默认都是 **0**，即默认角色**没有自然回复**。需要回复力必须显式设置。

## 复制与复活

- `Copy(bool copyEx = false, bool copyMagic = false, bool copyItem = false)`：**会回满 HP/MP**（末尾调用 `Recovery()`），
  `Guid` 是否复制取决于参数；要「带着伤」的副本，复制后自行回填。
- `Respawn(Character original)`：从原始角色还原，`Initial*` 与资源状态一并回来。

## `GamingQueue` 的传播（**写模组最容易踩**）

`Character.GamingQueue` 是**普通属性**，不会自动向下传播：

```csharp
character.GamingQueue = queue;          // 只设置了角色自己
skill.GamingQueue;                      // 仍为 null
effect.GamingQueue;                     // 仍为 null
```

队列的 `AddCharacter` 只会设置角色本身。而从工厂 / JSON 造出来的技能，其特效在构造时抓取的
`skill.GamingQueue` 也是 `null`，于是特效钩子里 `GamingQueue?.Map`、`queue.DamageToEnemy(...)` 会静默失效或抛空。

> 手写模组 / 测试夹具请自行接线：`skill.GamingQueue = queue; foreach (var e in skill.Effects) e.GamingQueue = queue;`
> （本仓库的测试夹具就是这样做的，参见 `tests/FunGame.Core.Tests.Combat/CombatFixture.cs` 的 `Grant` 扩展方法。）

## 装备写入路径

`EquipSlot` 六个槽位的 setter 都是 `internal`，只能通过 `Character` 的入口改：

| 方法 | 行为 |
| --- | --- |
| `Equip(item, out Item? previous)` | 按物品自身的 `EquipSlotType` 选槽 |
| `Equip(item, EquipSlotType slot, out Item? previous)` | 指定槽位 |
| `Equip(item)` | 不关心被替换的装备 |
| `UnEquip(EquipSlotType type)` | 卸下并返回原物品 |

装备会驱动普攻：`NormalAttack.Damage` 的倍率与 `HardnessTime` 都由**武器类型**决定（无武器时硬直 10、倍率 1.0）。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `Level_IsClampedToConfiguredRange` | 0 → 1，9999 → `MaxLevel` |
| `LevelChange_RefillsHpAndMp` | 升级后满血满蓝 |
| `Recovery_ByTime_AddsRecoveryRate` | `HR`/`MR` = 1 时，50 → 60 |
| `Recovery_ByPastValues_KeepsHpRatio` | 按旧比例回填（40/100 → 新上限的 40%） |
| `Copy_RefillsHpAndProducesIndependentCharacter` | 副本满血且互不影响 |
| `Equip_PutsItemIntoMatchingSlotAndReturnsPrevious` | 返回被替换装备，自动选槽 |
| `NormalAttack_PicksUpWeaponHardnessAndDamage` | 装备法杖后普攻倍率与硬直改为法杖值 |
