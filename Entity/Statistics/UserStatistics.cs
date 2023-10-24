using System.Collections;

namespace Milimoe.FunGame.Core.Entity
{
    public class UserStatistics
    {
        /**
         * Key为赛季(long)，每个key代表第key赛季，key = 0时为生涯数据。
         */

        public int Id { get; set; }
        public User User { get; set; } = new User();
        public Dictionary<long, decimal> DamageStats { get; set; } = new Dictionary<long, decimal>();
        public Dictionary<long, decimal> PhysicalDamageStats { get; set; } = new Dictionary<long, decimal>();
        public Dictionary<long, decimal> MagicDamageStats { get; set; } = new Dictionary<long, decimal>();
        public Dictionary<long, decimal> RealDamageStats { get; set; } = new Dictionary<long, decimal>();
        public Dictionary<long, decimal> AvgDamageStats
        {
            get
            {
                Dictionary<long, decimal> avgdamage = new Dictionary<long, decimal>();
                foreach (long key in Plays.Keys)
                {
                    long plays = Plays[key];
                    decimal total = 0;
                    if (DamageStats.ContainsKey(key))
                    {
                        total = DamageStats.Values.Sum();
                    }
                    avgdamage.Add(key, total / plays);
                }
                return avgdamage;
            }
        }
        public Dictionary<long, decimal> AvgPhysicalDamageStats
        {
            get
            {
                Dictionary<long, decimal> avgdamage = new Dictionary<long, decimal>();
                foreach (long key in Plays.Keys)
                {
                    long plays = Plays[key];
                    decimal total = 0;
                    if (PhysicalDamageStats.ContainsKey(key))
                    {
                        total = PhysicalDamageStats.Values.Sum();
                    }
                    avgdamage.Add(key, total / plays);
                }
                return avgdamage;
            }
        }
        public Dictionary<long, decimal> AvgMagicDamageStats
        {
            get
            {
                Dictionary<long, decimal> avgdamage = new Dictionary<long, decimal>();
                foreach (long key in Plays.Keys)
                {
                    long plays = Plays[key];
                    decimal total = 0;
                    if (MagicDamageStats.ContainsKey(key))
                    {
                        total = MagicDamageStats.Values.Sum();
                    }
                    avgdamage.Add(key, total / plays);
                }
                return avgdamage;
            }
        }
        public Dictionary<long, decimal> AvgRealDamageStats
        {
            get
            {
                Dictionary<long, decimal> avgdamage = new Dictionary<long, decimal>();
                foreach (long key in Plays.Keys)
                {
                    long plays = Plays[key];
                    decimal total = 0;
                    if (RealDamageStats.ContainsKey(key))
                    {
                        total = RealDamageStats.Values.Sum();
                    }
                    avgdamage.Add(key, total / plays);
                }
                return avgdamage;
            }
        }
        public Dictionary<long, long> Kills { get; set; } = new Dictionary<long, long>();
        public Dictionary<long, long> Deaths { get; set; } = new Dictionary<long, long>();
        public Dictionary<long, long> Assists { get; set; } = new Dictionary<long, long>();
        public Dictionary<long, long> Plays { get; set; } = new Dictionary<long, long>();
        public Dictionary<long, long> Wins { get; set; } = new Dictionary<long, long>();
        public Dictionary<long, long> Loses { get; set; } = new Dictionary<long, long>();
        public Dictionary<long, decimal> Winrates
        {
            get
            {
                Dictionary<long, decimal> winrates = new Dictionary<long, decimal>();
                foreach (long key in Plays.Keys)
                {
                    long plays = Plays[key];
                    long wins = 0;
                    if (Wins.ContainsKey(key))
                    {
                        wins = Wins[key];
                    }
                    winrates.Add(key, wins / plays * 0.01M);
                }
                return winrates;
            }
        }
        public Dictionary<string, decimal> RatingStats { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> EloStats { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, string> RankStats { get; set; } = new Dictionary<string, string>();
    }
}
