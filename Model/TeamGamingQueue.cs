using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    /// <summary>
    /// 团队游戏队列，团队模式 <see cref="RoomType.Team"/>
    /// </summary>
    public class TeamGamingQueue : GamingQueue
    {
        /// <summary>
        /// 当前团灭的团队顺序(第一个是最早死的)
        /// </summary>
        public List<Team> EliminatedTeams => _eliminatedTeams;

        /// <summary>
        /// 团队及其成员
        /// </summary>
        public Dictionary<string, Team> Teams => _teams;

        /// <summary>
        /// 当前团灭的团队顺序(第一个是最早死的)
        /// </summary>
        protected readonly List<Team> _eliminatedTeams = [];

        /// <summary>
        /// 团队及其成员
        /// </summary>
        protected readonly Dictionary<string, Team> _teams = [];

        /// <summary>
        /// 添加一个团队
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="characters"></param>
        public void AddTeam(string teamName, IEnumerable<Character> characters)
        {
            if (teamName != "" && characters.Any(c => c.HP > 0))
            {
                _teams.Add(teamName, new(teamName, characters));
            }
        }

        /// <summary>
        /// 获取角色的团队
        /// </summary>
        /// <param name="character"></param>
        public Team? GetTeam(Character character)
        {
            foreach (Team team in _teams.Values)
            {
                if (team.IsOnThisTeam(character))
                {
                    return team;
                }
            }
            return null;
        }

        /// <summary>
        /// 从已淘汰的团队中获取角色的团队
        /// </summary>
        /// <param name="character"></param>
        public Team? GetTeamFromEliminated(Character character)
        {
            foreach (Team team in _eliminatedTeams)
            {
                if (team.IsOnThisTeam(character))
                {
                    return team;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取某角色的团队成员
        /// </summary>
        /// <param name="character"></param>
        public override List<Character> GetTeammates(Character character)
        {
            foreach (string team in _teams.Keys)
            {
                if (_teams[team].IsOnThisTeam(character))
                {
                    List<Character> list = _teams[team].GetTeammates(character);
                    list.AddRange(_queue.Where(c => c.Master != null && _teams[team].IsOnThisTeam(c.Master)));
                    return list;
                }
            }
            return [];
        }

        /// <summary>
        /// 当角色完成决策后
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        protected override void AfterCharacterDecision(Character character, DecisionPoints dp)
        {
            // 如果目标都是队友，会考虑非伤害型助攻
            Team? team = GetTeam(character);
            if (team != null)
            {
                SetNotDamageAssistTime(character, LastRound.Targets.Values.SelectMany(c => c).Where(team.IsOnThisTeam));
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
                Team? team = GetTeam(character);
                if ((!_teams.Keys.Where(str => str != team?.Name).Any()) || (MaxScoreToWin > 0 && (team?.Score ?? 0) >= MaxScoreToWin))
                {
                    return false;
                }
            }
            return result;
        }

        /// <summary>
        /// 死亡结算时
        /// </summary>
        /// <param name="death"></param>
        /// <param name="killer"></param>
        /// <returns></returns>
        protected override void OnDeathCalculation(Character death, Character killer)
        {
            if (killer == death)
            {
                return;
            }
            Team? team = GetTeam(killer);
            if (team != null)
            {
                Team? team2 = GetTeam(death);
                if (team == team2)
                {
                    WriteLine($"[ {team} ] 受到了击杀队友惩罚！减少死亡竞赛得分！！");
                    team.Score--;
                }
                else team.Score++;
            }
        }

        /// <summary>
        /// 死亡结算后
        /// </summary>
        /// <param name="death"></param>
        /// <param name="killer"></param>
        /// <param name="assists"></param>
        /// <returns></returns>
        protected override void AfterDeathCalculation(Character death, Character killer, Character[] assists)
        {
            Team? killTeam = GetTeam(killer);
            Team? deathTeam = GetTeam(death);

            if (MaxRespawnTimes != 0)
            {
                string[] teamActive = [.. Teams.OrderByDescending(kv => kv.Value.Score).Select(kv =>
                {
                    int activeCount = kv.Value.GetActiveCharacters().Count;
                    return kv.Key + "：" + kv.Value.Score + "（剩余存活人数：" + activeCount + "）";
                })];
                WriteLine($"\r\n=== 当前死亡竞赛比分 ===\r\n{string.Join("\r\n", teamActive)}");
            }

            if (deathTeam != null)
            {
                List<Character> remain = deathTeam.GetActiveCharacters();
                int remainCount = remain.Count;
                if (remainCount == 0)
                {
                    // 团灭了
                    _eliminatedTeams.Add(deathTeam);
                    _teams.Remove(deathTeam.Name);
                }
                else if (MaxRespawnTimes == 0)
                {
                    WriteLine($"[ {deathTeam} ] 剩余成员：[ {string.Join(" ] / [ ", remain)} ]（{remainCount} 人）");
                }
            }

            if (killTeam != null)
            {
                List<Character> actives = killTeam.GetActiveCharacters();
                int remainCount = actives.Count;
                if (remainCount > 0 && MaxRespawnTimes == 0)
                {
                    WriteLine($"[ {killTeam} ] 剩余成员：[ {string.Join(" ] / [ ", actives)} ]（{remainCount} 人）");
                }
                if (!_teams.Keys.Where(str => str != killTeam.Name).Any())
                {
                    // 没有其他的团队了，游戏结束
                    EndGameInfo(killTeam);
                    return;
                }
                if (MaxScoreToWin > 0 && killTeam.Score >= MaxScoreToWin)
                {
                    List<Team> combinedTeams = [.. _eliminatedTeams, .. _teams.Values];
                    combinedTeams.Remove(killTeam);
                    _eliminatedTeams.Clear();
                    _eliminatedTeams.AddRange(combinedTeams.OrderByDescending(t => t.Score));
                    EndGameInfo(killTeam);
                    return;
                }
            }
        }

        /// <summary>
        /// 游戏结束信息 [ 团队版 ] 
        /// </summary>
        public void EndGameInfo(Team winner)
        {
            winner.IsWinner = true;
            WriteLine("[ " + winner + " ] 是胜利者。");

            if (!OnGameEndTeamEvent(winner))
            {
                return;
            }

            int top = 1;
            WriteLine("");
            WriteLine("=== 排名 ===");
            WriteLine("");

            _eliminatedTeams.Add(winner);
            _teams.Remove(winner.Name);

            for (int i = _eliminatedTeams.Count - 1; i >= 0; i--)
            {
                Team team = _eliminatedTeams[i];
                string topTeam = "";
                if (top == 1)
                {
                    topTeam = "冠军";
                }
                if (top == 2)
                {
                    topTeam = "亚军";
                }
                if (top == 3)
                {
                    topTeam = "季军";
                }
                if (top > 3)
                {
                    topTeam = $"第 {top} 名";
                }
                topTeam = $"☆--- {topTeam}团队：" + team.Name + " ---☆" + $"（得分：{team.Score}）\r\n";
                foreach (Character ec in team.Members)
                {
                    CharacterStatistics statistics = CharacterStatistics[ec];

                    string respawning = "";
                    if (ec.HP <= 0)
                    {
                        respawning = "[ " + (_respawnCountdown.TryGetValue(ec, out double time) && time > 0 ? $"{time:0.##} {GameplayEquilibriumConstant.InGameTime}后复活" : "阵亡") + " ] ";
                    }

                    string topCharacter = respawning + ec.ToString() +
                        (statistics.FirstKills > 0 ? " [ 第一滴血 ]" : "") +
                        (_maxContinuousKilling.TryGetValue(ec, out int kills) && kills > 1 ? $" [ {CharacterSet.GetContinuousKilling(kills)} ]" : "") +
                        (_earnedMoney.TryGetValue(ec, out int earned) ? $" [ 已赚取 {earned} {GameplayEquilibriumConstant.InGameCurrency} ]" : "") +
                        $"（{statistics.Kills} / {statistics.Assists}{(MaxRespawnTimes != 0 ? " / " + statistics.Deaths : "")}）";
                    topTeam += topCharacter + "\r\n";
                    if (top == 1)
                    {
                        _stats[ec].Wins += 1;
                        _stats[ec].Top3s += 1;
                    }
                    else if (top == 2)
                    {
                        _stats[ec].Loses += 1;
                        _stats[ec].Top3s += 1;
                    }
                    else if (top == 3)
                    {
                        _stats[ec].Loses += 1;
                        _stats[ec].Top3s += 1;
                    }
                    else
                    {
                        _stats[ec].Loses += 1;
                    }
                    _stats[ec].Plays += 1;
                    _stats[ec].TotalEarnedMoney += earned;
                    _stats[ec].LastRank = top;
                }
                WriteLine(topTeam);
                top++;
            }
            WriteLine("");
            _isGameEnd = true;
        }

        /// <summary>
        /// 创建一个团队游戏队列
        /// </summary>
        /// <param name="writer"></param>
        public TeamGamingQueue(Action<string>? writer = null) : base(writer)
        {

        }

        /// <summary>
        /// 创建一个团队游戏队列并初始化角色
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="writer"></param>
        public TeamGamingQueue(List<Character> characters, Action<string>? writer = null) : base(characters, writer)
        {

        }

        public delegate bool GameEndTeamEventHandler(TeamGamingQueue queue, Team winner);
        /// <summary>
        /// 游戏结束事件（团队版）
        /// </summary>
        public event GameEndTeamEventHandler? GameEndTeamEvent;
        /// <summary>
        /// 游戏结束事件（团队版）
        /// </summary>
        /// <param name="winner"></param>
        /// <returns></returns>
        protected bool OnGameEndTeamEvent(Team winner)
        {
            return GameEndTeamEvent?.Invoke(this, winner) ?? true;
        }
    }
}
