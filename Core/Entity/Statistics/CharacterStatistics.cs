using System.Collections;

namespace Milimoe.FunGame.Core.Entity
{
    public class CharacterStatistics
    {
        public int Id { get; set; }
        public Character? Character { get; set; } = null;
        public Hashtable? DamageStats { get; set; } = new Hashtable();
        public Hashtable? PhysicalDamageStats { get; set; } = new Hashtable();
        public Hashtable? MagicDamageStats { get; set; } = new Hashtable();
        public Hashtable? RealDamageStats { get; set; } = new Hashtable();
        public Hashtable? AvgDamageStats { get; set; } = new Hashtable();
        public Hashtable? AvgLiveRoundStats { get; set; } = new Hashtable();
        public Hashtable? AvgDamageRoundStats { get; set; } = new Hashtable();
        public Hashtable? KillStats { get; set; } = new Hashtable();
        public Hashtable? DeathStats { get; set; } = new Hashtable();
        public Hashtable? AssistStats { get; set; } = new Hashtable();
        public Hashtable? Plays { get; set; } = new Hashtable();
        public Hashtable? Wins { get; set; } = new Hashtable();
        public Hashtable? Loses { get; set; } = new Hashtable();
        public Hashtable? Winrates { get; set; } = new Hashtable();
    }
}
