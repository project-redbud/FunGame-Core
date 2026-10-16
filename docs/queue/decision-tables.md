# 决策表

> 上级：[队列与回合](README.md) ｜ 源码：`src/Api/DecisionTableBuilder.cs`、`DecisionTableParser.cs`、`DecisionTableLoader.cs`、`DecisionTableCompiler.cs`、`src/Model/DecisionActionPlanner.cs`（规划核心）、`src/Model/DecisionTable/`（配置、评估局面、几何判定）、`src/Controller/AIController*.cs`、`src/Interface/Base/IDecisionPolicy.cs`
> 相关测试：`DecisionTableTests`、`DecisionTableBoundaryTests`、`DecisionTableRevalidationTests`、`DecisionRiskBudgetTests`、`BossBattleDecisionTableTests`、`CastingPlanningOptimizationTests`、`AllTargetSelectionTests`、`AIControllerLifecycleTests`

## 一句话结论

决策表是角色级的 `beforeDecision` 有序规则表：配置用纯 BCL JSON 描述「目标 + 条件 + 行动 + 位置偏好」，`DecisionTableLoader` 把它编译成不可变策略并按角色 ID 绑定；AI 决策时**先问角色策略**，未命中才回退 `fallback`，而模组事件与已有协同确认始终优先于两者。

## 决策优先级

| 顺序 | 来源 | 行为 |
| --- | --- | --- |
| 1 | 模组 `DecideActionEvent` | 返回非 `None` 即接管，不评估角色策略 |
| 2 | 协同确认 | 存在有效申请时，AI 直接选择第一份有效申请 |
| 3 | 角色策略 | `Character.DecisionPolicy`，命中即返回并携带 `RuleId` |
| 4 | 默认 AI | `AISelectionMode` 三档；空表 `defaultAI` 与未绑定角色都走到这里 |

`EnableAI = false` 时队列不提供 AI 控制器，角色策略因此不会被评估。协同申请按控制模式调度：AI 协作方用自己的决策表向当前回合角色提交原地申请，当前角色可以是手动玩家；手动协作方由宿主显式提交。申请本身不执行动作，详见[协同行动](chain-action.md)。

## 构建、导出与加载

```csharp
using FunGame.Core.Api;
using FunGame.Core.Model.DecisionTable;

DecisionTableConfig config = DecisionTableBuilder.Create("example:healer")
    .AddRule("heal", rule => rule.Priority(100)
        .Target(Targets.Allies("patient"))
        .When(Conditions.All(
            Conditions.TargetHpRatio("patient", "lessThan", 0.3),
            Conditions.SkillAvailable(1001)))
        .Then(Actions.CastSkill(1001, "patient")))
    .Build();

DecisionTableGenerator.Save(config, "healer.json");
DecisionTableLoader loader = new(new(skills: [1001]));
IReadOnlyList<DecisionDiagnostic> errors = loader.Load("healer.json");
if (errors.Count > 0) throw new InvalidOperationException(string.Join("; ", errors));
character.DecisionTableId = "example:healer";
errors = loader.Bind(character);
```

| 步骤 | 行为 |
| --- | --- |
| `Build` | 校验后经 JSON 往返生成独立快照；不编译策略、不绑定角色、不读写文件；`AddRule` 的 lambda 只在构建时执行，不导出 C# 委托 |
| `Generator.Save` | 默认拒绝覆盖，`overwrite: true` 才覆盖 |
| `Loader.Register(配置)` | 校验失败或 ID 重复时返回诊断并保留原有策略，不静默替换 |
| `Loader.Register(id, 策略)` | 注册代码策略（`IDecisionPolicy`），重复 ID 返回 `id` 诊断 |
| `Loader.Load(路径)` | 文件路径格式、读取权限、I/O 与 JSON 格式错误都转换为诊断；文件读取之外的编程异常继续抛出 |
| `Loader.Bind(角色)` | 按 `DecisionTableId` 查表；找不到时清除旧绑定并返回诊断；ID 为 `null` 时仅解绑 |
| `Compile` | 校验后复制配置与注册表，产出可共享的不可变策略；无效配置抛 `ArgumentException` |
| 资源校验 | 构造 `DecisionTableCompiler` 时给出技能/物品目录才会在编译期校验引用；省略目录时由运行时可用实例解析 |

