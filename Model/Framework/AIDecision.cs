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
    }
}
