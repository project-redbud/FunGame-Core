# 实体模型

> **面向**：要改 `src/Entity/` 的开发者。
> **规则本身**见文档站：[角色系统](https://project-redbud.github.io/guide/characters)、[道具](https://project-redbud.github.io/guide/items)。
> **测试**：[`tests/FunGame.Core.Tests.Entities`](../../tests/FunGame.Core.Tests.Entities)（49 个用例）

## 本模块文档

| 文档 | 讲什么 |
| --- | --- |
| [属性系统](attributes.md) | 两段式属性（`Initial*` → `Base*` → 最终值）、`Ex*` 三种形态、哪些可写 |
| [生命周期](lifecycle.md) | 构造、等级、三种 `Recovery`、复制与复活、装备写入路径 |
| [相等性语义](equality.md) | 三类 `Equals`（Guid / Id / `GetIdName`）与由此产生的容器行为 |
| [物品与用户](items-and-users.md) | `Item` 使用次数与删除语义、`SkillGroup`、`User` / `Inventory` |

## 代码地图

| 位置 | 职责 |
| --- | --- |
| `Character/Character.cs` | 属性、战斗引用、等级与回复、装备、`Copy` / `Respawn`、`Equals` |
| `Character/Unit.cs` | 单位（召唤物）子类 |
| `Character/CharacterBuilder.cs` | 按初始数值或模板角色构建角色 |
| `Character/CharacterClass.cs`、`Class.cs`、`SubClass.cs` | 职业、副职业、职业点与天赋 |
| `Character/EquipSlot.cs` | 六个装备槽（**setter 为 `internal`**） |
| `Character/Shield.cs`、`MagicResistance.cs` | 护盾与魔抗分桶 |
| `Character/AssistDetail.cs` | 助攻贡献记录 |
| `Item/Item.cs`、`OpenItem.cs`、`SkillGroup.cs` | 物品、动态物品、物品携带的技能组 |
| `User/User.cs`、`Inventory.cs`、`UserProfile.cs` | 用户、库存（**货币在这里**）、资料 |
| `Statistics/CharacterStatistics.cs`、`GameStatistics.cs` | 战斗统计计数器 |
| `Trade/`、`Explore/`、`System/` | 市场与商店、探索（活动/任务/地区）、房间与赛季等外围实体 |
| `BaseEntity.cs`、`Empty.cs` | 实体基类（`Id` / `Guid` / `Name` / `EntityState` / `GameplayEquilibriumConstant`） |

## 坑速查

| 症状 | 原因 | 处理 |
| --- | --- | --- |
| 新角色血量/攻击比预期低一大截 | `new Character()` 的 `Level` 是 **0**，派生属性按 0 级算 | 显式 `Level = 1`，见[生命周期](lifecycle.md) |
| 角色不回血 | `InitialHR` / `InitialMR` 默认 **0** | 按需设置；0 表示无自然回复 |
| 赋 `ExATK` / `ExDEF` 编译不过 | 它们是**只读派生值** | 写 `ExATK2` / `ExATKPercentage` / `ExDEF2` 等 |
| 改完等级血满了 | `Level` setter 会 `OnAttributeChanged()` + `Recovery()` | 需要保留血量就用 `SetLevel(level, recovery: false)` 或事后回填 |
| `Copy()` 出来血是满的 | `Copy()` 会回满 HP/MP | 要保留比例请自行回填 |
| 直接给 `EquipSlot.Weapon` 赋值编译不过 | 槽位 setter 是 `internal` | 走 `Character.Equip(...)` |
| 从工厂/JSON 造出来的技能，特效里 `GamingQueue` 是 `null` | `Character.GamingQueue` **不会**传播给技能与特效 | 见[生命周期](lifecycle.md)的传播一节 |

## 相关测试

| 测试类 | 守什么 |
| --- | --- |
| `CharacterAttributeTests` | `MaxHP` / `ATK` / `DEF` / `PDR` / 暴击闪避 / 系数钳制 / HP·MP·EP 钳制 / 魔抗与护盾分桶 |
| `CharacterProgressionTests` | 等级钳制与回复、`Recovery` 三种语义、`Copy`、装备与卸下、普攻与武器联动 |
| `EntityEqualityTests` | 三类 `Equals` 语义与 `GetIdName` |
| `ItemAndInventoryTests` | 物品默认值、使用次数、`SkillGroup`、库存货币与主角色回退 |
| `TradeAndExploreTests` | 市场合并、商店价格、商品限购、活动领奖、地区天气、任务 |
