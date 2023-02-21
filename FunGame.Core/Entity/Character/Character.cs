using Milimoe.FunGame.Core.Library.Constant;
using System.Collections;

namespace Milimoe.FunGame.Core.Entity
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string NickName { get; set; } = "";
        public User? User { get; set; } = null;
        public CharacterStatistics? Statistics { get; set; } = null; // 角色统计数据
        public MagicType MagicType { get; set; } // 魔法属性
        public RoleType FirstRoleType { get; set; } // 角色定位1
        public RoleType SecondRoleType { get; set; } // 角色定位2
        public RoleType ThirdRoleType { get; set; } // 角色定位3
        public RoleRating RoleRating { get; set; } // 角色评级
        public int Promotion { get; set; } // 晋升点数
        public int Level { get; set; } = 1;
        public decimal EXP { get; set; } // 经验值
        public decimal BaseHP { get; set; } // 基础生命值
        public decimal HP { get; set; }
        public decimal BaseMP { get; set; } // 基础魔法值
        public decimal MP { get; set; }
        public decimal EP { get; set; }
        public decimal BaseATK { get; set; } // 基础攻击力
        public decimal ATK { get; set; }
        public decimal DEF { get; set; } // Defence 物理护甲
        public decimal PDR { get; set; } // Physical Damage Reduction 物理伤害减免
        public decimal MDF { get; set; } // Magical Defence 魔法抗性
        public decimal PhysicalPenetration { get; set; } // Physical Penetration 物理穿透
        public decimal MagicalPenetration { get; set; } // Magical Penetration 魔法穿透
        public decimal HR { get; set; } = 0; // Health Regeneration 生命回复力
        public decimal MR { get; set; } = 0; // Mana Regeneration 魔法回复力
        public decimal ER { get; set; } = 0; // Eenergy Regeneration 能量回复力
        public decimal BaseSTR { get; set; } // 基础力量
        public decimal BaseAGI { get; set; } // 基础敏捷
        public decimal BaseINT { get; set; } // 基础智力
        public decimal STR { get; set; } // Strength 力量
        public decimal AGI { get; set; } // Agility 敏捷
        public decimal INT { get; set; } // Intelligence 智力
        public decimal STRGrowth { get; set; } // Strength Growth 力量成长值
        public decimal AGIGrowth { get; set; } // Agility Growth 敏捷成长值
        public decimal INTGrowth { get; set; } // Intelligence Growth 智力成长值
        public decimal SPD { get; set; } // Speed 速度
        public decimal ActionCoefficient { get; set; } // Action Coefficient 行动系数
        public decimal AccelerationCoefficient { get; set; } // Acceleration Coefficient 加速系数
        public decimal ATR { get; set; } // Attack Range 攻击距离
        public decimal CritRate { get; set; } = 0.05M; // 暴击率
        public decimal CritDMG { get; set; } = 1.25M; // 暴击伤害
        public decimal EvadeRate { get; set; } = 0.05M; // 闪避率
        public Hashtable? Skills { get; set; } = new Hashtable();
        public Hashtable? Items { get; set; } = new Hashtable();
    }
}
