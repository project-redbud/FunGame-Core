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
        /// 晋升点数上限
        /// </summary>
        public int PromotionThresholdXUpperLimit { get; set; } = 999;

        /// <summary>
        /// 角色评级 S 的晋升点数上限
        /// </summary>
        public int PromotionThresholdSUpperLimit { get; set; } = 998;

        /// <summary>
        /// 角色评级 A+ 的晋升点数上限
        /// </summary>
        public int PromotionThresholdAPlusUpperLimit { get; set; } = 850;

        /// <summary>
        /// 角色评级 A 的晋升点数上限
        /// </summary>
        public int PromotionThresholdAUpperLimit { get; set; } = 700;

        /// <summary>
        /// 角色评级 B 的晋升点数上限
        /// </summary>
        public int PromotionThresholdBUpperLimit { get; set; } = 550;

        /// <summary>
        /// 角色评级 C 的晋升点数上限
        /// </summary>
        public int PromotionThresholdCUpperLimit { get; set; } = 400;

        /// <summary>
        /// 角色评级 D 的晋升点数上限
        /// </summary>
        public int PromotionThresholdDUpperLimit { get; set; } = 300;

        /// <summary>
        /// 角色评级 E 的晋升点数上限
        /// </summary>
        public int PromotionThresholdEUpperLimit { get; set; } = 200;

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
        public double DEFReductionFactor { get; set; } = 120;

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
        public double STRtoHRFactor { get; set; } = 0.25;

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
        public double INTtoMRFactor { get; set; } = 0.1;

        /// <summary>
        /// 每 1 点智力减少魔法消耗
        /// </summary>
        public double INTtoCastMPReduce { get; set; } = 0.00125;

        /// <summary>
        /// 每 1 点智力减少能量消耗
        /// </summary>
        public double INTtoCastEPReduce { get; set; } = 0.00075;

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
        public double AGItoEvadeRateMultiplier { get; set; } = 0.0025;
    }
}
