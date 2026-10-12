# 战斗结算

> **面向**：要改伤害公式、技能施放、特效钩子的开发者。
> **规则本身**见文档站：[伤害计算](https://project-redbud.github.io/guide/damage)、[技能系统](https://project-redbud.github.io/guide/skills)、[特效系统](https://project-redbud.github.io/guide/effects)。
> **测试**：[`tests/FunGame.Core.Tests.Combat`](../../tests/FunGame.Core.Tests.Combat)（58 个用例）

## 本模块文档

| 文档 | 讲什么 |
| --- | --- |
| [伤害流水线](damage-pipeline.md) | 两段式结算：计算阶段（改写/加成/闪避/减伤/暴击）+ 应用阶段（加成/效能/免疫/护盾/偷取/死亡） |
| [技能施放](skill-casting.md) | 战技·爆发技·魔法三种施放路径、资源与冷却的扣费时机 |
| [特效钩子](effects.md) | 钩子体系、回读对象的聚合语义、上下文复用约定、钩子缓存 |
| [免疫与护盾](immunity-and-shield.md) | 免疫判定的四个分支、护盾分桶与特效护盾 |

## 代码地图

| 位置 | 职责 |
| --- | --- |
| `GamingQueue.cs` → `CalculatePhysicalDamage` / `CalculateMagicalDamage` | **计算阶段**：类型改写、加成、闪避、减伤、暴击 |
| `GamingQueue.cs` → `DamageToEnemy` | **应用阶段**：事后加成、魔法效能、免疫、护盾、生命偷取、EP、死亡 |
| `GamingQueue.cs` → `HealToTarget` | 治疗入口（触发 `BeforeHealToTarget` / `AlterHealValueBeforeHealToTarget`） |
| `GamingQueue.cs` → `CheckCanCast` | **仅**校验 MP/EP 是否够 |
| `GamingQueue.EffectDispatch.cs` | 每个钩子一份手写触发器（`TriggerXxx`），负责聚合回读对象 |
| `GamingQueue.ActionExecution.cs` | 动作执行：资源扣除、`CurrentCD = RealCD`、`RealCD > 0` 时置 `Enable = false`、局内删物 |
| `Entity/Skill/Skill.cs` | 技能基类：成本/CD/吟唱/目标选择/`Real*` 派生值 |
| `Entity/Skill/NormalAttack.cs` | 普攻：武器决定硬直与倍率、`SetMagicType` 切换伤害类型 |
| `Entity/Skill/Effect.cs` | 特效基类：50+ 可重写钩子、挂载/驱散/豁免 |
| `Model/EffectContext/`、`Model/EffectResult/` | 钩子入参与回读对象的契约 |
| `Model/Framework/DamageCalculationOptions.cs` | 分步开关与调试记录 |
| `Entity/Character/Shield.cs`、`MagicResistance.cs` | 护盾分桶、魔抗分桶 |

## 坑速查

| 症状 | 原因 | 处理 |
| --- | --- | --- |
| 冷却中的技能照样放出来了 | **冷却不是校验、是候选列表过滤** | 见[技能施放](skill-casting.md)；别指望 `CheckCanCast` 拦 CD |
| 绕过公式直接改血 | `DamageToEnemy` 只做应用阶段 | 走 `CalculatePhysicalDamage` → `DamageToEnemy`，或直接调 `NormalAttack.Attack` |
| 特效加成算进了错误的乘区 | 两段各有一个「加值」钩子 | `AlterExpectedDamageBeforeCalculation`（先）vs `AlterActualDamageAfterCalculation`（后） |
| 钩子观察到的敌人是上一个目标 | `DamageCalculationOptions.Context` 被复用 | 见[特效钩子](effects.md)的复用约定 |
| 真伤被免疫挡下 / 护盾没被跳过 | 跳过条件是 `damageType != True && !flag` | 语义见[免疫与护盾](immunity-and-shield.md) |
| 魔法的 MP 扣费时机不对 | 开始吟唱不扣、释放时扣 | 见[技能施放](skill-casting.md) |
| 给了 `CD = 0` 的技能每回合都能连放 | `Enable` 只在 `RealCD > 0` 时才置 `false` | 见[技能施放](skill-casting.md)：要「每回合至多一次」得靠 `IsInEffect` 或资源成本 |

## 相关测试

| 测试类 | 守什么 |
| --- | --- |
| `DamageCalculationTests` | 减伤、穿透、暴击、闪避、魔抗分桶、伤害类型改写、钩子加成位置 |
| `DamagePipelineTests` | 护盾吸收与分桶、免疫分支、生命偷取、EP 获取、致死与助攻、治疗上限 |
| `SkillCastTests` | 战技/爆发技/魔法的资源与冷却、候选列表过滤、回合归档与统计 |
| `EffectTests` | 特效挂载去重、状态推导、驱散/豁免标志、`SkillSet` 映射 |
| `NormalAttackTests` | 普攻伤害与硬直公式、等级钳制、伤害类型切换 |
