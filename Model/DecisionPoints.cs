using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    public class DecisionPoints
    {
        /// <summary>
        /// 当前决策点
        /// </summary>
        public int CurrentDecisionPoints { get; set; } = 2;

        /// <summary>
        /// 决策点上限
        /// </summary>
        public int MaxDecisionPoints { get; set; } = 2;

        /// <summary>
        /// 本回合已补充的决策点
        /// </summary>
        public int DecisionPointsRecovery { get; set; } = 0;

        /// <summary>
        /// 是否释放过勇气指令
        /// </summary>
        public bool CourageCommandSkill { get; set; } = false;

        /// <summary>
        /// 记录本回合已使用的行动类型
        /// </summary>
        public HashSet<CharacterActionType> ActionTypes { get; } = [];

        /// <summary>
        /// 记录本回合行动的硬直时间
        /// </summary>
        public List<double> ActionsHardnessTime { get; } = [];

        /// <summary>
        /// 本回合已进行的行动次数
        /// </summary>
        public int ActionsTaken { get; set; } = 0;

        /// <summary>
        /// 检查是否可以决策
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        public static int GetActionPointCost(CharacterActionType actionType, Skill? skill = null)
        {
            return actionType switch
            {
                CharacterActionType.NormalAttack => 1,
                CharacterActionType.PreCastSkill when skill?.SkillType == SkillType.SuperSkill => 2,
                CharacterActionType.PreCastSkill when skill?.SkillType == SkillType.Skill => 2,
                CharacterActionType.PreCastSkill when skill?.SkillType == SkillType.Magic => 2,
                CharacterActionType.UseItem => 1,
                CharacterActionType.CastSuperSkill => 3, // 回合外使用爆发技
                _ => 0
            };
        }
    }
}
