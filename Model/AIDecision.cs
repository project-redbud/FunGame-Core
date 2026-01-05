using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
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
        public bool IsPureMove { get; set; } = false;
    }
}
