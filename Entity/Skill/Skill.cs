using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 与 <see cref="Character"/> 不同，构造技能时，建议继承此类再构造
    /// </summary>
    public class Skill : BaseEntity, ISkill, IActiveEnable
    {
        /// <summary>
        /// 唯一标识符 [ 只有物品技能需要赋值，用于表示与其关联的物品：<see cref="Item.Guid"/> ]
        /// <para>其他情况请保持此属性为 <see cref="Guid.Empty"/></para>
        /// </summary>
        public override Guid Guid { get; set; } = Guid.Empty;

        /// <summary>
        /// 此技能所属的角色
        /// </summary>
        public Character? Character { get; set; } = null;

        /// <summary>
        /// 技能描述
        /// </summary>
        public virtual string Description { get; set; } = "";

        /// <summary>
        /// 技能的通用描述
        /// </summary>
        public virtual string GeneralDescription { get; set; } = "";

        /// <summary>
        /// 驱散性和被驱散性的描述
        /// </summary>
        public virtual string DispelDescription { get; set; } = "";

        /// <summary>
        /// 豁免性的描述
        /// </summary>
        public virtual string ExemptionDescription { get; set; } = "";

        /// <summary>
        /// 释放技能时的口号
        /// </summary>
        public virtual string Slogan { get; set; } = "";

        /// <summary>
        /// 技能等级，等于 0 时可以称之为尚未学习
        /// </summary>
        public int Level
        {
            get
            {
                return Math.Max(0, field);
            }
            set
            {
                int max = SkillSet.GetSkillMaxLevel(SkillType, GameplayEquilibriumConstant);
                field = Math.Min(Math.Max(0, value), max);
                OnLevelUp();
            }
        }

        /// <summary>
        /// 技能类型 [ 此项为最高优先级 ]
        /// </summary>
        [InitRequired]
        public SkillType SkillType { get; set; } = SkillType.Passive;

        /// <summary>
        /// 是否是主动技能 [ 此项为高优先级 ]
        /// </summary>
        public bool IsActive => SkillType != SkillType.Passive;

        /// <summary>
        /// 是否可用 [ 此项为高优先级 ]
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 效果持续生效中 [ 此项为高优先级 ] [ 此项设置为true后不允许再次释放，防止重复释放 ]
        /// </summary>
        public bool IsInEffect { get; set; } = false;

        /// <summary>
        /// 是否是爆发技 [ 此项为高优先级 ]
        /// </summary>
        public bool IsSuperSkill => SkillType == SkillType.SuperSkill;

        /// <summary>
        /// 是否属于魔法 [ <see cref="IsActive"/> 必须为 true ]，反之为战技
        /// </summary>
        public bool IsMagic => SkillType == SkillType.Magic;

        /// <summary>
        /// 是否无视施法距离（全图施法），魔法默认为 true，战技默认为 false
        /// </summary>
        public virtual bool CastAnywhere { get; set; } = false;

        /// <summary>
        /// 施法距离 [ 单位：格 ]
        /// </summary>
        [InitOptional]
        public int CastRange
        {
            get => Math.Max(1, CastAnywhere ? (GamingQueue?.Map != null ? GamingQueue.Map.Grids.Count : 999) : field);
            set => field = Math.Max(1, value);
        } = 3;

        /// <summary>
        /// 可选取自身
        /// </summary>
        public virtual bool CanSelectSelf { get; set; } = false;

        /// <summary>
        /// 可选取敌对角色
        /// </summary>
        public virtual bool CanSelectEnemy { get; set; } = true;

        /// <summary>
        /// 可选取友方角色
        /// </summary>
        public virtual bool CanSelectTeammate { get; set; } = false;

        /// <summary>
        /// 选取所有敌对角色，优先级大于 <see cref="CanSelectTargetCount"/>
        /// </summary>
        public virtual bool SelectAllEnemies { get; set; } = false;

        /// <summary>
        /// 选取所有友方角色，优先级大于 <see cref="CanSelectTargetCount"/>，默认包含自身
        /// </summary>
        public virtual bool SelectAllTeammates { get; set; } = false;

        /// <summary>
        /// 可选取的作用目标数量
        /// </summary>
        public virtual int CanSelectTargetCount { get; set; } = 1;

        /// <summary>
        /// 可选取的作用范围 [ 单位：格 ]
        /// </summary>
        public virtual int CanSelectTargetRange { get; set; } = 0;

        /// <summary>
        /// 如果为 true，表示非指向性技能，可以任意选取一个范围（<see cref="CanSelectTargetRange"/> = 0 时为单个格子）。<para/>
        /// 如果为 false，表示必须选取一个角色作为目标，当 <see cref="CanSelectTargetRange"/> > 0 时，技能作用范围将根据目标位置覆盖 <see cref="SkillRangeType"/> 形状的区域；= 0 时正常选取目标。
        /// </summary>
        public virtual bool IsNonDirectional { get; set; } = false;

        /// <summary>
        /// 在非指向性技能选取目标格子时，包括有角色的格子，默认为 true。仅 <see cref="IsNonDirectional"/> = true 时有效。<para/>
        /// 当此项为 false 时，必须设置 <see cref="AllowSelectNoCharacterGrid"/> = true，否则实际施法时会被拒绝。
        /// </summary>
        public virtual bool SelectIncludeCharacterGrid { get; set; } = true;

        /// <summary>
        /// 是否可以选择没有被角色占据的空地，为 false 时会阻止施法。仅 <see cref="IsNonDirectional"/> = true 时有效。<para/>
        /// </summary>
        public virtual bool AllowSelectNoCharacterGrid { get; set; } = false;

        /// <summary>
        /// 是否可以选择已死亡的角色。仅 <see cref="IsNonDirectional"/> = true 时有效。
        /// </summary>
        public virtual bool AllowSelectDead { get; set; } = false;

        /// <summary>
        /// 作用范围形状<para/>
        /// <see cref="SkillRangeType.Diamond"/> - 菱形。默认的曼哈顿距离正方形<para/>
        /// <see cref="SkillRangeType.Circle"/> - 圆形。基于欧几里得距离的圆形<para/>
        /// <see cref="SkillRangeType.Square"/> - 正方形<para/>
        /// <see cref="SkillRangeType.Line"/> - 施法者与目标之间的线段<para/>
        /// <see cref="SkillRangeType.LinePass"/> - 施法者与目标所在的直线，贯穿至地图边缘<para/>
        /// <see cref="SkillRangeType.Sector"/> - 扇形<para/>
        /// 注意，该属性不影响选取目标的范围。选取目标的范围由 <see cref="GameMap"/> 决定。
        /// </summary>
        public virtual SkillRangeType SkillRangeType { get; set; } = SkillRangeType.Diamond;

        /// <summary>
        /// 扇形的角度。仅 <see cref="SkillRangeType"/> 为 <see cref="SkillRangeType.Sector"/> 时有效，默认值为 90 度。
        /// </summary>
        public virtual double SectorAngle { get; set; } = 90;

        /// <summary>
        /// 选取角色的条件
        /// </summary>
        public List<Func<Character, bool>> SelectTargetPredicates { get; } = [];

        /// <summary>
        /// 实际魔法消耗 [ 魔法 ]
        /// </summary>
        public double RealMPCost => FreeCostMP ? 0 : Math.Max(0, MPCost * (1 - Calculation.PercentageCheck((Character?.INT ?? 0) * GameplayEquilibriumConstant.INTtoCastMPReduce)));

        /// <summary>
        /// 魔法消耗 [ 魔法 ]
        /// </summary>
        [InitOptional]
        public virtual double MPCost { get; set; } = 0;

        /// <summary>
        /// 无魔法消耗 [ 运行时 ]
        /// </summary>
        public bool FreeCostMP { get; set; } = false;

        /// <summary>
        /// 吟唱时间 [ 魔法 ]
        /// </summary>
        [InitOptional]
        public virtual double CastTime { get; set; } = 0;

        /// <summary>
        /// 实际吟唱时间 [ 魔法 ]
        /// </summary>
        public double RealCastTime => Math.Max(0, CastTime * (1 - Calculation.PercentageCheck(Character?.AccelerationCoefficient ?? 0)));

        /// <summary>
        /// 实际能量消耗 [ 战技 ]
        /// </summary>
        public double RealEPCost => FreeCostEP ? 0 : (CostAllEP ? Math.Max(MinCostEP, Character?.EP ?? MinCostEP) : (IsSuperSkill ? EPCost : Math.Max(0, EPCost * (1 - Calculation.PercentageCheck((Character?.INT ?? 0) * GameplayEquilibriumConstant.INTtoCastEPReduce)))));

        /// <summary>
        /// 能量消耗 [ 战技 ]
        /// </summary>
        [InitOptional]
        public virtual double EPCost { get; set; } = 0;

        /// <summary>
        /// 消耗所有能量 [ 战技 ]
        /// </summary>
        public virtual bool CostAllEP { get; set; } = false;

        /// <summary>
        /// 消耗所有能量的最小能量限制 [ 战技 ] 默认值：100
        /// </summary>
        public virtual double MinCostEP { get; set; } = 100;

        /// <summary>
        /// 无能量消耗 [ 运行时 ]
        /// </summary>
        public bool FreeCostEP { get; set; } = false;

        /// <summary>
        /// 上一次释放此技能消耗的魔法 [ 魔法 ]
        /// </summary>
        public double LastCostMP { get; set; } = 0;

        /// <summary>
        /// 上一次释放此技能消耗的能量 [ 战技 ]
        /// </summary>
        public double LastCostEP { get; set; } = 0;

        /// <summary>
        /// 实际冷却时间
        /// </summary>
        public double RealCD => InstantReset ? 0 : Math.Max(0, CD * (1 - (Character?.CDR ?? 0)));

        /// <summary>
        /// 冷却时间
        /// </summary>
        [InitRequired]
        public virtual double CD { get; set; } = 0;

        /// <summary>
        /// 剩余冷却时间 [ 和 <see cref="Enable"/> 属性配合使用 ]
        /// </summary>
        public double CurrentCD { get; set; } = 0;

        /// <summary>
        /// 无 CD [ 运行时 ]
        /// </summary>
        public bool InstantReset { get; set; } = false;

        /// <summary>
        /// 硬直时间
        /// </summary>
        [InitRequired]
        public virtual double HardnessTime { get; set; } = 0;

        /// <summary>
        /// 额外硬直时间 [ 技能和物品相关 ]
        /// </summary>
        public double ExHardnessTime { get; set; } = 0;

        /// <summary>
        /// 额外硬直时间% [ 技能和物品相关 ]
        /// </summary>
        public double ExHardnessTime2 { get; set; } = 0;

        /// <summary>
        /// 实际硬直时间
        /// </summary>
        public double RealHardnessTime => Math.Max(0, (HardnessTime + ExHardnessTime) * (1 + ExHardnessTime2) * (1 - Calculation.PercentageCheck(Character?.ActionCoefficient ?? 0)));

        /// <summary>
        /// 魔法瓶颈 [ 智力相关 ]
        /// </summary>
        public virtual double MagicBottleneck
        {
            get
            {
                return Math.Max(0, field);
            }
            set
            {
                field = Math.Max(0, value);
            }
        }

        /// <summary>
        /// 魔法效能% [ 智力相关 ] 该值决定魔法的实际施展效果（乘算），低于 1 时会使效果低于预期，最多增长至 2 倍
        /// <para/>计算方法：以魔法瓶颈为100%，智力每低于瓶颈的 1%，效能就减少 1%，反之提升
        /// </summary>
        public double MagicEfficacy
        {
            get
            {
                if (MagicBottleneck == 0 || Character is null) return 1.0;
                double percentageDiff = (Character.INT - MagicBottleneck) / MagicBottleneck;
                return Math.Clamp(1.0 + percentageDiff, 0.01, 2.0);
            }
        }

        /// <summary>
        /// 效果列表
        /// </summary>
        public HashSet<Effect> Effects { get; } = [];

        /// <summary>
        /// 用于动态扩展技能的参数
        /// </summary>
        public Dictionary<string, object> Values { get; } = [];

        /// <summary>
        /// 游戏中的行动顺序表实例，在技能效果被触发时，此实例会获得赋值，使用时需要判断其是否存在
        /// </summary>
        public IGamingQueue? GamingQueue { get; set; } = null;

        /// <summary>
        /// 技能是否属于某个物品
        /// </summary>
        public Item? Item { get; set; } = null;

        /// <summary>
        /// 设置该属性可以由框架自动进行豁免检定
        /// </summary>
        public virtual Effect? EffectForExemptionCheck { get; set; } = null;

        /// <summary>
        /// 继承此类实现时，调用基类的构造函数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="character"></param>
        protected Skill(SkillType type, Character? character = null)
        {
            SkillType = type;
            CastAnywhere = SkillType == SkillType.Magic;
            Character = character;
        }

        /// <summary>
        /// 用于构造 JSON
        /// </summary>
        internal Skill() { }

        /// <summary>
        /// 设置一些属性给从工厂构造出来的 <paramref name="newbyFactory"/> 对象
        /// </summary>
        /// <param name="newbyFactory"></param>
        public void SetPropertyToItemModuleNew(Skill newbyFactory)
        {
            newbyFactory.GamingQueue = GamingQueue;
            newbyFactory.Enable = Enable;
            newbyFactory.IsInEffect = IsInEffect;
            newbyFactory.CurrentCD = CurrentCD;
        }

        /// <summary>
        /// 触发技能升级
        /// </summary>
        public void OnLevelUp()
        {
            if (!IsActive && Level > 0)
            {
                foreach (Effect e in AddPassiveEffectToCharacter())
                {
                    e.GamingQueue = GamingQueue;
                    if (Character != null && !Character.Effects.Contains(e))
                    {
                        Character.Effects.Add(e);
                        e.OnEffectGained(Character);
                    }
                }
            }
            if (Level > 0 && Character != null)
            {
                Effect[] effects = [.. Character.Effects.Where(e => e.IsInEffect)];
                foreach (Effect e in effects)
                {
                    e.OnSkillLevelUp(Character, Level);
                }
            }
        }

        /// <summary>
        /// 当获得技能时
        /// </summary>
        /// <param name="queue"></param>
        public void OnSkillGained(IGamingQueue queue)
        {
            GamingQueue = queue;
            OnLevelUp();
        }

        /// <summary>
        /// 在技能持有者的回合开始前
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="skills"></param>
        /// <param name="items"></param>
        public virtual void OnTurnStart(Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items)
        {

        }

        /// <summary>
        /// 在选取目标前向角色（玩家）发起询问
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        public virtual InquiryOptions? InquiryBeforeTargetSelection(Character character, Skill skill)
        {
            return null;
        }

        /// <summary>
        /// 获取可选择的目标列表
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <returns></returns>
        public virtual List<Character> GetSelectableTargets(Character caster, List<Character> enemys, List<Character> teammates)
        {
            List<Character> selectable = [];

            if (CanSelectSelf)
            {
                selectable.Add(caster);
            }

            ImmuneType checkType = ImmuneType.Skilled | ImmuneType.All;
            if (IsMagic)
            {
                checkType |= ImmuneType.Magical;
            }

            foreach (Character character in enemys)
            {
                IEnumerable<Effect> effects = Effects.Where(e => e.IsInEffect);
                if (CanSelectEnemy && ((character.ImmuneType & checkType) == ImmuneType.None ||
                    effects.Any(e => e.IgnoreImmune == ImmuneType.All || e.IgnoreImmune == ImmuneType.Skilled || (IsMagic && e.IgnoreImmune == ImmuneType.Magical))))
                {
                    selectable.Add(character);
                }
            }

            foreach (Character character in teammates)
            {
                if (CanSelectTeammate)
                {
                    selectable.Add(character);
                }
            }

            // 其他条件
            selectable = [.. selectable.Where(c => SelectTargetPredicates.All(f => f(c)))];

            return selectable;
        }

        /// <summary>
        /// 实际可选取的目标数量
        /// </summary>
        public int RealCanSelectTargetCount(List<Character> enemys, List<Character> teammates)
        {
            int count = CanSelectTargetCount;
            if (SelectAllTeammates)
            {
                return teammates.Count + 1;
            }
            if (SelectAllEnemies)
            {
                return enemys.Count;
            }
            return count;
        }

        /// <summary>
        /// 选取技能目标
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <returns></returns>
        public virtual List<Character> SelectTargets(Character caster, List<Character> enemys, List<Character> teammates)
        {
            List<Character> tobeSelected = GetSelectableTargets(caster, enemys, teammates);

            List<Character> targets = [];

            if (SelectAllTeammates || SelectAllEnemies)
            {
                if (SelectAllTeammates)
                {
                    targets.AddRange(tobeSelected.Where(c => c == caster || teammates.Contains(c)));
                }
                if (SelectAllEnemies)
                {
                    targets.AddRange(tobeSelected.Where(enemys.Contains));
                }
            }
            else if (tobeSelected.Count <= CanSelectTargetCount)
            {
                targets.AddRange(tobeSelected);
            }
            else
            {
                targets.AddRange(tobeSelected.OrderBy(x => Random.Shared.Next()).Take(CanSelectTargetCount));
            }

            return [.. targets.Distinct()];
        }

        /// <summary>
        /// 默认行为：在指向性技能中，当 <see cref="CanSelectTargetRange"/> > 0 时，会额外选取一些被扩散的目标
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="allEnemys"></param>
        /// <param name="allTeammates"></param>
        /// <param name="selected"></param>
        /// <param name="union"></param>
        /// <returns></returns>
        public virtual List<Character> SelectTargetsByCanSelectTargetRange(Character caster, List<Character> allEnemys, List<Character> allTeammates, IEnumerable<Character> selected, bool union = true)
        {
            List<Grid> grids = [];

            if (IsNonDirectional || CanSelectTargetRange < 0 || GamingQueue?.Map is not GameMap map)
            {
                return [.. selected];
            }

            foreach (Character selectedCharacter in selected)
            {
                Grid? centerGrid = map.GetCharacterCurrentGrid(selectedCharacter);
                if (centerGrid == null || centerGrid == Grid.Empty)
                    continue;

                // 使用曼哈顿距离获取以主要目标为中心、范围内的所有格子（包括中心格子本身）
                grids.AddRange(map.GetGridsByRange(centerGrid, CanSelectTargetRange, true));
            }

            return SelectTargetsByRange(caster, allEnemys, allTeammates, selected, grids, union);
        }

        /// <summary>
        /// 选取范围内的目标
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="allEnemys"></param>
        /// <param name="allTeammates"></param>
        /// <param name="selected"></param>
        /// <param name="range"></param>
        /// <param name="union"></param>
        /// <returns></returns>
        public virtual List<Character> SelectTargetsByRange(Character caster, List<Character> allEnemys, List<Character> allTeammates, IEnumerable<Character> selected, IEnumerable<Grid> range, bool union = true)
        {
            List<Character> targets = [];

            foreach (Character character in range.SelectMany(g => g.Characters))
            {
                if (CanSelectSelf && character == caster)
                {
                    targets.Add(caster);
                }

                ImmuneType checkType = ImmuneType.Skilled | ImmuneType.All;
                if (IsMagic)
                {
                    checkType |= ImmuneType.Magical;
                }

                if (allEnemys.Contains(character))
                {
                    IEnumerable<Effect> effects = Effects.Where(e => e.IsInEffect);
                    if (CanSelectEnemy && ((AllowSelectDead && character.HP == 0) || (!AllowSelectDead && character.HP > 0)) &&
                        ((character.ImmuneType & checkType) == ImmuneType.None || effects.Any(e => e.IgnoreImmune == ImmuneType.All || e.IgnoreImmune == ImmuneType.Skilled || (IsMagic && e.IgnoreImmune == ImmuneType.Magical))))
                    {
                        targets.Add(character);
                    }
                }

                if (CanSelectTeammate && allTeammates.Contains(character) && ((AllowSelectDead && character.HP == 0) || (!AllowSelectDead && character.HP > 0)))
                {
                    targets.Add(character);
                }
            }

            // 如果和已经选择的列表合并
            if (union)
            {
                return [.. targets.Where(c => SelectTargetPredicates.All(f => f(c))).Union(selected).Distinct()];
            }

            return [.. targets.Where(c => SelectTargetPredicates.All(f => f(c))).Distinct()];
        }

        /// <summary>
        /// 选取非指向性目标
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="targetGrid"></param>
        /// <param name="includeCharacter"></param>
        /// <returns></returns>
        public virtual List<Grid> SelectNonDirectionalTargets(Character caster, Grid targetGrid, bool includeCharacter = false)
        {
            int range = CanSelectTargetRange;
            List<Grid> targets = [];

            if (GamingQueue?.Map == null || targetGrid == Grid.Empty || range < 0)
            {
                return targets;
            }

            GameMap map = GamingQueue.Map;
            Grid currentGrid = map.GetCharacterCurrentGrid(caster) ?? Grid.Empty;

            // 范围等于 0 时只返回中心格子
            if (range == 0)
            {
                targets.Add(targetGrid);
                return targets;
            }

            List<Grid> rangeGrids = SkillRangeType switch
            {
                SkillRangeType.Diamond => map.GetGridsByRange(targetGrid, range, includeCharacter),
                SkillRangeType.Circle => map.GetGridsByCircleRange(targetGrid, range, includeCharacter),
                SkillRangeType.Square => map.GetGridsBySquareRange(targetGrid, range, includeCharacter),
                SkillRangeType.Line => map.GetGridsOnThickLine(currentGrid, targetGrid, range, false, includeCharacter),
                SkillRangeType.LinePass => map.GetGridsOnThickLine(currentGrid, targetGrid, range, true, includeCharacter),
                SkillRangeType.Sector => map.GetGridsInSector(currentGrid, targetGrid, range, SectorAngle, includeCharacter),
                _ => map.GetGridsByRange(targetGrid, range, includeCharacter)
            };

            targets.AddRange(rangeGrids);

            return [.. targets.Distinct()];
        }

        /// <summary>
        /// 技能开始吟唱时 [ 吟唱魔法、释放战技和爆发技、预释放爆发技均可触发 ]
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="caster"></param>
        /// <param name="targets"></param>
        /// <param name="grids"></param>
        public void OnSkillCasting(IGamingQueue queue, Character caster, List<Character> targets, List<Grid> grids)
        {
            GamingQueue = queue;
            foreach (Effect e in Effects)
            {
                e.GamingQueue = GamingQueue;
                e.OnSkillCasting(caster, targets, grids);
            }
        }

        /// <summary>
        /// 技能效果触发前
        /// </summary>
        public void BeforeSkillCasted()
        {
            LastCostMP = RealMPCost;
            LastCostEP = RealEPCost;
        }

        /// <summary>
        /// 触发技能效果 [ 局内版 ]
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="caster"></param>
        /// <param name="targets"></param>
        /// <param name="grids"></param>
        public void OnSkillCasted(IGamingQueue queue, Character caster, List<Character> targets, List<Grid> grids)
        {
            GamingQueue = queue;
            if (EffectForExemptionCheck != null)
            {
                Character[] exemptionTargets = [.. targets];
                foreach (Character target in exemptionTargets)
                {
                    if (!GamingQueue.CheckExemption(target, caster, EffectForExemptionCheck, true))
                    {
                        targets.Remove(target);
                    }
                }
            }
            Character[] characters = [caster, .. targets];
            foreach (Character target in characters)
            {
                Effect[] effects = [.. target.Effects.Where(e => e.IsInEffect)];
                foreach (Effect e in effects)
                {
                    e.GamingQueue = GamingQueue;
                    if (!e.BeforeSkillCasted(caster, this, targets, grids, Values))
                    {
                        targets.Remove(target);
                    }
                }
            }
            foreach (Effect e in Effects)
            {
                e.GamingQueue = GamingQueue;
                e.OnSkillCasted(caster, targets, grids, Values);
            }
        }

        /// <summary>
        /// 对目标触发技能效果
        /// </summary>
        /// <param name="user"></param>
        /// <param name="targets"></param>
        public void OnSkillCasted(User user, List<Character> targets)
        {
            foreach (Effect e in Effects)
            {
                e.OnSkillCasted(user, targets, Values);
            }
        }

        /// <summary>
        /// 检查角色是否在 AI 控制状态
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool IsCharacterInAIControlling(Character character)
        {
            return GamingQueue?.IsCharacterInAIControlling(character) ?? false;
        }

        /// <summary>
        /// 被动技能，需要重写此方法，返回被动特效给角色 [ 此方法会在技能学习时触发 ]
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return [];
        }

        /// <summary>
        /// 在复活时，因为复活是重新构建角色，如果需要继承死亡角色的技能数据，可以重写此方法并设置相关属性
        /// </summary>
        /// <param name="newSkill"></param>
        /// <returns></returns>
        public virtual void OnCharacterRespawn(Skill newSkill)
        {

        }

        /// <summary>
        /// 返回技能的详细说明
        /// </summary>
        /// <param name="showOriginal"></param>
        /// <param name="showCD"></param>
        /// <param name="showHardness"></param>
        /// <returns></returns>
        public string GetInfo(bool showOriginal = false, bool showCD = true, bool showHardness = true)
        {
            StringBuilder builder = new();

            string type = IsSuperSkill ? "【爆发技】" : (IsMagic ? "【魔法】" : (IsActive ? "【主动】" : "【被动】"));
            string level = Level > 0 ? " - 等级 " + Level : " - 尚未学习";
            builder.AppendLine(type + Name + level);
            builder.AppendLine("技能描述：" + (Level == 0 && GeneralDescription.Trim() != "" ? GeneralDescription : Description));
            if (CurrentCD > 0)
            {
                builder.AppendLine($"正在冷却：剩余 {CurrentCD:0.##} {GameplayEquilibriumConstant.InGameTime}");
            }
            if (!Enable)
            {
                builder.AppendLine("技能当前不可用");
            }
            if (IsInEffect)
            {
                builder.AppendLine("效果结束前不可用");
            }
            if (DispelDescription != "")
            {
                builder.AppendLine($"{DispelDescription}");
            }
            if (ExemptionDescription != "")
            {
                builder.AppendLine($"{ExemptionDescription}");
            }
            if (GamingQueue?.Map != null && SkillType != SkillType.Passive)
            {
                builder.AppendLine($"施法距离：{(CastAnywhere ? "全图" : CastRange)}");
            }
            if (IsActive && (Item?.IsInGameItem ?? true))
            {
                if (SkillType == SkillType.Item)
                {
                    if (RealMPCost > 0)
                    {
                        builder.AppendLine($"魔法消耗：{RealMPCost:0.##}{(showOriginal && RealMPCost != MPCost ? $"（原始值：{MPCost:0.##}）" : "")}");
                    }
                    if (RealEPCost > 0)
                    {
                        builder.AppendLine($"能量消耗：{RealEPCost:0.##}{(showOriginal && RealEPCost != EPCost ? $"（原始值：{EPCost:0.##}）" : "")}");
                    }
                }
                else
                {
                    if (IsSuperSkill)
                    {
                        builder.AppendLine($"能量消耗：{RealEPCost:0.##}{(showOriginal && RealEPCost != EPCost ? $"（原始值：{EPCost:0.##}）" : "")}");
                    }
                    else
                    {
                        if (IsMagic)
                        {
                            if (MagicBottleneck > 0) builder.AppendLine($"魔法瓶颈：{MagicBottleneck:0.##}{(Character != null ? $"（{MagicEfficacy * 100:0.##}%）" : "")}");
                            builder.AppendLine($"魔法消耗：{RealMPCost:0.##}{(showOriginal && RealMPCost != MPCost ? $"（原始值：{MPCost:0.##}）" : "")}");
                            builder.AppendLine($"吟唱时间：{RealCastTime:0.##}{(showOriginal && RealCastTime != CastTime ? $"（原始值：{CastTime:0.##}）" : "")}");
                        }
                        else
                        {
                            builder.AppendLine($"能量消耗：{RealEPCost:0.##}{(showOriginal && RealEPCost != EPCost ? $"（原始值：{EPCost:0.##}）" : "")}");
                        }
                    }
                }
                if (showCD && CD > 0)
                {
                    builder.AppendLine($"冷却时间：{RealCD:0.##}{(showOriginal && RealCD != CD ? $"（原始值：{CD:0.##}）" : "")}");
                }
                if (showHardness && HardnessTime > 0)
                {
                    builder.AppendLine($"硬直时间：{RealHardnessTime:0.##}{(showOriginal && RealHardnessTime != HardnessTime ? $"（原始值：{HardnessTime:0.##}）" : "")}");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 返回技能的详细说明
        /// </summary>
        /// <returns></returns>
        public override string ToString() => GetInfo(true);

        /// <summary>
        /// 返回技能的详细说明，有选项
        /// </summary>
        /// <param name="showOriginal"></param>
        /// <param name="showCD"></param>
        /// <param name="showHardness"></param>
        /// <returns></returns>
        public string ToString(bool showOriginal, bool showCD, bool showHardness)
        {
            return GetInfo(showOriginal, showCD, showHardness);
        }

        /// <summary>
        /// 判断两个技能是否相同 检查Id.Name
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IBaseEntity? other)
        {
            return other is Skill c && c.Id + "." + c.Name == Id + "." + Name;
        }

        /// <summary>
        /// 复制一个技能
        /// </summary>
        /// <returns></returns>
        public Skill Copy(bool copyProperty = true, IEnumerable<Skill>? skillsDefined = null)
        {
            Dictionary<string, object> args = new()
            {
                { "values", Values }
            };
            Skill? skillDefined = null;
            if (skillsDefined != null && skillsDefined.FirstOrDefault(i => i.GetIdName() == GetIdName()) is Skill temp)
            {
                skillDefined = temp;
            }
            if (skillDefined != null)
            {
                args["values"] = skillDefined.Values;
            }
            Skill skill = Factory.OpenFactory.GetInstance<Skill>(Id, Name, args);
            skillDefined ??= this;
            if (copyProperty) SetPropertyToItemModuleNew(skill);
            skill.Id = skillDefined.Id;
            skill.Name = skillDefined.Name;
            skill.Description = skillDefined.Description;
            skill.GeneralDescription = skillDefined.GeneralDescription;
            skill.DispelDescription = skillDefined.DispelDescription;
            skill.ExemptionDescription = skillDefined.ExemptionDescription;
            skill.SkillType = skillDefined.SkillType;
            skill.MPCost = skillDefined.MPCost;
            skill.CastTime = skillDefined.CastTime;
            skill.EPCost = skillDefined.EPCost;
            skill.FreeCostMP = skillDefined.FreeCostMP;
            skill.FreeCostEP = skillDefined.FreeCostEP;
            skill.CD = skillDefined.CD;
            skill.InstantReset = skillDefined.InstantReset;
            skill.HardnessTime = skillDefined.HardnessTime;
            skill.MagicBottleneck = skillDefined.MagicBottleneck;
            skill.GamingQueue = skillDefined.GamingQueue;
            if (skill is OpenSkill)
            {
                foreach (Effect e in skillDefined.Effects)
                {
                    // 特效没法动态扩展，必须使用编程钩子实现，因此动态扩展的技能需要使用代码定义的特效
                    Effect neweffect = e.Copy(skill, true);
                    if (skill.GamingQueue != null)
                    {
                        neweffect.GamingQueue = skill.GamingQueue;
                    }
                    skill.Effects.Add(neweffect);
                }
            }
            return skill;
        }
    }
}
