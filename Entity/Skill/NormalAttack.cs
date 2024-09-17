using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class NormalAttack(Character character, bool isMagic = false, MagicType magicType = MagicType.None) : BaseEntity
    {
        /// <summary>
        /// 普通攻击名称
        /// </summary>
        public override string Name => "普通攻击";

        /// <summary>
        /// 普通攻击说明
        /// </summary>
        public string Description => $"对目标敌人造成 {Calculation.Round4Digits((1.0 + 0.05 * (Level - 1)) * 100)}% [ {Damage} ] 点{(IsMagic ? CharacterSet.GetMagicDamageName(MagicType) : "物理伤害")}。";

        /// <summary>
        /// 所属的角色
        /// </summary>
        public Character Character { get; } = character;

        /// <summary>
        /// 普通攻击的伤害
        /// </summary>
        public double Damage => Calculation.Round2Digits(Character.ATK * (1.0 + 0.05 * (Level - 1)));

        /// <summary>
        /// 普通攻击等级
        /// </summary>
        public int Level
        {
            get
            {
                return Math.Max(1, _Level);
            }
            set
            {
                _Level = Math.Min(Math.Max(1, value), 8);
            }
        }

        /// <summary>
        /// 是否是魔法伤害
        /// </summary>
        public bool IsMagic => _IsMagic;

        /// <summary>
        /// 魔法伤害需要指定魔法类型
        /// </summary>
        public MagicType MagicType => _MagicType;

        /// <summary>
        /// 硬直时间
        /// </summary>
        public double HardnessTime { get; set; } = 10;

        /// <summary>
        /// 对目标（或多个目标）发起普通攻击
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="attacker"></param>
        /// <param name="enemys"></param>
        public void Attack(ActionQueue queue, Character attacker, params Character[] enemys)
        {
            foreach (Character enemy in enemys)
            {
                queue.WriteLine("[ " + Character + $" ] 对 [ {enemy} ] 发起了普通攻击！");
                double expected = Damage;
                DamageResult result = IsMagic ? queue.CalculateMagicalDamage(attacker, enemy, true, MagicType, expected, out double damage) : queue.CalculatePhysicalDamage(attacker, enemy, true, expected, out damage);
                queue.DamageToEnemy(attacker, enemy, damage, true, IsMagic, MagicType, result);
            }
        }

        /// <summary>
        /// 修改伤害类型
        /// </summary>
        /// <param name="isMagic"></param>
        /// <param name="magicType"></param>
        public void SetMagicType(bool isMagic, MagicType magicType)
        {
            _IsMagic = isMagic;
            _MagicType = magicType;
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is NormalAttack c && c.Name == Name;
        }

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine(Name + " - 等级 " + Level);
            builder.AppendLine("技能描述：" + Description);
            builder.AppendLine("硬直时间：" + HardnessTime);

            return builder.ToString();
        }

        /// <summary>
        /// 等级
        /// </summary>
        private int _Level = 0;

        /// <summary>
        /// 是否是魔法伤害
        /// </summary>
        private bool _IsMagic = isMagic;

        /// <summary>
        /// 魔法类型
        /// </summary>
        private MagicType _MagicType = magicType;
    }
}
