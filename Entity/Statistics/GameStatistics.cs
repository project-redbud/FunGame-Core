using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class GameStatistics
    {
        public long Id => Room.Id;
        public Room Room { get; set; } = General.HallInstance;
        public DateTime RecordTime { get; set; } = DateTime.Now;
        public string Record { get; set; } = "";
        public Dictionary<User, decimal> DamageStats { get; set; } = new();
        public Dictionary<User, decimal> PhysicalDamageStats { get; } = new();
        public Dictionary<User, decimal> MagicDamageStats { get; } = new();
        public Dictionary<User, decimal> RealDamageStats { get; } = new();
        public Dictionary<User, decimal> AvgDamageStats { get; } = new();
        public Dictionary<User, decimal> AvgPhysicalDamageStats { get; } = new();
        public Dictionary<User, decimal> AvgMagicDamageStats { get; } = new();
        public Dictionary<User, decimal> AvgRealDamageStats { get; } = new();
        public Dictionary<User, decimal> KillStats { get; } = new();
        public Dictionary<User, Dictionary<User, int>> KillDetailStats { get; } = new(); // 子字典记录的是被击杀者以及被击杀次数
        public Dictionary<User, decimal> DeathStats { get; } = new();
        public Dictionary<User, Dictionary<User, int>> DeathDetailStats { get; } = new(); // 子字典记录的是击杀者以及击杀次数
        public Dictionary<User, long> AssistStats { get; } = new();
        public Dictionary<User, decimal> RatingStats { get; } = new(); // 结算后的Rating
        public Dictionary<User, decimal> EloStats { get; } = new(); // Elo分数变化(+/-)
        public Dictionary<User, string> RankStats { get; } = new(); // 结算后的Rank（非比赛前）
    }
}
