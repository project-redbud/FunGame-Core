using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Library.Common.Addon.Example
{
    /// <summary>
    /// 非指向性技能示例1：迷踪步<para/>
    /// 立即将角色传送到范围内的任意一个未被角色占据的地点<para/>
    /// 类型：战技
    /// </summary>
    public class ExampleNonDirectionalSkill1 : Skill
    {
        public override long Id => 1;
        public override string Name => "迷踪步";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => string.Join("", Effects.Select(e => e.DispelDescription));
        public override double EPCost => 25;
        public override double CD => 35 - 1.5 * Level;
        public override double HardnessTime { get; set; } = 3;
        public override bool IsNonDirectional => true;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => false;
        public override int CanSelectTargetRange => 0;
        public override bool SelectIncludeCharacterGrid => false;
        public override bool AllowSelectNoCharacterGrid => true;

        public ExampleNonDirectionalSkill1(Character? character = null) : base(SkillType.Skill, character)
        {
            CastRange = 9;
            // 所有的技能特效，如果能直接 new，建议就直接 new，提高性能和可读性（工厂效率低且不好调试，工厂更偏向于动态创建技能，而对于编码实现的技能来说，怎么简单怎么来）
            Effects.Add(new ExampleNonDirectionalSkill1Effect(this));
        }
    }

    /// <summary>
    /// 注意：特效包含于技能之中，多个特效组合成一个技能
    /// </summary>
    /// <param name="skill"></param>
    public class ExampleNonDirectionalSkill1Effect(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"立即将角色传送到范围内的任意{Skill.TargetDescription()}。";
        public override string DispelDescription => "";

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            // 只有开启了地图模式才有效
            if (GamingQueue?.Map is GameMap map && grids.Count > 0)
            {
                map.CharacterMove(caster, map.GetCharacterCurrentGrid(caster), grids[0]);
            }
        }
    }

    /// <summary>
    /// 主动技能特效示例：基于攻击力的伤害（带基础伤害）
    /// </summary>
    public class ExampleDamageBasedOnATKWithBasicDamage : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{Skill.TargetDescription()}造成 {BaseDamage:0.##} + {ATKCoefficient * 100:0.##}% 攻击力 [ {Damage:0.##} ] 点{CharacterSet.GetDamageTypeName(DamageType, MagicType)}。";

        private double BaseDamage => Skill.Level > 0 ? BaseNumericDamage + BaseNumericDamageLevelGrowth * (Skill.Level - 1) : BaseNumericDamage;
        private double ATKCoefficient => Skill.Level > 0 ? BaseATKCoefficient + BaseATKCoefficientLevelGrowth * (Skill.Level - 1) : BaseATKCoefficient;
        private double Damage => BaseDamage + (ATKCoefficient * Skill.Character?.ATK ?? 0);
        private double BaseNumericDamage { get; set; } = 100;
        private double BaseNumericDamageLevelGrowth { get; set; } = 50;
        private double BaseATKCoefficient { get; set; } = 0.2;
        private double BaseATKCoefficientLevelGrowth { get; set; } = 0.2;
        private DamageType DamageType { get; set; } = DamageType.Magical;

        public ExampleDamageBasedOnATKWithBasicDamage(Skill skill, double baseNumericDamage, double baseNumericDamageLevelGrowth, double baseATKCoefficient, double baseATKCoefficientLevelGrowth, DamageType damageType = DamageType.Magical, MagicType magicType = MagicType.None) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            BaseNumericDamage = baseNumericDamage;
            BaseNumericDamageLevelGrowth = baseNumericDamageLevelGrowth;
            BaseATKCoefficient = baseATKCoefficient;
            BaseATKCoefficientLevelGrowth = baseATKCoefficientLevelGrowth;
            DamageType = damageType;
            MagicType = magicType;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character enemy in targets)
            {
                DamageToEnemy(caster, enemy, DamageType, MagicType, Damage);
            }
            // 或者：
            //double damage = Damage;
            //foreach (Character enemy in targets)
            //{
            //    DamageToEnemy(caster, enemy, DamageType, MagicType, damage);
            //}
        }
    }

    /// <summary>
    /// 非指向性技能示例2：钻石星尘<para/>
    /// 对半径为 2 格的圆形区域造成魔法伤害<para/>
    /// 类型：魔法
    /// </summary>
    public class ExampleNonDirectionalSkill2 : Skill
    {
        public override long Id => 2;
        public override string Name => "钻石星尘";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override double MPCost => Level > 0 ? 80 + (75 * (Level - 1)) : 80;
        public override double CD => Level > 0 ? 35 + (2 * (Level - 1)) : 35;
        public override double CastTime => 9;
        public override double HardnessTime { get; set; } = 6;
        public override int CanSelectTargetCount
        {
            get
            {
                return Level switch
                {
                    4 or 5 or 6 => 2,
                    7 or 8 => 3,
                    _ => 1
                };
            }
        }
        public override bool IsNonDirectional => true;
        public override SkillRangeType SkillRangeType => SkillRangeType.Circle;
        public override int CanSelectTargetRange => 2;
        public override double MagicBottleneck => 35 + 24 * (Level - 1);

        public ExampleNonDirectionalSkill2(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new ExampleDamageBasedOnATKWithBasicDamage(this, 20, 20, 0.03, 0.02, DamageType.Magical));
        }
    }

    /// <summary>
    /// 指向性技能示例：全力一击<para/>
    /// 对目标造成物理伤害并打断施法<para/>
    /// 类型：战技
    /// </summary>
    public class ExampleSkill : Skill
    {
        public override long Id => 3;
        public override string Name => "全力一击";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => string.Join("", Effects.Select(e => e.Description));
        public override string ExemptionDescription => Effects.Count > 0 ? Effects.First(e => e is ExampleInterruptCastingEffect).ExemptionDescription : "";
        public override double EPCost => 60;
        public override double CD => 20;
        public override double HardnessTime { get; set; } = 8;
        // 豁免检定有两种方式，通过直接设置技能的属性可自动触发豁免，但是豁免成功让整个技能都失效，包括伤害（或其他 Effects），另一种方式比较安全，但需要手动调用方法，看下面
        public override Effect? EffectForExemptionCheck => Effects.FirstOrDefault(e => e is ExampleInterruptCastingEffect);

        public ExampleSkill(Character? character = null) : base(SkillType.Skill, character)
        {
            Effects.Add(new ExampleDamageBasedOnATKWithBasicDamage(this, 65, 65, 0.09, 0.04, DamageType.Physical));
            Effects.Add(new ExampleInterruptCastingEffect(this));
        }
    }

    public class ExampleInterruptCastingEffect : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{Skill.TargetDescription()}施加打断施法效果：中断其正在进行的吟唱。";
        public override EffectType EffectType => EffectType.InterruptCasting;

        public ExampleInterruptCastingEffect(Skill skill) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                // 这是另一种豁免检定方式，在技能实现时，自行调用 CheckExemption，只对该特效有效
                if (!CheckExemption(caster, target, this))
                {
                    InterruptCasting(target, caster);
                }
            }
        }
    }

    /// <summary>
    /// 被动技能示例：心灵之弦
    /// </summary>
    public class ExamplePassiveSkill : Skill
    {
        public override long Id => 4;
        public override string Name => "心灵之弦";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public ExamplePassiveSkill(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new ExamplePassiveSkillEffect(this));
        }

        /// <summary>
        /// 特别注意：被动技能必须重写此方法，否则它不会自动添加到角色身上
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class ExamplePassiveSkillEffect(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"普通攻击硬直时间减少 20%。每次使用普通攻击时，额外再发动一次普通攻击，伤害特效可叠加，但伤害折减一半，冷却 {CD:0.##} {GameplayEquilibriumConstant.InGameTime}。额外普通攻击立即发动，不占用决策点配额。" +
            (CurrentCD > 0 ? $"（正在冷却：剩余 {CurrentCD:0.##} {GameplayEquilibriumConstant.InGameTime}）" : "");

        /// <summary>
        /// 被动技能的冷却时间可以借用技能的冷却时间（<see cref="Skill.CD"/> 等属性）实现，也可以内部实现，看喜好
        /// </summary>
        public double CurrentCD { get; set; } = 0;
        public double CD { get; set; } = 10;
        private bool IsNested = false;

        // 该钩子属于伤害计算流程的特效乘区2
        public override double AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult, ref bool isEvaded, Dictionary<Effect, double> totalDamageBonus)
        {
            if (character == Skill.Character && IsNested && isNormalAttack && damage > 0)
            {
                // 此方法返回的是加值
                return -(damage / 2);
            }
            return 0;
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && isNormalAttack && CurrentCD == 0 && !IsNested && GamingQueue != null && enemy.HP > 0)
            {
                WriteLine($"[ {character} ] 发动了{Skill.Name}！额外进行一次普通攻击！");
                CurrentCD = CD;
                IsNested = true;
                character.NormalAttack.Attack(GamingQueue, character, null, enemy);
            }

            if (character == Skill.Character && IsNested)
            {
                IsNested = false;
            }
        }

        public override void OnTimeElapsed(Character character, double elapsed)
        {
            // 时间流逝时，手动减少CD。如果借用了技能的冷却时间属性，就不需要写了
            if (CurrentCD > 0)
            {
                CurrentCD -= elapsed;
                if (CurrentCD <= 0)
                {
                    CurrentCD = 0;
                }
            }
        }

        public override void AlterHardnessTimeAfterNormalAttack(Character character, ref double baseHardnessTime, ref bool isCheckProtected)
        {
            // 普攻后调整硬直时间。ref 变量直接修改
            baseHardnessTime *= 0.8;
        }
    }

    /// <summary>
    /// 爆发技示例：千羽瞬华<para/>
    /// 给自己加属性，并联动其他技能
    /// </summary>
    public class ExampleSuperSkill : Skill
    {
        public override long Id => 5;
        public override string Name => "千羽瞬华";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 60;
        public override double HardnessTime { get; set; } = 10;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public ExampleSuperSkill(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new ExampleSuperSkillEffect(this));
        }
    }

    public class ExampleSuperSkillEffect(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Duration:0.##} {GameplayEquilibriumConstant.InGameTime}内，增加{Skill.SkillOwner()} {ATKMultiplier * 100:0.##}% 攻击力 [ {ATKBonus:0.##} ]、{PhysicalPenetrationBonus * 100:0.##}% 物理穿透和 {EvadeRateBonus * 100:0.##}% 闪避率（不可叠加），普通攻击硬直时间额外减少 20%，基于 {Coefficient * 100:0.##}% 敏捷 [ {DamageBonus:0.##} ] 强化普通攻击的伤害。在持续时间内，[ 心灵之弦 ] 的冷却时间降低至 3 {GameplayEquilibriumConstant.InGameTime}。";
        public override bool Durative => true;
        public override double Duration => 30;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private double Coefficient => 1.2 * (1 + 0.5 * (Skill.Level - 1));
        private double DamageBonus => Coefficient * Skill.Character?.AGI ?? 0;
        private double ATKMultiplier => Skill.Level > 0 ? 0.15 + 0.03 * (Skill.Level - 1) : 0.15;
        private double ATKBonus => ATKMultiplier * Skill.Character?.BaseATK ?? 0;
        private double PhysicalPenetrationBonus => Skill.Level > 0 ? 0.1 + 0.03 * (Skill.Level - 1) : 0.1;
        private double EvadeRateBonus => Skill.Level > 0 ? 0.1 + 0.02 * (Skill.Level - 1) : 0.1;

        // 用于保存状态和恢复
        private double ActualATKBonus = 0;
        private double ActualPhysicalPenetrationBonus = 0;
        private double ActualEvadeRateBonus = 0;

        public override void OnEffectGained(Character character)
        {
            // 记录状态并修改属性
            ActualATKBonus = ATKBonus;
            ActualPhysicalPenetrationBonus = PhysicalPenetrationBonus;
            ActualEvadeRateBonus = EvadeRateBonus;
            character.ExATK2 += ActualATKBonus;
            character.PhysicalPenetration += ActualPhysicalPenetrationBonus;
            character.ExEvadeRate += ActualEvadeRateBonus;
            if (character.Effects.FirstOrDefault(e => e is ExamplePassiveSkillEffect) is ExamplePassiveSkillEffect e)
            {
                e.CD = 3;
                if (e.CurrentCD > e.CD) e.CurrentCD = e.CD;
            }
        }

        public override void OnEffectLost(Character character)
        {
            // 从记录的状态中恢复
            character.ExATK2 -= ActualATKBonus;
            character.PhysicalPenetration -= ActualPhysicalPenetrationBonus;
            character.ExEvadeRate -= ActualEvadeRateBonus;
            if (character.Effects.FirstOrDefault(e => e is ExamplePassiveSkillEffect) is ExamplePassiveSkillEffect e)
            {
                e.CD = 10;
            }
        }

        public override CharacterActionType AlterActionTypeBeforeAction(Character character, DecisionPoints dp, CharacterState state, ref bool canUseItem, ref bool canCastSkill, ref double pUseItem, ref double pCastSkill, ref double pNormalAttack, ref bool forceAction)
        {
            // 对于 AI，可以提高角色的普攻积极性，调整决策偏好，这样可以充分利用技能效果
            pNormalAttack += 0.1;
            return CharacterActionType.None;
        }

        public override double AlterExpectedDamageBeforeCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, Dictionary<Effect, double> totalDamageBonus)
        {
            if (character == Skill.Character && isNormalAttack)
            {
                return DamageBonus;
            }
            return 0;
        }

        public override void AlterHardnessTimeAfterNormalAttack(Character character, ref double baseHardnessTime, ref bool isCheckProtected)
        {
            // 可以和上面的心灵之弦叠加，最终硬直时间=硬直时间*0.8*0.8
            baseHardnessTime *= 0.8;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            ActualATKBonus = 0;
            ActualPhysicalPenetrationBonus = 0;
            ActualEvadeRateBonus = 0;
            // 不叠加的效果通常只刷新持续时间
            RemainDuration = Duration;
            // 通常，this（Effect本身）在整局战斗中都是唯一的，需要只需要判断 this 就行
            if (!caster.Effects.Contains(this))
            {
                // 加也是加 this
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            // 施加状态记录到回合日志中
            RecordCharacterApplyEffects(caster, EffectType.DamageBoost, EffectType.PenetrationBoost);
        }
    }
}
