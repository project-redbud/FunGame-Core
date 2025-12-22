using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model.PrefabricatedEntity
{
    /// <summary>
    /// 魔法卡包的基础实现
    /// </summary>
    public class MagicCardPack : OpenItem
    {
        public override ItemType ItemType => ItemType.MagicCardPack;

        /// <summary>
        /// 魔法技能组
        /// </summary>
        public HashSet<Skill> Magics => Skills.Magics;

        /// <summary>
        /// 动态矩阵：增加角色额外核心属性
        /// </summary>
        public Dictionary<PrimaryAttribute, double> AttributeBoosts { get; } = new()
        {
            { PrimaryAttribute.STR, 0 },
            { PrimaryAttribute.AGI, 0 },
            { PrimaryAttribute.INT, 0 }
        };

        /// <summary>
        /// 同频共振：强制转换角色的核心属性为该属性 [ 优先级：仅在装备时改变，覆盖该时刻的核心属性，后续可被其他物品/技能覆盖 ]
        /// </summary>
        public PrimaryAttribute Resonance { get; set; } = PrimaryAttribute.None;

        /// <summary>
        /// 神经校准：角色在使用某种武器时获得额外特效
        /// </summary>
        public NeuralCalibrationEffect? NeuralCalibration { get; set; } = null;

        /// <summary>
        /// 勇气指令：行动回合内的附赠指令技能（使用后不会结束回合）
        /// </summary>
        public CourageCommandSkill? CourageCommand { get; set; } = null;

        /// <summary>
        /// 灵魂绑定：一个至少消耗 100 能量、每额外消耗 20 能量效果增强 10% 的爆发技
        /// </summary>
        public SoulboundSkill? Soulbound { get; set; } = null;

        /// <summary>
        /// 备份同频共振前的核心属性类型
        /// </summary>
        private PrimaryAttribute _originalAttribute = PrimaryAttribute.None;

        public MagicCardPack(long id, string name, Dictionary<string, object> args) : base(id, name, args)
        {
            foreach (string key in args.Keys)
            {
                switch (key.ToLower())
                {
                    case "exstr":
                        if (double.TryParse(args[key].ToString(), out double strValue))
                        {
                            AttributeBoosts[PrimaryAttribute.STR] = strValue;
                        }
                        break;
                    case "exagi":
                        if (double.TryParse(args[key].ToString(), out double agiValue))
                        {
                            AttributeBoosts[PrimaryAttribute.AGI] = agiValue;
                        }
                        break;
                    case "exint":
                        if (double.TryParse(args[key].ToString(), out double intValue))
                        {
                            AttributeBoosts[PrimaryAttribute.INT] = intValue;
                        }
                        break;
                    case "res":
                    case "resonance":
                        if (Enum.TryParse(args[key].ToString(), out PrimaryAttribute resonance))
                        {
                            Resonance = resonance;
                        }
                        break;
                }
            }
        }

        protected override void OnItemEquipped(Character character, EquipSlotType type)
        {
            character.ExSTR += AttributeBoosts[PrimaryAttribute.STR];
            character.ExAGI += AttributeBoosts[PrimaryAttribute.AGI];
            character.ExINT += AttributeBoosts[PrimaryAttribute.INT];
            if (Resonance != PrimaryAttribute.None)
            {
                _originalAttribute = character.PrimaryAttribute;
                character.PrimaryAttribute = Resonance;
            }
            if (NeuralCalibration != null)
            {
                character.Effects.Add(NeuralCalibration);
                NeuralCalibration.OnEffectGained(character);
            }
            if (CourageCommand != null)
            {
                character.Skills.Add(CourageCommand);
            }
            if (Soulbound != null)
            {
                character.Skills.Add(Soulbound);
            }
        }

        protected override void OnItemUnEquipped(Character character, EquipSlotType type)
        {
            character.ExSTR -= AttributeBoosts[PrimaryAttribute.STR];
            character.ExAGI -= AttributeBoosts[PrimaryAttribute.AGI];
            character.ExINT -= AttributeBoosts[PrimaryAttribute.INT];
            if (_originalAttribute != PrimaryAttribute.None)
            {
                character.PrimaryAttribute = _originalAttribute;
            }
            if (NeuralCalibration != null)
            {
                character.Effects.Remove(NeuralCalibration);
                NeuralCalibration.OnEffectLost(character);
            }
            if (CourageCommand != null)
            {
                character.Skills.Remove(CourageCommand);
            }
            if (Soulbound != null)
            {
                character.Skills.Remove(Soulbound);
            }
        }
    }
}
