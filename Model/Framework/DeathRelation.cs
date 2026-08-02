using FunGame.Core.Entity;

namespace FunGame.Core.Model.Framework
{
    public class DeathRelation(Character death, Character? killer, params Character[] assists)
    {
        public Character Death { get; set; } = death;
        public Character? Killer { get; set; } = killer;
        public Character[] Assists { get; set; } = assists;
    }
}
