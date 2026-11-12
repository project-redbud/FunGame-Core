# 测试文件与用例索引

> 上级：[设计文档](README.md)

本索引对应当前解决方案的四个 xUnit 项目，共 553 个运行用例、52 个测试类。包含测试用例的 C# 源文件共 44 个，另有 4 个夹具与辅助文件，共 48 个测试目录下的 C# 文件；不计 bin / obj 生成文件。

## 统计口径

- 运行用例数采用 VSTest / xUnit 实际发现并执行的数量。一个 Fact 通常算一个用例；Theory 按每组数据展开，方法数与运行用例数分别统计。
- 一个文件可以包含多个测试类；partial 测试类也可以分布在多个文件。表中同一测试类的用例数只计一次，文件数按路径去重。
- 夹具、辅助类及 JSON 场景配置不单独算运行用例。同一测试方法内循环验证多个随机种子，或执行多条断言，也不直接增加 xUnit 运行用例数。
- 新增、删除、移动测试或修改数据行时，同步维护本页、模块索引和正文中的精确用例名称。

## 总览

| 项目 | 含用例源文件 | 夹具 / 辅助文件 | 测试类 | 测试方法 | 运行用例 |
| --- | ---: | ---: | ---: | ---: | ---: |
| [Tests.Queue](../tests/FunGame.Core.Tests.Queue) | 22 | 2 | 20 | 217 | 312 |
| [Tests.Combat](../tests/FunGame.Core.Tests.Combat) | 6 | 1 | 6 | 62 | 65 |
| [Tests.Entities](../tests/FunGame.Core.Tests.Entities) | 7 | 1 | 7 | 61 | 81 |
| [Tests.Infrastructure](../tests/FunGame.Core.Tests.Infrastructure) | 9 | 0 | 19 | 71 | 95 |
| 合计 | 44 | 4 | 52 | 411 | 553 |

## 复核命令

```powershell
dotnet build FunGame.Core.slnx -c Release
dotnet test FunGame.Core.slnx -c Release --no-build --no-restore
# 发现测试名称；Theory 的显示方式取决于适配器，最终数量以执行结果为准
dotnet test FunGame.Core.slnx -c Release --no-build --list-tests
```

以下文件链接均相对于本页，指向实际测试源码。方法名可在对应文件中定位；分部测试需查看同一行的全部文件。

## Queue

| 测试类 | 源文件 | 方法数 | 运行用例 |
| --- | --- | ---: | ---: |
| `ActionQueueTests` | [ActionQueueTests.cs](../tests/FunGame.Core.Tests.Queue/ActionQueueTests.cs) | 16 | 16 |
| `AIControllerLifecycleTests` | [AIControllerLifecycleTests.cs](../tests/FunGame.Core.Tests.Queue/AIControllerLifecycleTests.cs) | 7 | 9 |
| `AIDecisionBaselineTests` | [AIDecisionBaselineTests.cs](../tests/FunGame.Core.Tests.Queue/AIDecisionBaselineTests.cs) | 2 | 9 |
| `AIPlanningTests` | [AIPlanningTests.cs](../tests/FunGame.Core.Tests.Queue/AIPlanningTests.cs) | 9 | 10 |
| `AllTargetSelectionTests` | [AllTargetSelectionTests.cs](../tests/FunGame.Core.Tests.Queue/AllTargetSelectionTests.cs) | 13 | 24 |
| `BossBattleDecisionTableTests` | [BossBattleDecisionTableTests.cs](../tests/FunGame.Core.Tests.Queue/BossBattleDecisionTableTests.cs) | 5 | 10 |
| `CastingPlanningOptimizationTests` | [CastingPlanningOptimizationTests.cs](../tests/FunGame.Core.Tests.Queue/CastingPlanningOptimizationTests.cs) | 14 | 24 |
| `CastTerminationTests` | [CastTerminationTests.cs](../tests/FunGame.Core.Tests.Queue/CastTerminationTests.cs) | 24 | 33 |
| `ChainActionAITests` | [ChainActionAITests.cs](../tests/FunGame.Core.Tests.Queue/ChainActionAITests.cs) | 14 | 19 |
| `ChainActionCostRecordTests` | [ChainActionCostRecordTests.cs](../tests/FunGame.Core.Tests.Queue/ChainActionCostRecordTests.cs) | 1 | 2 |
| `ChainActionRegressionTests` | [ChainActionRegressionTests.cs](../tests/FunGame.Core.Tests.Queue/ChainActionRegressionTests.cs) | 25 | 25 |
| `ChainActionSettlementTests` | [ChainActionSettlementTests.cs](../tests/FunGame.Core.Tests.Queue/ChainActionSettlementTests.cs) | 11 | 11 |
| `ChainActionTests` | [ChainActionTests.cs](../tests/FunGame.Core.Tests.Queue/ChainActionTests.cs) | 14 | 14 |
| `DecisionPointsRecoveryTests` | [DecisionPointsRecoveryTests.cs](../tests/FunGame.Core.Tests.Queue/DecisionPointsRecoveryTests.cs) | 5 | 13 |
| `DecisionPointsTests` | [DecisionPointsTests.cs](../tests/FunGame.Core.Tests.Queue/DecisionPointsTests.cs) | 9 | 9 |
| `DecisionRiskBudgetTests` | [DecisionRiskBudgetTests.Budget.cs](../tests/FunGame.Core.Tests.Queue/DecisionRiskBudgetTests.Budget.cs)、[DecisionRiskBudgetTests.cs](../tests/FunGame.Core.Tests.Queue/DecisionRiskBudgetTests.cs)、[DecisionRiskBudgetTests.Risk.cs](../tests/FunGame.Core.Tests.Queue/DecisionRiskBudgetTests.Risk.cs) | 18 | 31 |
| `DecisionTableBoundaryTests` | [DecisionTableBoundaryTests.cs](../tests/FunGame.Core.Tests.Queue/DecisionTableBoundaryTests.cs) | 8 | 23 |
| `DecisionTableRevalidationTests` | [DecisionTableRevalidationTests.cs](../tests/FunGame.Core.Tests.Queue/DecisionTableRevalidationTests.cs) | 1 | 6 |
| `DecisionTableTests` | [DecisionTableTests.cs](../tests/FunGame.Core.Tests.Queue/DecisionTableTests.cs) | 15 | 18 |
| `TeamQueueTests` | [TeamQueueTests.cs](../tests/FunGame.Core.Tests.Queue/TeamQueueTests.cs) | 6 | 6 |

