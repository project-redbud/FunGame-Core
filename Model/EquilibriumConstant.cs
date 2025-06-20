using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    /// <summary>
    /// 游戏平衡常数
    /// </summary>
    public class EquilibriumConstant
    {
        /// <summary>
        /// 游戏货币名称
        /// </summary>
        public string InGameCurrency { get; set; } = "金币";

        /// <summary>
        /// 游戏材料名称（第二货币）
        /// </summary>
        public string InGameMaterial { get; set; } = "材料";

        /// <summary>
        /// 游戏时间名称（如技能冷却、硬直）
        /// </summary>
        public string InGameTime { get; set; } = "时间";

        /// <summary>
        /// 晋升点数上限
        /// </summary>
        public Dictionary<string, int> PromotionsUpperLimit { get; set; } = new()
        {
            { "X", 999 },
            { "S", 998 },
            { "A+", 850 },
            { "A", 700 },
            { "B", 550 },
            { "C", 400 },
            { "D", 300 },
            { "E", 200 },
        };

        /// <summary>
        /// 使用的魔法类型
        /// </summary>
        public HashSet<MagicType> UseMagicType { get; set; } = [
            MagicType.None, MagicType.Starmark, MagicType.PurityNatural, MagicType.PurityContemporary,
            MagicType.Element, MagicType.Bright, MagicType.Shadow, MagicType.Aster, MagicType.SpatioTemporal
        ];

        /// <summary>
        /// 初始生命值
        /// </summary>
        public double InitialHP { get; set; } = 60;

        /// <summary>
        /// 初始魔法值
        /// </summary>
        public double InitialMP { get; set; } = 10;

        /// <summary>
        /// 角色最高等级
        /// </summary>
        public int MaxLevel { get; set; } = 60;

        /// <summary>
        /// 经验值上限
        /// </summary>
        public Dictionary<int, double> EXPUpperLimit { get; set; } = new()
        {
            { 1, 1000 }, { 2, 1000 }, { 3, 1000 }, { 4, 1000 }, { 5, 1000 }, { 6, 1000 }, { 7, 1000 }, { 8, 1000 }, { 9, 1000 },
            { 10, 1500 }, { 11, 1500 }, { 12, 1500 }, { 13, 1500 }, { 14, 1500 }, { 15, 1500 }, { 16, 1500 }, { 17, 1500 }, { 18, 1500 }, { 19, 1500 },
            { 20, 2000 }, { 21, 2000 }, { 22, 2000 }, { 23, 2000 }, { 24, 2000 }, { 25, 2000 }, { 26, 2000 }, { 27, 2000 }, { 28, 2000 }, { 29, 2000 },
            { 30, 3000 }, { 31, 3000 }, { 32, 3000 }, { 33, 3000 }, { 34, 3000 }, { 35, 3000 }, { 36, 3000 }, { 37, 3000 }, { 38, 3000 }, { 39, 3000 },
            { 40, 4000 }, { 41, 4000 }, { 42, 4000 }, { 43, 4000 }, { 44, 4000 }, { 45, 4000 }, { 46, 4000 }, { 47, 4000 }, { 48, 4000 }, { 49, 4000 },
            { 50, 5000 }, { 51, 5000 }, { 52, 5000 }, { 53, 5000 }, { 54, 5000 }, { 55, 5000 }, { 56, 5000 }, { 57, 5000 }, { 58, 5000 }, { 59, 5000 },
            { 60, 9999999999999 }
        };

        /// <summary>
        /// 使用等级突破机制
        /// </summary>
        public bool UseLevelBreak { get; set; } = true;

        /// <summary>
        /// 使用等级突破机制后，角色处于这些等级时需要突破才能继续升级
        /// </summary>
        public HashSet<int> LevelBreakList { get; set; } = [10, 20, 30, 40, 50, 60];

        /// <summary>
        /// 魔法最高等级
        /// </summary>
        public int MaxMagicLevel { get; set; } = 8;

        /// <summary>
        /// 战技最高等级
        /// </summary>
        public int MaxSkillLevel { get; set; } = 6;

        /// <summary>
        /// 爆发技最高等级
        /// </summary>
        public int MaxSuperSkillLevel { get; set; } = 6;

        /// <summary>
        /// 被动最高等级
        /// </summary>
        public int MaxPassiveSkillLevel { get; set; } = 6;

        /// <summary>
        /// 普通攻击最高等级
        /// </summary>
        public int MaxNormalAttackLevel { get; set; } = 8;

        /// <summary>
        /// 最大能量值
        /// </summary>
        public double MaxEP { get; set; } = 200;

        /// <summary>
        /// 初始攻击力
        /// </summary>
        public double InitialATK { get; set; } = 15;

        /// <summary>
        /// 初始物理护甲
        /// </summary>
        public double InitialDEF { get; set; } = 5;

        /// <summary>
        /// 初始力量
        /// </summary>
        public double InitialSTR { get; set; } = 0;

        /// <summary>
        /// 初始敏捷
        /// </summary>
        public double InitialAGI { get; set; } = 0;

        /// <summary>
        /// 初始智力
        /// </summary>
        public double InitialINT { get; set; } = 0;

        /// <summary>
        /// 力量成长
        /// </summary>
        public double STRGrowth { get; set; } = 0;

        /// <summary>
        /// 敏捷成长
        /// </summary>
        public double AGIGrowth { get; set; } = 0;

        /// <summary>
        /// 智力成长
        /// </summary>
        public double INTGrowth { get; set; } = 0;

        /// <summary>
        /// 初始暴击率
        /// </summary>
        public double CritRate { get; set; } = 0.05;

        /// <summary>
        /// 初始暴击伤害
        /// </summary>
        public double CritDMG { get; set; } = 1.25;

        /// <summary>
        /// 初始闪避率
        /// </summary>
        public double EvadeRate { get; set; } = 0.05;

        /// <summary>
        /// 每级增加基础生命值
        /// </summary>
        public double LevelToHPFactor { get; set; } = 17;

        /// <summary>
        /// 生命值增长因子
        /// </summary>
        public double HPGrowthFactor { get; set; } = 0.68;

        /// <summary>
        /// 每级增加基础魔法值
        /// </summary>
        public double LevelToMPFactor { get; set; } = 1.5;

        /// <summary>
        /// 魔法值增长因子
        /// </summary>
        public double MPGrowthFactor { get; set; } = 0.14;

        /// <summary>
        /// 每级增加基础攻击力
        /// </summary>
        public double LevelToATKFactor { get; set; } = 0.95;

        /// <summary>
        /// 攻击力增长因子
        /// </summary>
        public double ATKGrowthFactor { get; set; } = 0.045;

        /// <summary>
        /// 物理伤害减免因子
        /// </summary>
        public double DEFReductionFactor { get; set; } = 240;

        /// <summary>
        /// 行动速度上限
        /// </summary>
        public double SPDUpperLimit { get; set; } = 1500;

        /// <summary>
        /// 每 1 点力量增加生命值
        /// </summary>
        public double STRtoHPFactor { get; set; } = 9;

        /// <summary>
        /// 每 1 点力量增加生命回复力
        /// </summary>
        public double STRtoHRFactor { get; set; } = 0.1;

        /// <summary>
        /// 每 1 点力量增加物理护甲
        /// </summary>
        public double STRtoDEFFactor { get; set; } = 0.75;

        /// <summary>
        /// 每 1 点力量增加暴击伤害
        /// </summary>
        public double STRtoCritDMGMultiplier { get; set; } = 0.00575;

        /// <summary>
        /// 每 1 点智力增加魔法值
        /// </summary>
        public double INTtoMPFactor { get; set; } = 8;

        /// <summary>
        /// 每 1 点智力增加冷却缩减
        /// </summary>
        public double INTtoCDRMultiplier { get; set; } = 0.0025;

        /// <summary>
        /// 每 1 点智力增加魔法回复力
        /// </summary>
        public double INTtoMRFactor { get; set; } = 0.04;

        /// <summary>
        /// 每 1 点智力减少魔法消耗
        /// </summary>
        public double INTtoCastMPReduce { get; set; } = 0.00125;

        /// <summary>
        /// 每 1 点智力减少能量消耗
        /// </summary>
        public double INTtoCastEPReduce { get; set; } = 0.00075;

        /// <summary>
        /// 每 1 点智力增加加速系数
        /// </summary>
        public double INTtoAccelerationCoefficientMultiplier { get; set; } = 0.00125;

        /// <summary>
        /// 每 1 点敏捷增加行动速度
        /// </summary>
        public double AGItoSPDMultiplier { get; set; } = 0.65;

        /// <summary>
        /// 每 1 点敏捷增加暴击率
        /// </summary>
        public double AGItoCritRateMultiplier { get; set; } = 0.0025;

        /// <summary>
        /// 每 1 点敏捷增加闪避率
        /// </summary>
        public double AGItoEvadeRateMultiplier { get; set; } = 0.00175;

        /// <summary>
        /// 造成伤害获得能量值因子
        /// </summary>
        public double DamageGetEPFactor { get; set; } = 0.03;

        /// <summary>
        /// 造成伤害获得能量值上限
        /// </summary>
        public double DamageGetEPMax { get; set; } = 30;

        /// <summary>
        /// 受到伤害获得能量值因子
        /// </summary>
        public double TakenDamageGetEPFactor { get; set; } = 0.015;

        /// <summary>
        /// 受到伤害获得能量值上限
        /// </summary>
        public double TakenDamageGetEPMax { get; set; } = 15;

        /// <summary>
        /// 应用此游戏平衡常数给实体
        /// </summary>
        /// <param name="entities"></param>
        public void SetEquilibriumConstant(params BaseEntity[] entities)
        {
            foreach (BaseEntity entity in entities)
            {
                entity.GameplayEquilibriumConstant = this;
            }
        }
    }
}
