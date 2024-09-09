using Milimoe.FunGame.Core.Api.Utility;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 用于记录对哪个角色造成了多少伤害
    /// </summary>
    public class AssistDetail : Dictionary<Character, double>
    {
        /// <summary>
        /// 此详情类属于哪个角色
        /// </summary>
        public Character Character { get; }

        /// <summary>
        /// 初始化一个助攻详情类
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemys"></param>
        public AssistDetail(Character character, IEnumerable<Character> enemys)
        {
            Character = character;
            foreach (Character enemy in enemys)
            {
                this[enemy] = 0;
            }
        }

        /// <summary>
        /// 获取和设置对 <paramref name="enemy"/> 的伤害
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public new double this[Character enemy]
        {
            get
            {
                return base[enemy];
            }
            set
            {
                if (!base.TryAdd(enemy, Calculation.Round2Digits(value)))
                {
                    base[enemy] = Calculation.Round2Digits(value);
                }
            }
        }

        /// <summary>
        /// 获取对 <paramref name="enemy"/> 的伤害
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns>目标的 <see cref="Character.MaxHP"/> 的百分比形式</returns>
        public double GetPercentage(Character enemy)
        {
            return Calculation.Round2Digits(base[enemy] / enemy.MaxHP);
        }
    }
}
