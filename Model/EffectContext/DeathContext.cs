using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 死亡域上下文：死亡结算、死亡结算后广播、角色死亡事件
    /// </summary>
    public class DeathContext(IGamingQueue queue, Character death, Character? killer = null) : HookContext(queue, death)
    {
        /// <summary>
        /// 击杀者
        /// </summary>
        public Character? Killer { get; set; } = killer;

        /// <summary>
        /// 亡者是否有主人（召唤物/随从）
        /// </summary>
        public bool HasMaster { get; set; } = false;

        /// <summary>
        /// 连杀统计
        /// </summary>
        public Dictionary<Character, int> ContinuousKilling { get; set; } = [];

        /// <summary>
        /// 金币获取统计
        /// </summary>
        public Dictionary<Character, int> EarnedMoney { get; set; } = [];

        /// <summary>
        /// 助攻角色
        /// </summary>
        public Character[] Assists { get; set; } = [];
    }
}