建议先给角色模板绑定策略，再创建战斗队列。`Character.Copy`、`CharacterBuilder` 与 `Respawn` 继承 ID 与可共享策略；JSON 只保存 ID，反序列化后必须重新 `Bind`；修改 `DecisionTableId` 会清除运行时绑定。策略不持有角色、技能实例或可变战斗状态。

## 配置字段

| 字段 | 有效值或含义 |
| --- | --- |
| schemaVersion | 1 |
| id | module:name |
| fallback | defaultAI 或 endTurn |
| rules[].priority | 越大越先匹配，同级保持配置顺序 |
| trigger | beforeDecision |
| target.relation | self、ally（包含自身）、enemy |
| target.selector | self、lowestHpRatio、highestHpRatio、nearest |
| target.bind | 规则内的目标绑定名 |
| action.type | normalAttack、castSkill、useItem、approach、retreat、endTurn |
| action.skillId / itemId | 从当前角色可用实例解析，不捕获模板实例 |
| action.target | 已定义的目标绑定 |
| action.movement | never、ifNeeded、required |
| action.positionPreference | current、coverage、safe |
| action.bindPlan | 条件引用的范围方案名称 |
| action.maxRisk | 位置近似威胁上限，非负有限数 |

从动作合法目标中逐个尝试绑定目标，规划位置与范围后评估条件，返回满足条件的**同一份方案**（目标、位置、范围一同确定）。无法执行的规则继续匹配后续规则；全部未命中按 `fallback` 处理。省略 `when` 表示无条件，但仍要形成合法行动。`endTurn` 是有效决策；空表 `defaultAI` 是未命中，且不消费默认 AI 的随机序列。

`castSkill` 映射到普通行动的 `PreCastSkill` 入口，队列再按技能类型执行战技、吟唱魔法或爆发技。协同使用对应的协同动作和预算，禁止移动；协同 `endTurn` 表示不提交申请。

## 位置偏好与风险

| 偏好 | 排序 | 典型用途 |
| --- | --- | --- |
| current | 移动步数升序，风险作次级键 | 默认 |
| coverage | 覆盖合法角色数量，其次移动步数，再次风险 | 范围技能 |
| safe | 位置风险，其次覆盖数，再移动步数 | 吟唱魔法、`maxRisk` |

三档偏好都计算实际位置风险，并按 `maxRisk` 过滤候选。风险是「能在现有移动与射程内威胁该格」的敌人数量（`DecisionActionPlanner.EstimateRisk`）：只考虑存活且非不可行动状态的敌人，按普攻与已启用、冷却就绪且可选敌方的技能射程估算，不逐一验证敌方资源或完整行动合法性；**不预测**隐藏效果、敌方策略、控制成功率或队友保护，也不保证吟唱绝对安全。求值本身不额外启动站位搜索；风险按格缓存、敌方威胁集按敌人缓存，命中缓存另行计数。目标名称与方案名称的引用在编译期校验。

## 条件

条件组合是 `all`、`any`、`not`，每个节点只能使用一种形式。空 `all` 成立，空 `any` 不成立。

| type | value / 引用 |
| --- | --- |
| self.hpRatio / target.hpRatio | 0 至 1；目标条件引用 `target.bind` |
| self.mp / self.ep | 资源值 |
| self.state | `CharacterState` 的整数值 |
| skill.available | skillId；包含资源、状态、DP 与配额检查 |
| decisionPoints | 当前 DP |
| target.exists | 已定义的目标绑定 |
| distance | 已定义的目标绑定；从规划位置计算曼哈顿距离 |
| plan.targetCount | action.bindPlan；当前方案覆盖的合法角色数量 |
| plan.risk | action.bindPlan；当前方案位置的威胁值 |

