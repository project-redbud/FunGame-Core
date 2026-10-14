using FunGame.Core.Entity;
using FunGame.Core.Interface.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.PrefabricatedEntity;

namespace FunGame.Core.Controller
{
    /// <summary>只读行动规划与执行前合法性检查。</summary>
    public static class DecisionActionPlanner
    {
        /// <summary>按预算生成绑定目标与位置的合法方案。</summary>
        /// <param name="context">当前局面。</param>
        /// <param name="action">动作描述。</param>
        /// <param name="selector">目标描述。</param>
        /// <returns>候选方案。</returns>
        public static IReadOnlyList<DecisionPlan> Plan(DecisionEvaluationContext context, ActionConfig action, TargetSelectorConfig? selector)
            => PlanForRule(context, action, selector, false);

        internal static IReadOnlyList<DecisionPlan> PlanForRule(DecisionEvaluationContext context, ActionConfig action, TargetSelectorConfig? selector, bool evaluatePositions)
        {
            Character actor = context.Actor;
            GameMap? map = context.Queue.Map;
            Grid? current = map?.GetCharacterCurrentGrid(actor);
            if (action.Type == "endTurn") return [new() { Decision = new() { ActionType = CharacterActionType.EndTurn, HasCandidate = true } }];
            if (map != null && current == null || map == null && (action.Movement == "required" || action.Type is "approach" or "retreat")) return [];
            Skill? skill = action.Type == "castSkill" ? context.Skills.FirstOrDefault(s => s.Id == action.SkillId) : null;
            Item? item = action.Type == "useItem" ? context.Items.FirstOrDefault(i => i.Id == action.ItemId) : null;
            ISkill? ability = action.Type == "normalAttack" ? actor.NormalAttack : skill ?? item?.Skills.Active;
            bool movementOnly = action.Type is "approach" or "retreat";
            if (movementOnly && (actor.HP <= 0 || actor.CharacterState != CharacterState.Actionable)) return [];
            if (!movementOnly && (ability == null || !CanUse(context, action.Type, skill, item))) return [];
            Skill? castSkill = ability as Skill;
            bool global = map != null && castSkill != null && DecisionGeometry.CoversPlane(map, castSkill);
            bool anchored = map != null && castSkill is { IsNonDirectional: true } && DecisionGeometry.CanAnchorArea(map, castSkill);
            HashSet<Grid>? anchoredCenters = anchored ? BuildAnchoredCenters(context, castSkill!, current!, false) : null;
            int castRange = action.Type == "normalAttack" ? actor.ATR : castSkill?.CastRange ?? 0;
            bool allReachable = map != null && current != null && ability != null && castSkill is not { IsNonDirectional: true } &&
                (ability.CanSelectEnemy ? context.SelectableEnemies : []).Concat(ability.CanSelectTeammate ? context.SelectableAllies : [])
                    .Where(c => !c.IsUnselectable).All(c => map.GetCharacterCurrentGrid(c) is Grid target && context.IsInRange(current, target, castRange));
            bool independent = global && (castSkill is { IsNonDirectional: false } || anchored && !castSkill!.CanSelectSelf);
            independent |= allReachable || anchoredCenters != null && !castSkill!.CanSelectSelf && anchoredCenters.All(g => context.IsInRange(current!, g, castRange));
            bool considerMovement = evaluatePositions || !independent || action.PositionPreference == "safe" || action.MaxRisk != null || action.Movement == "required";
            List<Grid> positions = current == null ? [Grid.Empty] : [current];
            if (considerMovement && map != null && current != null && !context.IsChainAction && action.Movement != "never")
            {
                Grid origin = context.TurnOrigin ?? current;
                HashSet<Grid> movable = [.. context.GetRange(origin, actor.MOV, false)];
                positions.AddRange(context.MoveGrids.Where(g => g.Id != current.Id && g.Characters.Count == 0 &&
                    movable.Contains(g) && context.GetMoveSteps(actor, current, g) >= 0));
            }
            positions = [.. positions.Distinct()];
            List<Character>? globalLegal = null;
            List<Character>? directionalPool = null;
            int globalCount = 0;
            if (map != null && ability != null && ability is not Skill { IsNonDirectional: true })
            {
                int reach = action.Type == "normalAttack" ? actor.ATR : castSkill!.CastRange;
                List<Character> potential;
                if (global || allReachable && !considerMovement)
                {
                    (globalLegal, globalCount) = DirectionalTargets(context, ability, current!, reach);
                    potential = globalLegal;
                }
                else
                {
                    potential = ability.GetSelectableTargets(actor, [.. context.Enemies], [.. context.Allies], [.. context.SelectableEnemies], [.. context.SelectableAllies]);
                    if (castSkill != null && DecisionGeometry.UsesDefaultMethod(castSkill, typeof(Skill), nameof(Skill.GetSelectableTargets))) directionalPool = potential;
                }
                potential = [.. potential.Intersect(RelatedTargets(context, selector))];
                if (!potential.Contains(actor) && !potential.Any(c => c.HP == 0))
                    positions = [.. positions.Where(g => g.Id == current?.Id || potential.Any(c => map.GetCharacterCurrentGrid(c) is Grid target && context.IsInRange(g, target, reach)))];
            }
            List<(Character Target, List<Character> Targets)>? globalTargets = globalLegal == null ? null :
                BuildDirectionalPlans(context, globalLegal, globalCount, selector);
            Dictionary<Grid, (List<Grid> Grids, List<Character> Targets)> areaCache = [];
            bool reuseArea = anchored && !castSkill!.CanSelectSelf;
            context.Metrics.PositionsGenerated += positions.Count;
            Dictionary<long, double> risks = [];
            double Risk(Grid grid)
            {
                if (!risks.TryGetValue(grid.Id, out double risk)) risks[grid.Id] = risk = EstimateRisk(context, grid);
                return risk;
            }
            IEnumerable<Grid> ordered = action.PositionPreference == "safe" || action.Type == "retreat"
                ? positions.OrderBy(Risk).ThenBy(g => g.Id == current?.Id ? 0 : 1).ThenBy(g => g.Id)
                : positions.OrderBy(g => g.Id == current?.Id ? 0 : 1).ThenBy(g => NearestTargetDistance(context, g, selector)).ThenBy(g => g.Id);
            List<DecisionPlan> plans = [];
            int remaining = Math.Max(0, context.PlanBudget - context.Metrics.PlansEvaluated);
            List<Grid> evaluatedPositions = [.. ordered.Take(Math.Max(1, context.PositionBudget))];
            if (current != null && action.Movement != "required" && !evaluatedPositions.Contains(current)) evaluatedPositions[^1] = current;
            foreach (Grid position in evaluatedPositions)
            {
                if (plans.Count >= remaining) break;
                if (action.Movement == "required" && position.Id == current?.Id) continue;
                double risk = map == null || action.MaxRisk == null && action.PositionPreference != "safe" && action.Type != "retreat" ? 0 : Risk(position);
                if (action.MaxRisk is double limit && risk > limit) continue;
                int steps = map == null || current == null ? 0 : context.GetMoveSteps(actor, current, position);
                context.Metrics.PositionsEvaluated++;
                if (movementOnly)
                {
                    if (context.IsChainAction || position.Id == current?.Id) continue;
                    Character? target = OrderTargets(context, RelatedTargets(context, selector).Where(c => c.HP > 0 && !c.IsUnselectable && map?.GetCharacterCurrentGrid(c) != null), selector).FirstOrDefault();
                    if (action.Type == "approach" && (target == null || NearestTargetDistance(context, position, selector) >= NearestTargetDistance(context, current!, selector))) continue;
                    if (action.Type == "retreat" && risk >= Risk(current!)) continue;
                    plans.Add(new() { Decision = new() { ActionType = CharacterActionType.Move, TargetMoveGrid = position, IsPureMove = true, HasCandidate = true, RequiresValidation = true }, Target = target, Risk = risk, MoveSteps = steps });
                    continue;
                }
                int range = action.Type == "normalAttack" ? actor.ATR : castSkill!.CastRange;
                if (castSkill is { IsNonDirectional: true } && map != null)
                {
                    IEnumerable<Grid> candidates = anchoredCenters == null ? context.GetRange(position, range, true) :
                        anchoredCenters.Concat(castSkill.CanSelectSelf ? BuildAnchoredCenters(context, castSkill, position, true) : [])
                            .Distinct().Where(g => context.IsInRange(position, g, range));
                    List<Grid> centers = [.. candidates];
                    context.Metrics.CentersGenerated += centers.Count;
                    foreach (Grid center in centers.OrderBy(g => NearestTargetDistance(context, g, selector)).ThenBy(g => g.Id).Take(Math.Max(1, context.CenterBudget)))
                    {
                        if (plans.Count >= remaining) break;
                        if (!reuseArea || !areaCache.TryGetValue(center, out (List<Grid> Grids, List<Character> Targets) projection))
                        {
                            List<Grid> projected = position.Id == current?.Id
                                ? castSkill.SelectNonDirectionalTargets(actor, center, castSkill.SelectIncludeCharacterGrid)
                                : castSkill.SelectNonDirectionalTargetsFrom(actor, position, center, castSkill.SelectIncludeCharacterGrid);
                            List<Character> targets = castSkill.SelectTargetsByRangeFrom(actor, position, [.. context.Enemies], [.. context.Allies], projected);
                            projection = (projected, [.. targets.Where(c => c == actor || !c.IsUnselectable).Distinct()]);
                            if (reuseArea) areaCache[center] = projection;
                            context.Metrics.CentersEvaluated++;
                        }
                        List<Grid> grids = projection.Grids;
                        List<Character> affected = [.. projection.Targets];
                        if (actor.CharacterState == CharacterState.BattleRestricted) affected = [.. affected.Where(c => c == actor)];
                        if (affected.Count == 0 && !castSkill.AllowSelectNoCharacterGrid) continue;
                        Character? target = OrderTargets(context, affected.Intersect(RelatedTargets(context, selector)), selector).FirstOrDefault();
                        if (selector != null && target == null) continue;
                        plans.Add(CreatePlan(action, ability!, item, position, target, affected, grids, center, risk, steps));
                    }
                }
                else
                {
                    List<(Character Target, List<Character> Targets)> targetsForPosition;
                    if (globalTargets != null) targetsForPosition = globalTargets;
                    else
                    {
                        (List<Character> legal, int count) = DirectionalTargets(context, ability!, position, range, directionalPool);
                        targetsForPosition = BuildDirectionalPlans(context, legal, count, selector);
                    }
                    foreach ((Character target, List<Character> targets) in targetsForPosition)
                    {
                        if (plans.Count >= remaining) break;
                        plans.Add(CreatePlan(action, ability!, item, position, target, [.. targets], [], null, risk, steps));
                    }
                }
            }
            context.Metrics.PlansEvaluated += plans.Count;
            return plans;
        }

