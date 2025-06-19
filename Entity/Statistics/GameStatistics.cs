namespace Milimoe.FunGame.Core.Entity
{
    public class GameStatistics(Room Room)
    {
        public long Id => Room.Id;
        public Room Room { get; } = Room;
        public DateTime RecordTime { get; set; } = DateTime.Now;
        public string Record { get; set; } = "";
        public Dictionary<User, double> DamageStats { get; set; } = [];
        public Dictionary<User, double> PhysicalDamageStats { get; } = [];
        public Dictionary<User, double> MagicDamageStats { get; } = [];
        public Dictionary<User, double> TrueDamageStats { get; } = [];
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
        public double AvgTrueDamageStats
        {
            get
            {
                double total = 0;
                foreach (User user in TrueDamageStats.Keys)
                {
                    total += TrueDamageStats[user];
                }
                return Math.Round(total / TrueDamageStats.Count, 2);
            }
        }
        public Dictionary<User, double> KillStats { get; } = [];
        public Dictionary<User, Dictionary<User, int>> KillDetailStats { get; } = []; // 子字典记录的是被击杀者以及被击杀次数
        public Dictionary<User, double> DeathStats { get; } = [];
        public Dictionary<User, Dictionary<User, int>> DeathDetailStats { get; } = []; // 子字典记录的是击杀者以及击杀次数
        public Dictionary<User, long> AssistStats { get; } = [];
        public Dictionary<User, double> RatingStats { get; } = []; // 结算后的Rating
        public Dictionary<User, double> EloStats { get; } = []; // Elo分数变化(+/-)
        public Dictionary<User, string> RankStats { get; } = []; // 结算后的Rank（非比赛前）
    }
}
