# 物品与用户

> 上级：[实体模型](README.md) ｜ 源码：`src/Entity/Item/`、`User/`、`GamingQueue.ActionExecution.cs`
> 相关测试：`ItemAndInventoryTests`、`ChainActionRegressionTests.ChainItemConsumesTimesAndResources`

## 物品

`Item` 的默认值（`new Item()`）值得记一遍，因为大多数「怎么不生效」都出在这里：

| 属性 | 默认 | 备注 |
| --- | --- | --- |
| `ItemType` | `Others` | 武器/防具/鞋子/饰品/魔法卡包…由子类或配置指定 |
| `Equipable` / `Unequipable` | `true` | 能否装备/卸下 |
| `IsInGameItem` | `true` | **局内可用**（物品技能候选列表要求它为真） |
| `IsPurchasable` / `IsSellable` / `IsTradable` | `true` | 商店与交易开关 |
| `IsReduceTimesAfterUse` | `true` | 是否消耗使用次数 |
| `IsRemoveAfterUse` | `true` | 次数耗尽是否标记删除 |
| `RemainUseTimes` | `0` | **默认 0**——不设直接用会立刻「用完」 |
| `Stackable` / `StackCount` | `false` / `1` | 堆叠 |
| `QualityType` | `White` | 品质 |
| `Skills`（`SkillGroup`） | `Active = null`、`Passives` / `Magics` 空 | `IsActive => Skills.Active != null` |

### 使用次数与删除：两个地方分工

```csharp
// ActionExecution.cs —— 局内使用物品时
item.ReduceTimesAndRemove();
if (item.IsReduceTimesAfterUse && item.RemainUseTimes == 0) character.Items.Remove(item);
```

- `Item.ReduceTimesAndRemove(times = 1)`：扣次数（下限 0），并把 `EntityState` 置 `Deleted`，
  **不动任何容器**。
- **从 `Character.Items` 移除**是**队列**的职责（`GamingQueue.ActionExecution.cs:64-66`）。
  局外（不进战斗）不会遍历角色身上的 `Items`，所以不需要在物品侧做移除。

> 写单元测试时若只调 `ReduceTimesAndRemove()`，看到的是 `EntityState.Deleted`，集合里还在——这是设计，不是 bug。

### 动态物品与工厂

- `OpenItem(long id, string name, Dictionary<string, object> args)`：把动态参数原样带进实体；
- `OpenSkill` / `OpenItem` 由 `Factory` 在未注册具体类型时回退创建，见[工厂与模块](../infrastructure/factory-and-modules.md)；
- `Copy(copyLevel, copyGuid, copyProperty, copyOthers)` 的参数决定副本是否保留身份与等级。

## 用户与库存

| 要点 | 说明 |
| --- | --- |
| `User.Name` | `=> Username`（只读别名），判等用 `Id` |
| 预留 Id | `new User(UserType.Guest)` → `UserSet.GuestUserId`（-5）；`LocalUser` → -6 |
| 构造 | `User(id, username, regTime, lastTime, email, nickName, isAdmin, isOperator, isEnable, gameTime, autoKey)` |
| **货币在哪** | **`User.Inventory.Credits`**（还有 `Materials`），不在 `User` 上 |
| `Inventory.Id` | `=> User.Id`（与用户绑定） |
| `Inventory.MainCharacter` | 未显式设置时返回 `Characters.First()`；集合为空时返回一个**新的空 `Character`**（不是 `null`） |
| `Inventory.Name` | 为空时返回「{Username}的库存」 |
| `Squad` / `Training` | `HashSet<long>` / `Dictionary<long, DateTime>` |
| `User.Profile` / `Statistics` | 只读属性，构造时建好 |

## 坑

1. `RemainUseTimes` 默认 0：动态物品忘了配次数，用一次就被判「耗尽」并移出背包。
2. `IsInGameItem = false` 的物品不会出现在局内物品候选里——上架商品常这么配。
3. `Inventory.MainCharacter` 在空库存时给的是**临时新角色**，不是 `null`；
   想判断「有没有角色」请查 `Characters.Count`，别判 `MainCharacter == null`。
4. 给 `item.Character` 赋值会同步到 `Skills.Active.Character`（`Item.Character` 的 setter 有联动），
   但**不会**传播 `GamingQueue`，见[生命周期](lifecycle.md)的传播一节。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `NewItem_HasExpectedDefaults` | 上表全部默认值 |
| `IsActive_FollowsActiveSkill` | `Skills.Active` 决定 `IsActive` |
| `CharacterAssignment_PropagatesToActiveSkill` | `Character` 联动到主动技能 |
| `ReduceTimesAndRemove_MarksItemDeletedWhenExhausted` | 扣减次数 + `EntityState.Deleted`（**集合里仍在**） |
| `ReduceTimesAndRemove_KeepsItemWhenNotRemovable` | `IsRemoveAfterUse = false` 时只标记，不删除 |
| `Inventory_MainCharacterDefaultsToFirstCharacter` | 有角色时取第一个 |
| `Inventory_Starts…FallsBackToEmptyCharacter` | 空库存返回非 null 空角色 |
| `User_GuestAndLocalFactories_UseReservedIds` | 保留 Id -5 / -6 |
| `ChainActionRegressionTests.ChainItemConsumesTimesAndResources` | 局内真删：次数扣减、MP/EP 扣除、`Items` 里消失 |
