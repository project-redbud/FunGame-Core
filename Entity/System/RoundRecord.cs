using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity.System
{
    public class RoundRecord(int round, Character actor)
    {
        public int Round { get; set; } = round;
        public Character Actor { get; set; } = actor;
        public CharacterActionType ActionType { get; set; } = CharacterActionType.None;
        public List<Character> Targets { get; set; } = [];
        public Skill? Skill { get; set; } = null;
        public Item? Item { get; set; } = null;
    }
}