## Combat

| 测试类 | 源文件 | 方法数 | 运行用例 |
| --- | --- | ---: | ---: |
| `DamageCalculationTests` | [DamageCalculationTests.cs](../tests/FunGame.Core.Tests.Combat/DamageCalculationTests.cs) | 13 | 13 |
| `DamagePipelineTests` | [DamagePipelineTests.cs](../tests/FunGame.Core.Tests.Combat/DamagePipelineTests.cs) | 16 | 16 |
| `EffectTests` | [EffectTests.cs](../tests/FunGame.Core.Tests.Combat/EffectTests.cs) | 11 | 11 |
| `ItemCastRecordTests` | [ItemCastRecordTests.cs](../tests/FunGame.Core.Tests.Combat/ItemCastRecordTests.cs) | 1 | 4 |
| `NormalAttackTests` | [NormalAttackTests.cs](../tests/FunGame.Core.Tests.Combat/NormalAttackTests.cs) | 9 | 9 |
| `SkillCastTests` | [SkillCastTests.cs](../tests/FunGame.Core.Tests.Combat/SkillCastTests.cs) | 12 | 12 |

## Entities

| 测试类 | 源文件 | 方法数 | 运行用例 |
| --- | --- | ---: | ---: |
| `CharacterAggregationTests` | [CharacterAggregationTests.cs](../tests/FunGame.Core.Tests.Entities/CharacterAggregationTests.cs) | 5 | 9 |
| `CharacterAttributeTests` | [CharacterAttributeTests.cs](../tests/FunGame.Core.Tests.Entities/CharacterAttributeTests.cs) | 11 | 11 |
| `CharacterPrimaryAttributeTests` | [CharacterPrimaryAttributeTests.cs](../tests/FunGame.Core.Tests.Entities/CharacterPrimaryAttributeTests.cs) | 7 | 23 |
| `CharacterProgressionTests` | [CharacterProgressionTests.cs](../tests/FunGame.Core.Tests.Entities/CharacterProgressionTests.cs) | 9 | 9 |
| `EntityEqualityTests` | [EntityEqualityTests.cs](../tests/FunGame.Core.Tests.Entities/EntityEqualityTests.cs) | 7 | 7 |
| `ItemAndInventoryTests` | [ItemAndInventoryTests.cs](../tests/FunGame.Core.Tests.Entities/ItemAndInventoryTests.cs) | 12 | 12 |
| `TradeAndExploreTests` | [TradeAndExploreTests.cs](../tests/FunGame.Core.Tests.Entities/TradeAndExploreTests.cs) | 10 | 10 |

## Infrastructure

