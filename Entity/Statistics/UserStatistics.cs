namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 记录 <see cref="Entity.User"/> 的生涯、赛季统计数据<para/>
    /// Key为赛季(long)，每个key代表第key赛季，key = 0时为生涯数据。
    /// </summary>
    public class UserStatistics
    {
        public long Id => User.Id;
        public User User { get; }
        public Dictionary<long, double> DamageStats { get; } = [];
        public Dictionary<long, double> PhysicalDamageStats { get; } = [];
        public Dictionary<long, double> MagicDamageStats { get; } = [];
        public Dictionary<long, double> TrueDamageStats { get; } = [];
        public Dictionary<long, double> AvgDamageStats
        {
            get
            {
                Dictionary<long, double> avgdamage = [];
                foreach (long key in Plays.Keys)
                {
                    long plays = Plays[key];
                    double total = 0;
                    if (DamageStats.ContainsKey(key))
                    {
                        total = DamageStats.Values.Sum();
                    }
                    avgdamage.Add(key, Math.Round(total / plays, 2));
                }
                return avgdamage;
            }
        }
        public Dictionary<long, double> AvgPhysicalDamageStats
        {
            get
            {
                Dictionary<long, double> avgdamage = [];
                foreach (long key in Plays.Keys)
                {
                    long plays = Plays[key];
                    double total = 0;
                    if (PhysicalDamageStats.ContainsKey(key))
                    {
                        total = PhysicalDamageStats.Values.Sum();
                    }
                    avgdamage.Add(key, Math.Round(total / plays, 2));
                }
                return avgdamage;
            }
        }
        public Dictionary<long, double> AvgMagicDamageStats
        {
            get
            {
                Dictionary<long, double> avgdamage = [];
                foreach (long key in Plays.Keys)
                {
                    long plays = Plays[key];
                    double total = 0;
                    if (MagicDamageStats.ContainsKey(key))
                    {
                        total = MagicDamageStats.Values.Sum();
                    }
                    avgdamage.Add(key, Math.Round(total / plays, 2));
                }
                return avgdamage;
            }
        }
        public Dictionary<long, double> AvgTrueDamageStats
        {
            get
            {
                Dictionary<long, double> avgdamage = [];
                foreach (long key in Plays.Keys)
                {
                    long plays = Plays[key];
                    double total = 0;
                    if (TrueDamageStats.ContainsKey(key))
                    {
                        total = TrueDamageStats.Values.Sum();
                    }
                    avgdamage.Add(key, Math.Round(total / plays, 2));
                }
                return avgdamage;
            }
        }
        public Dictionary<long, long> Kills { get; } = [];
        public Dictionary<long, long> Deaths { get; } = [];
        public Dictionary<long, long> Assists { get; } = [];
        public Dictionary<long, long> Plays { get; } = [];
        public Dictionary<long, long> Wins { get; } = [];
        public Dictionary<long, long> Loses { get; } = [];
        public Dictionary<long, double> Winrates
        {
            get
            {
                Dictionary<long, double> winrates = [];
                foreach (long key in Plays.Keys)
                {
                    long plays = Plays[key];
                    long wins = 0;
                    if (Wins.TryGetValue(key, out long value))
                    {
                        wins = value;
                    }
                    winrates.Add(key, Math.Round(wins * 1.0 / plays * 1.0, 4));
                }
                return winrates;
            }
        }
        public Dictionary<long, double> RatingStats { get; } = [];
        public Dictionary<long, double> EloStats { get; } = [];
        public Dictionary<long, string> RankStats { get; } = [];

        public string GetWinrate(long season)
        {
            if (Winrates.TryGetValue(season, out double value))
            {
                return value.ToString("0.##%");
            }
            return "0%";
        }

        internal UserStatistics(User user)
        {
            User = user;
        }
    }
}
