using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 与 <see cref="Character"/> 不同，构造技能时，建议继承此类再构造
    /// </summary>
    public class Skill : BaseEntity, IActiveEnable
    {
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
        /// 技能等级，等于 0 时可以称之为尚未学习
        /// </summary>
        public int Level
        {
            get
            {
                return Math.Max(0, _Level);
            }
            set
            {
                int max = SkillSet.GetSkillMaxLevel(SkillType);
                _Level = Math.Min(Math.Max(0, value), max);
                OnLevelUp();
            }
        }

        /// <summary>
        /// 技能类型 [ 此项为最高优先级 ]
        /// </summary>
        [InitRequired]
        public SkillType SkillType { get; set; }

        /// <summary>
        /// 是否是主动技能 [ 此项为高优先级 ]
        /// </summary>
        [InitRequired]
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
        [InitRequired]
        public bool IsSuperSkill => SkillType == SkillType.SuperSkill;

        /// <summary>
        /// 是否属于魔法 [ <see cref="IsActive"/> 必须为 true ]，反之为战技
        /// </summary>
        [InitRequired]
        public bool IsMagic => SkillType == SkillType.Magic;

        /// <summary>
        /// 实际魔法消耗 [ 魔法 ]
        /// </summary>
        public double RealMPCost => Math.Max(0, MPCost * (1 - Calculation.PercentageCheck((Character?.INT ?? 0) * 0.00125)));

        /// <summary>
        /// 魔法消耗 [ 魔法 ]
        /// </summary>
        [InitOptional]
        public virtual double MPCost { get; set; } = 0;

        /// <summary>
        /// 实际吟唱时间 [ 魔法 ]
        /// </summary>
        public double RealCastTime => Math.Max(0, CastTime * (1 - Calculation.PercentageCheck(Character?.AccelerationCoefficient ?? 0)));

        /// <summary>
        /// 吟唱时间 [ 魔法 ]
        /// </summary>
        [InitOptional]
        public virtual double CastTime { get; set; } = 0;

        /// <summary>
        /// 实际能量消耗 [ 战技 ]
        /// </summary>
        public double RealEPCost => IsSuperSkill ? EPCost : Math.Max(0, EPCost * (1 - Calculation.PercentageCheck((Character?.INT ?? 0) * 0.00075)));

        /// <summary>
        /// 能量消耗 [ 战技 ]
        /// </summary>
        [InitOptional]
        public virtual double EPCost { get; set; } = 0;

        /// <summary>
        /// 实际冷却时间
        /// </summary>
        public double RealCD => Math.Max(0, CD * (1 - Character?.CDR ?? 0));

        /// <summary>
        /// 冷却时间
        /// </summary>
        [InitRequired]
        public virtual double CD { get; set; } = 0;

        /// <summary>
        /// 剩余冷却时间 [ 建议配合 <see cref="Enable"/>  属性使用 ]
        /// </summary>
        public double CurrentCD { get; set; } = 0;

        /// <summary>
        /// 硬直时间
        /// </summary>
        [InitRequired]
        public virtual double HardnessTime { get; set; } = 0;

        /// <summary>
        /// 效果列表
        /// </summary>
        public HashSet<Effect> Effects { get; } = [];

        /// <summary>
        /// 其他参数
        /// </summary>
        public Dictionary<string, object> OtherArgs { get; } = [];

        /// <summary>
        /// 游戏中的行动顺序表实例，在技能效果被触发时，此实例会获得赋值，使用时需要判断其是否存在
        /// </summary>
        public ActionQueue? ActionQueue { get; set; } = null;

        /// <summary>
        /// 技能是否属于某个物品
        /// </summary>
        public Item? Item { get; set; } = null;

        /// <summary>
        /// 继承此类实现时，调用基类的构造函数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="character"></param>
        protected Skill(SkillType type, Character? character = null)
        {
            SkillType = type;
            Character = character;
        }

        /// <summary>
        /// 用于构造 JSON
        /// </summary>
        internal Skill() { }

        /// <summary>
        /// 触发技能升级
        /// </summary>
        public void OnLevelUp()
        {
            if (!IsActive && Level > 0)
            {
                foreach (Effect e in AddInactiveEffectToCharacter())
                {
                    e.ActionQueue = ActionQueue;
                    if (Character != null && !Character.Effects.Contains(e))
                    {
                        Character.Effects.Add(e);
                        e.OnEffectGained(Character);
                    }
                }
            }
        }

        /// <summary>
        /// 当获得技能时
        /// </summary>
        /// <param name="queue"></param>
        public void OnSkillGained(ActionQueue queue)
        {
            ActionQueue = queue;
            OnLevelUp();
        }

        /// <summary>
        /// 技能开始吟唱时 [ 吟唱魔法、释放战技和爆发技、预释放爆发技均可触发 ]
        /// </summary>
        public void OnSkillCasting(ActionQueue queue, Character caster)
        {
            ActionQueue = queue;
            foreach (Effect e in Effects)
            {
                e.ActionQueue = ActionQueue;
                e.OnSkillCasting(caster);
            }
        }

        /// <summary>
        /// 触发技能效果
        /// </summary>
        public void OnSkillCasted(ActionQueue queue, Character caster, List<Character> enemys, List<Character> teammates)
        {
            ActionQueue = queue;
            foreach (Effect e in Effects)
            {
                e.ActionQueue = ActionQueue;
                e.OnSkillCasted(caster, enemys, teammates, OtherArgs);
            }
        }

        /// <summary>
        /// 被动技能，需要重写此方法，返回被动特效给角色 [ 此方法会在技能学习时触发 ]
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Effect> AddInactiveEffectToCharacter()
        {
            return [];
        }

        /// <summary>
        /// 返回技能的详细说明
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new();

            string type = IsSuperSkill ? "【爆发技】" : (IsMagic ? "【魔法】" : (IsActive ? "【主动】" : "【被动】"));
            string level = Level > 0 ? " - 等级 " + Level : " - 尚未学习";
            builder.AppendLine(type + Name + level);
            builder.AppendLine("技能描述：" + Description);
            if (CurrentCD > 0)
            {
                builder.AppendLine($"正在冷却：剩余 {CurrentCD:0.##} 时间");
            }
            if (!Enable)
            {
                builder.AppendLine("技能当前不可用");
            }
            if (IsInEffect)
            {
                builder.AppendLine("效果结束前不可用");
            }
            if (IsActive)
            {
                if (SkillType == SkillType.Item)
                {
                    if (RealMPCost > 0)
                    {
                        builder.AppendLine($"魔法消耗：{RealMPCost:0.##}");
                    }
                    if (RealEPCost > 0)
                    {
                        builder.AppendLine($"能量消耗：{RealEPCost:0.##}");
                    }
                }
                else
                {
                    if (IsSuperSkill)
                    {
                        builder.AppendLine($"能量消耗：{RealEPCost:0.##}");
                    }
                    else
                    {
                        if (IsMagic)
                        {
                            builder.AppendLine($"魔法消耗：{RealMPCost:0.##}");
                            builder.AppendLine($"吟唱时间：{RealCastTime:0.##}");
                        }
                        else
                        {
                            builder.AppendLine($"能量消耗：{RealEPCost:0.##}");
                        }
                    }
                }
                builder.AppendLine($"冷却时间：{RealCD:0.##}");
                builder.AppendLine($"硬直时间：{HardnessTime:0.##}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Id.Name
        /// </summary>
        /// <returns></returns>
        public string GetIdName()
        {
            return Id + "." + Name;
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
        public Skill Copy()
        {
            Skill s = new()
            {
                Id = Id,
                Name = Name,
                Description = Description,
                GeneralDescription = GeneralDescription,
                SkillType = SkillType,
                MPCost = MPCost,
                CastTime = CastTime,
                EPCost = EPCost,
                CD = CD,
                CurrentCD = CurrentCD,
                HardnessTime = HardnessTime,
                ActionQueue = ActionQueue
            };
            foreach (Effect e in Effects)
            {
                Effect neweffect = e.Copy(s);
                s.Effects.Add(neweffect);
            }
            return s;
        }

        /// <summary>
        /// 等级
        /// </summary>
        private int _Level = 0;
    }
}
