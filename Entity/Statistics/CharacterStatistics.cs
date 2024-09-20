namespace Milimoe.FunGame.Core.Entity
{
    public class CharacterStatistics
    {
        public double TotalDamage { get; set; } = 0;
        public double TotalPhysicalDamage { get; set; } = 0;
        public double TotalMagicDamage { get; set; } = 0;
        public double TotalRealDamage { get; set; } = 0;
        public double TotalTakenDamage { get; set; } = 0;
        public double TotalTakenPhysicalDamage { get; set; } = 0;
        public double TotalTakenMagicDamage { get; set; } = 0;
        public double TotalTakenRealDamage { get; set; } = 0;
        public double AvgDamage { get; set; } = 0;
        public double AvgPhysicalDamage { get; set; } = 0;
        public double AvgMagicDamage { get; set; } = 0;
        public double AvgRealDamage { get; set; } = 0;
        public double AvgTakenDamage { get; set; } = 0;
        public double AvgTakenPhysicalDamage { get; set; } = 0;
        public double AvgTakenMagicDamage { get; set; } = 0;
        public double AvgTakenRealDamage { get; set; } = 0;
        public int LiveRound { get; set; } = 0;
        public int AvgLiveRound { get; set; } = 0;
        public int ActionTurn { get; set; } = 0;
        public int AvgActionTurn { get; set; } = 0;
        public double LiveTime { get; set; } = 0;
        public double AvgLiveTime { get; set; } = 0;
        public double DamagePerRound { get; set; } = 0;
        public double DamagePerTurn { get; set; } = 0;
        public double DamagePerSecond { get; set; } = 0;
        public int TotalEarnedMoney { get; set; } = 0;
        public int AvgEarnedMoney { get; set; } = 0;
        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public int Assists { get; set; } = 0;
        public int Plays { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Top3s { get; set; } = 0;
        public int Loses { get; set; } = 0;
        public double Winrates { get; set; } = 0;
        public double Top3rates { get; set; } = 0;
    }
}
