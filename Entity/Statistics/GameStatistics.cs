namespace Milimoe.FunGame.Core.Entity
{
    public class GameStatistics
    {
        public long Id => Room.Id;
        public Room Room { get; }
        public DateTime RecordTime { get; set; } = DateTime.Now;
        public string Record { get; set; } = "";
        public Dictionary<User, decimal> DamageStats { get; set; } = new();
        public Dictionary<User, decimal> PhysicalDamageStats { get; } = new();
        public Dictionary<User, decimal> MagicDamageStats { get; } = new();
        public Dictionary<User, decimal> RealDamageStats { get; } = new();
        public decimal AvgDamageStats
        {
            get
            {
                decimal total = 0;
                foreach (User user in DamageStats.Keys)
                {
                    total += DamageStats[user];
                }
                return Math.Round(total / DamageStats.Count, 2);
            }
        }
        public decimal AvgPhysicalDamageStats
        {
            get
            {
                decimal total = 0;
                foreach (User user in PhysicalDamageStats.Keys)
                {
                    total += PhysicalDamageStats[user];
                }
                return Math.Round(total / PhysicalDamageStats.Count, 2);
            }
        }
        public decimal AvgMagicDamageStats
        {
            get
            {
                decimal total = 0;
                foreach (User user in MagicDamageStats.Keys)
                {
                    total += MagicDamageStats[user];
                }
                return Math.Round(total / MagicDamageStats.Count, 2);
            }
        }
        public decimal AvgRealDamageStats
        {
            get
            {
                decimal total = 0;
                foreach (User user in RealDamageStats.Keys)
                {
                    total += RealDamageStats[user];
                }
                return Math.Round(total / RealDamageStats.Count, 2);
            }
        }
        public Dictionary<User, decimal> KillStats { get; } = new();
        public Dictionary<User, Dictionary<User, int>> KillDetailStats { get; } = new(); // 子字典记录的是被击杀者以及被击杀次数
        public Dictionary<User, decimal> DeathStats { get; } = new();
        public Dictionary<User, Dictionary<User, int>> DeathDetailStats { get; } = new(); // 子字典记录的是击杀者以及击杀次数
        public Dictionary<User, long> AssistStats { get; } = new();
        public Dictionary<User, decimal> RatingStats { get; } = new(); // 结算后的Rating
        public Dictionary<User, decimal> EloStats { get; } = new(); // Elo分数变化(+/-)
        public Dictionary<User, string> RankStats { get; } = new(); // 结算后的Rank（非比赛前）

        public GameStatistics(Room Room)
        {
            this.Room = Room;
        }
    }
}
