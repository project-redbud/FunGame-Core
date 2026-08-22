using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Interface.Entity;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 选取域上下文：行动前/选取目标前修改可选列表、选取移动目标、选取技能/物品/攻击目标
    /// </summary>
    public class SelectionContext(IGamingQueue queue, Character actor) : HookContext(queue, actor)
    {
        /// <summary>
        /// 正在选取目标的技能（注意判断是 <see cref="Entity.Skill"/> 还是 <see cref="NormalAttack"/>）
        /// </summary>
        public ISkill? Skill { get; set; } = null;

        /// <summary>
        /// 普通攻击目标选取时的攻击实例
        /// </summary>
        public NormalAttack? NormalAttack { get; set; } = null;

        /// <summary>
        /// 可选择的技能列表
        /// </summary>
        public List<Skill> Skills { get; set; } = [];

        /// <summary>
        /// 可选择的物品列表
        /// </summary>
        public List<Item> Items { get; set; } = [];

        /// <summary>
        /// 全部敌人列表
        /// </summary>
        public List<Character> AllEnemys { get; set; } = [];

        /// <summary>
        /// 全部队友列表
        /// </summary>
        public List<Character> AllTeammates { get; set; } = [];

        /// <summary>
        /// 可选择的敌人列表
        /// </summary>
        public List<Character> Enemys { get; set; } = [];

        /// <summary>
        /// 可选择的队友列表
        /// </summary>
        public List<Character> Teammates { get; set; } = [];

        /// <summary>
        /// 技能/攻击的施放范围
        /// </summary>
        public List<Grid> CastRange { get; set; } = [];

        /// <summary>
        /// 地图实例
        /// </summary>
        public GameMap? Map { get; set; } = null;

        /// <summary>
        /// 可移动范围
        /// </summary>
        public List<Grid> MoveRange { get; set; } = [];

        /// <summary>
        /// 连杀统计（副本，修改无效）
        /// </summary>
        public Dictionary<Character, int> ContinuousKilling { get; set; } = [];

        /// <summary>
        /// 金币获取统计（副本，修改无效）
        /// </summary>
        public Dictionary<Character, int> EarnedMoney { get; set; } = [];
    }
}