比较方式为 `lessThan`、`atMost`、`equal`、`atLeast`、`greaterThan`。`distance` 与 `plan.risk` 在无地图模式不匹配；`required` 移动、`approach` 与 `retreat` 在无地图模式不可执行，`ifNeeded` 技能可以原地规划。

规则列表不可为 `null`，允许空表，最多 256 条。条件根节点深度为 0，最多嵌套 16 级，每个 `all` / `any` 组合最多 64 个直接子条件。非法数值、未知类型、未定义绑定和无效资源引用返回带配置路径的诊断。Parser 不接受未知 JSON 字段，JSON 最大深度为 64；它限制 JSON 对象与数组的嵌套，与条件树深度独立，扩展 `parameters` 中的 JSON 也受此限制。

## 规则预算分配

规则先按优先级降序、同级配置顺序排好。禁用、执行重验失败或可由内置条件预判为 `false` 的规则不占额度；预判不会执行自定义条件，目标与方案条件留到规划后检查。

记剩余方案额度为 `R`、待规划规则数为 `N`，本条最多使用 `ceil(R / N)` 个方案；未用额度留给后续规则，命中立即返回。若总额度少于待规划规则数，按原优先级截断，后序规则可能没有规划机会。`endTurn` 不消耗搜索额度，但仍判断其条件。

| 预算 | 默认值 | 作用域 |
| --- | --- | --- |
| 位置完整评估 | 每动作 32 | 单次动作规划 |
| 每位置施法中心 | 每位置 64 | 单次动作规划 |
| 方案额度 | 策略阶段与新模式默认 AI 阶段各 4096 | 单次决策的每个阶段 |

当前位置占用位置预算（`movement: required` 除外）。两个阶段的额度相互独立：策略未命中时，默认 AI 使用完整的一份额度。可在自建 `DecisionEvaluationContext` 中调整这些预算。自定义动作通过传入的上下文读取本条 `PlanBudget`，框架截断超额返回，内置规划器与外层包装按**同一方案实例**去重计费；扩展回调自身计算的耗时不受候选数量上限保证。

预算与观测计数独立：`PlansEvaluated` 表示已生成并保留的方案数，不包含常量 `endTurn`。

## 施法规划

规则表与 `Deterministic` / `WeightedNearBest` 使用目标驱动的施法规划：

- 全图指向性技能复用合法目标与目标组合，安全站位独立评估；即时魔法默认 `current`，实际吟唱时间大于零的魔法默认 `safe`。
- 基础圆形、菱形和正方形技能从敌方、友方或自身位置反推可覆盖它的落点，合并去重后按站位的施法射程过滤。敌人之间的空地也属于候选，有限射程允许效果覆盖施法距离之外的角色。
- 若当前站位已能触及全部指向性目标或全部范围落点，且不要求安全位置、风险限制或强制移动，则只规划当前站位；规则包含 `distance` 或自定义条件时保留位置搜索。
- 与施法者位置无关的基础范围投影按中心缓存。基础指向性技能复用目标筛选，再按有限射程筛选角色；基础地图用坐标距离校验射程，自定义地图仍调用自己的射程查询。

直线、贯穿线、扇形、自定义范围/目标查询和允许空地施法的技能保留完整落点查询。预算截断仍属于近似搜索，不保证任意局面的最优覆盖。

AI 模式下，行动前的模组选择列表包含可在移动后触及的完整可选角色；角色策略与默认规划按各候选位置检查具体射程。模组筛选及类别概率置零依然限制候选，规则不会绕过这些限制。

## 地图扩展与执行

