using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Entity;

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
        /// 快捷键
        /// </summary>
        public char Key { get; set; } = '/';

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
                int max = IsSuperSkill ? 6 : (IsMagic ? 8 : 6);
                _Level = Math.Min(Math.Max(0, value), max);
            }
        }

        /// <summary>
        /// 是否是主动技能
        /// </summary>
        [InitRequired]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 是否是爆发技 [ 此项为最高优先级 ]
        /// </summary>
        [InitRequired]
        public bool IsSuperSkill { get; set; } = false;

        /// <summary>
        /// 是否属于魔法 [ <see cref="IsActive"/> 会失效 ]，反之为战技
        /// </summary>
        [InitRequired]
        public bool IsMagic { get; set; } = true;

        /// <summary>
        /// 魔法消耗 [ 魔法 ]
        /// </summary>
        [InitOptional]
        public virtual double MPCost { get; } = 0;

        /// <summary>
        /// 基础魔法消耗 [ 魔法 ]
        /// </summary>
        [InitOptional]
        protected virtual double BaseMPCost { get; } = 0;

        /// <summary>
        /// 吟唱时间 [ 魔法 ]
        /// </summary>
        [InitOptional]
        public virtual double CastTime { get; } = 0;

        /// <summary>
        /// 能量消耗 [ 战技 ]
        /// </summary>
        [InitOptional]
        public virtual double EPCost { get; } = 0;

        /// <summary>
        /// 基础能量消耗 [ 战技 ]
        /// </summary>
        [InitOptional]
        protected virtual double BaseEPCost { get; } = 0;

        /// <summary>
        /// 冷却时间
        /// </summary>
        [InitRequired]
        public virtual double CD { get; } = 0;

        /// <summary>
        /// 剩余冷却时间 [ 建议配合 <see cref="Enable"/>  属性使用 ]
        /// </summary>
        public double CurrentCD { get; set; } = 0;

        /// <summary>
        /// 硬直时间
        /// </summary>
        [InitRequired]
        public virtual double HardnessTime { get; } = 0;

        /// <summary>
        /// 效果列表
        /// </summary>
        public Dictionary<string, Effect> Effects { get; } = [];

        /// <summary>
        /// 其他参数
        /// </summary>
        public Dictionary<string, object> OtherArgs { get; } = [];

        protected Skill(bool active = true, bool magic = true, Character? character = null)
        {
            IsActive = active;
            IsMagic = magic;
            Character = character;
        }

        protected Skill(bool super = false, Character? character = null)
        {
            IsSuperSkill = super;
            Character = character;
        }

        internal Skill() { }

        /// <summary>
        /// 触发技能效果
        /// </summary>
        public void Trigger(ActionQueue queue, Character actor, Skill skill, List<Character> enemys, List<Character> teammates)
        {
            foreach (Effect e in Effects.Values)
            {
                e.OnSkillCasted(queue, actor, enemys, teammates, OtherArgs);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new();

            string type = IsSuperSkill ? "【爆发技】" : (IsMagic ? "【魔法】" : (IsActive ? "【主动】" : "【被动】"));
            builder.AppendLine(type + Name + " - " + "等级 " + Level);
            builder.AppendLine("技能描述：" + Description);
            if (IsSuperSkill)
            {
                builder.AppendLine("能量消耗：" + EPCost);
            }
            else
            {
                if (IsMagic)
                {
                    builder.AppendLine("魔法消耗：" + MPCost);
                    builder.AppendLine("吟唱时间：" + CastTime);
                }
                else
                {
                    builder.AppendLine("能量消耗：" + EPCost);
                }
            }
            builder.AppendLine("冷却时间：" + CD);
            builder.AppendLine("硬直时间：" + HardnessTime);

            return builder.ToString();
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Skill c && c.Name == Name;
        }

        /// <summary>
        /// 等级
        /// </summary>
        private int _Level = 0;
    }
}
