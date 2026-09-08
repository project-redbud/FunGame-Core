using FunGame.Core.Entity;
using FunGame.Core.Interface.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    public class AIDecision
    {
        public CharacterActionType ActionType { get; set; } = CharacterActionType.EndTurn;
        public Grid? TargetMoveGrid { get; set; } = null;
        public ISkill? SkillToUse { get; set; } = null;
        public Item? ItemToUse { get; set; } = null;
        public List<Character> Targets { get; set; } = [];
        public List<Grid> TargetGrids { get; set; } = [];
        public double Score { get; set; } = 0;
        public double ProbabilityWeight { get; set; } = 0;
        public bool IsPureMove { get; set; } = false;

        /// <summary>
        /// 该决策是否来自 AI 实际评估出的候选行动
        /// <para>false 表示 AI 未能评估出任何可行行动（此时为默认的结束回合占位），宿主应回退到原有的事件与概率决策</para>
        /// </summary>
        public bool HasCandidate { get; set; } = false;
    }
}