| 扩展点 | 约定 |
| --- | --- |
| `GetMovementCosts` | 一次查询所有连通格的路径成本；`GetMovementSteps` 查询其中一个目标 |
| 基础实现 | 沿用八方向搜索；候选 MOV 仍使用现有曼哈顿范围 |
| 派生地图 | 应覆盖 `GetMovementCosts`，并让它与 `CharacterMove` 使用相同规则 |
| 缓存 | 一次决策中，同一角色和起点只计算一份路径成本表 |
| `SelectNonDirectionalTargetsFrom` | 提供显式假设施法起点 |
| `SelectTargetsByRangeFrom` | 使施法者按假设位置参与覆盖判定 |

自定义线形或范围技能应同时实现两个假设位置查询；规划器不会临时移动角色来测量范围。

`AIDecision.CastCenter` 表示选择中心/方向，`TargetGrids` 表示影响格。协同提交可传 `castCenter`，队列验证中心射程后重算影响格；直接提交格子的现有接口仍按直接格子契约验证。

行动在移动前、动作开始后、询问后以及技能与物品执行前重验。失效规则在当前行动尝试中跳过，沿用队列的有限重试；已经成功的移动不回滚，后续规划从真实位置执行。

## 默认 AI 与估值

| SelectionMode | 选择方式 |
| --- | --- |
| Legacy | 分数乘类别权重排序，随机性来自估值函数中的随机项 |
| Deterministic | 有界候选规划、稳定估值与排序，不加入随机评分 |
| WeightedNearBest | 在 `ScoreTolerance` 的近似收益集合中，先按调整后的行动类别权重抽样，再按类别内候选收益抽样；技能数量不会直接放大类别权重 |

规划路径的候选评分是「估值 − 移动步数成本 − 位置风险 − 往返惩罚」。普攻复用现有伤害估值；缺少技能语义时使用目标数量的保守基础值，并调用对应评估事件。`EvaluatePlanEvent` 可以返回实际伤害、有效治疗等估值，返回 `null` 使用内置值。回调必须只读；普通多播委托返回**最后一个订阅者**的值，不能把多次订阅当作自动求和。`MovementBenefitThreshold` 优先保留近似收益的原地行动，`RepeatedPositionPenalty` 降低返回近期实际位置的偏好。位置记录按控制器和角色对象隔离，不序列化。

Legacy 的评估事件可以并行调用，订阅者需保证线程安全；规划路径与有序规则在单次决策内串行评估。

`Deterministic` / `WeightedNearBest` 的默认 AI 阶段同样按动作分配方案额度：普攻、可用技能与物品各占份额，地图普通行动在总额至少为 2 时预留一个纯移动兜底方案。两阶段合计至多使用两份额度，位置和中心预算分别约束各次动作规划。单次决策缓存射程、路径、敌方威胁与位置风险，规则与回退共享只读查询缓存，预算分别结算；缓存不跨局面复用。Legacy 保留既有候选与随机评分算法，不使用新模式的动作额度分配。

## 控制器生命周期

| 行为 | 结果 |
| --- | --- |
| 换图、关闭再启用 | 队列持有的控制器保持同一实例，保留模式、评分阈值、实例事件订阅及随机决策序号 |
| `LoadGameMap` | 立即同步已有控制器的地图，包括 AI 关闭期间 |
| 地图实例确实改变 | 清除位置历史、`LastPolicyResult`、`LastMetrics` 与 `LastPlanningDiagnostics` |
| 同一地图实例 | 不触发上述清理 |
| `EnableAI = false` | `queue.AI` 返回 `null`，并清除短期决策状态；重新启用后继续使用保留实例 |
| 显式构造 `new AIController(queue, map)` | 保留自己的地图绑定，队列换图不会更新该独立实例，宿主需自行同步换图与决策操作 |
| 多队列 | 控制器与状态互相独立 |

同种子、同输入及同操作序列可重放随机决策；换图与开关不会重新使用开局序号。

## 扩展与诊断

