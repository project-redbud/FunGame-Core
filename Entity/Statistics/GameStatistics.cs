namespace Milimoe.FunGame.Core.Entity
{
    public class GameStatistics
    {
        public long Id => Room.Id;
        public Room Room { get; }
        public DateTime RecordTime { get; set; } = DateTime.Now;
        public string Record { get; set; } = "";
        public Dictionary<User, double> DamageStats { get; set; } = new();
        public Dictionary<User, double> PhysicalDamageStats { get; } = new();
        public Dictionary<User, double> MagicDamageStats { get; } = new();
        public Dictionary<User, double> RealDamageStats { get; } = new();
        public double AvgDamageStats
        {
            get
            {
                double total = 0;
                foreach (User user in DamageStats.Keys)
                {
                    total += DamageStats[user];
                }
                return Math.Round(total / DamageStats.Count, 2);
            }
        }
        public double AvgPhysicalDamageStats
        {
            get
            {
                double total = 0;
                foreach (User user in PhysicalDamageStats.Keys)
                {
                    total += PhysicalDamageStats[user];
                }
                return Math.Round(total / PhysicalDamageStats.Count, 2);
            }
        }
        public double AvgMagicDamageStats
        {
            get
            {
                double total = 0;
                foreach (User user in MagicDamageStats.Keys)
                {
                    total += MagicDamageStats[user];
                }
                return Math.Round(total / MagicDamageStats.Count, 2);
            }
        }
        public double AvgRealDamageStats
        {
            get
            {
                double total = 0;
                foreach (User user in RealDamageStats.Keys)
                {
                    total += RealDamageStats[user];
                }
                return Math.Round(total / RealDamageStats.Count, 2);
            }
        }
        public Dictionary<User, double> KillStats { get; } = new();
        public Dictionary<User, Dictionary<User, int>> KillDetailStats { get; } = new(); // 子字典记录的是被击杀者以及被击杀次数
        public Dictionary<User, double> DeathStats { get; } = new();
        public Dictionary<User, Dictionary<User, int>> DeathDetailStats { get; } = new(); // 子字典记录的是击杀者以及击杀次数
        public Dictionary<User, long> AssistStats { get; } = new();
        public Dictionary<User, double> RatingStats { get; } = new(); // 结算后的Rating
        public Dictionary<User, double> EloStats { get; } = new(); // Elo分数变化(+/-)
        public Dictionary<User, string> RankStats { get; } = new(); // 结算后的Rank（非比赛前）

        public GameStatistics(Room Room)
        {
            this.Room = Room;
        }
    }
}
