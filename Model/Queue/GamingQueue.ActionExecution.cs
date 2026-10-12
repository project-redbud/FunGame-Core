using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.Queue
{
    public partial class GamingQueue
    {
        private void ExecuteNormalAttackEffects(Character actor, DecisionPoints dp, List<Character> targets, ref double hardness, ref bool checkProtected)
        {
            OnCharacterNormalAttackEvent(new NormalAttackContext(this, actor, dp) { Targets = targets });
            actor.NormalAttack.Attack(this, actor, null, targets);
            TriggerAfterCharacterNormalAttack(actor, new NormalAttackContext(this, actor, dp)
            {
                NormalAttack = actor.NormalAttack, Targets = targets
            });
            hardness += actor.NormalAttack.RealHardnessTime;
            HardnessContext ctx = new(this, actor) { BaseHardnessTime = hardness, IsCheckProtected = checkProtected };
            TriggerAlterHardnessTimeAfterNormalAttack(actor, ctx);
            hardness = ctx.BaseHardnessTime;
            checkProtected = ctx.IsCheckProtected;
        }

        private void ExecuteSkillEffects(Character actor, DecisionPoints dp, SkillTarget target, double cost, ref double hardness, ref bool checkProtected)
        {
            Skill skill = target.Skill;
            List<Character> targets = target.Targets;
            List<Grid> grids = target.TargetGrids;
            skill.BeforeSkillCasted(actor, targets, grids);
            bool magic = skill.SkillType == SkillType.Magic;
            if (magic) actor.MP -= cost;
            else actor.EP -= cost;
            hardness += skill.RealHardnessTime;
            skill.CurrentCD = skill.RealCD;
            skill.Enable = false;
            string costText = $"{-cost:0.##} {(magic ? "MP" : "EP")}";
            LastRound.SkillsCost[skill] = costText;
            if (_currentAction != null)
            {
                _currentAction.Cost = costText;
                _currentAction.MPCost = magic ? cost : 0;
                _currentAction.EPCost = magic ? 0 : cost;
                _currentAction.SkillCD = skill.RealCD;
            }
            WriteLine($"[ {actor} ] 消耗了 {cost:0.##} 点{(magic ? "魔法值" : "能量")}，释放了 [ {skill.Name} ]！{skill.Slogan}");
            OnCharacterCastSkillEvent(new SkillCastContext(this, actor)
            {
                DP = dp, SkillTarget = target, Skill = skill, Targets = targets, Grids = grids, Cost = cost
            });
            skill.OnSkillCasted(this, actor, targets, grids);
            skill.AfterSkillCasted(actor, targets, grids);
            TriggerAfterCharacterCastSkill(actor, skill, targets);
            HardnessContext ctx = new(this, actor) { Skill = skill, BaseHardnessTime = hardness, IsCheckProtected = checkProtected };
            TriggerAlterHardnessTimeAfterCastSkill(actor, ctx);
            hardness = ctx.BaseHardnessTime;
            checkProtected = ctx.IsCheckProtected;
        }

        private void ExecuteItemEffects(Item item, Character character, DecisionPoints dp, Skill skill, List<Character> targets, List<Grid> grids, double costMP, double costEP)
        {
            WriteLine($"[ {character} ] 使用了物品 [ {item.Name} ]！");
            item.ReduceTimesAndRemove();
            if (item.IsReduceTimesAfterUse && item.RemainUseTimes == 0)
            {
                character.Items.Remove(item);
            }
            OnCharacterUseItemEvent(new ItemUseContext(this, character, dp) { Item = item, Skill = skill, Targets = targets });

            skill.OnSkillCasting(this, character, targets, grids);

            TriggerAfterCharacterStartCasting(character, skill, targets);

            skill.BeforeSkillCasted(character, targets, grids);

            skill.CurrentCD = skill.RealCD;
            skill.Enable = false;

            string line = $"[ {character} ] ";
            if (costMP > 0)
            {
                character.MP -= costMP;
                LastRound.ItemsCost[item] = $"{-costMP:0.##} MP";
                if (_currentAction != null)
                {
                    _currentAction.Cost = $"{-costMP:0.##} MP";
                    _currentAction.MPCost = costMP;
                }
                line += $"消耗了 {costMP:0.##} 点魔法值，";
            }

            if (costEP > 0)
            {
                character.EP -= costEP;
                if (LastRound.ItemsCost.GetValueOrDefault(item, "") != "") LastRound.ItemsCost[item] += " / ";
                else LastRound.ItemsCost[item] = "";
                LastRound.ItemsCost[item] += $"{-costEP:0.##} EP";
                if (_currentAction != null)
                {
                    _currentAction.Cost += (_currentAction.Cost != "" ? " / " : "") + $"{-costEP:0.##} EP";
                    _currentAction.EPCost = costEP;
                }
                line += $"消耗了 {costEP:0.##} 点能量，";
            }

            line += $"释放了物品技能 [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}";
            WriteLine(line);

            SkillTarget skillTarget = new(skill, targets, grids);
            OnCharacterCastItemSkillEvent(new SkillCastContext(this, character) { DP = dp, SkillTarget = skillTarget, Skill = skill, Item = item, Targets = targets, Grids = grids, MPCost = costMP, EPCost = costEP });

            skill.OnSkillCasted(this, character, targets, grids);

            skill.AfterSkillCasted(character, targets, grids);

            TriggerAfterCharacterCastSkill(character, skill, targets);

            TriggerAfterCharacterUseItem(character, dp, item, skill, targets);
        }
    }
}
