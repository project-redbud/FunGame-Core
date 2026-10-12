using FunGame.Core.Entity;

namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 排名条目（游戏结束时的胜者与排名信息，按名次顺序排列）<para/>
    /// 使用结构化引用（<see cref="Character"/> / <see cref="Team"/>）与统计数值，消费端可基于引用定制渲染
    /// </summary>
    public class RankingEntry
    {
        /// <summary>
        /// 名次（1 = 冠军/胜者）
        /// </summary>
        public int Rank { get; set; } = 0;

        /// <summary>
        /// 是否为胜者
        /// </summary>
        public bool IsWinner { get; set; } = false;

        /// <summary>
        /// 是否为团队条目（true 时使用 <see cref="Team"/>，false 时使用 <see cref="Character"/>）
        /// </summary>
        public bool IsTeam { get; set; } = false;

        /// <summary>
        /// 角色引用（角色排名条目）
        /// </summary>
        public Character? Character { get; set; } = null;

        /// <summary>
        /// 团队引用（团队排名条目）
        /// </summary>
        public Team? Team { get; set; } = null;

        /// <summary>
        /// 击杀数
        /// </summary>
        public int Kills { get; set; } = 0;

        /// <summary>
        /// 死亡数
        /// </summary>
        public int Deaths { get; set; } = 0;

        /// <summary>
        /// 助攻数
        /// </summary>
        public int Assists { get; set; } = 0;

        /// <summary>
        /// 第一滴血数
        /// </summary>
        public int FirstKills { get; set; } = 0;

        /// <summary>
        /// 累计赚取金币
        /// </summary>
        public int TotalEarnedMoney { get; set; } = 0;

        /// <summary>
        /// 最大连杀数
        /// </summary>
        public int MaxContinuousKilling { get; set; } = 0;

        /// <summary>
        /// 团队得分（团队条目）
        /// </summary>
        public int Score { get; set; } = 0;
    }
}
