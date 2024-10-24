﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    /// <summary>
    /// 回合制游戏的基础队列
    /// </summary>
    public interface IGamingQueue
    {
        /// <summary>
        /// 用于文本输出
        /// </summary>
        public Action<string> WriteLine { get; }

        /// <summary>
        /// 当前的行动顺序
        /// </summary>
        public List<Character> Queue { get; }

        /// <summary>
        /// 当前已死亡的角色顺序(第一个是最早死的)
        /// </summary>
        public List<Character> Eliminated { get; }

        /// <summary>
        /// 角色数据
        /// </summary>
        public Dictionary<Character, CharacterStatistics> CharacterStatistics { get; }

        /// <summary>
        /// 游戏运行的时间
        /// </summary>
        public double TotalTime { get; }

        /// <summary>
        /// 游戏运行的回合
        /// 对于某角色而言，在其行动的回合叫 Turn，而所有角色行动的回合，都称之为 Round。
        /// </summary>
        public int TotalRound { get; }

        /// <summary>
        /// 显示队列信息
        /// </summary>
        public void DisplayQueue();

        /// <summary>
        /// 处理回合动作
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool ProcessTurn(Character character);

        /// <summary>
        /// 造成伤害
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="isMagicDamage"></param>
        /// <param name="magicType"></param>
        /// <param name="damageResult"></param>
        public void DamageToEnemy(Character actor, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage = false, MagicType magicType = MagicType.None, DamageResult damageResult = DamageResult.Normal);

        /// <summary>
        /// 计算物理伤害
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="expectedDamage"></param>
        /// <param name="finalDamage"></param>
        /// <returns></returns>
        public DamageResult CalculatePhysicalDamage(Character actor, Character enemy, bool isNormalAttack, double expectedDamage, out double finalDamage);

        /// <summary>
        /// 计算魔法伤害
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="magicType"></param>
        /// <param name="expectedDamage"></param>
        /// <param name="finalDamage"></param>
        /// <returns></returns>
        public DamageResult CalculateMagicalDamage(Character actor, Character enemy, bool isNormalAttack, MagicType magicType, double expectedDamage, out double finalDamage);

        /// <summary>
        /// 死亡结算
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="death"></param>
        public void DeathCalculation(Character killer, Character death);

        /// <summary>
        /// 打断施法
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="interrupter"></param>
        public void InterruptCasting(Character caster, Character interrupter);
    }
}
