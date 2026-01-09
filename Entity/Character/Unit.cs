using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// <see cref="Unit"/> 是一个用于描述生物的单元对象（单位），而 <see cref="Character"/> 是一种高级单位（单位单位/英雄单位）<para />
    /// 和单位一样，使用 <see cref="InitRequired"/> 标记的需要初始赋值的属性<para />
    /// </summary>
    public class Unit : Character
    {
        /// <summary>
        /// 单位名称
        /// </summary>
        public override string Name { get; set; } = "";

        /// <summary>
        /// 单位标识
        /// </summary>
        public override bool IsUnit => true;

        /// <summary>
        /// 获取单位名称以及所属玩家
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = Name;
            if (User != null && User.Username != "")
            {
                str += "(" + User.Username + ")";
            }
            return str;
        }

        /// <summary>
        /// 获取单位名称以及所属玩家，包含等级
        /// </summary>
        /// <returns></returns>
        public new string ToStringWithLevel()
        {
            string str = Name + " - 等级 " + Level;
            if (User != null && User.Username != "")
            {
                str += "（" + User.Username + "）";
            }
            return str;
        }

        /// <summary>
        /// 获取单位的详细信息
        /// </summary>
        /// <returns></returns>
        public new string GetInfo(bool showUser = true, bool showGrowth = true, bool showEXP = false, bool showMapRelated = false)
        {
            StringBuilder builder = new();

            builder.AppendLine(showUser ? ToStringWithLevel() : ToStringWithLevelWithOutUser());
            builder.AppendLine($"等级：{Level} / {GameplayEquilibriumConstant.MaxLevel}");
            double exHP = ExHP + ExHP2 + ExHP3;
            List<string> shield = [];
            if (Shield.TotalPhysical > 0) shield.Add($"物理：{Shield.TotalPhysical:0.##}");
            if (Shield.TotalMagical > 0) shield.Add($"魔法：{Shield.TotalMagical:0.##}");
            if (Shield.Mix > 0) shield.Add($"混合：{Shield.Mix:0.##}");
            builder.AppendLine($"生命值：{HP:0.##} / {MaxHP:0.##}" + (exHP != 0 ? $" [{BaseHP:0.##} {(exHP >= 0 ? "+" : "-")} {Math.Abs(exHP):0.##}]" : "") + (shield.Count > 0 ? $"（{string.Join("，", shield)}）" : ""));
            double exMP = ExMP + ExMP2 + ExMP3;
            builder.AppendLine($"魔法值：{MP:0.##} / {MaxMP:0.##}" + (exMP != 0 ? $" [{BaseMP:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exMP):0.##}]" : ""));
            double exATK = ExATK + ExATK2 + ExATK3;
            builder.AppendLine($"攻击力：{ATK:0.##}" + (exATK != 0 ? $" [{BaseATK:0.##} {(exATK >= 0 ? "+" : "-")} {Math.Abs(exATK):0.##}]" : ""));
            double exDEF = ExDEF + ExDEF2 + ExDEF3;
            builder.AppendLine($"物理护甲：{DEF:0.##}" + (exDEF != 0 ? $" [{BaseDEF:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exDEF):0.##}]" : "") + $" ({PDR * 100:0.##}%)");
            builder.AppendLine($"魔法抗性：{MDF.Avg:0.##}%（平均）");
            double exSPD = AGI * GameplayEquilibriumConstant.AGItoSPDMultiplier + ExSPD;
            builder.AppendLine($"行动速度：{SPD:0.##}" + (exSPD != 0 ? $" [{InitialSPD:0.##} {(exSPD >= 0 ? "+" : "-")} {Math.Abs(exSPD):0.##}]" : "") + $" ({ActionCoefficient * 100:0.##}%)");
            builder.AppendLine($"生命回复：{HR:0.##}" + (ExHR != 0 ? $" [{InitialHR + STR * GameplayEquilibriumConstant.STRtoHRFactor:0.##} {(ExHR >= 0 ? "+" : "-")} {Math.Abs(ExHR):0.##}]" : ""));
            builder.AppendLine($"魔法回复：{MR:0.##}" + (ExMR != 0 ? $" [{InitialMR + INT * GameplayEquilibriumConstant.INTtoMRFactor:0.##} {(ExMR >= 0 ? "+" : "-")} {Math.Abs(ExMR):0.##}]" : ""));
            builder.AppendLine($"暴击率：{CritRate * 100:0.##}%");
            builder.AppendLine($"暴击伤害：{CritDMG * 100:0.##}%");
            builder.AppendLine($"闪避率：{EvadeRate * 100:0.##}%");
            builder.AppendLine($"冷却缩减：{CDR * 100:0.##}%");
            builder.AppendLine($"加速系数：{AccelerationCoefficient * 100:0.##}%");
            builder.AppendLine($"物理穿透：{PhysicalPenetration * 100:0.##}%");
            builder.AppendLine($"魔法穿透：{MagicalPenetration * 100:0.##}%");

            if (showMapRelated)
            {
                builder.AppendLine($"移动距离：{MOV}");
                builder.AppendLine($"攻击距离：{ATR}");
            }

            if (CharacterState != CharacterState.Actionable)
            {
                builder.AppendLine(CharacterSet.GetCharacterState(CharacterState));
            }

            if (IsNeutral)
            {
                builder.AppendLine("单位是无敌的");
            }

            if (IsUnselectable)
            {
                builder.AppendLine("单位是不可选中的");
            }

            builder.AppendLine("== 普通攻击 ==");
            builder.Append(NormalAttack.ToString());

            if (Skills.Count > 0)
            {
                builder.AppendLine("== 单位技能 ==");
                foreach (Skill skill in Skills)
                {
                    builder.Append(skill.ToString());
                }
            }

            if (EquipSlot.Any())
            {
                builder.AppendLine("== 装备栏 ==");
                if (EquipSlot.MagicCardPack != null)
                {
                    builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.MagicCardPack.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.MagicCardPack) + "：" + EquipSlot.MagicCardPack.Name);
                    builder.AppendLine(EquipSlot.MagicCardPack.Description);
                }
                if (EquipSlot.Weapon != null)
                {
                    builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Weapon.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Weapon) + "：" + EquipSlot.Weapon.Name);
                    builder.AppendLine(EquipSlot.Weapon.Description);
                }
                if (EquipSlot.Armor != null)
                {
                    builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Armor.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Armor) + "：" + EquipSlot.Armor.Name);
                    builder.AppendLine(EquipSlot.Armor.Description);
                }
                if (EquipSlot.Shoes != null)
                {
                    builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Shoes.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Shoes) + "：" + EquipSlot.Shoes.Name);
                    builder.AppendLine(EquipSlot.Shoes.Description);
                }
                if (EquipSlot.Accessory1 != null)
                {
                    builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Accessory1.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Accessory1) + "：" + EquipSlot.Accessory1.Name);
                    builder.AppendLine(EquipSlot.Accessory1.Description);
                }
                if (EquipSlot.Accessory2 != null)
                {
                    builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Accessory2.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Accessory2) + "：" + EquipSlot.Accessory2.Name);
                    builder.AppendLine(EquipSlot.Accessory2.Description);
                }
            }

            if (Items.Count > 0)
            {
                builder.AppendLine("== 单位背包 ==");
                foreach (Item item in Items)
                {
                    builder.Append(item.ToString());
                }
            }

            Effect[] effects = [.. Effects.Where(e => e.ShowInStatusBar)];
            if (effects.Length > 0)
            {
                builder.AppendLine("== 状态栏 ==");
                foreach (Effect effect in effects)
                {
                    builder.Append(effect.ToString());
                }
            }

            /**
             * 意义不明（✖）的代码
             */
            if (showGrowth == showEXP)
            {
                showGrowth.ToString();
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取单位的简略信息
        /// </summary>
        /// <returns></returns>
        public new string GetSimpleInfo(bool showUser = true, bool showGrowth = true, bool showEXP = false, bool showBasicOnly = false, bool showMapRelated = false)
        {
            StringBuilder builder = new();

            builder.AppendLine(showUser ? ToStringWithLevel() : ToStringWithLevelWithOutUser());
            builder.AppendLine($"等级：{Level} / {GameplayEquilibriumConstant.MaxLevel}");
            double exHP = ExHP + ExHP2 + ExHP3;
            List<string> shield = [];
            if (Shield.TotalPhysical > 0) shield.Add($"物理：{Shield.TotalPhysical:0.##}");
            if (Shield.TotalMagical > 0) shield.Add($"魔法：{Shield.TotalMagical:0.##}");
            if (Shield.Mix > 0) shield.Add($"混合：{Shield.Mix:0.##}");
            builder.AppendLine($"生命值：{HP:0.##} / {MaxHP:0.##}" + (exHP != 0 ? $" [{BaseHP:0.##} {(exHP >= 0 ? "+" : "-")} {Math.Abs(exHP):0.##}]" : "") + (shield.Count > 0 ? $"（{string.Join("，", shield)}）" : ""));
            double exMP = ExMP + ExMP2 + ExMP3;
            builder.AppendLine($"魔法值：{MP:0.##} / {MaxMP:0.##}" + (exMP != 0 ? $" [{BaseMP:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exMP):0.##}]" : ""));
            double exATK = ExATK + ExATK2 + ExATK3;
            builder.AppendLine($"攻击力：{ATK:0.##}" + (exATK != 0 ? $" [{BaseATK:0.##} {(exATK >= 0 ? "+" : "-")} {Math.Abs(exATK):0.##}]" : ""));
            double exDEF = ExDEF + ExDEF2 + ExDEF3;
            builder.AppendLine($"物理护甲：{DEF:0.##}" + (exDEF != 0 ? $" [{BaseDEF:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exDEF):0.##}]" : "") + $" ({PDR * 100:0.##}%)");
            builder.AppendLine($"魔法抗性：{MDF.Avg:0.##}%（平均）");
            double exSPD = AGI * GameplayEquilibriumConstant.AGItoSPDMultiplier + ExSPD;
            builder.AppendLine($"行动速度：{SPD:0.##}" + (exSPD != 0 ? $" [{InitialSPD:0.##} {(exSPD >= 0 ? "+" : "-")} {Math.Abs(exSPD):0.##}]" : "") + $" ({ActionCoefficient * 100:0.##}%)");
            builder.AppendLine($"生命回复：{HR:0.##}" + (ExHR != 0 ? $" [{InitialHR + STR * GameplayEquilibriumConstant.STRtoHRFactor:0.##} {(ExHR >= 0 ? "+" : "-")} {Math.Abs(ExHR):0.##}]" : ""));
            builder.AppendLine($"魔法回复：{MR:0.##}" + (ExMR != 0 ? $" [{InitialMR + INT * GameplayEquilibriumConstant.INTtoMRFactor:0.##} {(ExMR >= 0 ? "+" : "-")} {Math.Abs(ExMR):0.##}]" : ""));

            if (showMapRelated)
            {
                builder.AppendLine($"移动距离：{MOV}");
                builder.AppendLine($"攻击距离：{ATR}");
            }

            if (!showBasicOnly)
            {
                if (CharacterState != CharacterState.Actionable)
                {
                    builder.AppendLine(CharacterSet.GetCharacterState(CharacterState));
                }

                if (IsNeutral)
                {
                    builder.AppendLine("单位是无敌的");
                }

                if (IsUnselectable)
                {
                    builder.AppendLine("单位是不可选中的");
                }

                if (Skills.Count > 0)
                {
                    builder.AppendLine("== 单位技能 ==");
                    builder.AppendLine(string.Join("，", Skills.Select(s => s.Name)));
                }

                if (EquipSlot.Any())
                {
                    builder.AppendLine("== 已装备槽位 ==");
                    List<EquipSlotType> types = [];
                    if (EquipSlot.MagicCardPack != null)
                    {
                        types.Add(EquipSlotType.MagicCardPack);
                    }
                    if (EquipSlot.Weapon != null)
                    {
                        types.Add(EquipSlotType.Weapon);
                    }
                    if (EquipSlot.Armor != null)
                    {
                        types.Add(EquipSlotType.Armor);
                    }
                    if (EquipSlot.Shoes != null)
                    {
                        types.Add(EquipSlotType.Shoes);
                    }
                    if (EquipSlot.Accessory1 != null)
                    {
                        types.Add(EquipSlotType.Accessory1);
                    }
                    if (EquipSlot.Accessory2 != null)
                    {
                        types.Add(EquipSlotType.Accessory2);
                    }
                    builder.AppendLine(string.Join("，", types.Select(ItemSet.GetEquipSlotTypeName)));
                }

                Effect[] effects = [.. Effects.Where(e => e.ShowInStatusBar)];
                if (effects.Length > 0)
                {
                    builder.AppendLine("== 状态栏 ==");
                    builder.Append(string.Join("，", effects.Select(e => e.Name)));
                }
            }

            /**
             * 意义不明（✖）的代码
             */
            if (showGrowth == showEXP)
            {
                showGrowth.ToString();
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取战斗状态的信息
        /// </summary>
        /// <param name="hardnessTimes"></param>
        /// <returns></returns>
        public new string GetInBattleInfo(double hardnessTimes)
        {
            StringBuilder builder = new();

            builder.AppendLine(ToStringWithLevel());
            double exHP = ExHP + ExHP2 + ExHP3;
            List<string> shield = [];
            if (Shield.TotalPhysical > 0) shield.Add($"物理：{Shield.TotalPhysical:0.##}");
            if (Shield.TotalMagical > 0) shield.Add($"魔法：{Shield.TotalMagical:0.##}");
            if (Shield.Mix > 0) shield.Add($"混合：{Shield.Mix:0.##}");
            builder.AppendLine($"生命值：{HP:0.##} / {MaxHP:0.##}" + (exHP != 0 ? $" [{BaseHP:0.##} {(exHP >= 0 ? "+" : "-")} {Math.Abs(exHP):0.##}]" : "") + (shield.Count > 0 ? $"（{string.Join("，", shield)}）" : ""));
            double exMP = ExMP + ExMP2 + ExMP3;
            builder.AppendLine($"魔法值：{MP:0.##} / {MaxMP:0.##}" + (exMP != 0 ? $" [{BaseMP:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exMP):0.##}]" : ""));
            double exATK = ExATK + ExATK2 + ExATK3;
            builder.AppendLine($"攻击力：{ATK:0.##}" + (exATK != 0 ? $" [{BaseATK:0.##} {(exATK >= 0 ? "+" : "-")} {Math.Abs(exATK):0.##}]" : ""));

            if (CharacterState != CharacterState.Actionable)
            {
                builder.AppendLine(CharacterSet.GetCharacterState(CharacterState));
            }

            if (IsNeutral)
            {
                builder.AppendLine("单位是中立单位，处于无敌状态");
            }

            if (IsUnselectable)
            {
                builder.AppendLine("单位是不可选中的");
            }

            builder.AppendLine($"硬直时间：{hardnessTimes:0.##}");

            Effect[] effects = [.. Effects.Where(e => e.ShowInStatusBar)];
            if (effects.Length > 0)
            {
                builder.AppendLine("== 状态栏 ==");
                foreach (Effect effect in effects)
                {
                    builder.Append(effect.ToString());
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取战斗状态的信息（简略版）
        /// </summary>
        /// <param name="hardnessTimes"></param>
        /// <returns></returns>
        public new string GetSimpleInBattleInfo(double hardnessTimes)
        {
            StringBuilder builder = new();

            builder.AppendLine(ToStringWithLevel());
            double exHP = ExHP + ExHP2 + ExHP3;
            List<string> shield = [];
            if (Shield.TotalPhysical > 0) shield.Add($"物理：{Shield.TotalPhysical:0.##}");
            if (Shield.TotalMagical > 0) shield.Add($"魔法：{Shield.TotalMagical:0.##}");
            if (Shield.Mix > 0) shield.Add($"混合：{Shield.Mix:0.##}");
            builder.AppendLine($"生命值：{HP:0.##} / {MaxHP:0.##}" + (exHP != 0 ? $" [{BaseHP:0.##} {(exHP >= 0 ? "+" : "-")} {Math.Abs(exHP):0.##}]" : "") + (shield.Count > 0 ? $"（{string.Join("，", shield)}）" : ""));
            double exMP = ExMP + ExMP2 + ExMP3;
            builder.AppendLine($"魔法值：{MP:0.##} / {MaxMP:0.##}" + (exMP != 0 ? $" [{BaseMP:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exMP):0.##}]" : ""));
            double exATK = ExATK + ExATK2 + ExATK3;
            builder.AppendLine($"攻击力：{ATK:0.##}" + (exATK != 0 ? $" [{BaseATK:0.##} {(exATK >= 0 ? "+" : "-")} {Math.Abs(exATK):0.##}]" : ""));
            builder.AppendLine($"硬直时间：{hardnessTimes:0.##}");

            Effect[] effects = [.. Effects.Where(e => e.ShowInStatusBar)];
            if (effects.Length > 0)
            {
                builder.AppendLine("== 状态栏 ==");
                builder.Append(string.Join("，", effects.Select(e => e.Name)));
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取单位的技能信息
        /// </summary>
        /// <returns></returns>
        public new string GetSkillInfo(bool showUser = true)
        {
            StringBuilder builder = new();

            builder.AppendLine(showUser ? ToStringWithLevel() : ToStringWithLevelWithOutUser());

            if (CharacterState != CharacterState.Actionable)
            {
                builder.AppendLine(CharacterSet.GetCharacterState(CharacterState));
            }

            if (IsNeutral)
            {
                builder.AppendLine("单位是无敌的");
            }

            if (IsUnselectable)
            {
                builder.AppendLine("单位是不可选中的");
            }

            builder.AppendLine("== 普通攻击 ==");
            builder.Append(NormalAttack.ToString());

            if (Skills.Count > 0)
            {
                builder.AppendLine("== 单位技能 ==");
                foreach (Skill skill in Skills)
                {
                    builder.Append(skill.ToString());
                }
            }

            Effect[] effects = [.. Effects.Where(e => e.ShowInStatusBar)];
            if (effects.Length > 0)
            {
                builder.AppendLine("== 状态栏 ==");
                foreach (Effect effect in effects)
                {
                    builder.Append(effect.ToString());
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取单位的物品信息
        /// </summary>
        /// <returns></returns>
        public new string GetItemInfo(bool showUser = true, bool showGrowth = true, bool showEXP = false, bool showMapRelated = false)
        {
            StringBuilder builder = new();

            builder.AppendLine(showUser ? ToStringWithLevel() : ToStringWithLevelWithOutUser());
            builder.AppendLine($"等级：{Level} / {GameplayEquilibriumConstant.MaxLevel}");
            double exHP = ExHP + ExHP2 + ExHP3;
            List<string> shield = [];
            if (Shield.TotalPhysical > 0) shield.Add($"物理：{Shield.TotalPhysical:0.##}");
            if (Shield.TotalMagical > 0) shield.Add($"魔法：{Shield.TotalMagical:0.##}");
            if (Shield.Mix > 0) shield.Add($"混合：{Shield.Mix:0.##}");
            builder.AppendLine($"生命值：{HP:0.##} / {MaxHP:0.##}" + (exHP != 0 ? $" [{BaseHP:0.##} {(exHP >= 0 ? "+" : "-")} {Math.Abs(exHP):0.##}]" : "") + (shield.Count > 0 ? $"（{string.Join("，", shield)}）" : ""));
            double exMP = ExMP + ExMP2 + ExMP3;
            builder.AppendLine($"魔法值：{MP:0.##} / {MaxMP:0.##}" + (exMP != 0 ? $" [{BaseMP:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exMP):0.##}]" : ""));
            double exATK = ExATK + ExATK2 + ExATK3;
            builder.AppendLine($"攻击力：{ATK:0.##}" + (exATK != 0 ? $" [{BaseATK:0.##} {(exATK >= 0 ? "+" : "-")} {Math.Abs(exATK):0.##}]" : ""));
            double exDEF = ExDEF + ExDEF2 + ExDEF3;
            builder.AppendLine($"物理护甲：{DEF:0.##}" + (exDEF != 0 ? $" [{BaseDEF:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exDEF):0.##}]" : "") + $" ({PDR * 100:0.##}%)");
            builder.AppendLine($"魔法抗性：{MDF.Avg:0.##}%（平均）");
            double exSPD = AGI * GameplayEquilibriumConstant.AGItoSPDMultiplier + ExSPD;
            builder.AppendLine($"行动速度：{SPD:0.##}" + (exSPD != 0 ? $" [{InitialSPD:0.##} {(exSPD >= 0 ? "+" : "-")} {Math.Abs(exSPD):0.##}]" : "") + $" ({ActionCoefficient * 100:0.##}%)");
            builder.AppendLine($"生命回复：{HR:0.##}" + (ExHR != 0 ? $" [{InitialHR + STR * GameplayEquilibriumConstant.STRtoHRFactor:0.##} {(ExHR >= 0 ? "+" : "-")} {Math.Abs(ExHR):0.##}]" : ""));
            builder.AppendLine($"魔法回复：{MR:0.##}" + (ExMR != 0 ? $" [{InitialMR + INT * GameplayEquilibriumConstant.INTtoMRFactor:0.##} {(ExMR >= 0 ? "+" : "-")} {Math.Abs(ExMR):0.##}]" : ""));
            builder.AppendLine($"暴击率：{CritRate * 100:0.##}%");
            builder.AppendLine($"暴击伤害：{CritDMG * 100:0.##}%");
            builder.AppendLine($"闪避率：{EvadeRate * 100:0.##}%");
            builder.AppendLine($"冷却缩减：{CDR * 100:0.##}%");
            builder.AppendLine($"加速系数：{AccelerationCoefficient * 100:0.##}%");
            builder.AppendLine($"物理穿透：{PhysicalPenetration * 100:0.##}%");
            builder.AppendLine($"魔法穿透：{MagicalPenetration * 100:0.##}%");

            if (showMapRelated)
            {
                builder.AppendLine($"移动距离：{MOV}");
                builder.AppendLine($"攻击距离：{ATR}");
            }

            if (EquipSlot.Any())
            {
                builder.AppendLine("== 装备栏 ==");
                if (EquipSlot.MagicCardPack != null)
                {
                    builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.MagicCardPack.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.MagicCardPack) + "：" + EquipSlot.MagicCardPack.Name);
                    builder.AppendLine(EquipSlot.MagicCardPack.Description);
                }
                if (EquipSlot.Weapon != null)
                {
                    builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Weapon.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Weapon) + "：" + EquipSlot.Weapon.Name);
                    builder.AppendLine(EquipSlot.Weapon.Description);
                }
                if (EquipSlot.Armor != null)
                {
                    builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Armor.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Armor) + "：" + EquipSlot.Armor.Name);
                    builder.AppendLine(EquipSlot.Armor.Description);
                }
                if (EquipSlot.Shoes != null)
                {
                    builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Shoes.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Shoes) + "：" + EquipSlot.Shoes.Name);
                    builder.AppendLine(EquipSlot.Shoes.Description);
                }
                if (EquipSlot.Accessory1 != null)
                {
                    builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Accessory1.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Accessory1) + "：" + EquipSlot.Accessory1.Name);
                    builder.AppendLine(EquipSlot.Accessory1.Description);
                }
                if (EquipSlot.Accessory2 != null)
                {
                    builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Accessory2.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Accessory2) + "：" + EquipSlot.Accessory2.Name);
                    builder.AppendLine(EquipSlot.Accessory2.Description);
                }
            }

            if (Items.Count > 0)
            {
                builder.AppendLine("== 单位背包 ==");
                foreach (Item item in Items)
                {
                    builder.Append(item.ToString());
                }
            }

            /**
             * 意义不明（✖）的代码
             */
            if (showGrowth == showEXP)
            {
                showGrowth.ToString();
            }

            return builder.ToString();
        }
    }
}
