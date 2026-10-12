# 相等性语义

> 上级：[实体模型](README.md) ｜ 源码：`BaseEntity.cs` 与各实体的 `Equals` 重写
> 相关测试：`EntityEqualityTests`

## 一句话结论

`Equals` **不是统一的**：三类实体各用一套判据。放到 `HashSet` / `Dictionary` 之前，先确认自己用的是哪一类。

| 判据 | 实体 | 位置 |
| --- | --- | --- |
| **`Guid`** | `Character`、`Item`（及 `OpenItem`）、`Skill`、`NormalAttack` | `Character.cs` / `Item.cs` 的 `Equals` 与 `GetHashCode` |
| **`Id`** | `User`、`MarketItem`、`Offer` | `User.cs` 等 |
| **`GetIdName()`**（`Id + "." + Name`） | `Inventory`、`Class`、`SubClass`、`Market`、`Store`、`Region`、`Quest`、`Activity`、`InteractionPoint` | 各自的 `Equals` |
| **引用相等** | `Effect` | `Effect.cs` 末尾 `ReferenceEquals(this, other)` |

```csharp
public abstract class BaseEntity : IBaseEntity
{
    public virtual Guid Guid { get; set; } = Guid.Empty;   // 基类默认 Guid.Empty！
    public string GetIdName() => Id + "." + Name;
}
```

## 由此产生的容器行为

| 容器 | 影响 |
| --- | --- |
| `Character.Effects`（`HashSet<Effect>`） | 同实例幂等、不同实例叠加——**同名特效可以叠**，见[特效钩子](../combat/effects.md) |
| `Character.Skills` / `Items`（`HashSet`） | 按 `Guid` 去重：`Guid` 相同才算同一个 |
| `Inventory.Characters`（`HashSet`） | 按 `Guid` |
| 队列的 `Dictionary<Character, …>` | 按 `Guid`；**`Guid.Empty` 的两个角色会互相覆盖** |

> **`Guid` 的默认值**：`Character` / `Item` / `Skill` 的构造函数都填了 `Guid.NewGuid()`，
> 所以新实例天然唯一；但 `BaseEntity.Guid` 本身默认是 `Guid.Empty`，
> 手工 `new Character { Guid = Guid.Empty }` 造两个，它们会**相等**。

## 坑

1. **不要用 `==` 判断实体是否同一个**。C# 会走 `operator ==`（若未重载则是引用比较），
   与 `Equals` 的语义可能不一致。判等请显式 `a.Equals(b)`。
2. `Equals(IBaseEntity?)` 是抽象入口，`Equals(object?)` 有对应重写；两者的目标类型不同，
   传错类型会静默返回 `false`。
3. 用 `GetIdName()` 判等的实体（`Inventory` 等），**改 `Name` 就会改变身份**——
   它们既是标识又是展示字段，改名要当心 `HashSet` 里的条目失联。
4. `Skill` / `Item` 复制时是否保留 `Guid` 由 `Copy` 的参数决定（`copyGuid`），
   这直接影响它们在新容器里算不算同一个。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `CharacterEquality_IsBasedOnGuid` | 改 `Guid` 后相等，`GetHashCode` 一致 |
| `ItemEquality_IsBasedOnGuid` | 同上 |
| `UserEquality_IsBasedOnId` | 不同 `Id` 不等，改 `Id` 后相等 |
| `InventoryAndMarket_CompareByIdName` | 同名相等、改名后不等 |
| `GetIdName_JoinsIdAndName` | `42.法杖` |
| `BaseEntity_DefaultsToEmptyGuidAndZeroId` | 基类 `Guid.Empty` vs `Item` 构造已生成 `Guid` |
| `EffectTests.AddToCharacter_KeepsDistinctInstancesApart` | 特效引用相等的实际后果 |
