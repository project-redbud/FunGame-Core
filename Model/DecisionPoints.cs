using System.Text;
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

        /// <summary>
        /// 本回合已使用的决策点
        /// </summary>
        public int DecisionPointsCost { get; set; } = 0;

        /// <summary>
        /// 临时全能决策点配额加成
        /// </summary>
        public int TempActionQuotaAllRound => _tempActionQuotaAllRound.Values.Sum();

        /// <summary>
        /// 临时普通攻击决策点配额加成
        /// </summary>
        public int TempActionQuotaNormalAttack => _tempActionQuotaNormalAttack.Values.Sum();

        /// <summary>
        /// 临时爆发技决策点配额加成
        /// </summary>
        public int TempActionQuotaSuperSkill => _tempActionQuotaSuperSkill.Values.Sum();

        /// <summary>
        /// 临时战技决策点配额加成
        /// </summary>
        public int TempActionQuotaSkill => _tempActionQuotaSkill.Values.Sum();

        /// <summary>
        /// 临时使用物品决策点配额加成
        /// </summary>
        public int TempActionQuotaItem => _tempActionQuotaItem.Values.Sum();

        /// <summary>
        /// 临时其他决策点配额加成
        /// </summary>
        public int TempActionQuotaOther => _tempActionQuotaOther.Values.Sum();

        // 回合内的临时决策点配额加成
        private readonly Dictionary<Effect, int> _tempActionQuotaAllRound = [];
        private readonly Dictionary<Effect, int> _tempActionQuotaNormalAttack = [];
        private readonly Dictionary<Effect, int> _tempActionQuotaSuperSkill = [];
        private readonly Dictionary<Effect, int> _tempActionQuotaSkill = [];
        private readonly Dictionary<Effect, int> _tempActionQuotaItem = [];
        private readonly Dictionary<Effect, int> _tempActionQuotaOther = [];

        /// <summary>
        /// 获取决策点配额
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int this[CharacterActionType type]
        {
            get
            {
                return type switch
                {
                    CharacterActionType.NormalAttack => GameplayEquilibriumConstant.ActionQuotaNormalAttack + TempActionQuotaNormalAttack + TempActionQuotaAllRound,
                    CharacterActionType.CastSuperSkill => GameplayEquilibriumConstant.ActionQuotaSuperSkill + TempActionQuotaSuperSkill + TempActionQuotaAllRound,
                    CharacterActionType.CastSkill => GameplayEquilibriumConstant.ActionQuotaSkill + TempActionQuotaSkill + TempActionQuotaAllRound,
                    CharacterActionType.PreCastSkill => GameplayEquilibriumConstant.ActionQuotaMagic,
                    CharacterActionType.UseItem => GameplayEquilibriumConstant.ActionQuotaItem + TempActionQuotaItem + TempActionQuotaAllRound,
                    _ => GameplayEquilibriumConstant.ActionQuotaOther + TempActionQuotaOther + TempActionQuotaAllRound,
                };
            }
        }

        /// <summary>
        /// 添加临时决策点配额 [ 回合结束时清除 ]
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="type"></param>
        /// <param name="add"></param>
        public void AddTempActionQuota(Effect effect, CharacterActionType? type = null, int add = 1)
        {
            Dictionary<Effect, int> dict = type switch
            {
                null => _tempActionQuotaAllRound,
                CharacterActionType.NormalAttack => _tempActionQuotaNormalAttack,
                CharacterActionType.CastSkill => _tempActionQuotaSkill,
                CharacterActionType.CastSuperSkill => _tempActionQuotaSuperSkill,
                CharacterActionType.UseItem => _tempActionQuotaItem,
                _ => _tempActionQuotaOther
            };
            dict[effect] = dict.GetValueOrDefault(effect) + add;
        }

        /// <summary>
        /// 清除临时决策点配额
        /// </summary>
        public void ClearTempActionQuota()
        {
            _tempActionQuotaAllRound.Clear();
            _tempActionQuotaNormalAttack.Clear();
            _tempActionQuotaSuperSkill.Clear();
            _tempActionQuotaSkill.Clear();
            _tempActionQuotaItem.Clear();
            _tempActionQuotaOther.Clear();
        }

        /// <summary>
        /// 累计行动类型和次数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="skill"></param>
        /// <param name="addActionTaken"></param>
        public void AddActionType(CharacterActionType type, Skill? skill = null, bool addActionTaken = true)
        {
            if (addActionTaken) ActionsTaken++;
            if (!ActionTypes.TryAdd(type, 1))
            {
                ActionTypes[type]++;
            }
            DecisionPointsCost += GetActionPointCost(type, skill);
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

        /// <summary>
        /// 获取当前决策点信息
        /// </summary>
        /// <returns></returns>
        public string GetDecisionPointsInfo()
        {
            StringBuilder builder = new();

            builder.AppendLine($"===[ 决策点信息 ]===");
            builder.AppendLine($"当前决策点：{CurrentDecisionPoints} / {MaxDecisionPoints}");
            builder.AppendLine($"普通攻击决策点配额：{this[CharacterActionType.NormalAttack] - ActionTypes.GetValueOrDefault(CharacterActionType.NormalAttack)} / {this[CharacterActionType.NormalAttack]}");
            builder.AppendLine($"战技决策点配额：{this[CharacterActionType.CastSkill] - ActionTypes.GetValueOrDefault(CharacterActionType.CastSkill)} / {this[CharacterActionType.CastSkill]}");
            builder.AppendLine($"魔法决策点配额：{this[CharacterActionType.PreCastSkill] - ActionTypes.GetValueOrDefault(CharacterActionType.PreCastSkill)} / {this[CharacterActionType.PreCastSkill]}");
            builder.AppendLine($"爆发技决策点配额：{this[CharacterActionType.CastSuperSkill] - ActionTypes.GetValueOrDefault(CharacterActionType.CastSuperSkill)} / {this[CharacterActionType.CastSuperSkill]}");
            builder.AppendLine($"使用物品决策点配额：{this[CharacterActionType.UseItem] - ActionTypes.GetValueOrDefault(CharacterActionType.UseItem)} / {this[CharacterActionType.UseItem]}");

            if (_tempActionQuotaAllRound.Count > 0)
            {
                builder.AppendLine($"全能决策点配额：你拥有以下全能决策点配额加成并已自动计入每个行动决策所需的配额中。");
                builder.AppendLine(string.Join("\r\n", _tempActionQuotaAllRound.Select(kv => $"{kv.Key.Name}：{kv.Value}")));
            }

            return builder.ToString();
        }
    }
}
