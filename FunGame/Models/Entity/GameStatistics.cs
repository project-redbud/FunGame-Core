using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Models.Entity
{
    public class GameStatistics
    {
        public int Id { get; set; }
        public Room? Room { get; set; } = null;
        public string? GameRecord { get; set; } = null;
        public string? RunTime { get; set; } = null;
        public Hashtable? DamageStats { get; set; } = new Hashtable();
        public Hashtable? PhysicalDamageStats { get; set; } = new Hashtable();
        public Hashtable? MagicDamageStats { get; set; } = new Hashtable();
        public Hashtable? RealDamageStats { get; set; } = new Hashtable();
        public Hashtable? AvgDamageStats { get; set; } = new Hashtable();
        public Hashtable? KillStats { get; set; } = new Hashtable();
        public Hashtable? KillDetailStats { get; set; } = new Hashtable();
        public Hashtable? DeathStats { get; set; } = new Hashtable();
        public Hashtable? DeathDetailStats { get; set; } = new Hashtable();
        public Hashtable? AssistStats { get; set; } = new Hashtable();
        public Hashtable? RatingStats { get; set; } = new Hashtable();
        public Hashtable? EloStats { get; set; } = new Hashtable();
        public Hashtable? RankStats { get; set; } = new Hashtable();

        public GameStatistics()
        {
            
        }
    }
}
