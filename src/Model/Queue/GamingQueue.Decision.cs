using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Interface.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.Queue
{
    public partial class GamingQueue
    {
        private bool RejectInvalidAIDecision(Character actor, DecisionPoints points, AIDecision? decision, HashSet<string> rejectedRules, bool beforeMove = false, Grid? turnOrigin = null)
        {
            if (ValidateAIDecision(actor, points, decision, beforeMove, turnOrigin)) return false;
            if (decision?.RuleId is string rule) rejectedRules.Add(rule);
            return true;
        }

        private bool ValidateAIDecision(Character actor, DecisionPoints points, AIDecision? decision, bool beforeMove = false, Grid? turnOrigin = null)
        {
            if (decision == null || !decision.RequiresValidation || decision.ActionType == CharacterActionType.EndTurn) return true;
            List<Character> allies = GetTeammates(actor);
            List<Character> enemies = GetEnemies(actor);
            (List<Character> selectableAllies, List<Character> selectableEnemies, List<Skill> skills, List<Item> items) = GetTurnStartNeedyList(actor, allies, enemies);
            DecisionEvaluationContext context = new()
            {
                Queue = this, Actor = actor, Points = points, Allies = allies, Enemies = enemies, Skills = skills, Items = items,
                SelectableAllies = selectableAllies, SelectableEnemies = selectableEnemies
            };
            Grid? origin = _map?.GetCharacterCurrentGrid(actor);
            Grid? position = beforeMove ? decision.TargetMoveGrid ?? origin : origin;
            if (_map != null)
            {
                if (origin == null || position == null) return false;
                if (beforeMove && position.Id != origin.Id && (position.Characters.Count > 0 ||
                    !_map.GetGridsByRange(turnOrigin ?? origin, actor.MOV, false).Contains(position) || _map.GetMovementSteps(actor, origin, position) < 0)) return false;
                if (!beforeMove && decision.TargetMoveGrid != null && position.Id != decision.TargetMoveGrid.Id) return false;
            }
            if (decision.ActionType == CharacterActionType.Move) return _map != null && actor.CharacterState == CharacterState.Actionable;
            string type = decision.ActionType == CharacterActionType.NormalAttack ? "normalAttack" : decision.ItemToUse != null ? "useItem" : "castSkill";
            Skill? skill = decision.SkillToUse as Skill;
            if (!DecisionActionPlanner.CanUse(context, type, skill, decision.ItemToUse)) return false;
            ISkill ability = decision.ActionType == CharacterActionType.NormalAttack ? actor.NormalAttack : skill!;
            int range = skill?.CastRange ?? actor.NormalAttack.AttackRange;
            bool Reachable(Character character) => _map == null || _map.GetCharacterCurrentGrid(character) is Grid target &&
                target.Characters.Contains(character) && context.IsInRange(position!, target, range);
            List<Character> reachableEnemies = [.. selectableEnemies.Where(Reachable)];
            List<Character> reachableAllies = [.. selectableAllies.Where(c => !c.IsUnselectable && Reachable(c))];
            if (skill is { IsNonDirectional: true } && _map != null)
            {
                if (decision.CastCenter == null || !context.IsInRange(position!, decision.CastCenter, range)) return false;
                List<Grid> grids = position!.Id == origin!.Id ? skill.SelectNonDirectionalTargets(actor, decision.CastCenter, skill.SelectIncludeCharacterGrid) :
                    skill.SelectNonDirectionalTargetsFrom(actor, position, decision.CastCenter, skill.SelectIncludeCharacterGrid);
                if (!grids.ToHashSet().SetEquals(decision.TargetGrids)) return false;
                List<Character> targets = skill.SelectTargetsByRangeFrom(actor, position, enemies, allies, grids);
                targets = [.. targets.Where(c => c == actor || !c.IsUnselectable).Distinct()];
                return targets.ToHashSet().SetEquals(decision.Targets) && (targets.Count > 0 || skill.AllowSelectNoCharacterGrid);
            }
            List<Character> legal = ability.GetSelectableTargets(actor, enemies, allies, reachableEnemies, reachableAllies);
            if (actor.CharacterState == CharacterState.BattleRestricted) legal = [.. legal.Where(c => c == actor)];
            if (ability.SelectAllEnemies || ability.SelectAllTeammates)
                return legal.Count > 0 && decision.Targets.Distinct().Count() == decision.Targets.Count && legal.ToHashSet().SetEquals(decision.Targets);
            return decision.Targets.Count > 0 && decision.Targets.Count <= ability.RealCanSelectTargetCount(reachableEnemies, reachableAllies) && decision.Targets.All(legal.Contains);
        }
    }
}