        private static (List<Character> Legal, int Count) DirectionalTargets(DecisionEvaluationContext context, ISkill ability, Grid position, int range, List<Character>? pool = null)
        {
            GameMap? map = context.Queue.Map;
            bool Reachable(Character character) => !character.IsUnselectable && (map == null ||
                map.GetCharacterCurrentGrid(character) is Grid target && target.Characters.Contains(character) && context.IsInRange(position, target, range));
            List<Character> enemies = [.. context.SelectableEnemies.Where(Reachable)];
            List<Character> allies = [.. context.SelectableAllies.Where(Reachable)];
            List<Character> legal = pool == null ? ability.GetSelectableTargets(context.Actor, [.. context.Enemies], [.. context.Allies], enemies, allies) :
                [.. pool.Where(c => c == context.Actor || enemies.Contains(c) || allies.Contains(c) || ability is Skill { AllowSelectDead: true } && c.HP == 0)];
            if (context.Actor.CharacterState == CharacterState.BattleRestricted) legal = [.. legal.Where(c => c == context.Actor)];
            return (legal, ability.RealCanSelectTargetCount(enemies, allies));
        }

        private static List<(Character Target, List<Character> Targets)> BuildDirectionalPlans(DecisionEvaluationContext context,
            List<Character> legal, int count, TargetSelectorConfig? selector)
        {
            if (count <= 0) return [];
            List<(Character, List<Character>)> plans = [];
            foreach (Character target in OrderTargets(context, legal.Intersect(RelatedTargets(context, selector)), selector))
                plans.Add((target, [target, .. legal.Where(c => c != target).OrderBy(c => c.Guid).Take(count - 1)]));
            return plans;
        }

