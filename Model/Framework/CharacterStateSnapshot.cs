using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 角色状态快照（状态检查点中的单个角色状态）<para/>
    /// 定期检查点 + 事件流推算：作为精确基准，检查点之间的状态由事件流沿时间轴推算
    /// </summary>
    public class CharacterStateSnapshot
    {
        /// <summary>
        /// 角色引用（轻量快照）
        /// </summary>
        public Character Character { get; set; } = new();

        /// <summary>
        /// 当前生命值
        /// </summary>
        public double HP { get; set; } = 0;

        /// <summary>
        /// 最大生命值
        /// </summary>
        public double MaxHP { get; set; } = 0;

        /// <summary>
        /// 当前魔法值
        /// </summary>
        public double MP { get; set; } = 0;

        /// <summary>
        /// 最大魔法值
        /// </summary>
        public double MaxMP { get; set; } = 0;

        /// <summary>
        /// 当前能量值
        /// </summary>
        public double EP { get; set; } = 0;

        /// <summary>
        /// 角色全部属性（属性名 -> 展示值，与 Character.GetInfo() 中出现的属性一致，检查点回合完整记录）<para/>
        /// 由队列在生成检查点时写入 Character.GetAttributeValues() 的结果
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = [];

        /// <summary>
        /// 生命回复速率（随时间流逝回复，用于状态推算）
        /// </summary>
        public double HR { get; set; } = 0;

        /// <summary>
        /// 魔法回复速率（随时间流逝回复，用于状态推算）
        /// </summary>
        public double MR { get; set; } = 0;

        /// <summary>
        /// 当前装备栏：装备槽位 -> 物品 ID
        /// </summary>
        public Dictionary<EquipSlotType, long> Equipments { get; set; } = [];

        /// <summary>
        /// 装备栏明细（槽位 + 物品 ID + 物品名字，用于展示；序列化后名字可直接使用）
        /// </summary>
        public List<EquipmentStateSnapshot> EquipmentsDetail { get; set; } = [];

        /// <summary>
        /// 技能状态列表（技能 ID、名称、等级、当前冷却）
        /// </summary>
        public List<SkillStateSnapshot> Skills { get; } = [];

        /// <summary>
        /// 物品栏（背包）列表（物品 ID、名称）
        /// </summary>
        public List<ItemStateSnapshot> Items { get; } = [];

        /// <summary>
        /// 状态栏特效列表（特效 ID、名称、类型、剩余时间）
        /// </summary>
        public List<EffectStateSnapshot> Effects { get; } = [];
    }

    /// <summary>
    /// 技能状态快照
    /// </summary>
    public class SkillStateSnapshot
    {
        /// <summary>
        /// 技能 ID
        /// </summary>
        public long SkillId { get; set; } = 0;

        /// <summary>
        /// 技能名称（展示用）
        /// </summary>
        public string SkillName { get; set; } = "";

        /// <summary>
        /// 技能等级
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// 当前冷却时间
        /// </summary>
        public double CurrentCD { get; set; } = 0;

        /// <summary>
        /// 技能描述
        /// </summary>
        public string Description { get; set; } = "";
    }

    /// <summary>
    /// 物品状态快照
    /// </summary>
    public class ItemStateSnapshot
    {
        /// <summary>
        /// 物品 ID
        /// </summary>
        public long ItemId { get; set; } = 0;

        /// <summary>
        /// 物品名称（展示用）
        /// </summary>
        public string ItemName { get; set; } = "";

        /// <summary>
        /// 物品描述
        /// </summary>
        public string Description { get; set; } = "";
    }

    /// <summary>
    /// 状态栏特效快照
    /// </summary>
    public class EffectStateSnapshot
    {
        /// <summary>
        /// 特效 ID
        /// </summary>
        public long EffectId { get; set; } = 0;

        /// <summary>
        /// 特效名称（展示用）
        /// </summary>
        public string EffectName { get; set; } = "";

        /// <summary>
        /// 特效类型
        /// </summary>
        public EffectType EffectType { get; set; } = EffectType.None;

        /// <summary>
        /// 剩余持续时间
        /// </summary>
        public double RemainDuration { get; set; } = 0;

        /// <summary>
        /// 剩余持续回合数
        /// </summary>
        public int RemainDurationTurn { get; set; } = 0;

        /// <summary>
        /// 特效施加者（Source 角色）的 Guid（无施加者时为 <see cref="Guid.Empty"/>）
        /// </summary>
        public Guid SourceGuid { get; set; } = Guid.Empty;

        /// <summary>
        /// 特效描述
        /// </summary>
        public string Description { get; set; } = "";
    }
}
