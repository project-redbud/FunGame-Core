using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model.PrefabricatedEntity
{
    /// <summary>
    /// 魔法卡包的基础实现
    /// </summary>
    public class MagicCardPack : Item
    {
        public override ItemType ItemType => ItemType.MagicCardPack;

        /// <summary>
        /// 魔法技能组
        /// </summary>
        public HashSet<Skill> Magics => Skills.Magics;

        /// <summary>
        /// 属性增强：增加角色额外核心属性
        /// </summary>
        public HashSet<AttributeBoost> AttributeBoosts { get; } = [];

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

        protected override void OnItemEquipped(Character character, EquipSlotType type)
        {
            foreach (AttributeBoost ab in AttributeBoosts)
            {
                switch (ab.PrimaryAttribute)
                {
                    case PrimaryAttribute.AGI:
                        character.ExAGI += ab.Value;
                        break;
                    case PrimaryAttribute.INT:
                        character.ExINT += ab.Value;
                        break;
                    default:
                        character.ExSTR += ab.Value;
                        break;
                }
            }
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
            foreach (AttributeBoost ab in AttributeBoosts)
            {
                switch (ab.PrimaryAttribute)
                {
                    case PrimaryAttribute.AGI:
                        character.ExAGI -= ab.Value;
                        break;
                    case PrimaryAttribute.INT:
                        character.ExINT -= ab.Value;
                        break;
                    default:
                        character.ExSTR -= ab.Value;
                        break;
                }
            }
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