        private static HashSet<Grid> BuildAnchoredCenters(DecisionEvaluationContext context, Skill skill, Grid position, bool selfOnly)
        {
            HashSet<Grid> centers = [];
            IEnumerable<Character> anchors = selfOnly ? [context.Actor] :
                (skill.CanSelectEnemy ? context.Enemies : []).Concat(skill.CanSelectTeammate ? context.Allies : []);
            foreach (Character anchor in anchors.Distinct())
            {
                Grid? target = anchor == context.Actor ? position : context.Queue.Map!.GetCharacterCurrentGrid(anchor);
                if (target != null) centers.UnionWith(skill.SelectNonDirectionalTargetsFrom(context.Actor, position, target, true));
            }
            return centers;
        }

        /// <summary>检查状态、资源、持有实例、预算与配额。</summary>
        /// <param name="context">局面。</param>
        /// <param name="type">配置动作标识。</param>
        /// <param name="skill">当前技能实例。</param>
        /// <param name="item">当前物品实例。</param>
        /// <returns>是否可用。</returns>
        public static bool CanUse(DecisionEvaluationContext context, string type, Skill? skill = null, Item? item = null)
        {
            Character actor = context.Actor;
            CharacterState state = actor.CharacterState;
            if (actor.HP <= 0 || state is CharacterState.NotActionable or CharacterState.Casting or CharacterState.PreCastSuperSkill) return false;
            CharacterActionType quota;
            if (type == "normalAttack")
            {
                if (!context.AllowAttack || state is CharacterState.ActionRestricted or CharacterState.BattleRestricted or CharacterState.AttackRestricted) return false;
                quota = CharacterActionType.NormalAttack;
            }
            else if (type == "useItem")
            {
                if (!context.AllowItem || item == null || !context.Items.Contains(item) || !item.Enable ||
                    !item.IsActive || !item.IsInGameItem || item.Skills.Active is not Skill active || active.SkillType != SkillType.Item || !active.Enable || active.IsInEffect || active.CurrentCD != 0 ||
                    item.IsReduceTimesAfterUse && item.RemainUseTimes <= 0 || state == CharacterState.ActionRestricted && item.ItemType != ItemType.Consumable) return false;
                if (!HasResources(actor, active, true)) return false;
                skill = active;
                quota = CharacterActionType.UseItem;
            }
            else
            {
                if (!context.AllowSkill || skill == null || !context.Skills.Contains(skill) || !skill.Enable || skill.CurrentCD != 0 || skill.IsInEffect ||
                    skill.SkillType is not (SkillType.Magic or SkillType.Skill or SkillType.SuperSkill) ||
                    state is CharacterState.ActionRestricted or CharacterState.BattleRestricted || state == CharacterState.SkillRestricted && skill.Source != SkillSource.Reward || !HasResources(actor, skill, false)) return false;
                quota = skill.SkillType switch { SkillType.Magic => CharacterActionType.PreCastSkill, SkillType.SuperSkill => CharacterActionType.CastSuperSkill, _ => CharacterActionType.CastSkill };
                if (skill is CourageCommandSkill) return !context.Points.CourageCommandSkill;
            }
            return context.Points.CheckActionTypeQuota(quota) && context.Points.CurrentDecisionPoints >= context.Points.GetActionPointCost(quota, skill, context.IsChainAction);
        }

