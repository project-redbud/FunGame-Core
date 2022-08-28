using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Models.Entity
{
    public class UserStatistics
    {
        public int Id { get; set; }
        public User? User { get; set; } = null;
        public Hashtable? DamageStats { get; set; } = new Hashtable();
        public Hashtable? PhysicalDamageStats { get; set; } = new Hashtable();
        public Hashtable? MagicDamageStats { get; set; } = new Hashtable();
        public Hashtable? RealDamageStats { get; set; } = new Hashtable();
        public Hashtable? AvgDamageStats { get; set; } = new Hashtable();
        public Hashtable? KillStats { get; set; } = new Hashtable();
        public Hashtable? DeathStats { get; set; } = new Hashtable();
        public Hashtable? AssistStats { get; set; } = new Hashtable();
        public Hashtable? Plays { get; set; } = new Hashtable();
        public Hashtable? Wins { get; set; } = new Hashtable();
        public Hashtable? Loses { get; set; } = new Hashtable();
        public Hashtable? Winrates { get; set; } = new Hashtable();
        public Hashtable? RatingStats { get; set; } = new Hashtable();
        public Hashtable? EloStats { get; set; } = new Hashtable();
        public Hashtable? RankStats { get; set; } = new Hashtable();

        public UserStatistics()
        {

        }
    }
}
