using Milimoe.FunGame.Core.Api.Utility;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 用于记录对哪个角色造成了多少伤害
    /// </summary>
    public class AssistDetail
    {
        /// <summary>
        /// 此详情类属于哪个角色
        /// </summary>
        public Character Character { get; }

        /// <summary>
        /// 对敌人造成的伤害
        /// </summary>
        public Dictionary<Character, double> Damages { get; } = [];

        /// <summary>
        /// 最后一次造成伤害的时间
        /// </summary>
        public Dictionary<Character, double> DamageLastTime { get; } = [];

        /// <summary>
        /// 对某角色最后一次友方非伤害辅助的时间
        /// </summary>
        public Dictionary<Character, double> NotDamageAssistLastTime { get; } = [];

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
        /// 获取和设置对 <paramref name="enemy"/> 的伤害，并设置时间
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public double this[Character enemy, double? time = null]
        {
            get
            {
                return Damages[enemy];
            }
            set
            {
                Damages[enemy] = Calculation.Round2Digits(value);
                if (time.HasValue)
                {
                    DamageLastTime[enemy] = time.Value;
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
            return Calculation.Round2Digits(Damages[enemy] / enemy.MaxHP);
        }

        /// <summary>
        /// 获取对 <paramref name="enemy"/> 造成伤害的最后时间
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns><see cref="double.MinValue"/> 意味着没有时间</returns>
        public double GetLastTime(Character enemy)
        {
            if (DamageLastTime.TryGetValue(enemy, out double time))
            {
                return time;
            }
            return double.MinValue;
        }

        /// <summary>
        /// 获取对某角色友方非伤害辅助的最后时间
        /// </summary>
        /// <param name="character"></param>
        /// <returns><see cref="double.MinValue"/> 意味着没有时间</returns>
        public double GetNotDamageAssistLastTime(Character character)
        {
            if (NotDamageAssistLastTime.TryGetValue(character, out double time))
            {
                return time;
            }
            return double.MinValue;
        }
    }
}