        private static bool HasResources(Character actor, Skill skill, bool item) =>
            item ? skill.RealMPCost >= 0 && skill.RealEPCost >= 0 && skill.RealMPCost <= actor.MP && skill.RealEPCost <= actor.EP :
            skill.SkillType == SkillType.Magic ? skill.RealMPCost >= 0 && skill.RealMPCost <= actor.MP : skill.RealEPCost >= 0 && skill.RealEPCost <= actor.EP;

        private static DecisionPlan CreatePlan(ActionConfig action, ISkill ability, Item? item, Grid position, Character? target, List<Character> targets, List<Grid> grids, Grid? center, double risk, int steps) => new()
        {
            Target = target, Center = center, SelectedGrids = center == null ? [] : [center], Risk = risk, MoveSteps = steps,
            Decision = new() { ActionType = action.Type == "normalAttack" ? CharacterActionType.NormalAttack : item != null ? CharacterActionType.UseItem : CharacterActionType.PreCastSkill,
                SkillToUse = ability, ItemToUse = item, Targets = targets, TargetGrids = grids, CastCenter = center,
                TargetMoveGrid = position.Id < 0 ? null : position, HasCandidate = true, RequiresValidation = true }
        };

        private static IEnumerable<Character> RelatedTargets(DecisionEvaluationContext context, TargetSelectorConfig? selector) => selector?.Relation switch
        {
            "self" => [context.Actor], "ally" => context.Allies.Append(context.Actor), "enemy" => context.Enemies,
            _ => context.Enemies.Concat(context.Allies).Append(context.Actor)
        };