| 测试类 | 源文件 | 方法数 | 运行用例 |
| --- | --- | ---: | ---: |
| `BattleStatePredictorTests` | [BattleStatePredictorTests.cs](../tests/FunGame.Core.Tests.Infrastructure/BattleStatePredictorTests.cs) | 1 | 1 |
| `CalculationTests` | [CalculationAndUtilityTests.cs](../tests/FunGame.Core.Tests.Infrastructure/CalculationAndUtilityTests.cs) | 4 | 4 |
| `CharacterAttributeSerializationTests` | [CharacterAttributeSerializationTests.cs](../tests/FunGame.Core.Tests.Infrastructure/CharacterAttributeSerializationTests.cs) | 1 | 3 |
| `ConstantSetTests` | [FactoryAndConstantTests.cs](../tests/FunGame.Core.Tests.Infrastructure/FactoryAndConstantTests.cs) | 4 | 4 |
| `DateTimeUtilityTests` | [CalculationAndUtilityTests.cs](../tests/FunGame.Core.Tests.Infrastructure/CalculationAndUtilityTests.cs) | 5 | 5 |
| `EncryptionTests` | [CalculationAndUtilityTests.cs](../tests/FunGame.Core.Tests.Infrastructure/CalculationAndUtilityTests.cs) | 4 | 4 |
| `EventCenterTests` | [EventCenterAndRecordTests.cs](../tests/FunGame.Core.Tests.Infrastructure/EventCenterAndRecordTests.cs) | 5 | 5 |
| `ExceptionTests` | [FactoryAndConstantTests.cs](../tests/FunGame.Core.Tests.Infrastructure/FactoryAndConstantTests.cs) | 2 | 2 |
| `FactoryTests` | [FactoryAndConstantTests.cs](../tests/FunGame.Core.Tests.Infrastructure/FactoryAndConstantTests.cs) | 5 | 5 |
| `JsonConverterTests` | [JsonConverterTests.cs](../tests/FunGame.Core.Tests.Infrastructure/JsonConverterTests.cs) | 5 | 5 |
| `JsonServiceTests` | [JsonServiceTests.cs](../tests/FunGame.Core.Tests.Infrastructure/JsonServiceTests.cs) | 11 | 11 |
| `JsonToolRegistrationTests` | [FactoryAndConstantTests.cs](../tests/FunGame.Core.Tests.Infrastructure/FactoryAndConstantTests.cs) | 1 | 1 |
| `LinqExtensionTests` | [CalculationAndUtilityTests.cs](../tests/FunGame.Core.Tests.Infrastructure/CalculationAndUtilityTests.cs) | 2 | 2 |
| `NetworkUtilityTests` | [CalculationAndUtilityTests.cs](../tests/FunGame.Core.Tests.Infrastructure/CalculationAndUtilityTests.cs) | 5 | 20 |
| `RoundRecordCostTests` | [RoundRecordCostTests.cs](../tests/FunGame.Core.Tests.Infrastructure/RoundRecordCostTests.cs) | 5 | 8 |
| `RoundRecordTests` | [EventCenterAndRecordTests.cs](../tests/FunGame.Core.Tests.Infrastructure/EventCenterAndRecordTests.cs) | 4 | 4 |
| `SkillExtensionTests` | [EventCenterAndRecordTests.cs](../tests/FunGame.Core.Tests.Infrastructure/EventCenterAndRecordTests.cs) | 2 | 2 |
| `UnitAttributeSerializationTests` | [UnitAttributeSerializationTests.cs](../tests/FunGame.Core.Tests.Infrastructure/UnitAttributeSerializationTests.cs) | 3 | 5 |
| `VerificationTests` | [CalculationAndUtilityTests.cs](../tests/FunGame.Core.Tests.Infrastructure/CalculationAndUtilityTests.cs) | 2 | 4 |

## 夹具与场景配置

| 文件 | 职责 |
| --- | --- |
| [BattleFixture.cs](../tests/FunGame.Core.Tests.Queue/BattleFixture.cs) | 队列、协同行动与可控回合辅助 |
| [BossBattleFixture.cs](../tests/FunGame.Core.Tests.Queue/BossBattleFixture.cs) | BOSS 技能与效果、战斗及规则构建辅助 |
| [CombatFixture.cs](../tests/FunGame.Core.Tests.Combat/CombatFixture.cs) | 战斗角色、技能、特效与资源夹具 |
| [EntitiesFixture.cs](../tests/FunGame.Core.Tests.Entities/EntitiesFixture.cs) | 实体与装备构造辅助 |
| [Fixtures/boss.json](../tests/FunGame.Core.Tests.Queue/Fixtures/boss.json)、[Fixtures/empty.json](../tests/FunGame.Core.Tests.Queue/Fixtures/empty.json) | 决策表配置，由 AIPlanningTests 加载验证 |

跨项目场景按实际测试类所在项目统计。例如属性 JSON 在 Infrastructure，协同动作费用往返在 Queue，物品逐次费用在 Combat；不能仅按正文讨论的机制把它们计入同一项目。
