# FunGame.Core 设计文档

面向**改这个引擎的人**：讲清楚关键代码落在哪、机制为什么这样设计、以及哪些地方容易踩。规则本身的说明（怎么用、怎么设计数值）在[文档站](https://project-redbud.github.io/)，这里不重复。

## 分模块

按功能域划分，与测试项目一一对应：

| 模块 | 源码 | 测试 | 讲什么 |
| :---: | --- | --- | --- |
| [队列与回合](queue/README.md) | `src/Model/Queue/`、`Model/Framework/DecisionPoints.cs` | `Tests.Queue` | 行动顺序表、时间流逝、决策点预算、协同行动、回合记录 |
| [战斗结算](combat/README.md) | `Model/Queue/GamingQueue.*.cs` 的结算段、`src/Entity/Skill/` | `Tests.Combat` | 六乘区伤害、技能施放与资源、特效钩子、免疫与护盾 |
| [实体模型](entities/README.md) | `src/Entity/` | `Tests.Entities` | 属性两段式、生命周期与回复、相等性语义、装备与物品 |
| [基础设施](infrastructure/README.md) | `src/Api/`、`src/Library/` | `Tests.Infrastructure` | JSON 与转换器、工厂与模块、常量映射、运行时服务 |

每个模块目录下的 `README.md` 是**该模块的索引**，包含代码地图与「坑速查」——遇到问题先看这张表。

## 这份文档的定位

| | 本目录 | [文档站](https://project-redbud.github.io/) |
| :---: | --- | --- |
| 读者 | 改引擎的人 | 用引擎做游戏/模组的人 |
| 内容 | 关键代码坐标、不变量、设计取舍、坑 | 规则书、开发者指南、API 参考 |
| 判据 | **说法一变、代码就得跟着变** | **读者不打开 `src/` 也该能读完** |

同一份说明不要两边各写一遍；新机制先在对应模块下写设计文档，等稳定后再整理进文档站。

## 书写约定

- **引用代码用「文件 + 符号名」为主**，行号只作参考（形如 `GamingQueue.cs:636 AddCharacter()`）。
  行号会随重构漂移，**符号名才是可靠坐标**——发现行号失效，改成符号名即可，不必追着改数字。
- 结论要能验证：给出断言值、公式或对应测试用例名。
- 每个模块的文档末尾都列出**守着它的测试**，改代码前先看那里期望什么。