        private static IOrderedEnumerable<Character> OrderTargets(DecisionEvaluationContext context, IEnumerable<Character> targets, TargetSelectorConfig? selector) =>
            targets.Distinct().OrderBy(c => selector?.Selector switch
            {
                "nearest" => context.Queue.Map?.GetCharacterCurrentGrid(context.Actor) is Grid origin && context.Queue.Map.GetCharacterCurrentGrid(c) is Grid grid ? GameMap.CalculateManhattanDistance(origin, grid) : 0,
                "highestHpRatio" => -HpRatio(c), _ => HpRatio(c)
            }).ThenBy(c => c.Guid);

        private static double HpRatio(Character actor) => actor.MaxHP > 0 ? actor.HP / actor.MaxHP : 0;

        private static int NearestTargetDistance(DecisionEvaluationContext context, Grid position, TargetSelectorConfig? selector)
        {
            (string?, int) key = (selector?.Relation, position.Id);
            if (context.TargetDistanceCache.TryGetValue(key, out int distance)) return distance;
            int nearest = int.MaxValue;
            bool found = false;
            foreach (Character target in RelatedTargets(context, selector))
            {
                Grid? grid = context.Queue.Map?.GetCharacterCurrentGrid(target);
                if (grid == null) continue;
                found = true;
                nearest = Math.Min(nearest, GameMap.CalculateManhattanDistance(position, grid));
            }
            return context.TargetDistanceCache[key] = found ? nearest : 0;
        }

        /// <summary>按敌方移动与攻击/施法射程估计威胁，不预测隐藏技能效果。</summary>
        /// <param name="context">当前局面。</param>
        /// <param name="position">候选位置。</param>
        /// <returns>能够威胁该位置的敌人数量。</returns>
        public static double EstimateRisk(DecisionEvaluationContext context, Grid position)
        {
            GameMap? map = context.Queue.Map;
            if (map == null) return 0;
            double risk = 0;
            foreach (Character enemy in context.Enemies.Where(c => c.HP > 0 && c.CharacterState != CharacterState.NotActionable))
            {
                Grid? origin = map.GetCharacterCurrentGrid(enemy);
                if (origin == null) continue;
                int range = Math.Max(enemy.ATR, enemy.Skills.Where(s => s.Enable && s.CurrentCD == 0 && s.CanSelectEnemy).Select(s => s.CastRange).DefaultIfEmpty(0).Max());
                if (context.IsInRange(origin, position, range))
                {
                    risk++;
                    continue;
                }
                if (!context.ThreatCache.TryGetValue(enemy, out HashSet<Grid>? threat))
                {
                    threat = [.. context.GetRange(origin, range, true)];
                    foreach (Grid candidate in context.GetRange(origin, enemy.MOV, false))
                        if (context.GetMoveSteps(enemy, origin, candidate) >= 0) threat.UnionWith(context.GetRange(candidate, range, true));
                    context.ThreatCache[enemy] = threat;
                }
                if (threat.Contains(position)) risk++;
            }
            return risk;
        }
    }
}
