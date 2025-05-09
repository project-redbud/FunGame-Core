using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
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
        /// <returns></returns>
        protected override async Task AfterDeathCalculation(Character death, Character killer)
        {
            if (MaxRespawnTimes != 0 && MaxScoreToWin > 0)
            {
                WriteLine($"\r\n=== 当前死亡竞赛比分 ===\r\n{string.Join("\r\n", _stats.OrderByDescending(kv => kv.Value.Kills)
                    .Select(kv => $"[ {kv.Key} ] {kv.Value.Kills} 分"))}\r\n剩余存活人数：{_queue.Count}");
            }

            if (!_queue.Where(c => c != killer).Any())
            {
                // 没有其他的角色了，游戏结束
                await EndGameInfo(killer);
            }

            if (MaxScoreToWin > 0 && _stats[killer].Kills >= MaxScoreToWin)
            {
                await EndGameInfo(killer);
                return;
            }
        }

        /// <summary>
        /// 游戏结束信息
        /// </summary>
        public async Task EndGameInfo(Character winner)
        {
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

            if (!await OnGameEndAsync(winner))
            {
                return;
            }

            int top = 1;
            WriteLine("");
            WriteLine("=== 排名 ===");
            for (int i = _eliminated.Count - 1; i >= 0; i--)
            {
                Character ec = _eliminated[i];
                CharacterStatistics statistics = CharacterStatistics[ec];
                string topCharacter = ec.ToString() +
                    (statistics.FirstKills > 0 ? " [ 第一滴血 ]" : "") +
                    (_maxContinuousKilling.TryGetValue(ec, out int kills) && kills > 1 ? $" [ {CharacterSet.GetContinuousKilling(kills)} ]" : "") +
                    (_earnedMoney.TryGetValue(ec, out int earned) ? $" [ 已赚取 {earned} {GameplayEquilibriumConstant.InGameCurrency} ]" : "");
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
        }

        /// <summary>
        /// 创建一个混战游戏队列
        /// </summary>
        /// <param name="writer"></param>
        public MixGamingQueue(Action<string>? writer = null) : base(writer)
        {

        }

        /// <summary>
        /// 创建一个混战游戏队列并初始化行动顺序表
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="writer"></param>
        public MixGamingQueue(List<Character> characters, Action<string>? writer = null) : base(characters, writer)
        {

        }
    }
}
