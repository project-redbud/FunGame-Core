using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Interface.Entity;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 选取域上下文：行动前/选取目标前修改可选列表、选取移动目标、选取技能/物品/攻击目标
    /// <para/>列表字段（<see cref="Enemys"/> 等）为框架传入的可修改集合：模组可通过读取引用后对集合内容
    /// Add/Remove/Clear 等方式就地修改（框架最终读取该集合内容），但不能整体替换集合引用。
    /// </summary>
    public class SelectionContext(IGamingQueue queue, Character actor) : HookContext(queue, actor)
    {
        /// <summary>
        /// 正在选取目标的技能（注意判断是 <see cref="Entity.Skill"/> 还是 <see cref="NormalAttack"/>）
        /// </summary>
        public ISkill? Skill { get; internal set; } = null;

        /// <summary>
        /// 普通攻击目标选取时的攻击实例
        /// </summary>
        public NormalAttack? NormalAttack { get; internal set; } = null;

        /// <summary>
        /// 可选择的技能列表（框架填充；可就地修改内容，不可替换引用）
        /// </summary>
        public List<Skill> Skills { get; internal set; } = [];

        /// <summary>
        /// 可选择的物品列表（框架填充；可就地修改内容，不可替换引用）
        /// </summary>
        public List<Item> Items { get; internal set; } = [];

        /// <summary>
        /// 全部敌人列表（框架填充；可就地修改内容，不可替换引用）
        /// </summary>
        public List<Character> AllEnemys { get; internal set; } = [];

        /// <summary>
        /// 全部队友列表（框架填充；可就地修改内容，不可替换引用）
        /// </summary>
        public List<Character> AllTeammates { get; internal set; } = [];

        /// <summary>
        /// 可选择的敌人列表（框架填充；可就地修改内容，不可替换引用）
        /// </summary>
        public List<Character> Enemys { get; internal set; } = [];

        /// <summary>
        /// 可选择的队友列表（框架填充；可就地修改内容，不可替换引用）
        /// </summary>
        public List<Character> Teammates { get; internal set; } = [];

        /// <summary>
        /// 技能/攻击的施放范围（框架填充；可就地修改内容，不可替换引用）
        /// </summary>
        public List<Grid> CastRange { get; internal set; } = [];

        /// <summary>
        /// 地图实例（框架填充）
        /// </summary>
        public GameMap? Map { get; internal set; } = null;

        /// <summary>
        /// 可移动范围（框架填充；可就地修改内容，不可替换引用）
        /// </summary>
        public List<Grid> MoveRange { get; internal set; } = [];

        /// <summary>
        /// 连杀统计（副本，只读展示用；修改无效，框架不读取）
        /// </summary>
        public Dictionary<Character, int> ContinuousKilling { get; internal set; } = [];

        /// <summary>
        /// 金币获取统计（副本，只读展示用；修改无效，框架不读取）
        /// </summary>
        public Dictionary<Character, int> EarnedMoney { get; internal set; } = [];
    }
}