`DecisionTableRegistry.RegisterCondition` 与 `RegisterAction` 接收只读求值器/规划器。扩展配置仅保存标识和 JSON 参数；`AddRule` 的 lambda 只在构建时执行，不导出任意 C# 委托。编译器会复制配置和注册表。

> 两处错误口径不同：Registry 的重复扩展注册抛出 `ArgumentException`，已有扩展保持；Loader 的重复策略标识返回 `id` 诊断，保留已有策略。

宿主也可以实现 `IDecisionPolicy`，通过 `Loader.Register(id, policy)` 注册代码策略，再按 ID 绑定角色。代码策略不能导出为独立 JSON 规则；实现者负责隔离自身的角色与战斗状态。

| 观测点 | 内容 |
| --- | --- |
| `AIController.LastPolicyResult` | 最近一次策略结果，未评估策略时为 `null`；`Matched` 表示取得动作，包含 `endTurn` 回退 |
| `AIController.LastPlanningDiagnostics` | 策略与默认 AI 的规划诊断汇总 |
| `PolicyEvaluatedEvent` | 每次策略评估完成时的独立统计快照，包括协同申请；后续默认 AI 不改写此快照 |
| `LastMetrics` | 完整决策结束后的两阶段汇总，`PolicyPlansEvaluated` / `DefaultAIPlansEvaluated` 分别计数 |

诊断 `Code` 区分 `budget.exhausted`（没有方案额度）、`budget.truncated`（搜索候选被截断）、`plan.unavailable`、`condition.unmatched`、`execution.rejected` 和 `plan.invalid`；未分类的配置诊断代码为空。条件未匹配并伴随截断诊断，只说明已检查候选未命中。

度量计数：`BudgetTruncations` 按被截断的预算范围计数（每个范围至多一次），`BudgetExhaustions` 记录因无额度跳过规划的次数，`RiskEvaluations` / `RiskCacheHits` 分列风险计算与缓存命中，`CentersGenerated` 按位置累计候选，`CentersEvaluated` 只统计实际完整范围投影（投影缓存命中不增加）。各次决策的计数不跨局面累积，采集者不能只依赖战斗结束时的最后一次结果。

## 相关测试

```powershell
dotnet test tests/FunGame.Core.Tests.Queue/FunGame.Core.Tests.Queue.csproj -c Release
```

| 测试类 | 守什么 |
| --- | --- |
| `DecisionTableTests` | 空表回退序列、规则优先级与同级顺序、配置快照隔离、复制与 JSON 重新绑定、注册与绑定诊断、代码策略 |
| `DecisionTableBoundaryTests` | 256 条规则、null 与空表、64 子条件、条件深度、JSON 深度、加载失败、注册表重复扩展 |
| `DecisionTableRevalidationTests` | 移动前与动作开始后的重验、失效方案不扣资源、协同策略 |
| `DecisionRiskBudgetTests` | `plan.risk` 三档偏好、嵌套条件、预判不执行扩展、配额分配与未用额度传递、耗尽与截断诊断、扩展动作去重计费、阶段隔离、缓存复用 |
| `BossBattleDecisionTableTests` | 五规则 BOSS 配置的实际命中与结算、固定 20 种子对照 |
| `CastingPlanningOptimizationTests` | 全图指向性单次筛选、锚定范围与全量参考一致、有限射程保留站位搜索、假设位置投影 |
| `AllTargetSelectionTests` | `SelectAll*` 的目标集合与自身资格、协同全选重验、JSON 与复制保真 |
| `AIControllerLifecycleTests` | 换图/开关保留实例与订阅、短期状态清理、独立控制器地图绑定、队列隔离、固定种子重放 |
| `AIPlanningTests` | `Fixtures/boss.json` 与 `Fixtures/empty.json` 的加载与配置一致性 |

完整五规则 BOSS 配置及空表位于 `tests/FunGame.Core.Tests.Queue/Fixtures/`，链式构建示例和固定技能效果位于 `BossBattleFixture`。
