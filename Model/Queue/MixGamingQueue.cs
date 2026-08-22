using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.Queue
{
    /// <summary>
    /// 混战游戏队列，增强版混战模式 <see cref="RoomType.Mix"/>
    /// </summary>
    public class MixGamingQueue : GamingQueue
    {
        /// <summary>
        /// 死亡结算后
        /// </summary>
        /// <param name="death"></param>
        /// <param name="killer"></param>
        /// <param name="assists"></param>
        /// <returns></returns>
        protected override void AfterDeathCalculation(Character death, Character? killer, Character[] assists)
        {
            if (MaxRespawnTimes != 0 && MaxScoreToWin > 0)
            {
                WriteLine($"\r\n=== 当前死亡竞赛比分 ===\r\n{string.Join("\r\n", _stats.OrderByDescending(kv => kv.Value.Kills)
                    .Select(kv => $"[ {kv.Key} ] {kv.Value.Kills} 分"))}\r\n剩余存活人数：{_queue.Count}");
            }

            if (_queue.All(c => IsSameFactionAs(c, killer)))
            {
                // 没有其他的角色了，游戏结束
                EndGameInfo(killer);
            }

            if (MaxScoreToWin > 0 && killer != null && _stats[killer].Kills >= MaxScoreToWin)
            {
                EndGameInfo(killer);
                return;
            }
        }

        /// <summary>
        /// 角色行动后，进行死亡竞赛幸存者检定
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected override bool AfterCharacterAction(Character character, CharacterActionType type)
        {
            bool result = base.AfterCharacterAction(character, type);
            if (result)
            {
                if (MaxRespawnTimes != 0 && MaxScoreToWin > 0 && _stats[character].Kills >= MaxScoreToWin)
                {
                    return false;
                }
            }
            return result;
        }

        /// <summary>
        /// 游戏结束信息
        /// </summary>
        public void EndGameInfo(Character? winner)
        {
            winner ??= _queue.FirstOrDefault();
            if (winner is null)
            {
                WriteLine("游戏结束。");
                return;
            }
            if (winner.Master != null)
            {
                winner = winner.Master;
            }
            WriteLine("[ " + winner + " ] 是胜利者。");
            foreach (Character character in _stats.OrderBy(kv => kv.Value.Kills)
                .ThenByDescending(kv => kv.Value.Deaths)
                .ThenBy(kv => kv.Value.Assists).Select(kv => kv.Key))
            {
                if (character != winner && !_eliminated.Contains(character))
                {
                    _eliminated.Add(character);
                }
            }
            _eliminated.Add(winner);
            _queue.Clear();
            _isGameEnd = true;

            if (!OnGameEndEvent(new HookContext(this, winner)))
            {
                return;
            }

            int top = 1;
            WriteLine("");
            WriteLine("=== 排名 ===");
            LastRound.GameResult.Clear();
            for (int i = _eliminated.Count - 1; i >= 0; i--)
            {
                Character ec = _eliminated[i];
                CharacterStatistics statistics = CharacterStatistics[ec];
                _earnedMoney.TryGetValue(ec, out int earned);
                _maxContinuousKilling.TryGetValue(ec, out int kills);
                // 结构化排名条目（引用 + 统计，按名次顺序；消费端可定制渲染）
                LastRound.GameResult.Add(new RankingEntry
                {
                    Rank = top,
                    IsWinner = ec == winner,
                    IsTeam = false,
                    Character = ec,
                    Kills = statistics.Kills,
                    Deaths = statistics.Deaths,
                    Assists = statistics.Assists,
                    FirstKills = statistics.FirstKills,
                    TotalEarnedMoney = earned,
                    MaxContinuousKilling = kills
                });
                string topCharacter = ec.ToString() +
                    (statistics.FirstKills > 0 ? " [ 第一滴血 ]" : "") +
                    (kills > 1 ? $" [ {CharacterSet.GetContinuousKilling(kills)} ]" : "") +
                    (earned > 0 ? $" [ 已赚取 {earned} {GameplayEquilibriumConstant.InGameCurrency} ]" : "");
                if (top == 1)
                {
                    WriteLine("冠军：" + topCharacter);
                    _stats[ec].Wins += 1;
                    _stats[ec].Top3s += 1;
                }
                else if (top == 2)
                {
                    WriteLine("亚军：" + topCharacter);
                    _stats[ec].Loses += 1;
                    _stats[ec].Top3s += 1;
                }
                else if (top == 3)
                {
                    WriteLine("季军：" + topCharacter);
                    _stats[ec].Loses += 1;
                    _stats[ec].Top3s += 1;
                }
                else
                {
                    WriteLine($"第 {top} 名：" + topCharacter);
                    _stats[ec].Loses += 1;
                }
                _stats[ec].Plays += 1;
                _stats[ec].TotalEarnedMoney += earned;
                _stats[ec].LastRank = top;
                top++;
            }
            WriteLine("");
            // 游戏结束，通知外发通道停止签名验证重试等
            RoundRecordSink?.End();
        }

        /// <summary>
        /// 创建一个混战游戏队列
        /// </summary>
        /// <param name="writer"></param>
        public MixGamingQueue(Action<string>? writer = null) : base(writer)
        {

        }

        /// <summary>
        /// 创建一个混战游戏队列并初始化角色
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="writer"></param>
        public MixGamingQueue(List<Character> characters, Action<string>? writer = null) : base(characters, writer)
        {

        }
    }
}
