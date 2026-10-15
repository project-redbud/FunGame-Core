# 决策表

Core 提供版本 1 的 beforeDecision 有序规则表。配置使用纯 BCL JSON，角色保存策略 ID，运行时通过 DecisionTableLoader 绑定不可变策略。模组明确选择的动作和已有协同确认优先于角色普通决策；EnableAI 关闭时不评估角色策略。

协同申请按申请者的控制模式调度：AI 协作方使用自己的决策表向当前回合角色提交原地申请，当前角色可以是手动玩家。手动协作方由宿主显式提交，不被 AI 代选。申请本身不执行动作，手动确认者自行选择是否接受。

## 构建、导出与加载

```csharp
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

Build 创建独立配置快照，不编译策略、不绑定角色、不读写文件。Generator.Save 默认拒绝覆盖；overwrite=true 才覆盖。Loader.Register 拒绝重复策略 ID。提供资源标识目录时，Compiler 会验证引用；省略目录时由运行时可用技能与物品列表解析。无法绑定时返回诊断并清除旧绑定。

建议先给角色模板绑定策略，再创建战斗队列。Character.Copy、CharacterBuilder 和模板恢复继承 ID 与可共享策略；JSON 仅保存 ID，反序列化后需要重新 Bind。修改 DecisionTableId 会清除运行时绑定。策略不保存角色、技能实例或可变战斗状态。

## 配置与规则语义

| 字段 | 有效值或含义 |
| --- | --- |
| schemaVersion | 1 |
| id | module:name |
| fallback | defaultAI 或 endTurn |
| rules[].priority | 越大越先执行，同级保持配置顺序 |
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

从动作合法目标中逐个尝试绑定目标，规划位置与范围后评估条件，返回满足条件的同一方案。无法执行的规则继续匹配后续规则；全部未命中按 fallback 处理。省略 when 表示无条件，但仍要形成合法行动。endTurn 是有效决策，空表 defaultAI 则是未命中，且不消费默认 AI 的随机序列。

current 优先低移动成本；coverage 优先覆盖数，其次移动成本与风险；safe 优先低风险，其次覆盖数和移动成本。目标名称及方案名称的引用会在编译时校验。

动作技能名 castSkill 映射到普通行动的 PreCastSkill 入口；队列根据技能类型执行战技、吟唱魔法或爆发技。协同使用对应的协同动作和预算，禁止移动；协同 endTurn 表示不提交申请。

AI 模式下，行动前的模组选择列表包含可在移动后触及的完整可选角色；角色策略与默认规划按各候选位置检查具体射程。模组筛选及类别概率置零依然限制候选，规则不会绕过这些限制。

## 条件

条件组合是 all、any、not，每个节点只能使用一种形式。空 all 成立，空 any 不成立。叶节点支持：

| type | value / 引用 |
| --- | --- |
| self.hpRatio / target.hpRatio | 0 至 1；目标条件引用 target.bind |
| self.mp / self.ep | 资源值 |
| self.state | CharacterState 的整数值 |
| skill.available | skillId；包含资源、状态、DP 与配额检查 |
| decisionPoints | 当前 DP |
| target.exists | 已定义的目标绑定 |
| distance | 已定义的目标绑定；从规划位置计算曼哈顿距离 |
| plan.targetCount | action.bindPlan；当前方案覆盖的合法角色数量 |
| plan.risk | action.bindPlan；当前方案位置的威胁值 |

比较方式为 lessThan、atMost、equal、atLeast、greaterThan。distance 与 plan.risk 在无地图模式不匹配；required 移动、approach 和 retreat 在无地图模式不可执行，ifNeeded 技能可以原地规划。

最多 256 条规则，条件树最多 16 层，每个组合最多 64 个子条件。非法数值、未知类型、未定义绑定和无效资源引用返回带配置路径的诊断。Parser 不接受未知 JSON 字段。

## 默认 AI 与估值

通过 queue.AI 配置 SelectionMode：

- Legacy：分数乘类别权重，保留现有随机评分。
- Deterministic：有界候选规划、稳定估值与排序，不加入随机评分。
- WeightedNearBest：在 ScoreTolerance 的近似收益集合中，先按调整后的行动类别权重抽样，再按类别内候选收益抽样。技能数量不会直接放大类别权重。

队列持有的 AIController 在换图和关闭再启用时保持同一实例，保留模式、评分阈值、实例事件订阅及随机决策序号。LoadGameMap 会立即同步已有控制器的 Map，包括 AI 关闭期间；实际地图实例改变时清除位置历史、LastPolicyResult、LastMetrics 与 LastPlanningDiagnostics。同一地图实例不触发清理。EnableAI=false 时 queue.AI 返回 null，并清除这些短期状态；重新启用后继续使用保留实例。各队列的控制器与状态互相独立。

直接构造的 AIController(queue, map) 保留显式地图绑定，队列换图不会更新该独立实例。宿主需同步换图与决策操作。同种子、同输入及同操作序列可重放随机决策，换图与开关不会重新使用开局序号。

新模式普攻使用现有伤害估值；缺少技能语义时使用目标数量的保守基础值，并调用对应评估事件。EvaluatePlanEvent 可以返回实际伤害、有效治疗等估值，返回 null 使用内置值。回调必须只读；普通多播委托返回最后一个订阅者的值，不能把多次订阅当作自动求和。

新模式的普攻、物品、即时技能及吟唱技能都使用实际位置风险扣分，保留移动步数成本；MovementBenefitThreshold 优先保留近似收益的原地行动，RepeatedPositionPenalty 降低返回近期实际位置的偏好。位置记录按控制器和角色对象隔离，不序列化。

Legacy 的既有评估事件可以并行调用，订阅者还需保证线程安全。新模式与有序规则在单次决策内串行评估。

默认预算为每动作 32 个位置、每位置 64 个施法中心；策略阶段和新模式默认 AI 阶段各有 4096 个方案额度。可在自建 DecisionEvaluationContext 中调整预算。当前位置占用位置预算，强制移动除外。单次决策缓存射程、路径、敌方威胁与位置风险，规则及回退共享只读查询缓存，预算分别结算；缓存不跨局面复用。

规则先按优先级降序、同级配置顺序排好。禁用、执行重验失败或可由内置条件预判为 false 的规则不占额度；预判不会执行自定义条件，目标与方案条件留到规划后检查。记剩余方案额度为 R、待规划规则数为 N，本条最多使用 `ceil(R / N)` 个方案；未用额度留给后续规则，命中立即返回。若总额度少于待规划规则数，按原优先级截断，后序规则可能没有规划机会。endTurn 不消耗搜索额度，但仍判断其条件。

策略未命中时，defaultAI 使用独立完整阶段额度。新模式的普攻、可用技能和物品同样分配本阶段额度；地图普通行动在总额至少为 2 时预留一个纯移动兜底方案。Legacy 保留既有候选和随机评分算法，不使用新模式的动作额度分配。策略与新模式回退合计至多使用两阶段额度，位置和中心预算分别约束各次动作规划。

自定义动作通过传入上下文读取本条 PlanBudget；框架截断超额返回，内置规划器与外层包装按同一方案实例去重计费。扩展回调自身计算的耗时不受候选数量上限保证。预算与观测计数独立，PlansEvaluated 表示已生成并保留的方案数，不包含常量 endTurn。

规则表与 Deterministic、WeightedNearBest 使用目标驱动的施法规划：

- 全图指向性技能复用合法目标与目标组合，安全站位独立评估；即时魔法默认使用 current，实际吟唱时间大于零的魔法默认使用 safe。
- 基础圆形、菱形和正方形技能从敌方、友方或自身位置反推可覆盖它的落点，合并去重后按站位的施法射程过滤。敌人之间的空地也属于候选，有限射程允许效果覆盖施法距离之外的角色。
- 若当前站位已能触及全部指向性目标或全部范围落点，且不要求安全位置、风险限制或强制移动，则只规划当前站位。规则包含 distance 或自定义条件时保留位置搜索。
- 与施法者位置无关的基础范围投影按中心缓存。基础指向性技能复用目标筛选，再按有限射程筛选角色；基础地图用坐标距离校验射程，自定义地图仍调用自己的射程查询。

直线、贯穿线、扇形、自定义范围/目标查询和允许空地施法的技能保留完整落点查询。预算截断仍属于近似搜索，不保证任意局面的最优覆盖。Legacy 保留既有候选与随机评分方式。

## 地图扩展与执行

GetMovementCosts 一次查询所有连通格的路径成本，GetMovementSteps 查询其中一个目标。基础实现沿用八方向搜索；候选 MOV 仍使用现有曼哈顿范围。派生地图应覆盖 GetMovementCosts，并让它与 CharacterMove 使用相同规则。一次决策中，同一角色和起点只计算一份路径成本表。

技能通过 SelectNonDirectionalTargetsFrom 提供显式假设施法起点；SelectTargetsByRangeFrom 使施法者按假设位置参与覆盖判定。自定义线形或范围技能应同时实现假设位置查询。规划器不会临时移动角色来测量范围。

AIDecision.CastCenter 表示选择中心/方向，TargetGrids 表示影响格。协同提交可传 castCenter，队列验证中心射程后重算影响格；直接提交格子的现有接口仍按直接格子契约验证。

行动在移动前、动作开始后及询问后重验，失效规则在当前行动尝试中跳过，沿用队列的有限重试。已经成功的移动不回滚，后续规划从真实位置执行。

内置方案在 current、coverage、safe 下均计算实际位置风险；求值风险本身不额外启动站位搜索。风险是能在现有移动与射程内威胁该格的敌人数量。它不预测隐藏效果、敌方策略、控制成功率或队友保护，也不保证吟唱绝对安全。

## 扩展与诊断

DecisionTableRegistry.RegisterCondition 与 RegisterAction 接收只读求值器/规划器。扩展配置仅保存标识和 JSON 参数；AddRule 的 lambda 只在构建时执行，不导出任意 C# 委托。编译器会复制配置和注册表。

宿主也可以实现 IDecisionPolicy，通过 Loader.Register(id, policy) 注册代码策略，再按 ID 绑定角色。代码策略不能导出为独立 JSON 规则；实现者负责隔离自身的角色与战斗状态。

AIController.LastPolicyResult 提供最近一次策略结果，LastPlanningDiagnostics 汇总策略与默认 AI 的规划诊断。DecisionDiagnostic.Code 区分 budget.exhausted（没有方案额度）、budget.truncated（搜索候选被截断）、plan.unavailable、condition.unmatched、execution.rejected 和 plan.invalid；未分类的配置诊断代码为空。条件未匹配且伴随截断诊断，只说明已检查候选未命中。

PolicyEvaluatedEvent 在每次策略评估完成时提供该时刻的独立统计快照，包括协同申请；后续默认 AI 不改写此快照。LastMetrics 在完整决策结束后包含两阶段的汇总次数与耗时，PolicyPlansEvaluated / DefaultAIPlansEvaluated 分别计数。BudgetTruncations 按被截断的预算范围计数，BudgetExhaustions 记录因无额度跳过规划的次数。RiskEvaluations 记录实际风险计算，RiskCacheHits 记录位置风险缓存命中。

CentersGenerated 按位置累计候选，CentersEvaluated 只统计实际完整范围计算；投影缓存命中不增加 CentersEvaluated。各次决策的计数不跨局面累积，采集者不能只依赖战斗结束时的最后一次结果。

完整五规则 BOSS 配置及空表位于 `tests/FunGame.Core.Tests.Queue/Fixtures/`，链式构建示例和固定技能效果位于 BossBattleFixture。风险与预算专项回归见 DecisionRiskBudgetTests，覆盖嵌套条件、阶段隔离、扩展去重计费与移动预留。测试覆盖规则专项实际执行、地图/非地图完整战斗、固定 20 种子对照、配置往返、预算与状态限制、目标绑定、概率类别分布和假设位置范围。
