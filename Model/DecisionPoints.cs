using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    public class DecisionPoints
    {
        /// <summary>
        /// 所用的游戏平衡常数
        /// </summary>
        [JsonIgnore]
        public EquilibriumConstant GameplayEquilibriumConstant { get; set; } = General.GameplayEquilibriumConstant;

        /// <summary>
        /// 当前决策点
        /// </summary>
        public int CurrentDecisionPoints { get; set; } = General.GameplayEquilibriumConstant.InitialDecisionPoints;

        /// <summary>
        /// 决策点上限
        /// </summary>
        public int MaxDecisionPoints { get; set; } = General.GameplayEquilibriumConstant.InitialDecisionPoints;

        /// <summary>
        /// 每回合决策点补充数量
        /// </summary>
        public int RecoverDecisionPointsPerRound { get; set; } = General.GameplayEquilibriumConstant.RecoverDecisionPointsPerRound;

        /// <summary>
        /// 本回合已补充的决策点
        /// </summary>
        public int DecisionPointsRecovery { get; set; } = 0;

        /// <summary>
        /// 是否释放过勇气指令
        /// </summary>
        public bool CourageCommandSkill { get; set; } = false;

        /// <summary>
        /// 记录本回合已使用的行动类型和次数
        /// </summary>
        public Dictionary<CharacterActionType, int> ActionTypes { get; } = [];

        /// <summary>
        /// 记录本回合行动的硬直时间
        /// </summary>
        public List<double> ActionsHardnessTime { get; } = [];

        /// <summary>
        /// 本回合已进行的行动次数
        /// </summary>
        public int ActionsTaken { get; set; } = 0;

        // 回合内的临时决策点配额加成
        private int _tempActionQuotaNormalAttack = 0;
        private int _tempActionQuotaSuperSkill = 0;
        private int _tempActionQuotaSkill = 0;
        private int _tempActionQuotaItem = 0;
        private int _tempActionQuotaOther = 0;

        /// <summary>
        /// 获取当前决策点配额
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int this[CharacterActionType type]
        {
            get
            {
                return type switch
                {
                    CharacterActionType.NormalAttack => GameplayEquilibriumConstant.ActionQuotaNormalAttack + _tempActionQuotaNormalAttack,
                    CharacterActionType.CastSuperSkill => GameplayEquilibriumConstant.ActionQuotaSuperSkill + _tempActionQuotaSuperSkill,
                    CharacterActionType.CastSkill => GameplayEquilibriumConstant.ActionQuotaSkill + _tempActionQuotaSkill,
                    CharacterActionType.PreCastSkill => GameplayEquilibriumConstant.ActionQuotaMagic,
                    CharacterActionType.UseItem => GameplayEquilibriumConstant.ActionQuotaItem + _tempActionQuotaItem,
                    _ => GameplayEquilibriumConstant.ActionQuotaOther + _tempActionQuotaOther,
                };
            }
        }

        /// <summary>
        /// 添加临时决策点配额 [ 回合结束时清除 ]
        /// </summary>
        /// <param name="type"></param>
        /// <param name="add"></param>
        public void AddTempActionQuota(CharacterActionType type, int add = 1)
        {
            switch (type)
            {
                case CharacterActionType.NormalAttack:
                    _tempActionQuotaNormalAttack += add;
                    break;
                case CharacterActionType.CastSkill:
                    _tempActionQuotaSkill += add;
                    break;
                case CharacterActionType.CastSuperSkill:
                    _tempActionQuotaSuperSkill += add;
                    break;
                case CharacterActionType.UseItem:
                    _tempActionQuotaItem += add;
                    break;
                default:
                    _tempActionQuotaOther += add;
                    break;
            }
        }

        /// <summary>
        /// 清除临时决策点配额
        /// </summary>
        public void ClearTempActionQuota()
        {
            _tempActionQuotaNormalAttack = 0;
            _tempActionQuotaSuperSkill = 0;
            _tempActionQuotaSkill = 0;
            _tempActionQuotaItem = 0;
            _tempActionQuotaOther = 0;
        }

        /// <summary>
        /// 累计行动类型和次数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="addActionTaken"></param>
        public void AddActionType(CharacterActionType type, bool addActionTaken = true)
        {
            if (addActionTaken) ActionsTaken++;
            if (!ActionTypes.TryAdd(type, 1))
            {
                ActionTypes[type]++;
            }
        }

        /// <summary>
        /// 判断行动类型是否达到配额
        /// </summary>
        /// <param name="type"></param>
        public bool CheckActionTypeQuota(CharacterActionType type)
        {
            if (ActionTypes.TryGetValue(type, out int times))
            {
                return times < this[type];
            }
            return true;
        }

        /// <summary>
        /// 获取决策点消耗
        /// </summary>
        /// <param name="type"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        public int GetActionPointCost(CharacterActionType type, Skill? skill = null)
        {
            return type switch
            {
                CharacterActionType.NormalAttack => GameplayEquilibriumConstant.DecisionPointsCostNormalAttack,
                CharacterActionType.PreCastSkill when skill?.SkillType == SkillType.SuperSkill => GameplayEquilibriumConstant.DecisionPointsCostSuperSkill,
                CharacterActionType.PreCastSkill when skill?.SkillType == SkillType.Skill => GameplayEquilibriumConstant.DecisionPointsCostSkill,
                CharacterActionType.PreCastSkill when skill?.SkillType == SkillType.Magic => GameplayEquilibriumConstant.DecisionPointsCostMagic,
                CharacterActionType.UseItem => GameplayEquilibriumConstant.DecisionPointsCostItem,
                CharacterActionType.CastSuperSkill => GameplayEquilibriumConstant.DecisionPointsCostSuperSkillOutOfTurn, // 回合外使用爆发技
                _ => GameplayEquilibriumConstant.DecisionPointsCostOther
            };
        }
    }
}
