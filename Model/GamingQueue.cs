using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model.PrefabricatedEntity;

namespace Milimoe.FunGame.Core.Model
{
    /// <summary>
    /// 提供一个基础的回合制游戏队列基类实现，可继承扩展
    /// <para/>该默认实现为混战模式 <see cref="RoomType.Mix"/>
    /// </summary>
    public class GamingQueue : IGamingQueue
    {
        #region 公开属性

        /// <summary>
        /// 使用的游戏平衡常数
        /// </summary>
        public EquilibriumConstant GameplayEquilibriumConstant { get; set; } = General.GameplayEquilibriumConstant;

        /// <summary>
        /// 用于文本输出
        /// </summary>
        public Action<string> WriteLine { get; }

        /// <summary>
        /// 调试模式
        /// </summary>
        public bool IsDebug { get; set; } = false;

        /// <summary>
        /// 参与本次游戏的所有角色列表
        /// </summary>
        public List<Character> AllCharacters => _allCharacters;

        /// <summary>
        /// 原始的角色字典
        /// </summary>
        public Dictionary<Guid, Character> Original => _original;

        /// <summary>
        /// 当前的行动顺序
        /// </summary>
        public List<Character> Queue => _queue;

        /// <summary>
        /// 硬直时间表
        /// </summary>
        public Dictionary<Character, double> HardnessTime => _hardnessTimes;

        /// <summary>
        /// 当前已死亡的角色顺序(第一个是最早死的)
        /// </summary>
        public List<Character> Eliminated => _eliminated;

        /// <summary>
        /// 角色是否在 AI 控制下
        /// </summary>
        public List<Character> CharactersInAI => [.. _charactersInAIBySystem.Union(_charactersInAIByUser).Distinct()];

        /// <summary>
        /// 角色数据
        /// </summary>
        public Dictionary<Character, CharacterStatistics> CharacterStatistics => _stats;

        /// <summary>
        /// 助攻记录
        /// </summary>
        public Dictionary<Character, AssistDetail> AssistDetails => _assistDetail;

        /// <summary>
        /// 游戏运行的时间
        /// </summary>
        public double TotalTime { get; set; } = 0;

        /// <summary>
        /// 游戏运行的回合
        /// 对于某角色而言，在其行动的回合叫 Turn，而所有角色行动的回合，都称之为 Round。
        /// </summary>
        public int TotalRound { get; set; } = 0;

        /// <summary>
        /// 第一滴血获得者
        /// </summary>
        public Character? FirstKiller { get; set; } = null;

        /// <summary>
        /// 最大复活次数 [ 不为 0 时开启死斗模式 ]
        /// 0：不复活 / -1：无限复活
        /// </summary>
        public int MaxRespawnTimes { get; set; } = 0;

        /// <summary>
        /// 复活次数统计
        /// </summary>
        public Dictionary<Character, int> RespawnTimes => _respawnTimes;

        /// <summary>
        /// 复活倒计时
        /// </summary>
        public Dictionary<Character, double> RespawnCountdown => _respawnCountdown;

        /// <summary>
        /// 最大获胜积分
        /// 设置一个大于0的数以启用
        /// </summary>
        public int MaxScoreToWin { get; set; } = 0;

        /// <summary>
        /// 上回合记录
        /// </summary>
        public RoundRecord LastRound { get; set; } = new(0);

        /// <summary>
        /// 所有回合的记录
        /// </summary>
        public List<RoundRecord> Rounds { get; } = [];

        /// <summary>
        /// 回合奖励
        /// </summary>
        public Dictionary<int, List<Skill>> RoundRewards => _roundRewards;

        /// <summary>
        /// 是否使用插队保护机制
        /// </summary>
        public bool UseQueueProtected { get; set; }

        /// <summary>
        /// 插队保护机制检查的最多被插队次数：-1 为默认，即队列长度，最少为 5；0 为不保护
        /// </summary>
        public int MaxCutQueueTimes
        {
            get
            {
                if (!UseQueueProtected || _maxCutQueueTimes == 0) return 0;
                else if (_maxCutQueueTimes == -1) return Math.Max(5, _queue.Count);
                else if (_maxCutQueueTimes < 5) return 5;
                else return _maxCutQueueTimes;
            }
            set
            {
                _maxCutQueueTimes = value;
            }
        }

        /// <summary>
        /// 自定义数据
        /// </summary>
        public Dictionary<string, object> CustomData { get; } = [];

        /// <summary>
        /// 金币奖励记录
        /// </summary>
        public Dictionary<Character, int> EarnedMoney => _earnedMoney;

        /// <summary>
        /// 使用的地图
        /// </summary>
        public GameMap? Map => _map;

        /// <summary>
        /// 角色的决策点
        /// </summary>
        public Dictionary<Character, DecisionPoints> CharacterDecisionPoints => _decisionPoints;

        /// <summary>
        /// 游戏结束标识
        /// </summary>
        public bool GameOver => _isGameEnd;

        #endregion

        #region 保护变量

        /// <summary>
        /// 参与本次游戏的所有角色列表
        /// </summary>
        protected readonly List<Character> _allCharacters = [];

        /// <summary>
        /// 原始的角色字典
        /// </summary>
        protected readonly Dictionary<Guid, Character> _original = [];

        /// <summary>
        /// 当前的行动顺序
        /// </summary>
        protected readonly List<Character> _queue = [];

        /// <summary>
        /// 当前已死亡的角色顺序(第一个是最早死的)
        /// </summary>
        protected readonly List<Character> _eliminated = [];

        /// <summary>
        /// 角色是否在 AI 控制下 [ 系统控制 ]
        /// </summary>
        protected readonly HashSet<Character> _charactersInAIBySystem = [];

        /// <summary>
        /// 角色是否在 AI 控制下 [ 玩家手动设置 ]
        /// </summary>
        protected readonly HashSet<Character> _charactersInAIByUser = [];

        /// <summary>
        /// 硬直时间表
        /// </summary>
        protected readonly Dictionary<Character, double> _hardnessTimes = [];

        /// <summary>
        /// 角色正在吟唱的技能（通常是魔法）
        /// </summary>
        protected readonly Dictionary<Character, SkillTarget> _castingSkills = [];

        /// <summary>
        /// 角色预释放的爆发技
        /// </summary>
        protected readonly Dictionary<Character, Skill> _castingSuperSkills = [];

        /// <summary>
        /// 角色即将使用的物品
        /// </summary>
        protected readonly Dictionary<Character, Skill> _willUseItems = [];

        /// <summary>
        /// 角色目前赚取的金钱
        /// </summary>
        protected readonly Dictionary<Character, int> _earnedMoney = [];

        /// <summary>
        /// 角色最高连杀数
        /// </summary>
        protected readonly Dictionary<Character, int> _maxContinuousKilling = [];

        /// <summary>
        /// 角色目前的连杀数
        /// </summary>
        protected readonly Dictionary<Character, int> _continuousKilling = [];

        /// <summary>
        /// 角色被插队次数
        /// </summary>
        protected readonly Dictionary<Character, int> _cutCount = [];

        /// <summary>
        /// 助攻伤害
        /// </summary>
        protected readonly Dictionary<Character, AssistDetail> _assistDetail = [];

        /// <summary>
        /// 角色数据
        /// </summary>
        protected readonly Dictionary<Character, CharacterStatistics> _stats = [];

        /// <summary>
        /// 复活次数统计
        /// </summary>
        protected readonly Dictionary<Character, int> _respawnTimes = [];

        /// <summary>
        /// 复活倒计时
        /// </summary>
        protected readonly Dictionary<Character, double> _respawnCountdown = [];

        /// <summary>
        /// 当前回合死亡角色和参与击杀的人
        /// </summary>
        protected readonly List<DeathRelation> _roundDeaths = [];

        /// <summary>
        /// 回合奖励
        /// </summary>
        protected readonly Dictionary<int, List<Skill>> _roundRewards = [];

        /// <summary>
        /// 回合奖励的特效工厂
        /// </summary>
        protected Func<long, Dictionary<string, object>> _factoryRoundRewardEffects = id => [];

        /// <summary>
        /// 角色的决策点
        /// </summary>
        protected readonly Dictionary<Character, DecisionPoints> _decisionPoints = [];

        /// <summary>
        /// 最多被插队次数，-1 为默认，即队列长度，最少为 5
        /// </summary>
        protected int _maxCutQueueTimes = 5;

        /// <summary>
        /// 游戏是否结束
        /// </summary>
        protected bool _isGameEnd = false;

        /// <summary>
        /// 是否在回合内
        /// </summary>
        protected bool _isInRound = false;

        /// <summary>
        /// 使用的地图
        /// </summary>
        protected GameMap? _map = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 新建一个基础回合制游戏队列
        /// </summary>
        /// <param name="writer">用于文本输出</param>
        /// <param name="map">游戏地图</param>
        public GamingQueue(Action<string>? writer = null, GameMap? map = null)
        {
            if (writer != null)
            {
                WriteLine = writer;
            }
            WriteLine ??= new Action<string>(Console.WriteLine);
            if (map != null)
            {
                LoadGameMap(map);
            }
        }

        /// <summary>
        /// 新建一个基础回合制游戏队列并初始化角色
        /// </summary>
        /// <param name="characters">参与本次游戏的角色列表</param>
        /// <param name="writer">用于文本输出</param>
        /// <param name="map">游戏地图</param>
        public GamingQueue(List<Character> characters, Action<string>? writer = null, GameMap? map = null)
        {
            if (writer != null)
            {
                WriteLine = writer;
            }
            WriteLine ??= new Action<string>(Console.WriteLine);
            if (map != null)
            {
                LoadGameMap(map);
            }
            InitCharacters(characters);
        }

        #endregion

        #region 战棋地图

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="map"></param>
        public void LoadGameMap(GameMap map)
        {
            _map = map.InitGamingQueue(this);
        }

        /// <summary>
        /// 将角色从地图上移除
        /// </summary>
        public void RemoveCharacterFromMap(params IEnumerable<Character> characters)
        {
            if (Map is null)
            {
                return;
            }

            foreach (Character character in characters)
            {
                RemoveCharacterFromQueue(character);
                Map.Characters.Remove(character);
                Grid[] grids = [.. Map.Grids.Values.Where(g => g.Characters.Contains(character))];
                foreach (Grid grid in grids)
                {
                    grid.Characters.Remove(character);
                }
            }
        }

        /// <summary>
        /// 将角色拥有的单位从地图上移除
        /// </summary>
        public void RemoveCharactersUnitFromMap(params IEnumerable<Character> characters)
        {
            if (Map is null)
            {
                return;
            }

            foreach (Character character in characters)
            {
                Character[] willRemove = [.. _queue.Where(c => c.Master == character)];
                RemoveCharacterFromMap(willRemove);
            }
        }

        #endregion

        #region 队列框架

        /// <summary>
        /// 初始化角色表
        /// </summary>
        /// <param name="characters"></param>
        public void InitCharacters(List<Character> characters)
        {
            // 保存原始的角色信息。用于复活时还原状态
            foreach (Character character in characters)
            {
                if (character.IsUnit) continue;
                // 添加角色引用到所有角色列表
                _allCharacters.Add(character);
                // 复制原始角色对象
                Character original = character.Copy();
                original.Guid = Guid.NewGuid();
                character.Guid = original.Guid;
                _original.Add(original.Guid, original);
            }

            // 完全初始化
            foreach (Character character in _allCharacters)
            {
                // 添加统计数据
                _stats[character] = new();
                // 添加助攻对象
                _assistDetail[character] = new AssistDetail(character, _allCharacters.Where(c => c != character));
            }

            // 获取 HP 小于等于 0 的角色
            List<Character> deadCharacters = [.. characters.Where(c => c.HP <= 0 && !c.IsUnit)];
            foreach (Character death in deadCharacters)
            {
                _eliminated.Add(death);
                if (MaxRespawnTimes != 0 || (MaxRespawnTimes != -1 && _respawnTimes.TryGetValue(death, out int times) && times < MaxRespawnTimes))
                {
                    // 进入复活倒计时
                    double respawnTime = 5;
                    _respawnCountdown.TryAdd(death, respawnTime);
                    WriteLine($"[ {death} ] 进入复活倒计时：{respawnTime:0.##} {GameplayEquilibriumConstant.InGameTime}！");
                }
            }
        }

        /// <summary>
        /// 初始化行动顺序表
        /// </summary>
        public void InitActionQueue()
        {
            // 初始排序：按速度排序
            List<IGrouping<double, Character>> groupedBySpeed = [.. _allCharacters
                .Where(c => c.HP > 0)
                .GroupBy(c => c.SPD)
                .OrderByDescending(g => g.Key)];

            Random random = new();

            foreach (IGrouping<double, Character> group in groupedBySpeed)
            {
                if (group.Count() == 1)
                {
                    // 如果只有一个角色，直接加入队列
                    Character character = group.First();
                    AddCharacter(character, Calculation.Round2Digits(_queue.Count * 0.1), false);
                    // 初始化技能
                    foreach (Skill skill in character.Skills)
                    {
                        skill.OnSkillGained(this);
                    }
                }
                else
                {
                    // 如果有多个角色，进行先行决定
                    List<Character> sortedList = [.. group];

                    while (sortedList.Count > 0)
                    {
                        Character? selectedCharacter = null;
                        bool decided = false;
                        if (sortedList.Count == 1)
                        {
                            selectedCharacter = sortedList[0];
                            decided = true;
                        }

                        while (!decided)
                        {
                            // 每个角色进行两次随机数抽取
                            var randomNumbers = sortedList.Select(c => new
                            {
                                Character = c,
                                FirstRoll = random.Next(1, 21),
                                SecondRoll = random.Next(1, 21)
                            }).ToList();

                            randomNumbers.ForEach(a => WriteLine(a.Character.Name + ": " + a.FirstRoll + " / " + a.SecondRoll));

                            // 找到两次都大于其他角色的角色
                            int maxFirstRoll = randomNumbers.Max(r => r.FirstRoll);
                            int maxSecondRoll = randomNumbers.Max(r => r.SecondRoll);

                            var candidates = randomNumbers
                                .Where(r => r.FirstRoll == maxFirstRoll && r.SecondRoll == maxSecondRoll)
                                .ToList();

                            if (candidates.Count == 1)
                            {
                                selectedCharacter = candidates.First().Character;
                                decided = true;
                            }
                        }

                        // 将决定好的角色加入队列
                        if (selectedCharacter != null)
                        {
                            AddCharacter(selectedCharacter, Calculation.Round2Digits(_queue.Count * 0.1), false);
                            // 初始化技能
                            foreach (Skill skill in selectedCharacter.Skills)
                            {
                                skill.OnSkillGained(this);
                            }
                            WriteLine("decided: " + selectedCharacter.Name + "\r\n");
                            sortedList.Remove(selectedCharacter);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将角色加入行动顺序表
        /// </summary>
        /// <param name="character"></param>
        /// <param name="hardnessTime"></param>
        /// <param name="isCheckProtected"></param>
        public void AddCharacter(Character character, double hardnessTime, bool isCheckProtected = true)
        {
            // 确保角色不在队列中
            _queue.RemoveAll(c => c == character);

            // 增加硬直时间直到唯一
            double ResolveConflict(double time, Character c)
            {
                while (_hardnessTimes.Any(kv => kv.Key != c && kv.Value == time))
                {
                    time = Calculation.Round2Digits(time + 0.01);
                }
                return time;
            }

            // 初始插入索引：第一个硬直时间大于当前值的角色位置
            int insertIndex = _queue.FindIndex(c => _hardnessTimes[c] > hardnessTime);

            // 调整硬直时间以避免冲突
            hardnessTime = ResolveConflict(hardnessTime, character);

            // 重新计算插入索引
            insertIndex = _queue.FindIndex(c => _hardnessTimes[c] > hardnessTime);

            if (isCheckProtected)
            {
                // 查找保护条件 被插队超过此次数便能获得插队补偿 即行动保护
                int countProtected = MaxCutQueueTimes;
                if (countProtected > 0)
                {
                    // 查找队列中是否有满足插队补偿条件的角色（最后一个）
                    var list = _queue
                        .Select((c, index) => new { Character = c, Index = index })
                        .Where(x => _cutCount.ContainsKey(x.Character) && _cutCount[x.Character] >= countProtected);

                    // 如果没有找到满足条件的角色，返回 -1
                    int protectIndex = list.Select(x => x.Index).LastOrDefault(-1);

                    if (protectIndex != -1)
                    {
                        // 获取最后一个符合条件的角色
                        Character lastProtectedCharacter = list.Last().Character;
                        double lastProtectedHardnessTime = _hardnessTimes[lastProtectedCharacter];

                        // 查找与最后一个受保护角色相同硬直时间的其他角色
                        var sameHardnessList = _queue
                            .Select((c, index) => new { Character = c, Index = index })
                            .Where(x => _hardnessTimes[x.Character] == lastProtectedHardnessTime && x.Index > protectIndex);

                        // 如果找到了相同硬直时间的角色，更新 protectIndex 为它们中最后一个的索引
                        if (sameHardnessList.Any())
                        {
                            protectIndex = sameHardnessList.Select(x => x.Index).Last();
                        }

                        // 判断是否需要插入到受保护角色的后面
                        // 只有当插入位置在受保护角色之前或相同时才触发保护
                        if (insertIndex != -1 && insertIndex <= protectIndex)
                        {
                            // 插入到受保护角色的后面一位
                            insertIndex = protectIndex + 1;

                            // 设置硬直时间为保护角色的硬直时间 + 0.01
                            hardnessTime = lastProtectedHardnessTime + 0.01;
                            hardnessTime = ResolveConflict(hardnessTime, character);

                            // 重新计算插入索引
                            insertIndex = _queue.FindIndex(c => _hardnessTimes[c] > hardnessTime);

                            // 列出受保护角色的名单
                            WriteLine($"由于 [ {string.Join(" ]，[ ", list.Select(x => x.Character))} ] 受到行动保护，因此角色 [ {character} ] 将插入至顺序表第 {insertIndex + 1} 位。");
                        }
                    }
                }
            }

            // 如果插入索引无效（为-1 或 大于等于队列长度），则添加到队列尾部
            if (insertIndex == -1 || insertIndex >= _queue.Count)
            {
                _queue.Add(character);
            }
            else
            {
                _queue.Insert(insertIndex, character);
            }
            _hardnessTimes[character] = hardnessTime;

            // 为所有被插队的角色增加 _cutCount
            if (isCheckProtected && insertIndex != -1 && insertIndex < _queue.Count)
            {
                for (int i = insertIndex + 1; i < _queue.Count; i++)
                {
                    Character queuedCharacter = _queue[i];
                    if (!_cutCount.TryAdd(queuedCharacter, 1))
                    {
                        _cutCount[queuedCharacter] += 1;
                    }
                }
            }

            // 重新排序
            _queue.Sort((a, b) => _hardnessTimes[a].CompareTo(_hardnessTimes[b]));
        }

        /// <summary>
        /// 清空行动队列
        /// </summary>
        public void ClearQueue()
        {
            FirstKiller = null;
            CustomData.Clear();
            _allCharacters.Clear();
            _original.Clear();
            _queue.Clear();
            _hardnessTimes.Clear();
            _assistDetail.Clear();
            _stats.Clear();
            _cutCount.Clear();
            _castingSkills.Clear();
            _castingSuperSkills.Clear();
            _willUseItems.Clear();
            _maxContinuousKilling.Clear();
            _continuousKilling.Clear();
            _earnedMoney.Clear();
            _eliminated.Clear();
            _charactersInAIBySystem.Clear();
            _charactersInAIByUser.Clear();
        }

        /// <summary>
        /// 将角色彻底移出行动顺序表
        /// </summary>
        /// <param name="characters"></param>
        public void RemoveCharacterFromQueue(params IEnumerable<Character> characters)
        {
            foreach (Character character in characters)
            {
                _queue.Remove(character);
                _hardnessTimes.Remove(character);
            }
        }

        #endregion

        #region 时间流逝

        /// <summary>
        /// 从行动顺序表取出第一个角色
        /// </summary>
        /// <returns></returns>
        public Character? NextCharacter()
        {
            if (_queue.Count == 0) return null;

            // 硬直时间为 0 的角色
            Character? character = null;
            Character temp = _queue[0];
            if (_hardnessTimes[temp] == 0)
            {
                character = temp;
            }

            if (character != null)
            {
                // 进入下一回合
                TotalRound++;
                LastRound = new(TotalRound);
                Rounds.Add(LastRound);

                if (TotalRound == 1)
                {
                    // 触发游戏开始事件
                    OnGameStartEvent();
                    Effect[] effects = [.. _queue.SelectMany(c => c.Effects).Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                    foreach (Effect effect in effects)
                    {
                        effect.OnGameStart();
                    }
                }

                _queue.Remove(character);
                _cutCount.Remove(character);

                return character;
            }
            else
            {
                TimeLapse();
                return NextCharacter();
            }
        }

        /// <summary>
        /// 时间进行流逝，减少硬直时间，减少技能冷却时间，角色也会因此回复状态
        /// </summary>
        /// <returns>流逝的时间</returns>
        public double TimeLapse()
        {
            if (_queue.Count == 0) return 0;

            // 获取第一个角色的硬直时间
            double timeToReduce = _hardnessTimes[_queue[0]];
            // 如果复活时间更快，应该先流逝复活时间
            if (_respawnCountdown.Count != 0)
            {
                double timeToRespawn = _respawnCountdown.Values.Min();
                if (timeToRespawn < timeToReduce)
                {
                    timeToReduce = Calculation.Round2Digits(timeToRespawn);
                }
            }

            TotalTime = Calculation.Round2Digits(TotalTime + timeToReduce);
            WriteLine($"时间流逝：{timeToReduce}");

            if (IsDebug)
            {
                // 记录行动顺序表
                WriteLine(string.Join("\r\n", _queue.Select(c => c + ": " + _hardnessTimes[c])));
            }

            Character[] characters = [.. _queue];
            foreach (Character character in characters)
            {
                // 减少所有角色的硬直时间
                double past = _hardnessTimes[character];
                _hardnessTimes[character] = Calculation.Round2Digits(_hardnessTimes[character] - timeToReduce);

                if (_hardnessTimes[character] < 0)
                {
                    WriteLine($"异常的硬直时间警告，原时间：{past}，现时间：{_hardnessTimes[character]}，时间流逝：{timeToReduce}。");
                }

                // 统计
                Character statsCharacter = character;
                if (character.Master != null)
                {
                    statsCharacter = character.Master;
                }
                if (character.Master is null)
                {
                    _stats[statsCharacter].LiveRound += 1;
                    _stats[statsCharacter].LiveTime += timeToReduce;
                }
                _stats[statsCharacter].DamagePerRound = _stats[statsCharacter].LiveRound == 0 ? 0 : _stats[statsCharacter].TotalDamage / _stats[statsCharacter].LiveRound;
                _stats[statsCharacter].DamagePerTurn = _stats[statsCharacter].ActionTurn == 0 ? 0 : _stats[statsCharacter].TotalDamage / _stats[statsCharacter].ActionTurn;
                _stats[statsCharacter].DamagePerSecond = _stats[statsCharacter].LiveTime == 0 ? 0 : _stats[statsCharacter].TotalDamage / _stats[statsCharacter].LiveTime;

                // 回血回蓝
                double recoveryHP = character.HR * timeToReduce;
                double recoveryMP = character.MR * timeToReduce;
                double needHP = character.MaxHP - character.HP;
                double needMP = character.MaxMP - character.MP;
                double reallyReHP = needHP >= recoveryHP ? recoveryHP : needHP;
                double reallyReMP = needMP >= recoveryMP ? recoveryMP : needMP;
                bool allowRecovery = true;
                Effect[] effects = [.. character.Effects.OrderByDescending(e => e.Priority)];
                foreach (Effect effect in effects)
                {
                    if (!effect.BeforeApplyRecoveryAtTimeLapsing(character, ref reallyReHP, ref reallyReMP))
                    {
                        allowRecovery = false;
                    }
                }

                if (allowRecovery)
                {
                    if (reallyReHP > 0 && reallyReMP > 0)
                    {
                        character.HP += reallyReHP;
                        character.MP += reallyReMP;
                        if (IsDebug) WriteLine($"角色 {character} 回血：{recoveryHP:0.##} [{character.HP:0.##} / {character.MaxHP:0.##}] / 回蓝：{recoveryMP:0.##} [{character.MP:0.##} / {character.MaxMP:0.##}] / 当前能量：{character.EP:0.##}");
                    }
                    else
                    {
                        if (reallyReHP > 0)
                        {
                            character.HP += reallyReHP;
                            if (IsDebug) WriteLine($"角色 {character} 回血：{recoveryHP:0.##} [{character.HP:0.##} / {character.MaxHP:0.##}] / 当前能量：{character.EP:0.##}");
                        }
                        if (reallyReMP > 0)
                        {
                            character.MP += reallyReMP;
                            if (IsDebug) WriteLine($"角色 {character} 回蓝：{recoveryMP:0.##} [{character.MP:0.##} / {character.MaxMP:0.##}] / 当前能量：{character.EP:0.##}");
                        }
                    }
                }

                // 减少所有技能的冷却时间
                List<Skill> skills = [.. character.Skills.Union(character.Items.Where(i => i.Skills.Active != null).Select(i => i.Skills.Active!))];
                AddCharacterEquipSlotSkills(character, skills);
                foreach (Skill skill in skills)
                {
                    if (skill.CurrentCD > 0)
                    {
                        skill.CurrentCD -= timeToReduce;
                        if (skill.CurrentCD <= 0)
                        {
                            skill.CurrentCD = 0;
                            skill.Enable = true;
                        }
                    }
                }

                // 处理地图上的特效
                _map?.OnTimeElapsed(timeToReduce);

                // 移除到时间的特效
                effects = [.. character.Effects.OrderByDescending(e => e.Priority)];
                foreach (Effect effect in effects)
                {
                    if (!character.Shield.ShieldOfEffects.ContainsKey(effect))
                    {
                        character.Shield.RemoveShieldOfEffect(effect);
                    }

                    if (effect.Level == 0)
                    {
                        character.Effects.Remove(effect);
                        continue;
                    }

                    if (!effect.Durative)
                    {
                        // 防止特效在时间流逝后，持续时间已结束还能继续生效的情况
                        effect.OnTimeElapsed(character, timeToReduce);
                    }

                    // 进行持续时间豁免
                    if (effect.Exemptable && effect.ExemptDuration && (effect.RemainDuration > 0 || effect.RemainDurationTurn > 0))
                    {
                        CheckExemption(character, effect.Source, effect, false);
                    }

                    if (effect.IsBeingTemporaryDispelled)
                    {
                        effect.IsBeingTemporaryDispelled = false;
                        effect.OnEffectGained(character);
                    }

                    // 自身被动不会考虑
                    if (effect.EffectType == EffectType.None && effect.Skill.SkillType == SkillType.Passive)
                    {
                        continue;
                    }

                    // 统计控制时长
                    if (effect.Source != null && SkillSet.GetCharacterStateByEffectType(effect.EffectType) != CharacterState.Actionable)
                    {
                        Character source = effect.Source;
                        if (effect.Source.Master != null)
                        {
                            source = effect.Source.Master;
                        }
                        _stats[source].ControlTime += timeToReduce;
                        if (character.Master is null)
                        {
                            _assistDetail[source][character, TotalTime] += 1;
                        }
                    }

                    if (effect.Durative)
                    {
                        if (effect.RemainDuration < timeToReduce)
                        {
                            // 移除特效前也完成剩余时间内的效果
                            effect.OnTimeElapsed(character, effect.RemainDuration);
                            effect.RemainDuration = 0;
                            character.Effects.Remove(effect);
                            effect.OnEffectLost(character);
                            WriteLine($"[ {character} ] 失去了 [ {effect.Name} ] 效果。");
                        }
                        else
                        {
                            effect.RemainDuration -= timeToReduce;
                            effect.OnTimeElapsed(character, timeToReduce);
                        }
                    }
                }

                // 如果特效具备临时驱散或者持续性驱散的功能
                effects = [.. character.Effects.Where(e => e.Source != null && (e.EffectType == EffectType.WeakDispelling || e.EffectType == EffectType.StrongDispelling)).OrderByDescending(e => e.Priority)];
                foreach (Effect effect in effects)
                {
                    if (effect.Source is null) continue;
                    effect.Dispel(effect.Source, character, !IsTeammate(character, effect.Source) && character != effect.Source);
                }

                // 还原临时驱散后的吟唱状态
                if (character.CharacterState == CharacterState.Actionable && _castingSkills.ContainsKey(character))
                {
                    character.CharacterState = CharacterState.Casting;
                }

                _eliminated.Remove(character);
            }

            // 减少复活倒计时
            Character[] willRespawns = [.. _respawnCountdown.Keys];
            foreach (Character character in willRespawns)
            {
                _respawnCountdown[character] = Calculation.Round2Digits(_respawnCountdown[character] - timeToReduce);
                if (_respawnCountdown[character] <= 0)
                {
                    SetCharacterRespawn(character);
                }
            }

            ProcessCharacterDeath();

            WriteLine("\r\n");

            return timeToReduce;
        }

        /// <summary>
        /// 显示当前所有角色的状态和硬直时间
        /// </summary>
        public void DisplayQueue()
        {
            WriteLine("==== 角色状态 ====");
            foreach (Character c in _queue)
            {
                WriteLine(c.GetInBattleInfo(_hardnessTimes[c]));
            }
        }

        #endregion

        #region 回合内

        /// <summary>
        /// 角色 <paramref name="character"/> 的回合进行中
        /// </summary>
        /// <param name="character"></param>
        /// <returns>是否结束游戏</returns>
        public bool ProcessTurn(Character character)
        {
            _isInRound = true;
            LastRound.Actor = character;
            _roundDeaths.Clear();
            Character statsCharacter = character;
            if (character.Master != null)
            {
                statsCharacter = character.Master;
            }

            if (!BeforeTurn(character))
            {
                _isInRound = false;
                return _isGameEnd;
            }

            // 决策点补充
            DecisionPoints dp = DecisionPointsRecovery(character);

            // 获取回合奖励
            List<Skill> rewards = GetRoundRewards(TotalRound, character);

            // 基础硬直时间
            double baseTime = 0;
            bool isCheckProtected = true;

            // 队友列表
            List<Character> allTeammates = GetTeammates(character);

            // 敌人列表
            List<Character> allEnemys = [.. _allCharacters.Union(_queue).Distinct().Where(c => c != character && !allTeammates.Contains(c) && !_eliminated.Contains(c) && c.Master != character && character.Master != c)];

            // 取得可选列表
            (List<Character> selectableTeammates, List<Character> selectableEnemys, List<Skill> skills, List<Item> items) = GetTurnStartNeedyList(character, allTeammates, allEnemys);

            // 回合开始事件，允许事件返回 false 接管回合操作
            // 如果事件全程接管回合操作，需要注意触发特效
            if (!OnTurnStartEvent(character, dp, selectableEnemys, selectableTeammates, skills, items))
            {
                _isInRound = false;
                return _isGameEnd;
            }

            List<Skill> skillsTurnStart = [.. character.Skills];
            AddCharacterEquipSlotSkills(character, skillsTurnStart);
            foreach (Skill skillTurnStart in skillsTurnStart)
            {
                skillTurnStart.OnTurnStart(character, selectableEnemys, selectableTeammates, skills, items);
            }

            List<Effect> effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
            foreach (Effect effect in effects)
            {
                effect.OnTurnStart(character, selectableEnemys, selectableTeammates, skills, items);
            }

            // 角色的起始地点，确保角色该回合移动的范围不超过 MOV
            Grid? startGrid = null;
            // 可移动的格子列表
            List<Grid> canMoveGrids = [];
            if (_map != null)
            {
                startGrid = _map.GetCharacterCurrentGrid(character);
                if (startGrid != null)
                {
                    canMoveGrids = _map.GetGridsByRange(startGrid, character.MOV, false);
                }
            }

            // 作出了什么行动
            CharacterActionType type = CharacterActionType.None;

            // 是否结束回合
            bool endTurn = false;
            bool isAI = IsCharacterInAIControlling(character);

            // 循环条件：未结束回合、决策点大于0（AI控制下为0时自动结束）或角色处于吟唱态
            while (!endTurn && (!isAI || dp.CurrentDecisionPoints > 0 || character.CharacterState == CharacterState.Casting || character.CharacterState == CharacterState.PreCastSuperSkill))
            {
                // 刷新可选列表
                (selectableTeammates, selectableEnemys, skills, items) = GetTurnStartNeedyList(character, allTeammates, allEnemys);

                // 并且要筛选最远可选取角色
                List<Grid> canAttackGridsByStartGrid = [];
                List<Grid> canCastGridsByStartGrid = [];
                if (_map != null && startGrid != null)
                {
                    canAttackGridsByStartGrid = _map.GetGridsByRange(startGrid, character.ATR, true);
                    Skill[] canCastSkills = [.. skills, .. items.Select(i => i.Skills.Active!)];
                    foreach (Skill skill in canCastSkills)
                    {
                        canCastGridsByStartGrid.AddRange(_map.GetGridsByRange(startGrid, skill.CastRange, true));
                    }
                }

                // 此变量用于在取消选择时，能够重新行动
                bool decided = false;
                // 最大取消次数
                int cancelTimes = 3;
                // 此变量指示角色是否移动
                bool moved = false;

                // AI 决策控制器，适用于启用战棋地图的情况
                AIController? ai = null;

                // 循环条件：
                // AI 控制下：未决策、取消次数大于0
                // 手动控制下：未决策
                isAI = IsCharacterInAIControlling(character);
                while (!decided && (!isAI || cancelTimes > 0))
                {
                    // 根据当前位置，更新可选取角色列表
                    Grid? realGrid = null;
                    List<Grid> canAttackGrids = [];
                    List<Grid> canCastGrids = [];
                    List<Grid> willMoveGridWithSkill = [];
                    List<Character> enemys = [];
                    List<Character> teammates = [];
                    if (_map != null)
                    {
                        if (isAI)
                        {
                            ai ??= new(this, _map);
                        }

                        realGrid = _map.GetCharacterCurrentGrid(character);

                        if (realGrid != null)
                        {
                            canAttackGrids = _map.GetGridsByRange(realGrid, character.ATR, true);
                            Skill[] canCastSkills = [.. skills, .. items.Select(i => i.Skills.Active!)];
                            foreach (Skill skill in canCastSkills)
                            {
                                canCastGrids.AddRange(_map.GetGridsByRange(realGrid, skill.CastRange, true));
                            }
                        }

                        enemys = [.. selectableEnemys.Where(canAttackGrids.Union(canCastGrids).SelectMany(g => g.Characters).Contains)];
                        teammates = [.. selectableTeammates.Where(canAttackGrids.Union(canCastGrids).SelectMany(g => g.Characters).Contains)];
                        willMoveGridWithSkill = [.. canMoveGrids.Where(g => canAttackGrids.Union(canCastGrids).Contains(g))];
                    }
                    else
                    {
                        enemys = selectableEnemys;
                        teammates = selectableTeammates;
                    }

                    // AI 决策结果（适用于启用战棋地图的情况）
                    AIDecision? aiDecision = null;

                    // 行动开始前，可以修改可选取的角色列表
                    Dictionary<Character, int> continuousKillingTemp = new(_continuousKilling);
                    Dictionary<Character, int> earnedMoneyTemp = new(_earnedMoney);
                    effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                    foreach (Effect effect in effects)
                    {
                        effect.AlterSelectListBeforeAction(character, enemys, teammates, skills, continuousKillingTemp, earnedMoneyTemp);
                    }

                    // 这里筛掉重复角色
                    enemys = [.. enemys.Distinct()];
                    teammates = [.. teammates.Distinct()];

                    baseTime = 0;
                    if (moved) moved = false;
                    else cancelTimes--;
                    type = CharacterActionType.None;

                    // 是否能使用物品和释放技能
                    bool canUseItem = items.Count > 0;
                    bool canCastSkill = skills.Count > 0;

                    // 使用物品和释放技能、使用普通攻击的概率
                    double pUseItem = 0.33;
                    double pCastSkill = 0.33;
                    double pNormalAttack = 0.34;

                    // 是否强制执行（跳过状态检查等）
                    bool forceAction = false;

                    // 不允许在吟唱和预释放状态下，修改角色的行动
                    if (character.CharacterState != CharacterState.Casting && character.CharacterState != CharacterState.PreCastSuperSkill)
                    {
                        CharacterActionType actionTypeTemp = CharacterActionType.None;
                        effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                        foreach (Effect effect in effects)
                        {
                            bool force = false;
                            CharacterActionType forceType = effect.AlterActionTypeBeforeAction(character, dp, character.CharacterState, ref canUseItem, ref canCastSkill, ref pUseItem, ref pCastSkill, ref pNormalAttack, ref force);
                            if (force && forceType != CharacterActionType.None)
                            {
                                forceAction = true;
                                actionTypeTemp = forceType;
                                break;
                            }
                        }
                        if (actionTypeTemp != CharacterActionType.None && actionTypeTemp != CharacterActionType.CastSkill && actionTypeTemp != CharacterActionType.CastSuperSkill)
                        {
                            type = actionTypeTemp;
                        }
                    }

                    if (type == CharacterActionType.None)
                    {
                        if (character.CharacterState != CharacterState.NotActionable && character.CharacterState != CharacterState.Casting && character.CharacterState != CharacterState.PreCastSuperSkill)
                        {
                            // 根据角色状态，设置一些参数
                            if (character.CharacterState == CharacterState.Actionable)
                            {
                                // 可以任意行动
                                if (canUseItem && canCastSkill)
                                {
                                    // 不做任何处理
                                }
                                else if (canUseItem && !canCastSkill)
                                {
                                    pCastSkill = 0;
                                }
                                else if (!canUseItem && canCastSkill)
                                {
                                    pUseItem = 0;
                                }
                                else
                                {
                                    pUseItem = 0;
                                    pCastSkill = 0;
                                }
                            }
                            else if (character.CharacterState == CharacterState.ActionRestricted)
                            {
                                // 行动受限，只能使用消耗品
                                items = [.. items.Where(i => i.ItemType == ItemType.Consumable)];
                                canUseItem = items.Count > 0;
                                if (canUseItem)
                                {
                                    pCastSkill = 0;
                                    pNormalAttack = 0;
                                }
                                else
                                {
                                    pUseItem = 0;
                                    pCastSkill = 0;
                                    pNormalAttack = 0;
                                }
                            }
                            else if (character.CharacterState == CharacterState.BattleRestricted)
                            {
                                // 战斗不能，只能对自己使用物品
                                enemys.Clear();
                                teammates.Clear();
                                skills.Clear();
                                if (canUseItem)
                                {
                                    pCastSkill = 0;
                                    pNormalAttack = 0;
                                }
                                else
                                {
                                    pUseItem = 0;
                                    pCastSkill = 0;
                                    pNormalAttack = 0;
                                }
                            }
                            else if (character.CharacterState == CharacterState.SkillRestricted)
                            {
                                // 技能受限，无法使用技能，可以普通攻击，可以使用物品
                                skills.Clear();
                                if (canUseItem)
                                {
                                    pCastSkill = 0;
                                }
                                else
                                {
                                    pUseItem = 0;
                                    pCastSkill = 0;
                                }
                            }
                            else if (character.CharacterState == CharacterState.AttackRestricted)
                            {
                                // 攻击受限，无法普通攻击，可以使用技能，可以使用物品
                                pNormalAttack = 0;
                                if (!canUseItem)
                                {
                                    pUseItem = 0;
                                }
                            }

                            // 启用战棋地图时的专属 AI 决策方法
                            if (isAI && ai != null && startGrid != null)
                            {
                                List<Character> allEnemysInGame = [.. allEnemys.Where(canAttackGridsByStartGrid.Union(canCastGridsByStartGrid).SelectMany(g => g.Characters).Contains)];
                                List<Character> allTeammatesInGame = [.. allTeammates.Where(canAttackGridsByStartGrid.Union(canCastGridsByStartGrid).SelectMany(g => g.Characters).Contains)];

                                aiDecision = ai.DecideAIAction(character, dp, startGrid, canMoveGrids, skills, items, allEnemys, allTeammates, enemys, teammates, pUseItem, pCastSkill, pNormalAttack);
                                type = aiDecision.ActionType;
                            }
                            else
                            {
                                // 模组可以通过此事件来决定角色的行动
                                type = OnDecideActionEvent(character, dp, enemys, teammates, skills, items);
                            }
                            // 若事件未完成决策，则将通过概率对角色进行自动化决策
                            if (type == CharacterActionType.None)
                            {
                                type = GetActionType(dp, pUseItem, pCastSkill, pNormalAttack);
                            }
                        }
                        else if (character.CharacterState == CharacterState.Casting)
                        {
                            // 如果角色上一次吟唱了魔法，这次的行动则是结算这个魔法
                            type = CharacterActionType.CastSkill;
                        }
                        else if (character.CharacterState == CharacterState.PreCastSuperSkill)
                        {
                            // 角色使用回合外爆发技插队
                            type = CharacterActionType.CastSuperSkill;
                        }
                        else
                        {
                            // 完全行动不能
                            type = CharacterActionType.None;
                        }
                    }

                    if (aiDecision != null && aiDecision.ActionType != CharacterActionType.Move && aiDecision.TargetMoveGrid != null)
                    {
                        // 不是纯粹移动的情况，需要手动移动
                        moved = CharacterMove(character, dp, aiDecision.TargetMoveGrid, startGrid);
                    }

                    int costDP = dp.GetActionPointCost(type);

                    effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                    foreach (Effect effect in effects)
                    {
                        effect.OnCharacterActionStart(character, dp, type);
                    }

                    if (type == CharacterActionType.Move)
                    {
                        if (_map != null)
                        {
                            Grid target;
                            if (aiDecision != null && aiDecision.TargetMoveGrid != null)
                            {
                                target = aiDecision.TargetMoveGrid;
                            }
                            else
                            {
                                target = SelectTargetGrid(character, enemys, teammates, _map, canMoveGrids);
                            }
                            moved = CharacterMove(character, dp, target, startGrid);
                        }
                        if (isAI && (aiDecision?.IsPureMove ?? false))
                        {
                            // 取消 AI 的移动
                            SetOnlyMoveHardnessTime(character, dp, ref baseTime);
                            type = CharacterActionType.EndTurn;
                            decided = true;
                            endTurn = true;
                            WriteLine($"[ {character} ] 结束了回合！");
                            OnCharacterDoNothingEvent(character, dp);
                        }
                    }
                    else if (type == CharacterActionType.NormalAttack)
                    {
                        if (!forceAction && (character.CharacterState == CharacterState.NotActionable ||
                            character.CharacterState == CharacterState.ActionRestricted ||
                            character.CharacterState == CharacterState.BattleRestricted ||
                            character.CharacterState == CharacterState.AttackRestricted))
                        {
                            if (IsDebug) WriteLine($"[ {character} ] 的状态为：{CharacterSet.GetCharacterState(character.CharacterState)}，无法使用普通攻击！");
                        }
                        else if (dp.CurrentDecisionPoints < costDP)
                        {
                            if (IsDebug) WriteLine($"[ {character} ] 想要发起普通攻击，但决策点不足，无法使用普通攻击！");
                        }
                        else if (!dp.CheckActionTypeQuota(CharacterActionType.NormalAttack))
                        {
                            if (IsDebug) WriteLine($"[ {character} ] 想要发起普通攻击，但该回合使用普通攻击的次数已超过决策点配额，无法再次使用普通攻击！");
                        }
                        else
                        {
                            // 使用普通攻击逻辑
                            // 如果有询问，先进行询问
                            character.NormalAttack.GamingQueue = this;
                            if (character.NormalAttack.OnInquiryBeforeTargetSelection(character, character.NormalAttack) is InquiryOptions inquiry)
                            {
                                Inquiry(character, inquiry);
                            }
                            // 选择目标
                            List<Character> targets;
                            if (aiDecision != null)
                            {
                                targets = aiDecision.Targets;
                            }
                            else
                            {
                                List<Grid> attackRange = [];
                                if (_map != null && realGrid != null)
                                {
                                    attackRange = _map.GetGridsByRange(realGrid, character.ATR, true);
                                    enemys = [.. enemys.Where(attackRange.SelectMany(g => g.Characters).Contains)];
                                    teammates = [.. teammates.Where(attackRange.SelectMany(g => g.Characters).Contains)];
                                }
                                targets = SelectTargets(character, character.NormalAttack, enemys, teammates, attackRange);
                            }
                            if (targets.Count > 0)
                            {
                                LastRound.Targets[CharacterActionType.NormalAttack] = [.. targets];
                                LastRound.ActionTypes.Add(CharacterActionType.NormalAttack);
                                _stats[statsCharacter].UseDecisionPoints += costDP;
                                _stats[statsCharacter].TurnDecisions++;
                                dp.AddActionType(CharacterActionType.NormalAttack);
                                dp.CurrentDecisionPoints -= costDP;
                                decided = true;

                                OnCharacterNormalAttackEvent(character, dp, targets);

                                character.NormalAttack.Attack(this, character, null, targets);

                                effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                                foreach (Effect effect in effects)
                                {
                                    effect.AfterCharacterNormalAttack(character, character.NormalAttack, targets);
                                }

                                baseTime += character.NormalAttack.RealHardnessTime;
                                effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                                foreach (Effect effect in effects)
                                {
                                    effect.AlterHardnessTimeAfterNormalAttack(character, ref baseTime, ref isCheckProtected);
                                }
                            }
                        }
                    }
                    else if (type == CharacterActionType.PreCastSkill)
                    {
                        if (!forceAction && (character.CharacterState == CharacterState.NotActionable ||
                            character.CharacterState == CharacterState.ActionRestricted ||
                            character.CharacterState == CharacterState.BattleRestricted ||
                            character.CharacterState == CharacterState.SkillRestricted))
                        {
                            if (IsDebug) WriteLine($"[ {character} ] 的状态为：{CharacterSet.GetCharacterState(character.CharacterState)}，无法释放技能！");
                        }
                        else
                        {
                            // 预使用技能，即开始吟唱逻辑
                            Skill? skill;
                            if (aiDecision != null && aiDecision.SkillToUse is Skill s)
                            {
                                skill = s;
                            }
                            else
                            {
                                skill = OnSelectSkillEvent(character, skills);
                            }
                            if (skill is null && IsCharacterInAIControlling(character) && skills.Count > 0)
                            {
                                skill = skills[Random.Shared.Next(skills.Count)];
                            }
                            if (skill != null)
                            {
                                skill.GamingQueue = this;
                                List<Character> targets = [];
                                List<Grid> grids = [];
                                costDP = dp.GetActionPointCost(type, skill);
                                if (dp.CurrentDecisionPoints < costDP)
                                {
                                    if (IsDebug) WriteLine($"[ {character} ] 想要释放 [ {skill.Name} ]，但决策点不足，无法释放技能！");
                                }
                                else if (skill.SkillType == SkillType.Magic)
                                {
                                    if (CheckCanCast(character, skill, out double cost))
                                    {
                                        // 如果有询问，先进行询问
                                        if (skill.InquiryBeforeTargetSelection(character, skill) is InquiryOptions inquiry)
                                        {
                                            Inquiry(character, inquiry);
                                        }

                                        // 吟唱前需要先选取目标
                                        List<Grid> castRange = [];
                                        if (_map != null && realGrid != null)
                                        {
                                            castRange = _map.GetGridsByRange(realGrid, skill.CastRange, true);
                                            enemys = [.. enemys.Where(castRange.SelectMany(g => g.Characters).Contains)];
                                            teammates = [.. teammates.Where(castRange.SelectMany(g => g.Characters).Contains)];
                                        }
                                        (targets, grids) = GetSelectedSkillTargetsList(character, skill, enemys, teammates, castRange, allEnemys, allTeammates, aiDecision);

                                        if (targets.Count > 0)
                                        {
                                            // 免疫检定
                                            CheckSkilledImmune(character, targets, skill);
                                        }
                                        bool hasTarget = targets.Count > 0 || (skill.IsNonDirectional && grids.Count > 0 && (skill.AllowSelectNoCharacterGrid || !skill.AllowSelectNoCharacterGrid && targets.Count > 0));
                                        if (hasTarget)
                                        {
                                            LastRound.Skills[CharacterActionType.PreCastSkill] = skill;
                                            LastRound.Targets[CharacterActionType.PreCastSkill] = [.. targets];
                                            LastRound.ActionTypes.Add(CharacterActionType.PreCastSkill);
                                            _stats[statsCharacter].UseDecisionPoints += costDP;
                                            _stats[statsCharacter].TurnDecisions++;
                                            dp.AddActionType(CharacterActionType.PreCastSkill, skill);
                                            dp.CurrentDecisionPoints -= costDP;
                                            decided = true;
                                            endTurn = true;

                                            character.CharacterState = CharacterState.Casting;
                                            SkillTarget skillTarget = new(skill, targets, grids);
                                            OnCharacterPreCastSkillEvent(character, dp, skillTarget);

                                            _castingSkills[character] = skillTarget;
                                            baseTime += skill.RealCastTime;
                                            isCheckProtected = false;
                                            skill.OnSkillCasting(this, character, targets, grids);

                                            effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                                            foreach (Effect effect in effects)
                                            {
                                                effect.AfterCharacterStartCasting(character, skill, targets);
                                            }
                                        }
                                        else
                                        {
                                            if (IsDebug) WriteLine($"[ {character} ] 想要吟唱 [ {skill.Name} ]，但是没有目标！");
                                        }
                                    }
                                }
                                else if (skill is CourageCommandSkill && dp.CourageCommandSkill)
                                {
                                    if (IsDebug) WriteLine($"[ {character} ] 想要释放 [ {skill.Name} ]，但该回合已经使用过勇气指令，无法再次使用勇气指令！");
                                }
                                else if (skill is not CourageCommandSkill && !skill.IsSuperSkill && !dp.CheckActionTypeQuota(CharacterActionType.CastSkill))
                                {
                                    if (IsDebug) WriteLine($"[ {character} ] 想要释放 [ {skill.Name} ]，但该回合使用战技的次数已超过决策点配额，无法再次使用战技！");
                                }
                                else if (skill is not CourageCommandSkill && skill.IsSuperSkill && !dp.CheckActionTypeQuota(CharacterActionType.CastSuperSkill))
                                {
                                    if (IsDebug) WriteLine($"[ {character} ] 想要释放 [ {skill.Name} ]，但该回合使用爆发技的次数已超过决策点配额，无法再次使用爆发技！");
                                }
                                else
                                {
                                    // 只有魔法需要吟唱，战技和爆发技直接释放
                                    if (CheckCanCast(character, skill, out double cost))
                                    {
                                        // 如果有询问，先进行询问
                                        if (skill.InquiryBeforeTargetSelection(character, skill) is InquiryOptions inquiry)
                                        {
                                            Inquiry(character, inquiry);
                                        }

                                        List<Grid> castRange = [];
                                        if (_map != null && realGrid != null)
                                        {
                                            castRange = _map.GetGridsByRange(realGrid, skill.CastRange, true);
                                            enemys = [.. enemys.Where(castRange.SelectMany(g => g.Characters).Contains)];
                                            teammates = [.. teammates.Where(castRange.SelectMany(g => g.Characters).Contains)];
                                        }
                                        (targets, grids) = GetSelectedSkillTargetsList(character, skill, enemys, teammates, castRange, allEnemys, allTeammates, aiDecision);

                                        if (targets.Count > 0)
                                        {
                                            // 免疫检定
                                            CheckSkilledImmune(character, targets, skill);
                                        }
                                        bool hasTarget = targets.Count > 0 || (skill.IsNonDirectional && grids.Count > 0 && (skill.AllowSelectNoCharacterGrid || !skill.AllowSelectNoCharacterGrid && targets.Count > 0));
                                        if (hasTarget)
                                        {
                                            CharacterActionType skillType = skill.SkillType == SkillType.SuperSkill ? CharacterActionType.CastSuperSkill : CharacterActionType.CastSkill;
                                            LastRound.Skills[skillType] = skill;
                                            LastRound.Targets[skillType] = [.. targets];
                                            LastRound.ActionTypes.Add(skillType);
                                            if (skill is not CourageCommandSkill)
                                            {
                                                _stats[statsCharacter].UseDecisionPoints += costDP;
                                                _stats[statsCharacter].TurnDecisions++;
                                                dp.AddActionType(skillType, skill);
                                                dp.CurrentDecisionPoints -= costDP;
                                            }
                                            else
                                            {
                                                // 勇气指令不消耗决策点，但是有标记
                                                dp.CourageCommandSkill = true;
                                            }
                                            decided = true;

                                            SkillTarget skillTarget = new(skill, targets, grids);
                                            OnCharacterPreCastSkillEvent(character, dp, skillTarget);

                                            skill.OnSkillCasting(this, character, targets, grids);

                                            effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                                            foreach (Effect effect in effects)
                                            {
                                                effect.AfterCharacterStartCasting(character, skill, targets);
                                            }

                                            skill.BeforeSkillCasted();

                                            character.EP -= cost;
                                            baseTime += skill.RealHardnessTime;
                                            skill.CurrentCD = skill.RealCD;
                                            skill.Enable = false;
                                            LastRound.SkillsCost[skill] = $"{-cost:0.##} EP";
                                            WriteLine($"[ {character} ] 消耗了 {cost:0.##} 点能量，释放了{(skill.IsSuperSkill ? "爆发技" : "战技")} [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}");

                                            OnCharacterCastSkillEvent(character, dp, skillTarget, cost);

                                            skill.OnSkillCasted(this, character, targets, grids);

                                            effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                                            foreach (Effect effect in effects)
                                            {
                                                effect.AfterCharacterCastSkill(character, skill, targets);
                                            }

                                            effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                                            foreach (Effect effect in effects)
                                            {
                                                effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                                            }
                                        }
                                        else
                                        {
                                            if (IsDebug) WriteLine($"[ {character} ] 想要释放 [ {skill.Name} ]，但是没有目标！");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (type == CharacterActionType.CastSkill)
                    {
                        if (_castingSkills.TryGetValue(character, out SkillTarget skillTarget))
                        {
                            // 使用技能逻辑，结束吟唱状态
                            character.CharacterState = CharacterState.Actionable;
                            character.UpdateCharacterState();
                            Skill skill = skillTarget.Skill;
                            List<Character> targets = [];
                            List<Grid> grids = [];
                            if (skill.IsNonDirectional && _map != null)
                            {
                                grids = skillTarget.TargetGrids;
                                targets = skill.SelectTargetsByRange(character, allEnemys, allTeammates, targets, grids);
                            }
                            else
                            {
                                targets = [.. skillTarget.Targets.Where(c => c == character || !c.IsUnselectable)];
                                if (skill.CanSelectTargetRange > 0)
                                {
                                    targets = skill.SelectTargetsByCanSelectTargetRange(character, allEnemys, allTeammates, targets);
                                }
                            }

                            if (targets.Count > 0)
                            {
                                // 免疫检定
                                CheckSkilledImmune(character, targets, skill);
                            }

                            // 判断是否能够释放技能
                            bool hasTarget = targets.Count > 0 || (skill.IsNonDirectional && grids.Count > 0 && (skill.AllowSelectNoCharacterGrid || !skill.AllowSelectNoCharacterGrid && targets.Count > 0));

                            if (hasTarget && CheckCanCast(character, skill, out double cost))
                            {
                                decided = true;
                                endTurn = true;
                                LastRound.Targets[CharacterActionType.CastSkill] = [.. targets];
                                LastRound.Skills[CharacterActionType.CastSkill] = skill;
                                LastRound.ActionTypes.Add(CharacterActionType.CastSkill);
                                _castingSkills.Remove(character);

                                skill.BeforeSkillCasted();

                                character.MP -= cost;
                                baseTime += skill.RealHardnessTime;
                                skill.CurrentCD = skill.RealCD;
                                skill.Enable = false;
                                LastRound.SkillsCost[skill] = $"{-cost:0.##} MP";
                                WriteLine($"[ {character} ] 消耗了 {cost:0.##} 点魔法值，释放了魔法 [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}");

                                OnCharacterCastSkillEvent(character, dp, skillTarget, cost);

                                skill.OnSkillCasted(this, character, targets, grids);

                                effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                                foreach (Effect effect in effects)
                                {
                                    effect.AfterCharacterCastSkill(character, skill, targets);
                                }

                                effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                                foreach (Effect effect in effects)
                                {
                                    effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                                }
                            }
                            else
                            {
                                if (!hasTarget)
                                {
                                    WriteLine($"[ {character} ] 想要释放 [ {skill.Name} ]，但是没有目标！");
                                }
                                WriteLine($"[ {character} ] 放弃释放技能！");
                                character.CharacterState = CharacterState.Actionable;
                                character.UpdateCharacterState();
                                // 放弃释放技能会获得3的硬直时间
                                if (baseTime == 0) baseTime = 3;
                                decided = true;
                                endTurn = true;
                            }
                        }
                        else
                        {
                            // 原吟唱的技能丢失（被打断或者被取消），允许角色再次决策
                            character.CharacterState = CharacterState.Actionable;
                            character.UpdateCharacterState();
                        }
                    }
                    else if (type == CharacterActionType.CastSuperSkill)
                    {
                        _stats[statsCharacter].TurnDecisions++;
                        dp.AddActionType(CharacterActionType.CastSuperSkill);
                        LastRound.ActionTypes.Add(CharacterActionType.CastSuperSkill);
                        decided = true;
                        endTurn = true;
                        // 结束预释放爆发技的状态
                        character.CharacterState = CharacterState.Actionable;
                        character.UpdateCharacterState();
                        Skill skill = _castingSuperSkills[character];
                        LastRound.Skills[CharacterActionType.CastSuperSkill] = skill;
                        _castingSuperSkills.Remove(character);

                        // 判断是否能够释放技能
                        if (CheckCanCast(character, skill, out double cost))
                        {
                            // 预释放的爆发技不可取消
                            List<Grid> castRange = _map != null && realGrid != null ? _map.GetGridsByRange(realGrid, skill.CastRange, true) : [];
                            (List<Character> targets, List<Grid> grids) = GetSelectedSkillTargetsList(character, skill, enemys, teammates, castRange, allEnemys, allTeammates, aiDecision);
                            // 免疫检定
                            CheckSkilledImmune(character, targets, skill);
                            LastRound.Targets[CharacterActionType.CastSuperSkill] = [.. targets];

                            skill.BeforeSkillCasted();

                            character.EP -= cost;
                            baseTime += skill.RealHardnessTime;
                            skill.CurrentCD = skill.RealCD;
                            skill.Enable = false;
                            LastRound.SkillsCost[skill] = $"{-cost:0.##} EP";
                            WriteLine($"[ {character} ] 消耗了 {cost:0.##} 点能量值，释放了爆发技 [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}");

                            SkillTarget skillTarget = new(skill, targets, grids);
                            OnCharacterCastSkillEvent(character, dp, skillTarget, cost);

                            skill.OnSkillCasted(this, character, targets, grids);

                            effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                            foreach (Effect effect in effects)
                            {
                                effect.AfterCharacterCastSkill(character, skill, targets);
                            }

                            effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                            foreach (Effect effect in effects)
                            {
                                effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                            }
                        }
                        else
                        {
                            WriteLine($"[ {character} ] 因能量不足放弃释放爆发技！");
                            character.CharacterState = CharacterState.Actionable;
                            character.UpdateCharacterState();
                            // 放弃释放技能会获得3的硬直时间
                            if (baseTime == 0) baseTime = 3;
                            decided = true;
                            endTurn = true;
                        }
                    }
                    else if (type == CharacterActionType.UseItem)
                    {
                        // 使用物品逻辑
                        Item? item;
                        if (aiDecision != null && aiDecision.ItemToUse != null)
                        {
                            item = aiDecision.ItemToUse;
                        }
                        else
                        {
                            item = OnSelectItemEvent(character, items);
                        }
                        if (item is null && IsCharacterInAIControlling(character) && items.Count > 0)
                        {
                            // AI 控制下随机选取一个物品
                            item = items[Random.Shared.Next(items.Count)];
                        }
                        if (item != null && item.Skills.Active != null)
                        {
                            Skill skill = item.Skills.Active;

                            // 如果有询问，先进行询问
                            if (item.InquiryBeforeTargetSelection(character, item) is InquiryOptions inquiry)
                            {
                                Inquiry(character, inquiry);
                            }

                            if (skill.InquiryBeforeTargetSelection(character, skill) is InquiryOptions inquiry2)
                            {
                                Inquiry(character, inquiry2);
                            }

                            List<Grid> castRange = [];
                            if (_map != null && realGrid != null)
                            {
                                castRange = _map.GetGridsByRange(realGrid, skill.CastRange, true);
                                enemys = [.. enemys.Where(castRange.SelectMany(g => g.Characters).Contains)];
                                teammates = [.. teammates.Where(castRange.SelectMany(g => g.Characters).Contains)];
                            }
                            if (dp.CurrentDecisionPoints < costDP)
                            {
                                if (IsDebug) WriteLine($"[ {character} ] 想要使用物品 [ {item.Name} ]，但决策点不足，无法使用物品！");
                            }
                            else if (!dp.CheckActionTypeQuota(CharacterActionType.UseItem))
                            {
                                if (IsDebug) WriteLine($"[ {character} ] 想要使用物品 [ {item.Name} ]，但该回合使用物品的次数已超过决策点配额，无法再使用物品！");
                            }
                            else if (UseItem(item, character, dp, enemys, teammates, castRange, allEnemys, allTeammates, aiDecision))
                            {
                                _stats[statsCharacter].UseDecisionPoints += costDP;
                                _stats[statsCharacter].TurnDecisions++;
                                dp.AddActionType(CharacterActionType.UseItem);
                                dp.CurrentDecisionPoints -= costDP;
                                LastRound.ActionTypes.Add(CharacterActionType.UseItem);
                                LastRound.Items[CharacterActionType.UseItem] = item;
                                decided = true;
                                baseTime += skill.RealHardnessTime > 0 ? skill.RealHardnessTime : 5;
                                effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                                foreach (Effect effect in effects)
                                {
                                    effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                                }
                            }
                        }
                    }
                    else if (type == CharacterActionType.EndTurn)
                    {
                        _stats[statsCharacter].TurnDecisions++;
                        SetOnlyMoveHardnessTime(character, dp, ref baseTime);
                        decided = true;
                        endTurn = true;
                        WriteLine($"[ {character} ] 结束了回合！");
                        OnCharacterDoNothingEvent(character, dp);
                    }
                    else
                    {
                        if (baseTime == 0) baseTime += 7;
                        decided = true;
                        endTurn = true;
                        WriteLine($"[ {character} ] 完全行动不能！");
                    }

                    if (forceAction)
                    {
                        endTurn = true;
                    }
                }

                if (!decided && (isAI || cancelTimes == 0))
                {
                    endTurn = true;
                    baseTime += 4;
                    type = CharacterActionType.EndTurn;
                }

                if (type == CharacterActionType.None)
                {
                    endTurn = true;
                    WriteLine($"[ {character} ] 放弃了行动！");
                    OnCharacterGiveUpEvent(character, dp);
                }

                if (character.CharacterState != CharacterState.Casting) dp.ActionsHardnessTime.Add(baseTime);

                OnCharacterActionTakenEvent(character, dp, type, LastRound);

                effects = [.. _queue.Union([character]).SelectMany(c => c.Effects).Where(e => e.IsInEffect).OrderByDescending(e => e.Priority).Distinct()];
                foreach (Effect effect in effects)
                {
                    effect.OnCharacterActionTaken(character, dp, type);
                }

                if (!AfterCharacterAction(character, type))
                {
                    endTurn = true;
                }
            }

            if (character.CharacterState != CharacterState.Casting && dp.ActionsHardnessTime.Count > 0)
            {
                baseTime = dp.ActionsTaken > 1 ? (dp.ActionsHardnessTime.Max() + dp.DecisionPointsCost) : dp.ActionsHardnessTime.Max();
            }

            if (character.Master is null)
            {
                _stats[character].ActionTurn += 1;
            }

            AfterCharacterDecision(character, dp);
            OnCharacterDecisionCompletedEvent(character, dp, LastRound);
            effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
            foreach (Effect effect in effects)
            {
                effect.OnCharacterDecisionCompleted(character, dp);
            }

            // 统一在回合结束时处理角色的死亡
            ProcessCharacterDeath();

            // 移除回合奖励
            RemoveRoundRewards(character, rewards);

            if (_isGameEnd)
            {
                // 回合结束事件
                OnTurnEndEvent(character, dp);

                AfterTurn(character);

                _isInRound = false;
                return _isGameEnd;
            }

            // 减少硬直时间
            double newHardnessTime = baseTime;
            if (character.CharacterState != CharacterState.Casting)
            {
                newHardnessTime = Calculation.Round2Digits(baseTime);
                WriteLine($"[ {character} ] 回合结束，获得硬直时间：{newHardnessTime} {GameplayEquilibriumConstant.InGameTime}");
            }
            else
            {
                newHardnessTime = Calculation.Round2Digits(baseTime);
                WriteLine($"[ {character} ] 进行吟唱，持续时间：{newHardnessTime} {GameplayEquilibriumConstant.InGameTime}");
                LastRound.CastTime = newHardnessTime;
            }
            AddCharacter(character, newHardnessTime, isCheckProtected);
            LastRound.HardnessTime = newHardnessTime;
            OnQueueUpdatedEvent(_queue, character, dp, newHardnessTime, QueueUpdatedReason.Action, "设置角色行动后的硬直时间。");

            effects = [.. character.Effects.OrderByDescending(e => e.Priority)];
            foreach (Effect effect in effects)
            {
                if (effect.IsInEffect)
                {
                    effect.OnTurnEnd(character);
                }

                // 自身被动不会考虑
                if (effect.EffectType == EffectType.None && effect.Skill.SkillType == SkillType.Passive)
                {
                    continue;
                }

                // 在回合结束时移除技能持续回合，而不是等时间流逝
                if (!effect.Durative && effect.DurationTurn > 0)
                {
                    // 按回合移除特效
                    effect.RemainDurationTurn--;
                    if (effect.RemainDurationTurn <= 0)
                    {
                        effect.RemainDurationTurn = 0;
                        character.Effects.Remove(effect);
                        effect.OnEffectLost(character);
                        WriteLine($"[ {character} ] 失去了 [ {effect.Name} ] 效果。");
                    }
                }
            }

            // 清空临时决策点
            dp.ClearTempActionQuota();

            // 回合结束事件
            OnTurnEndEvent(character, dp);

            AfterTurn(character);

            WriteLine("");
            _isInRound = false;

            // 有人想要插队吗？
            WillPreCastSuperSkill();

            return _isGameEnd;
        }

        /// <summary>
        /// 处理角色死亡
        /// </summary>
        protected void ProcessCharacterDeath()
        {
            foreach (DeathRelation dr in _roundDeaths)
            {
                Character death = dr.Death;
                Character? killer = dr.Killer;
                Character[] assists = dr.Assists;

                if (!_isGameEnd)
                {
                    AfterDeathCalculation(death, killer, assists);
                }
            }
            _roundDeaths.Clear();
        }

        /// <summary>
        /// 角色行动后触发
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        /// <returns>返回 false 结束回合</returns>
        protected virtual bool AfterCharacterAction(Character character, CharacterActionType type)
        {
            List<Character> allTeammates = GetTeammates(character);
            Character[] allEnemys = [.. _allCharacters.Union(_queue).Distinct().Where(c => c != character && !allTeammates.Contains(c) && !_eliminated.Contains(c) && c.Master != character && character.Master != c)];
            if (!allEnemys.Any(c => c.HP > 0))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 角色完成回合决策后触发
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        protected virtual void AfterCharacterDecision(Character character, DecisionPoints dp)
        {

        }

        /// <summary>
        /// 死亡结算时触发
        /// </summary>
        /// <param name="death"></param>
        /// <param name="killer"></param>
        protected virtual void OnDeathCalculation(Character death, Character killer)
        {

        }

        /// <summary>
        /// 死亡结算完成后触发
        /// </summary>
        /// <param name="death"></param>
        /// <param name="killer"></param>
        /// <param name="assists"></param>
        protected virtual void AfterDeathCalculation(Character death, Character? killer, Character[] assists)
        {
            if (!_queue.Any(c => c != killer && (c.Master is null || c.Master != killer)))
            {
                // 没有其他的角色了，游戏结束
                if (killer != null)
                {
                    WriteLine("[ " + killer + " ] 是胜利者。");
                    _queue.Remove(killer);
                    _eliminated.Add(killer);
                    OnGameEndEvent(killer);
                }
                else
                {
                    WriteLine("游戏结束。");
                }
                _isGameEnd = true;
            }
        }

        /// <summary>
        /// 获取复活时间
        /// </summary>
        /// <param name="character"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        protected virtual double GetRespawnTime(Character character, int times)
        {
            _continuousKilling.TryGetValue(character, out int coefficient);
            return Calculation.Round2Digits(Math.Min(30, character.Level * 0.15 + times * 0.87 + coefficient));
        }

        /// <summary>
        /// 回合开始前触发
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeTurn(Character character)
        {
            return true;
        }

        /// <summary>
        /// 回合结束后触发
        /// </summary>
        /// <returns></returns>
        protected virtual void AfterTurn(Character character)
        {

        }

        #endregion

        #region 回合内-结算

        /// <summary>
        /// 对敌人造成伤害
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="damageType"></param>
        /// <param name="magicType"></param>
        /// <param name="damageResult"></param>
        /// <param name="options"></param>
        public void DamageToEnemy(Character actor, Character enemy, double damage, bool isNormalAttack, DamageType damageType = DamageType.Physical, MagicType magicType = MagicType.None, DamageResult damageResult = DamageResult.Normal, DamageCalculationOptions? options = null)
        {
            // 如果敌人在结算伤害之前就已经死亡，将不会继续下去
            if (enemy.HP <= 0)
            {
                return;
            }

            // 不管有没有暴击，都尝试往回合记录中添加目标，不暴击时不会修改原先值
            if (!LastRound.IsCritical.TryAdd(enemy, damageResult == DamageResult.Critical) && damageResult == DamageResult.Critical)
            {
                // 暴击了修改目标对应的值为 true
                LastRound.IsCritical[enemy] = true;
            }

            List<Character> characters = [actor, enemy];
            bool isEvaded = damageResult == DamageResult.Evaded;
            List<Effect> effects = [];
            options ??= new(actor);
            if (options.ExpectedDamage == 0) options.ExpectedDamage = damage;

            Dictionary<Effect, double> totalDamageBonus = [];
            if (options.TriggerEffects)
            {
                // 真实伤害跳过伤害加成区间
                if (damageType != DamageType.True)
                {
                    effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                    foreach (Effect effect in effects)
                    {
                        double damageBonus = effect.AlterActualDamageAfterCalculation(actor, enemy, damage, isNormalAttack, damageType, magicType, damageResult, ref isEvaded, totalDamageBonus);
                        if (damageBonus != 0) totalDamageBonus[effect] = damageBonus;
                        if (isEvaded)
                        {
                            damageResult = DamageResult.Evaded;
                        }
                    }
                    damage += totalDamageBonus.Sum(kv => kv.Value);
                }
                else
                {
                    effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                    foreach (Effect effect in effects)
                    {
                        if (effect.BeforeApplyTrueDamage(actor, enemy, damage, isNormalAttack, damageResult))
                        {
                            damageResult = DamageResult.Evaded;
                        }
                    }
                }
            }
            options.AfterDamageBonus = totalDamageBonus;

            // 魔法效能乘区
            if (options.IsMagicSkill)
            {
                options.MagicEfficacyDamage = damage * (options.MagicEfficacy - 1);
                damage *= options.MagicEfficacy;
            }

            options.FinalDamage = damage;
            double actualDamage = damage;

            // 闪避了就没伤害了
            if (damageResult != DamageResult.Evaded)
            {
                // 开始计算伤害免疫
                bool isImmune = false;
                // 真实伤害或者指定无视免疫则跳过免疫检定
                if (damageType != DamageType.True || !options.IgnoreImmune)
                {
                    // 此变量为是否无视免疫
                    bool ignore = false;
                    // 技能免疫无法免疫普通攻击，但是魔法免疫和物理免疫可以
                    isImmune = (enemy.ImmuneType & ImmuneType.All) == ImmuneType.All ||
                        (!isNormalAttack && (enemy.ImmuneType & ImmuneType.Skilled) == ImmuneType.Skilled) ||
                        (damageType == DamageType.Physical && (enemy.ImmuneType & ImmuneType.Physical) == ImmuneType.Physical) ||
                        (damageType == DamageType.Magical && (enemy.ImmuneType & ImmuneType.Magical) == ImmuneType.Magical);
                    if (isImmune)
                    {
                        if (isNormalAttack)
                        {
                            if (actor.NormalAttack.IgnoreImmune == ImmuneType.All ||
                                (damageType == DamageType.Physical && actor.NormalAttack.IgnoreImmune == ImmuneType.Physical) ||
                                (damageType == DamageType.Magical && actor.NormalAttack.IgnoreImmune == ImmuneType.Magical))
                            {
                                ignore = true;
                            }
                        }
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                        foreach (Effect effect in effects)
                        {
                            if (!effect.OnDamageImmuneCheck(actor, enemy, isNormalAttack, damageType, magicType, damage))
                            {
                                ignore = true;
                            }
                        }
                    }

                    if (ignore)
                    {
                        // 无视免疫
                        isImmune = false;
                    }

                    if (isImmune)
                    {
                        // 免疫
                        damageResult = DamageResult.Immune;
                        LastRound.IsImmune[enemy] = true;
                        WriteLine($"[ {enemy} ] 免疫了此伤害！");
                        actualDamage = 0;
                    }
                }

                // 继续计算伤害
                if (!isImmune)
                {
                    if (damage < 0) damage = 0;

                    string damageTypeString = CharacterSet.GetDamageTypeName(damageType, magicType);
                    string shieldMsg = "";

                    // 真实伤害或指定跳过护盾结算则跳过护盾结算
                    if (damageType != DamageType.True || !options.CalculateShield)
                    {
                        // 在护盾结算前，特效可以有自己的逻辑
                        bool change = false;
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                        foreach (Effect effect in effects)
                        {
                            double damageReduce = 0;
                            if (!effect.BeforeShieldCalculation(enemy, actor, damageType, magicType, damage, ref damageReduce, ref shieldMsg))
                            {
                                change = true;
                            }
                            if (damageReduce != 0)
                            {
                                actualDamage -= damageReduce;
                                if (actualDamage < 0) actualDamage = 0;
                            }
                        }

                        // 检查护盾
                        if (!change)
                        {
                            double remain = actualDamage;

                            // 检查特效护盾
                            effects = [.. enemy.Shield.ShieldOfEffects.Keys.OrderByDescending(e => e.Priority)];
                            foreach (Effect effect in effects)
                            {
                                ShieldOfEffect soe = enemy.Shield.ShieldOfEffects[effect];
                                bool checkType = false;
                                switch (damageType)
                                {
                                    case DamageType.Physical:
                                        checkType = soe.ShieldType == ShieldType.Physical || soe.ShieldType == ShieldType.Mix;
                                        break;
                                    case DamageType.Magical:
                                        checkType = (soe.ShieldType == ShieldType.Magical && soe.MagicType == magicType) || soe.ShieldType == ShieldType.Mix;
                                        break;
                                    default:
                                        break;
                                }
                                if (checkType && soe.Shield > 0)
                                {
                                    double effectShield = soe.Shield;
                                    // 判断护盾余额
                                    if (enemy.Shield.CalculateShieldOfEffect(effect, remain) > 0)
                                    {
                                        WriteLine($"[ {enemy} ] 发动了 [ {effect.Skill.Name} ] 的护盾效果，抵消了 {remain:0.##} 点{damageTypeString}！");
                                        remain = 0;
                                    }
                                    else
                                    {
                                        WriteLine($"[ {enemy} ] 发动了 [ {effect.Skill.Name} ] 的护盾效果，抵消了 {effectShield:0.##} 点{damageTypeString}，护盾已破碎！");
                                        remain -= effectShield;
                                        Effect[] effects2 = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                                        foreach (Effect effect2 in effects2)
                                        {
                                            if (!effect2.OnShieldBroken(enemy, actor, effect, remain))
                                            {
                                                WriteLine($"[ {(enemy.Effects.Contains(effect2) ? enemy : actor)} ] 因护盾破碎而发动了 [ {effect2.Skill.Name} ]，化解了本次伤害！");
                                                remain = 0;
                                            }
                                        }
                                    }
                                    if (remain <= 0)
                                    {
                                        Effect[] effects2 = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                                        foreach (Effect effect2 in effects2)
                                        {
                                            effect2.OnShieldNeutralizeDamage(enemy, actor, damageType, magicType, damage, ShieldType.Effect);
                                        }
                                        break;
                                    }
                                }
                            }

                            // 如果伤害仍然大于0，继续检查护盾
                            if (remain > 0)
                            {
                                // 检查指定类型的护盾值
                                bool isMagicDamage = damageType == DamageType.Magical;
                                double shield = enemy.Shield[isMagicDamage, magicType];
                                if (shield > 0)
                                {
                                    shield -= remain;
                                    string shieldTypeString = isMagicDamage ? "魔法" : "物理";
                                    ShieldType shieldType = isMagicDamage ? ShieldType.Magical : ShieldType.Physical;
                                    if (shield > 0)
                                    {
                                        WriteLine($"[ {enemy} ] 的{shieldTypeString}护盾抵消了 {remain:0.##} 点{damageTypeString}！");
                                        enemy.Shield[isMagicDamage, magicType] -= remain;
                                        remain = 0;
                                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                                        foreach (Effect effect in effects)
                                        {
                                            effect.OnShieldNeutralizeDamage(enemy, actor, damageType, magicType, damage, shieldType);
                                        }
                                    }
                                    else
                                    {
                                        WriteLine($"[ {enemy} ] 的{shieldTypeString}护盾抵消了 {enemy.Shield[isMagicDamage, magicType]:0.##} 点{damageTypeString}并破碎！");
                                        remain -= enemy.Shield[isMagicDamage, magicType];
                                        enemy.Shield[isMagicDamage, magicType] = 0;
                                        if (isMagicDamage && enemy.Shield.TotalMagical <= 0 || !isMagicDamage && enemy.Shield.TotalPhysical <= 0)
                                        {
                                            effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                                            foreach (Effect effect in effects)
                                            {
                                                if (!effect.OnShieldBroken(enemy, actor, shieldType, remain))
                                                {
                                                    WriteLine($"[ {(enemy.Effects.Contains(effect) ? enemy : actor)} ] 因护盾破碎而发动了 [ {effect.Skill.Name} ]，化解了本次伤害！");
                                                    remain = 0;
                                                }
                                            }
                                        }
                                    }
                                }

                                // 检查混合护盾
                                if (remain > 0 && enemy.Shield.TotalMix > 0)
                                {
                                    shield = enemy.Shield.TotalMix;
                                    shield -= remain;
                                    if (shield > 0)
                                    {
                                        WriteLine($"[ {enemy} ] 的混合护盾抵消了 {remain:0.##} 点{damageTypeString}！");
                                        enemy.Shield.Mix -= remain;
                                        remain = 0;
                                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                                        foreach (Effect effect in effects)
                                        {
                                            effect.OnShieldNeutralizeDamage(enemy, actor, damageType, magicType, damage, ShieldType.Mix);
                                        }
                                    }
                                    else
                                    {
                                        WriteLine($"[ {enemy} ] 的混合护盾抵消了 {enemy.Shield.TotalMix:0.##} 点{damageTypeString}并破碎！");
                                        remain -= enemy.Shield.TotalMix;
                                        enemy.Shield.Mix = 0;
                                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                                        foreach (Effect effect in effects)
                                        {
                                            if (!effect.OnShieldBroken(enemy, actor, ShieldType.Mix, remain))
                                            {
                                                WriteLine($"[ {(enemy.Effects.Contains(effect) ? enemy : actor)} ] 因护盾破碎而发动了 [ {effect.Skill.Name} ]，化解了本次伤害！");
                                                remain = 0;
                                            }
                                        }
                                    }
                                }
                            }

                            actualDamage = remain;
                        }

                        // 统计护盾
                        if (damage > actualDamage)
                        {
                            Character statsCharacter = actor;
                            if (statsCharacter.Master != null)
                            {
                                statsCharacter = statsCharacter.Master;
                            }
                            if (_stats.TryGetValue(statsCharacter, out CharacterStatistics? stats) && stats != null)
                            {
                                stats.TotalShield += damage - actualDamage;
                            }
                            options.ShieldReduction += damage - actualDamage;
                        }
                    }

                    options.ActualDamage = actualDamage;
                    enemy.HP -= actualDamage;
                    string strDamageMessage = $"[ {enemy} ] 受到了 {actualDamage:0.##} 点{damageTypeString}！{shieldMsg}";
                    effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                    foreach (Effect effect in effects)
                    {
                        effect.OnApplyDamage(enemy, actor, damage, actualDamage, isNormalAttack, damageType, magicType, damageResult, shieldMsg, ref strDamageMessage);
                    }
                    if (IsDebug)
                    {
                        string strBeforeBonus = "";
                        if (options.BeforeDamageBonus.Count > 0)
                        {
                            strBeforeBonus = string.Join("", options.BeforeDamageBonus.Select(kv => $"{(kv.Value >= 0 ? " + " : " - ")}{Math.Abs(kv.Value):0.##}（{kv.Key.Name}）"));
                        }
                        string strDefenseReduction = options.DefenseReduction == 0 ? "" : ($"{(options.DefenseReduction >= 0 ? " - " : " + ")}{Math.Abs(options.DefenseReduction):0.##}（减伤）");
                        string strCriticalDamage = options.CriticalDamage == 0 ? "" : ($"{(options.CriticalDamage >= 0 ? " + " : " - ")}{Math.Abs(options.CriticalDamage):0.##}（暴击）");
                        string strAfterBonus = "";
                        if (options.AfterDamageBonus.Count > 0)
                        {
                            strAfterBonus = string.Join("", options.AfterDamageBonus.Select(kv => $"{(kv.Value >= 0 ? " + " : " - ")}{Math.Abs(kv.Value):0.##}（{kv.Key.Name}）"));
                        }
                        string strMagicEfficacyDamage = options.MagicEfficacyDamage == 0 ? "" : ($"{(options.MagicEfficacyDamage >= 0 ? " + " : " - ")}{Math.Abs(options.MagicEfficacyDamage):0.##}（魔法效能：{options.MagicEfficacy * 100:0.##}%）");
                        string strShieldReduction = options.ShieldReduction == 0 ? "" : ($"{(options.ShieldReduction >= 0 ? " - " : " + ")}{Math.Abs(options.ShieldReduction):0.##}（护盾）");
                        strDamageMessage += $"【{options.ExpectedDamage:0.##}（基础）{strBeforeBonus}{strDefenseReduction}{strCriticalDamage}{strAfterBonus}{strMagicEfficacyDamage}{strShieldReduction} = {options.ActualDamage:0.##} 点{damageTypeString}】";
                    }
                    WriteLine(strDamageMessage);

                    // 生命偷取
                    double steal = actualDamage * actor.Lifesteal;
                    bool allowSteal = true;
                    effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                    foreach (Effect effect in effects)
                    {
                        if (!effect.BeforeLifesteal(actor, enemy, damage, steal))
                        {
                            allowSteal = false;
                        }
                    }
                    if (allowSteal)
                    {
                        HealToTarget(actor, actor, steal, false, true);
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                        foreach (Effect effect in effects)
                        {
                            effect.AfterLifesteal(actor, enemy, damage, steal);
                        }
                    }

                    // 造成伤害和受伤都可以获得能量。护盾抵消的伤害不算
                    double ep = GetEP(actualDamage, GameplayEquilibriumConstant.DamageGetEPFactor, GameplayEquilibriumConstant.DamageGetEPMax);
                    if (ep > 0)
                    {
                        effects = [.. actor.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                        foreach (Effect effect in effects)
                        {
                            effect.AlterEPAfterDamage(actor, ref ep);
                        }
                        actor.EP += ep;
                    }
                    ep = GetEP(actualDamage, GameplayEquilibriumConstant.TakenDamageGetEPFactor, GameplayEquilibriumConstant.TakenDamageGetEPMax);
                    if (ep > 0)
                    {
                        effects = [.. enemy.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                        foreach (Effect effect in effects)
                        {
                            effect.AlterEPAfterGetDamage(enemy, ref ep);
                        }
                        enemy.EP += ep;
                    }

                    // 统计伤害
                    CalculateCharacterDamageStatistics(actor, enemy, damage, damageType, actualDamage);

                    // 计算助攻
                    if (actor != enemy && !IsTeammate(actor, enemy))
                    {
                        Character a = actor, e = enemy;
                        if (a.Master != null)
                        {
                            a = a.Master;
                        }
                        if (e.Master != null)
                        {
                            e = e.Master;
                        }
                        if (a != e)
                        {
                            _assistDetail[a][e, TotalTime] += damage;
                        }
                    }
                }
            }
            else
            {
                LastRound.IsEvaded[enemy] = true;
                actualDamage = 0;
            }

            OnDamageToEnemyEvent(actor, enemy, damage, actualDamage, isNormalAttack, damageType, magicType, damageResult);

            if (options.TriggerEffects)
            {
                effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                foreach (Effect effect in effects)
                {
                    effect.AfterDamageCalculation(actor, enemy, damage, actualDamage, isNormalAttack, damageType, magicType, damageResult);
                }
            }

            if (enemy.HP <= 0 && !_eliminated.Contains(enemy) && !_respawnCountdown.ContainsKey(enemy))
            {
                LastRound.HasKill = true;
                DeathCalculation(actor, enemy);
            }
        }

        /// <summary>
        /// 死亡结算
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="death"></param>
        public void DeathCalculation(Character killer, Character death)
        {
            DeathRelation dr = new(death, killer);
            _roundDeaths.Add(dr);

            if (killer == death)
            {
                if (killer.Master is null)
                {
                    _stats[death].Deaths += 1;
                }
                WriteLine($"[ {death} ] 自杀了！");
                DealWithCharacterDied(killer, death, []);
                return;
            }

            if (killer.Master != null)
            {
                killer = killer.Master;
            }

            if (IsTeammate(killer, death))
            {
                DeathCalculationByTeammate(killer, death);
                return;
            }

            if (death.Master != null)
            {
                DealWithCharacterDied(killer, death, []);
                return;
            }

            if (!_continuousKilling.TryAdd(killer, 1)) _continuousKilling[killer] += 1;
            if (!_maxContinuousKilling.TryAdd(killer, 1) && _continuousKilling[killer] > _maxContinuousKilling[killer])
            {
                _maxContinuousKilling[killer] = _continuousKilling[killer];
            }
            _stats[killer].Kills += 1;
            _stats[death].Deaths += 1;

            // 基础击杀奖励
            int money = 300;

            // 按伤害比分配金钱 只有在 30 时间内造成的伤害才能参与
            // 现在 20 时间内的非伤害类型辅助也能参与助攻了
            Character[] assists = [.. _assistDetail.Keys.Where(c => c != death &&
                ((TotalTime - _assistDetail[c].GetLastTime(death) <= 30) || (TotalTime - _assistDetail[c].GetNotDamageAssistLastTime(killer) <= 20)))];

            // 获取队友列表
            Character[] teammates = [.. GetTeammates(killer)];

            // 获取贡献百分比 以伤害为主，非伤害助攻贡献不足 10% 的按 10% 计算
            double minPercentage = 0.1;
            Dictionary<Character, double> assistPercentage = _assistDetail.Keys.Where(assists.Contains).ToDictionary(c => c,
                c => _assistDetail[c].GetPercentage(death) < minPercentage &&
                TotalTime - _assistDetail[c].GetNotDamageAssistLastTime(killer) <= 20 ? minPercentage : _assistDetail[c].GetPercentage(death));
            double totalDamagePercentage = assistPercentage.Values.Sum();
            if (totalDamagePercentage < 1)
            {
                // 归一化
                foreach (Character assist in assistPercentage.Keys)
                {
                    if (totalDamagePercentage == 0) break;
                    assistPercentage[assist] /= totalDamagePercentage;
                }
                totalDamagePercentage = assistPercentage.Values.Sum();
                if (totalDamagePercentage == 0) totalDamagePercentage = 1;
            }

            // 如果算上助攻者总伤害贡献超过了100%，则会超过基础击杀奖励。防止刷伤害要设置金钱上限
            int totalMoney = Math.Min(Convert.ToInt32(money * totalDamagePercentage), 425);

            // 等级差和经济差补偿 d = death, koa = killer or assist
            int calDiff(Character d, Character koa)
            {
                int moreMoney = 0;
                int levelDiff = d.Level - koa.Level;
                if (levelDiff > 0)
                {
                    moreMoney += levelDiff * 10;
                }
                if (_earnedMoney.TryGetValue(d, out int deathMoney))
                {
                    int moneyDiff = deathMoney;
                    if (_earnedMoney.TryGetValue(koa, out int killerMoney))
                    {
                        moneyDiff = deathMoney - killerMoney;
                    }
                    if (moneyDiff > 0)
                    {
                        moreMoney += (int)Math.Min(moneyDiff * 0.25, 300);
                    }
                }
                return moreMoney;
            }

            // 分配金钱和累计助攻
            foreach (Character assist in assistPercentage.Keys)
            {
                int cmoney = Convert.ToInt32(assistPercentage[assist] / totalDamagePercentage * totalMoney);
                if (cmoney > 320) cmoney = 320;
                if (assist != killer)
                {
                    // 助攻者的等级差和经济差补偿
                    cmoney += calDiff(death, assist);
                    if (!_earnedMoney.TryAdd(assist, cmoney)) _earnedMoney[assist] += cmoney;
                    assist.User.Inventory.Credits += cmoney;
                    if (teammates.Length == 0 || (teammates.Length > 0 && teammates.Contains(assist)))
                    {
                        _stats[assist].Assists += 1;
                    }
                    if (!LastRound.Assists.Contains(assist)) LastRound.Assists.Add(assist);
                }
                else
                {
                    money = cmoney;
                }
            }

            // 击杀者的等级差和经济差补偿
            money += calDiff(death, killer);

            // 终结击杀的奖励仍然是全额的
            if (_continuousKilling.TryGetValue(death, out int coefficient) && coefficient > 1)
            {
                money += (coefficient + 1) * 60;
                string termination = CharacterSet.GetContinuousKilling(coefficient);
                string msg = $"[ {killer} ] 终结了 [ {death} ]{(termination != "" ? " 的" + termination : "")}，获得 {money} {GameplayEquilibriumConstant.InGameCurrency}！";
                LastRound.DeathContinuousKilling.Add(msg);
                if (assists.Length > 1)
                {
                    msg += "助攻：[ " + string.Join(" ] / [ ", assists.Where(c => c != killer)) + " ]";
                }
                WriteLine(msg);
            }
            else
            {
                string msg = $"[ {killer} ] 杀死了 [ {death} ]，获得 {money} {GameplayEquilibriumConstant.InGameCurrency}！";
                LastRound.DeathContinuousKilling.Add(msg);
                if (assists.Length > 1)
                {
                    msg += "助攻：[ " + string.Join(" ] / [ ", assists.Where(c => c != killer)) + " ]";
                }
                WriteLine(msg);
            }
            dr.Assists = assists;

            if (FirstKiller is null)
            {
                FirstKiller = killer;
                _stats[killer].FirstKills += 1;
                _stats[death].FirstDeaths += 1;
                money += 200;
                string firstKill = $"[ {killer} ] 拿下了第一滴血！额外奖励 200 {GameplayEquilibriumConstant.InGameCurrency}！！";
                WriteLine(firstKill);
                LastRound.ActorContinuousKilling.Add(firstKill);
            }

            if (!_earnedMoney.TryAdd(killer, money)) _earnedMoney[killer] += money;
            killer.User.Inventory.Credits += money;

            int kills = _continuousKilling[killer];
            string continuousKilling = CharacterSet.GetContinuousKilling(kills);
            string actorContinuousKilling = "";
            if (kills == 2 || kills == 3)
            {
                actorContinuousKilling = "[ " + killer + " ] 完成了一次" + continuousKilling + "！";
            }
            else if (kills == 4)
            {
                actorContinuousKilling = "[ " + killer + " ] 正在" + continuousKilling + "！";
            }
            else if (kills > 4 && kills < 10)
            {
                actorContinuousKilling = "[ " + killer + " ] 已经" + continuousKilling + "！";
            }
            else if (kills >= 10)
            {
                actorContinuousKilling = "[ " + killer + " ] 已经" + continuousKilling + "，拜托谁去杀了他吧！！！";
            }
            if (actorContinuousKilling != "")
            {
                LastRound.ActorContinuousKilling.Add(actorContinuousKilling);
                WriteLine(actorContinuousKilling);
            }

            DealWithCharacterDied(killer, death, assists);
        }

        /// <summary>
        /// 死亡结算，击杀队友的情况
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="death"></param>
        /// <returns></returns>
        public void DeathCalculationByTeammate(Character killer, Character death)
        {
            if (!OnDeathCalculationByTeammateEvent(killer, death))
            {
                return;
            }

            if (death.Master is null)
            {
                _stats[death].Deaths += 1;
                string msg = $"[ {killer} ] 反补了 [ {death} ]！[ {killer} ] 受到了击杀队友惩罚，扣除 200 {GameplayEquilibriumConstant.InGameCurrency}并且当前连杀计数减少一次！！";
                if (!_earnedMoney.TryAdd(killer, -200))
                {
                    _earnedMoney[killer] -= 200;
                }
                if (_continuousKilling.TryGetValue(killer, out int times) && times > 0)
                {
                    _continuousKilling[killer] -= 1;
                }
                LastRound.DeathContinuousKilling.Add(msg);
                WriteLine(msg);
            }

            DealWithCharacterDied(killer, death, []);
        }

        /// <summary>
        /// 治疗一个目标
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="target"></param>
        /// <param name="heal"></param>
        /// <param name="canRespawn"></param>
        /// <param name="triggerEffects"></param>
        /// <param name="skill"></param>
        public void HealToTarget(Character actor, Character target, double heal, bool canRespawn = false, bool triggerEffects = true, Skill? skill = null)
        {
            // 死人怎么能对自己治疗呢？
            if (actor.HP <= 0)
            {
                return;
            }

            if (target.HP == target.MaxHP)
            {
                return;
            }

            bool allowHealing = true;
            List<Effect> effects = [.. actor.Effects.Union(target.Effects).Distinct().Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
            foreach (Effect effect in effects)
            {
                if (!effect.BeforeHealToTarget(actor, target, heal, canRespawn))
                {
                    allowHealing = false;
                }
            }

            if (!allowHealing)
            {
                return;
            }

            bool isDead = target.HP <= 0;
            List<string> healStrings = [];
            healStrings.Add($"{heal:0.##}（{(skill is null ? $"生命偷取：{actor.Lifesteal * 100:0.##}%" : skill.Name)}）");

            if (triggerEffects)
            {
                Dictionary<Effect, double> totalHealBonus = [];
                effects = [.. actor.Effects.Union(target.Effects).Distinct().Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                foreach (Effect effect in effects)
                {
                    bool changeCanRespawn = false;
                    double healBonus = effect.AlterHealValueBeforeHealToTarget(actor, target, heal, ref changeCanRespawn, totalHealBonus);
                    if (changeCanRespawn && !canRespawn)
                    {
                        canRespawn = true;
                    }
                    if (healBonus != 0)
                    {
                        totalHealBonus[effect]= healBonus;
                        healStrings.Add($"{(healBonus > 0 ? " + " : " - ")}{Math.Abs(healBonus):0.##}（{effect.Name}）");
                    }
                }
                heal += totalHealBonus.Sum(kv => kv.Value);
            }

            if (skill != null && skill.MagicBottleneck > 0)
            {
                double efficacyHeal = heal * (skill.MagicEfficacy - 1);
                heal *= skill.MagicEfficacy;
                healStrings.Add($"{(efficacyHeal >= 0 ? " + " : " - ")}{Math.Abs(efficacyHeal):0.##}（魔法效能：{skill.MagicEfficacy * 100:0.##}%）");
            }

            healStrings.Add($" = {heal:0.##} 点生命值");
            string healString = "";
            if (!IsDebug) healStrings.Clear();
            else healString = $"【{string.Join("", healStrings)}】";

            if (target.HP > 0 || (isDead && canRespawn))
            {
                // 用于数据统计，不能是全额，溢出的部分需要扣除
                if (target.HP + heal > target.MaxHP)
                {
                    heal = target.MaxHP - target.HP;
                }
                target.HP += heal;
                if (!LastRound.Heals.TryAdd(target, heal))
                {
                    LastRound.Heals[target] += heal;
                }
            }

            if (heal <= 0)
            {
                return;
            }

            bool isRespawn = isDead && canRespawn;
            if (isRespawn)
            {
                if (target != actor)
                {
                    WriteLine($"[ {target} ] 被 [ {actor} ] 复苏了，并回复了 {heal:0.##} 点生命值！！{healString}");
                }
                else
                {
                    WriteLine($"[ {target} ] 复苏了，并回复了 {heal:0.##} 点生命值！！{healString}");
                }
                double hp = target.HP;
                double mp = target.MP;
                SetCharacterRespawn(target);
                target.HP = hp;
                target.MP = mp;
            }
            else
            {
                WriteLine($"[ {target} ] 回复了 {heal:0.##} 点生命值！{healString}");
            }

            // 添加助攻
            SetNotDamageAssistTime(actor, target);

            // 统计数据
            Character statsCharacter = actor;
            if (statsCharacter.Master != null)
            {
                statsCharacter = statsCharacter.Master;
            }
            if (_stats.TryGetValue(statsCharacter, out CharacterStatistics? stats) && stats != null)
            {
                stats.TotalHeal += heal;
            }

            OnHealToTargetEvent(actor, target, heal, isRespawn);
        }

        #endregion

        #region 回合内-辅助方法

        /// <summary>
        /// 将角色装备栏中的主动技能加入列表
        /// </summary>
        /// <param name="character"></param>
        /// <param name="list"></param>
        public static void AddCharacterEquipSlotSkills(Character character, List<Skill> list)
        {
            if (character.EquipSlot.MagicCardPack?.Skills.Active != null) list.Add(character.EquipSlot.MagicCardPack.Skills.Active);
            if (character.EquipSlot.Weapon?.Skills.Active != null) list.Add(character.EquipSlot.Weapon.Skills.Active);
            if (character.EquipSlot.Armor?.Skills.Active != null) list.Add(character.EquipSlot.Armor.Skills.Active);
            if (character.EquipSlot.Shoes?.Skills.Active != null) list.Add(character.EquipSlot.Shoes.Skills.Active);
            if (character.EquipSlot.Accessory1?.Skills.Active != null) list.Add(character.EquipSlot.Accessory1.Skills.Active);
            if (character.EquipSlot.Accessory2?.Skills.Active != null) list.Add(character.EquipSlot.Accessory2.Skills.Active);
        }

        /// <summary>
        /// 取得回合开始时必需的列表
        /// </summary>
        /// <returns></returns>
        public (List<Character>, List<Character>, List<Skill>, List<Item>) GetTurnStartNeedyList(Character character, List<Character> allTeammates, List<Character> allEnemys)
        {
            // 可选队友列表
            List<Character> selectableTeammates = [.. allTeammates.Where(_queue.Contains)];

            // 可选敌人列表
            List<Character> selectableEnemys = [.. allEnemys.Where(c => _queue.Contains(c) && !c.IsUnselectable)];

            // 技能列表
            List<Skill> skills = [.. character.Skills];

            // 将角色装备栏中的主动技能加入技能列表以供筛选
            AddCharacterEquipSlotSkills(character, skills);
            skills = [.. skills.Where(s => s.Level > 0 && s.SkillType != SkillType.Passive && s.Enable && !s.IsInEffect && s.CurrentCD == 0 &&
                ((s.SkillType == SkillType.SuperSkill || s.SkillType == SkillType.Skill) && s.RealEPCost <= character.EP || s.SkillType == SkillType.Magic && s.RealMPCost <= character.MP))];

            // 物品列表
            List<Item> items = [.. character.Items.Where(i => i.IsActive && i.Skills.Active != null && i.Enable && i.IsInGameItem &&
                i.Skills.Active.SkillType == SkillType.Item && i.Skills.Active.Enable && !i.Skills.Active.IsInEffect && i.Skills.Active.CurrentCD == 0 && i.Skills.Active.RealMPCost <= character.MP && i.Skills.Active.RealEPCost <= character.EP)];

            return (selectableTeammates, selectableEnemys, skills, items);
        }

        /// <summary>
        /// 同时考虑指向性和非指向性技能的目标选取方法
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="castRange"></param>
        /// <param name="allEnemys"></param>
        /// <param name="allTeammates"></param>
        /// <param name="aiDecision"></param>
        /// <returns></returns>
        public (List<Character>, List<Grid>) GetSelectedSkillTargetsList(Character character, Skill skill, List<Character> enemys, List<Character> teammates, List<Grid> castRange, List<Character> allEnemys, List<Character> allTeammates, AIDecision? aiDecision)
        {
            List<Character> targets = [];
            List<Grid> grids = [];
            // 对于非战棋模式，我们会把它退回到指向性目标选择器
            if (skill.IsNonDirectional && _map != null)
            {
                if (aiDecision != null) grids = aiDecision.TargetGrids;
                if (grids.Count == 0)
                {
                    grids = SelectNonDirectionalSkillTargetGrid(character, skill, enemys, teammates, castRange);
                }
                if (grids.Count > 0)
                {
                    targets = skill.SelectTargetsByRange(character, allEnemys, allTeammates, targets, grids);
                }
            }
            else
            {
                if (aiDecision != null) targets = aiDecision.Targets;
                if (targets.Count == 0)
                {
                    targets = SelectTargets(character, skill, enemys, teammates, castRange);
                }
                if (skill.CanSelectTargetRange > 0)
                {
                    // 扩散目标
                    targets = skill.SelectTargetsByCanSelectTargetRange(character, allEnemys, allTeammates, targets);
                }
            }
            return (targets, grids);
        }

        /// <summary>
        /// 需要处理复活和解除施法等
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="death"></param>
        /// <param name="assists"></param>
        /// <returns></returns>
        public void DealWithCharacterDied(Character killer, Character death, Character[] assists)
        {
            // 给所有角色的特效广播角色死亡结算
            List<Effect> effects = [.. _queue.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Union(killer.Effects).Distinct().OrderByDescending(e => e.Priority)];
            foreach (Effect effect in effects)
            {
                effect.AfterDeathCalculation(death, death.Master != null, killer, _continuousKilling, _earnedMoney, assists);
            }
            // 将死者移出队列
            _queue.Remove(death);

            if (!OnCharacterDeathEvent(death, killer, assists))
            {
                return;
            }

            OnDeathCalculation(death, killer);

            death.EP = 0;

            if (death.Master is null)
            {
                // 清除对死者的助攻数据
                List<AssistDetail> ads = [.. _assistDetail.Values.Where(ad => ad.Character != death)];
                foreach (AssistDetail ad in ads)
                {
                    ad[death, 0] = 0;
                }

                _continuousKilling.Remove(death);
                _eliminated.Add(death);
                if (MaxRespawnTimes == 0)
                {
                    // do nothing
                }
                else if (_respawnTimes.TryGetValue(death, out int times) && MaxRespawnTimes != -1 && times == MaxRespawnTimes)
                {
                    WriteLine($"[ {death} ] 已达到复活次数上限，将不能再复活！！");
                }
                else
                {
                    // 进入复活倒计时
                    double respawnTime = GetRespawnTime(death, times);
                    _respawnCountdown.TryAdd(death, respawnTime);
                    LastRound.RespawnCountdowns.TryAdd(death, respawnTime);
                    WriteLine($"[ {death} ] 进入复活倒计时：{respawnTime:0.##} {GameplayEquilibriumConstant.InGameTime}！");
                }
            }

            // 移除死者的施法
            _castingSkills.Remove(death);
            _castingSuperSkills.Remove(death);

            // 因丢失目标而中断施法
            List<Character> castingSkills = [.. _castingSkills.Keys];
            foreach (Character caster in castingSkills)
            {
                SkillTarget st = _castingSkills[caster];
                if (st.Targets.Remove(death) && st.Targets.Count == 0)
                {
                    _castingSkills.Remove(caster);
                    if (caster.CharacterState == CharacterState.Casting)
                    {
                        caster.CharacterState = CharacterState.Actionable;
                    }
                    WriteLine($"[ {caster} ] 终止了 [ {st.Skill.Name} ] 的施法" + (_hardnessTimes[caster] > 3 && _isInRound ? $"，并获得了 3 {GameplayEquilibriumConstant.InGameTime}的硬直时间的补偿。" : "。"));
                    if (_hardnessTimes[caster] > 3 && _isInRound)
                    {
                        AddCharacter(caster, 3, false);
                    }
                }
            }
        }

        /// <summary>
        /// 使用物品实际逻辑
        /// </summary>
        /// <param name="item"></param>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="castRange"></param>
        /// <param name="allEnemys"></param>
        /// <param name="allTeammates"></param>
        /// <param name="aiDecision"></param>
        /// <returns></returns>
        public bool UseItem(Item item, Character character, DecisionPoints dp, List<Character> enemys, List<Character> teammates, List<Grid> castRange, List<Character> allEnemys, List<Character> allTeammates, AIDecision? aiDecision = null)
        {
            if (CheckCanCast(character, item, out double costMP, out double costEP))
            {
                Skill? skill = item.Skills.Active;
                if (skill != null)
                {
                    skill.GamingQueue = this;
                    (List<Character> targets, List<Grid> grids) = GetSelectedSkillTargetsList(character, skill, enemys, teammates, castRange, allEnemys, allTeammates, aiDecision);

                    if (targets.Count > 0)
                    {
                        // 免疫检定
                        CheckSkilledImmune(character, targets, skill, item);
                    }
                    if (targets.Count > 0 && CheckCanCast(character, skill, out double cost))
                    {
                        LastRound.Targets[CharacterActionType.UseItem] = [.. targets];

                        WriteLine($"[ {character} ] 使用了物品 [ {item.Name} ]！");
                        item.ReduceTimesAndRemove();
                        if (item.IsReduceTimesAfterUse && item.RemainUseTimes == 0)
                        {
                            character.Items.Remove(item);
                        }
                        OnCharacterUseItemEvent(character, dp, item, targets);

                        skill.OnSkillCasting(this, character, targets, grids);

                        Effect[] effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                        foreach (Effect effect in effects)
                        {
                            effect.AfterCharacterStartCasting(character, skill, targets);
                        }

                        skill.BeforeSkillCasted();

                        skill.CurrentCD = skill.RealCD;
                        skill.Enable = false;

                        string line = $"[ {character} ] ";
                        if (costMP > 0)
                        {
                            character.MP -= costMP;
                            LastRound.ItemsCost[item] = $"{-costMP:0.##} MP";
                            line += $"消耗了 {costMP:0.##} 点魔法值，";
                        }

                        if (costEP > 0)
                        {
                            character.EP -= costEP;
                            if (LastRound.ItemsCost[item] != "") LastRound.ItemsCost[item] += " / ";
                            LastRound.ItemsCost[item] += $"{-costEP:0.##} EP";
                            line += $"消耗了 {costEP:0.##} 点能量，";
                        }

                        line += $"释放了物品技能 [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}";
                        WriteLine(line);

                        SkillTarget skillTarget = new(skill, targets, grids);
                        OnCharacterCastItemSkillEvent(character, dp, item, skillTarget, costMP, costEP);

                        skill.OnSkillCasted(this, character, targets, grids);

                        effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                        foreach (Effect effect in effects)
                        {
                            effect.AfterCharacterCastSkill(character, skill, targets);
                        }

                        effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                        foreach (Effect effect in effects)
                        {
                            effect.AfterCharacterUseItem(character, item, skill, targets);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 角色移动实际逻辑
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        /// <param name="target"></param>
        /// <param name="startGrid"></param>
        /// <returns></returns>
        public bool CharacterMove(Character character, DecisionPoints dp, Grid target, Grid? startGrid)
        {
            if (target.Id != -1)
            {
                int steps = _map?.CharacterMove(character, startGrid, target) ?? -1;
                if (steps > 0)
                {
                    WriteLine($"[ {character} ] 移动了 {steps} 步！");
                    OnCharacterMoveEvent(character, dp, target);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 通过概率计算角色要干嘛
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="pUseItem"></param>
        /// <param name="pCastSkill"></param>
        /// <param name="pNormalAttack"></param>
        /// <returns></returns>
        public static CharacterActionType GetActionType(DecisionPoints dp, double pUseItem, double pCastSkill, double pNormalAttack)
        {
            if (!dp.CheckActionTypeQuota(CharacterActionType.NormalAttack) || dp.CurrentDecisionPoints < dp.GameplayEquilibriumConstant.DecisionPointsCostNormalAttack)
            {
                pNormalAttack = 0;
            }

            if (!dp.CheckActionTypeQuota(CharacterActionType.UseItem) || dp.CurrentDecisionPoints < dp.GameplayEquilibriumConstant.DecisionPointsCostItem)
            {
                pUseItem = 0;
            }

            if (dp.CurrentDecisionPoints < dp.GameplayEquilibriumConstant.DecisionPointsCostSkill &&
                dp.CurrentDecisionPoints < dp.GameplayEquilibriumConstant.DecisionPointsCostSuperSkill &&
                dp.CurrentDecisionPoints < dp.GameplayEquilibriumConstant.DecisionPointsCostMagic)
            {
                pCastSkill = 0;
            }

            if (pUseItem == 0 && pCastSkill == 0 && pNormalAttack == 0)
            {
                return CharacterActionType.EndTurn;
            }

            double total = pUseItem + pCastSkill + pNormalAttack;

            // 浮点数比较时处理误差
            if (Math.Abs(total - 1) > 1e-6)
            {
                pUseItem /= total;
                pCastSkill /= total;
                pNormalAttack /= total;
            }

            double rand = Random.Shared.NextDouble();

            // 按概率进行检查
            if (rand < pUseItem)
            {
                return CharacterActionType.UseItem;
            }

            if (rand < pUseItem + pCastSkill)
            {
                return CharacterActionType.PreCastSkill;
            }

            if (rand < pUseItem + pCastSkill + pNormalAttack)
            {
                return CharacterActionType.NormalAttack;
            }

            return CharacterActionType.EndTurn;
        }

        /// <summary>
        /// 选取移动目标
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="map"></param>
        /// <param name="moveRange"></param>
        /// <returns></returns>
        public Grid SelectTargetGrid(Character character, List<Character> enemys, List<Character> teammates, GameMap map, List<Grid> moveRange)
        {
            List<Effect> effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
            foreach (Effect effect in effects)
            {
                effect.BeforeSelectTargetGrid(character, enemys, teammates, map, moveRange);
            }
            Grid target = OnSelectTargetGridEvent(character, enemys, teammates, map, moveRange);
            if (target.Id != -1)
            {
                return target;
            }
            else if (target.Id == -2 && map.Characters.TryGetValue(character, out Grid? current) && current != null)
            {
                if (moveRange.Count > 0)
                {
                    return moveRange[Random.Shared.Next(moveRange.Count)];
                }
            }
            return Grid.Empty;
        }

        /// <summary>
        /// 选取技能目标
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="castRange"></param>
        /// <returns></returns>
        public List<Character> SelectTargets(Character caster, Skill skill, List<Character> enemys, List<Character> teammates, List<Grid> castRange)
        {
            List<Effect> effects = [.. caster.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
            foreach (Effect effect in effects)
            {
                effect.AlterSelectListBeforeSelection(caster, skill, enemys, teammates);
            }
            List<Character> targets = OnSelectSkillTargetsEvent(caster, skill, enemys, teammates, castRange);
            if (targets.Count == 0 && IsCharacterInAIControlling(caster))
            {
                targets = skill.SelectTargets(caster, enemys, teammates);
            }
            return targets;
        }

        /// <summary>
        /// 选取非指向性技能目标
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="castRange"></param>
        /// <returns></returns>
        public List<Grid> SelectNonDirectionalSkillTargetGrid(Character caster, Skill skill, List<Character> enemys, List<Character> teammates, List<Grid> castRange)
        {
            List<Grid> targets = OnSelectNonDirectionalSkillTargetsEvent(caster, skill, enemys, teammates, castRange);
            if (targets.Count == 0 && IsCharacterInAIControlling(caster) && castRange.Count > 0)
            {
                targets = skill.SelectNonDirectionalTargets(caster, castRange.OrderBy(r => Random.Shared.Next()).FirstOrDefault(r => r.Characters.Count > 0) ?? castRange.First(), skill.SelectIncludeCharacterGrid);
            }
            return targets;
        }

        /// <summary>
        /// 选取普通攻击目标
        /// </summary>
        /// <param name="character"></param>
        /// <param name="attack"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="attackRange"></param>
        /// <returns></returns>
        public List<Character> SelectTargets(Character character, NormalAttack attack, List<Character> enemys, List<Character> teammates, List<Grid> attackRange)
        {
            List<Effect> effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
            foreach (Effect effect in effects)
            {
                effect.AlterSelectListBeforeSelection(character, attack, enemys, teammates);
            }
            List<Character> targets = OnSelectNormalAttackTargetsEvent(character, attack, enemys, teammates, attackRange);
            if (targets.Count == 0 && IsCharacterInAIControlling(character))
            {
                targets = character.NormalAttack.SelectTargets(character, enemys, teammates);
            }
            return targets;
        }

        /// <summary>
        /// 检查是否可以释放技能
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        public bool CheckCanCast(Character caster, Skill skill, out double cost)
        {
            if (skill.SkillType == SkillType.Magic)
            {
                cost = skill.RealMPCost;
                if (cost >= 0 && cost <= caster.MP)
                {
                    return true;
                }
                else
                {
                    WriteLine("[ " + caster + $" ] 魔法不足！");
                }
            }
            else
            {
                cost = skill.RealEPCost;
                if (cost >= 0 && cost <= caster.EP)
                {
                    return true;
                }
                else
                {
                    WriteLine("[ " + caster + $" ] 能量不足！");
                }
            }
            return false;
        }

        /// <summary>
        /// 检查是否可以释放技能（物品版）
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="item"></param>
        /// <param name="costMP"></param>
        /// <param name="costEP"></param>
        /// <returns></returns>
        public bool CheckCanCast(Character caster, Item item, out double costMP, out double costEP)
        {
            Skill? skill = item.Skills.Active;
            if (skill is null)
            {
                costMP = 0;
                costEP = 0;
                return false;
            }
            costMP = skill.RealMPCost;
            costEP = skill.RealEPCost;
            bool isMPOk = false;
            bool isEPOk = false;
            if (costMP >= 0 && costMP <= caster.MP)
            {
                isMPOk = true;
            }
            else
            {
                WriteLine("[ " + caster + $" ] 魔法不足！");
            }
            costEP = skill.RealEPCost;
            if (costEP >= 0 && costEP <= caster.EP)
            {
                isEPOk = true;
            }
            else
            {
                WriteLine("[ " + caster + $" ] 能量不足！");
            }
            return isMPOk && isEPOk;
        }

        /// <summary>
        /// 计算物理伤害
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="expectedDamage"></param>
        /// <param name="finalDamage"></param>
        /// <param name="changeCount"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public DamageResult CalculatePhysicalDamage(Character actor, Character enemy, bool isNormalAttack, double expectedDamage, out double finalDamage, ref int changeCount, ref DamageCalculationOptions? options)
        {
            options ??= new(actor);
            if (options.ExpectedDamage == 0) options.ExpectedDamage = expectedDamage;
            List<Character> characters = [actor, enemy];
            DamageType damageType = DamageType.Physical;
            MagicType magicType = MagicType.None;
            List<Effect> effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
            Dictionary<Effect, double> totalDamageBonus = [];
            if (options.TriggerEffects)
            {
                if (changeCount < 3)
                {
                    foreach (Effect effect in effects)
                    {
                        effect.AlterDamageTypeBeforeCalculation(actor, enemy, ref isNormalAttack, ref damageType, ref magicType);
                    }
                    if (damageType == DamageType.Magical)
                    {
                        changeCount++;
                        return CalculateMagicalDamage(actor, enemy, isNormalAttack, magicType, expectedDamage, out finalDamage, ref changeCount, ref options);
                    }
                }

                effects = [.. actor.Effects.Union(enemy.Effects).Distinct().Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                foreach (Effect effect in effects)
                {
                    double damageBonus = effect.AlterExpectedDamageBeforeCalculation(actor, enemy, expectedDamage, isNormalAttack, DamageType.Physical, MagicType.None, totalDamageBonus);
                    if (damageBonus != 0) totalDamageBonus[effect] = damageBonus;
                }
                expectedDamage += totalDamageBonus.Sum(kv => kv.Value);
            }
            options.BeforeDamageBonus = totalDamageBonus;

            double dice = Random.Shared.NextDouble();
            double throwingBonus = 0;
            bool checkEvade = true;
            bool checkCritical = true;
            if (isNormalAttack && options.CalculateEvade)
            {
                effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                foreach (Effect effect in effects)
                {
                    if (!effect.BeforeEvadeCheck(actor, enemy, ref throwingBonus))
                    {
                        checkEvade = false;
                    }
                }

                if (checkEvade)
                {
                    // 闪避检定
                    if (dice < (enemy.EvadeRate + throwingBonus))
                    {
                        bool isAlterEvaded = false;
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                        foreach (Effect effect in effects)
                        {
                            if (effect.OnEvadedTriggered(actor, enemy, dice))
                            {
                                isAlterEvaded = true;
                            }
                        }
                        if (!isAlterEvaded)
                        {
                            finalDamage = 0;
                            WriteLine("此物理攻击被完美闪避了！");
                            return DamageResult.Evaded;
                        }
                    }
                }
            }

            // 物理穿透后的护甲
            double penetratedDEF = 0;
            // 物理伤害减免
            double physicalDamageReduction = 0;
            // 最终的物理伤害
            finalDamage = expectedDamage;

            if (options.CalculateReduction)
            {
                penetratedDEF = (1 - actor.PhysicalPenetration) * enemy.DEF;
                physicalDamageReduction = penetratedDEF / (penetratedDEF + GameplayEquilibriumConstant.DEFReductionFactor);
                options.DefenseReduction = expectedDamage * Calculation.PercentageCheck(physicalDamageReduction + enemy.ExPDR);
                finalDamage = expectedDamage - options.DefenseReduction;
            }

            if (options.CalculateCritical)
            {
                // 暴击检定
                effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                foreach (Effect effect in effects)
                {
                    if (!effect.BeforeCriticalCheck(actor, enemy, isNormalAttack, ref throwingBonus))
                    {
                        checkCritical = false;
                    }
                }

                if (checkCritical)
                {
                    dice = Random.Shared.NextDouble();
                    if (dice < (actor.CritRate + throwingBonus))
                    {
                        options.CriticalDamage = finalDamage * (actor.CritDMG - 1);
                        finalDamage *= actor.CritDMG; // 暴击伤害倍率加成
                        WriteLine("暴击生效！！");
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                        foreach (Effect effect in effects)
                        {
                            effect.OnCriticalDamageTriggered(actor, enemy, dice);
                        }
                        return DamageResult.Critical;
                    }
                }
            }

            // 是否有效伤害
            return DamageResult.Normal;
        }

        /// <summary>
        /// 计算魔法伤害
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="magicType"></param>
        /// <param name="expectedDamage"></param>
        /// <param name="finalDamage"></param>
        /// <param name="changeCount"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public DamageResult CalculateMagicalDamage(Character actor, Character enemy, bool isNormalAttack, MagicType magicType, double expectedDamage, out double finalDamage, ref int changeCount, ref DamageCalculationOptions? options)
        {
            options ??= new(actor);
            if (options.ExpectedDamage == 0) options.ExpectedDamage = expectedDamage;
            List<Character> characters = [actor, enemy];
            DamageType damageType = DamageType.Magical;
            List<Effect> effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
            Dictionary<Effect, double> totalDamageBonus = [];
            if (options.TriggerEffects)
            {
                if (changeCount < 3)
                {
                    foreach (Effect effect in effects)
                    {
                        effect.AlterDamageTypeBeforeCalculation(actor, enemy, ref isNormalAttack, ref damageType, ref magicType);
                    }
                    if (damageType == DamageType.Physical)
                    {
                        changeCount++;
                        return CalculatePhysicalDamage(actor, enemy, isNormalAttack, expectedDamage, out finalDamage, ref changeCount, ref options);
                    }
                }

                effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                foreach (Effect effect in effects)
                {
                    double damageBonus = effect.AlterExpectedDamageBeforeCalculation(actor, enemy, expectedDamage, isNormalAttack, DamageType.Magical, magicType, totalDamageBonus);
                    if (damageBonus != 0) totalDamageBonus[effect] = damageBonus;
                }
                expectedDamage += totalDamageBonus.Sum(kv => kv.Value);
            }
            options.BeforeDamageBonus = totalDamageBonus;

            double dice = Random.Shared.NextDouble();
            double throwingBonus = 0;
            bool checkEvade = true;
            bool checkCritical = true;
            if (isNormalAttack && options.CalculateEvade)
            {
                effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                foreach (Effect effect in effects)
                {
                    if (!effect.BeforeEvadeCheck(actor, enemy, ref throwingBonus))
                    {
                        checkEvade = false;
                    }
                }

                if (checkEvade)
                {
                    // 闪避检定
                    if (dice < (enemy.EvadeRate + throwingBonus))
                    {
                        bool isAlterEvaded = false;
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                        foreach (Effect effect in effects)
                        {
                            if (effect.OnEvadedTriggered(actor, enemy, dice))
                            {
                                isAlterEvaded = true;
                            }
                        }
                        if (!isAlterEvaded)
                        {
                            finalDamage = 0;
                            WriteLine("此魔法攻击被完美闪避了！");
                            return DamageResult.Evaded;
                        }
                    }
                }
            }

            double MDF = enemy.MDF[magicType];
            finalDamage = 0;

            if (options.CalculateReduction)
            {
                // 魔法穿透后的魔法抗性
                MDF = (1 - actor.MagicalPenetration) * MDF;

                // 魔法抗性减伤
                options.DefenseReduction = expectedDamage * MDF;

                // 最终的魔法伤害
                finalDamage = expectedDamage - options.DefenseReduction;
            }

            if (options.CalculateCritical)
            {
                // 暴击检定
                effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                foreach (Effect effect in effects)
                {
                    if (!effect.BeforeCriticalCheck(actor, enemy, isNormalAttack, ref throwingBonus))
                    {
                        checkCritical = false;
                    }
                }

                if (checkCritical)
                {
                    dice = Random.Shared.NextDouble();
                    if (dice < (actor.CritRate + throwingBonus))
                    {
                        options.CriticalDamage = finalDamage * (actor.CritDMG - 1);
                        finalDamage *= actor.CritDMG; // 暴击伤害倍率加成
                        WriteLine("暴击生效！！");
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                        foreach (Effect effect in effects)
                        {
                            effect.OnCriticalDamageTriggered(actor, enemy, dice);
                        }
                        return DamageResult.Critical;
                    }
                }
            }

            // 是否有效伤害
            return DamageResult.Normal;
        }

        /// <summary>
        /// 获取EP
        /// </summary>
        /// <param name="a">参数1</param>
        /// <param name="b">参数2</param>
        /// <param name="max">最大获取量</param>
        public static double GetEP(double a, double b, double max)
        {
            return Math.Min((a + Random.Shared.Next(30)) * b, max);
        }

        /// <summary>
        /// 获取某角色的敌对角色
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public List<Character> GetEnemies(Character character)
        {
            List<Character> teammates = GetTeammates(character);
            return [.. _allCharacters.Union(_queue).Distinct().Where(c => c != character && !teammates.Contains(c) && !_eliminated.Contains(c) && c.Master != character && character.Master != c)];
        }

        /// <summary>
        /// 获取某角色的团队成员
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public virtual List<Character> GetTeammates(Character character)
        {
            return [.. _queue.Where(c => c != character && (character.Master == c || c == character.Master || (c.Master != null && character.Master != null && c.Master == character.Master)))];
        }

        /// <summary>
        /// 判断目标对于某个角色是否是队友（不包括自己）
        /// </summary>
        /// <param name="character"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsTeammate(Character character, Character target)
        {
            List<Character> teammates = GetTeammates(character);
            return teammates.Contains(target) || character.Master == target || target == character.Master || (target.Master != null && character.Master != null && target.Master == character.Master);
        }

        /// <summary>
        /// 获取目标对于某个角色是否是友方的字典（包括自己）
        /// </summary>
        /// <param name="character"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public Dictionary<Character, bool> GetIsTeammateDictionary(Character character, params IEnumerable<Character> targets)
        {
            Dictionary<Character, bool> dict = [];
            List<Character> teammates = GetTeammates(character);
            foreach (Character target in targets)
            {
                if (character == target) dict[target] = true;
                else dict[target] = teammates.Contains(target);
            }
            return dict;
        }

        /// <summary>
        /// 对角色设置仅移动的硬直时间
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        /// <param name="baseTime"></param>
        public void SetOnlyMoveHardnessTime(Character character, DecisionPoints dp, ref double baseTime)
        {
            if (dp.ActionsTaken > 0) return;

            baseTime += 3;
            if (character.CharacterState == CharacterState.NotActionable ||
                character.CharacterState == CharacterState.ActionRestricted ||
                character.CharacterState == CharacterState.BattleRestricted)
            {
                baseTime += 2;
                WriteLine($"[ {character} ] {CharacterSet.GetCharacterState(character.CharacterState)}，放弃行动将额外获得 3 {GameplayEquilibriumConstant.InGameTime}硬直时间！");
            }
        }

        /// <summary>
        /// 决策点补充
        /// </summary>
        public DecisionPoints DecisionPointsRecovery(Character character)
        {
            DecisionPoints dp;
            if (!_decisionPoints.TryGetValue(character, out DecisionPoints? value) || value is null)
            {
                value = new();
                _decisionPoints[character] = value;
            }
            dp = value;

            // 吟唱态不做处理
            if (character.CharacterState == CharacterState.Casting || character.CharacterState == CharacterState.PreCastSuperSkill)
            {
                return dp;
            }

            // 清空上回合的记录
            dp.CourageCommandSkill = false;
            dp.ActionsHardnessTime.Clear();
            dp.ActionTypes.Clear();
            dp.ActionsTaken = 0;
            dp.DecisionPointsCost = 0;

            // 根据角色状态补充决策点
            int pointsToAdd;

            // 每回合提升决策点上限
            if (dp.MaxDecisionPoints < dp.GameplayEquilibriumConstant.MaxDecisionPoints)
            {
                dp.MaxDecisionPoints++;
            }
            else if (dp.MaxDecisionPoints > dp.GameplayEquilibriumConstant.MaxDecisionPoints)
            {
                dp.MaxDecisionPoints = dp.GameplayEquilibriumConstant.MaxDecisionPoints;
            }

            if (character.CharacterState == CharacterState.NotActionable || character.CharacterState == CharacterState.ActionRestricted)
            {
                // 完全行动不能/行动受限：补充上限1/4
                pointsToAdd = Math.Max(1, dp.MaxDecisionPoints / 4);
                dp.CurrentDecisionPoints = Math.Min(dp.CurrentDecisionPoints + pointsToAdd, dp.MaxDecisionPoints);
            }
            else
            {
                // 正常状态：补充上限一半
                pointsToAdd = Math.Max(1, dp.MaxDecisionPoints / 2);
                dp.CurrentDecisionPoints = Math.Min(dp.CurrentDecisionPoints + pointsToAdd, dp.MaxDecisionPoints);
            }

            dp.DecisionPointsRecovery = pointsToAdd;

            if (IsDebug) WriteLine($"[ {character} ] 回合开始，补充 {pointsToAdd} 决策点，当前 {dp.CurrentDecisionPoints}/{dp.MaxDecisionPoints} 决策点。");
            return dp;
        }

        #endregion

        #region 回合奖励

        /// <summary>
        /// 初始化回合奖励
        /// </summary>
        /// <param name="maxRound">最大回合数</param>
        /// <param name="maxRewardsInRound">每个奖励回合生成多少技能</param>
        /// <param name="effects">key: 特效的数字标识符；value: 是否是主动技能的特效</param>
        /// <param name="factoryEffects">通过数字标识符来获取构造特效的参数</param>
        /// <returns></returns>
        public void InitRoundRewards(int maxRound, int maxRewardsInRound, Dictionary<long, bool> effects, Func<long, Dictionary<string, object>>? factoryEffects = null)
        {
            _roundRewards.Clear();
            int currentRound = 1;
            long[] effectIDs = [.. effects.Keys];
            while (currentRound <= maxRound)
            {
                currentRound += Random.Shared.Next(1, 9);

                if (currentRound <= maxRound)
                {
                    List<Skill> skills = [];
                    if (maxRewardsInRound <= 0) maxRewardsInRound = 1;

                    do
                    {
                        long effectID = effectIDs[Random.Shared.Next(effects.Count)];
                        Dictionary<string, object> args = [];
                        if (effects[effectID])
                        {
                            args.Add("active", true);
                            args.Add("self", true);
                            args.Add("enemy", false);
                        }
                        Skill skill = Factory.OpenFactory.GetInstance<Skill>(effectID, "", args);
                        Dictionary<string, object> effectArgs = factoryEffects != null ? factoryEffects(effectID) : [];
                        args.Clear();
                        args.Add("skill", skill);
                        args.Add("values", effectArgs);
                        Effect effect = Factory.OpenFactory.GetInstance<Effect>(effectID, "", args);
                        skill.Effects.Add(effect);
                        skill.Name = $"[R] {effect.Name}";
                        skills.Add(skill);
                    }
                    while (skills.Count < maxRewardsInRound);

                    _roundRewards[currentRound] = skills;
                }
            }
        }

        /// <summary>
        /// 获取回合奖励
        /// </summary>
        /// <param name="round"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        protected List<Skill> GetRoundRewards(int round, Character character)
        {
            if (_roundRewards.TryGetValue(round, out List<Skill>? value) && value is List<Skill> list && list.Count > 0)
            {
                foreach (Skill skill in list)
                {
                    skill.GamingQueue = this;
                    skill.Character = character;
                    skill.Level = 1;
                    LastRound.RoundRewards.Add(skill);
                    WriteLine($"[ {character} ] 获得了回合奖励！{skill.Description}".Trim());
                    if (skill.IsActive)
                    {
                        skill.OnSkillCasted(this, character, [character], []);
                    }
                    else
                    {
                        character.Skills.Add(skill);
                    }
                }
                return list;
            }
            return [];
        }

        /// <summary>
        /// 移除回合奖励
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skills"></param>
        protected static void RemoveRoundRewards(Character character, List<Skill> skills)
        {
            foreach (Skill skill in skills)
            {
                foreach (Effect e in skill.Effects)
                {
                    e.OnEffectLost(character);
                    character.Effects.Remove(e);
                }
                character.Skills.Remove(skill);
            }
        }

        #endregion

        #region 回合外

        /// <summary>
        /// 是否在回合外释放爆发技插队（仅自动化，手动设置请调用：<see cref="SetCharacterPreCastSuperSkill"/>）
        /// </summary>
        /// <returns></returns>
        protected void WillPreCastSuperSkill()
        {
            // 选取所有 AI 控制角色
            foreach (Character other in _queue.Where(c => c.CharacterState == CharacterState.Actionable && IsCharacterInAIControlling(c)).ToList())
            {
                if (!_decisionPoints.TryGetValue(other, out DecisionPoints? dp) || dp is null || dp.CurrentDecisionPoints < dp.GetActionPointCost(CharacterActionType.CastSuperSkill))
                {
                    continue;
                }

                // 有 65% 欲望插队
                if (Random.Shared.NextDouble() < 0.65)
                {
                    List<Skill> skills = [.. other.Skills.Where(s => s.Level > 0 && s.SkillType == SkillType.SuperSkill && s.Enable && !s.IsInEffect && s.CurrentCD == 0 && other.EP >= s.RealEPCost)];
                    if (skills.Count > 0)
                    {
                        Skill skill = skills[Random.Shared.Next(skills.Count)];
                        SetCharacterPreCastSuperSkill(other, skill);
                    }
                }
            }
        }

        #endregion

        #region 回合外-辅助方法

        /// <summary>
        /// 打断施法
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="interrupter"></param>
        public void InterruptCasting(Character caster, Character interrupter)
        {
            Skill? skill = null;
            if (_castingSkills.TryGetValue(caster, out SkillTarget target))
            {
                skill = target.Skill;
            }
            if (skill is null && caster.CharacterState == CharacterState.PreCastSuperSkill)
            {
                WriteLine($"因 [ {caster} ] 的预释放爆发技状态不可驱散，[ {interrupter} ] 打断失败！！");
            }
            if (skill != null)
            {
                bool interruption = true;
                List<Effect> effects = [.. caster.Effects.Union(interrupter.Effects).Distinct().Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                foreach (Effect e in effects)
                {
                    if (!e.BeforeSkillCastWillBeInterrupted(caster, skill, interrupter))
                    {
                        interruption = false;
                    }
                }
                if (interruption)
                {
                    _castingSkills.Remove(caster);
                    WriteLine($"[ {caster} ] 的施法被 [ {interrupter} ] 打断了！！");
                    effects = [.. caster.Effects.Union(interrupter.Effects).Distinct().Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                    foreach (Effect e in effects)
                    {
                        e.OnSkillCastInterrupted(caster, skill, interrupter);
                    }
                    OnInterruptCastingEvent(caster, skill, interrupter);
                }
            }
        }

        /// <summary>
        /// 打断施法 [ 用于使敌人目标丢失 ]
        /// </summary>
        /// <param name="interrupter"></param>
        public void InterruptCasting(Character interrupter)
        {
            WriteLine($"[ {interrupter} ] 人间蒸发了。");
            foreach (Character caster in _castingSkills.Keys)
            {
                SkillTarget skillTarget = _castingSkills[caster];
                if (skillTarget.Targets.Contains(interrupter))
                {
                    Skill skill = skillTarget.Skill;
                    WriteLine($"[ {caster} ] 丢失了施法目标！！");
                    List<Effect> effects = [.. caster.Effects.Union(interrupter.Effects).Distinct().Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                    foreach (Effect effect in effects)
                    {
                        effect.OnSkillCastInterrupted(caster, skill, interrupter);
                    }
                    OnInterruptCastingEvent(caster, skill, interrupter);
                }
            }
        }

        /// <summary>
        /// 设置角色复活
        /// </summary>
        /// <param name="character"></param>
        public void SetCharacterRespawn(Character character)
        {
            if (_original.TryGetValue(character.Guid, out Character? original) && original != null)
            {
                double hardnessTime = 5;
                character.Respawn(_original[character.Guid]);
                _eliminated.Remove(character);
                WriteLine($"[ {character} ] 已复活！获得 {hardnessTime} {GameplayEquilibriumConstant.InGameTime}的硬直时间。");
                AddCharacter(character, hardnessTime, false);
                LastRound.Respawns.Add(character);
                _respawnCountdown.Remove(character);
                if (!_respawnTimes.TryAdd(character, 1))
                {
                    _respawnTimes[character] += 1;
                }
                if (!_decisionPoints.TryGetValue(character, out DecisionPoints? dp) || dp is null)
                {
                    dp = new();
                    _decisionPoints[character] = dp;
                }
                OnQueueUpdatedEvent(_queue, character, dp, hardnessTime, QueueUpdatedReason.Respawn, "设置角色复活后的硬直时间。");
            }
        }

        /// <summary>
        /// 设置角色将预释放爆发技
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skill"></param>
        public void SetCharacterPreCastSuperSkill(Character character, Skill skill)
        {
            if (LastRound.Actor == character && _isInRound)
            {
                return;
            }
            if (_decisionPoints.TryGetValue(character, out DecisionPoints? dp) && dp != null)
            {
                if (dp.CurrentDecisionPoints < GameplayEquilibriumConstant.DecisionPointsCostSuperSkillOutOfTurn)
                {
                    WriteLine($"[ {character} ] 决策点不足，无法预释放爆发技。决策点剩余：{dp.CurrentDecisionPoints} / {dp.MaxDecisionPoints}");
                    return;
                }
            }
            else
            {
                dp = new();
                _decisionPoints[character] = dp;
            }
            if (character.CharacterState == CharacterState.Casting)
            {
                _castingSkills.Remove(character);
                character.CharacterState = CharacterState.Actionable;
                character.UpdateCharacterState();
                WriteLine("[ " + character + " ] 取消吟唱。");
            }
            if (character.CharacterState == CharacterState.Actionable)
            {
                dp.CurrentDecisionPoints -= GameplayEquilibriumConstant.DecisionPointsCostSuperSkillOutOfTurn;
                if (character.Master is Character statsCharacter)
                {
                    _stats[statsCharacter].UseDecisionPoints += GameplayEquilibriumConstant.DecisionPointsCostSuperSkillOutOfTurn;
                    _stats[statsCharacter].TurnDecisions++;
                }
                _castingSuperSkills[character] = skill;
                character.CharacterState = CharacterState.PreCastSuperSkill;
                _queue.Remove(character);
                _cutCount.Remove(character);
                WriteLine($"[ {character} ] 预释放了爆发技！！决策点剩余：{dp.CurrentDecisionPoints} / {dp.MaxDecisionPoints}");

                int preCastSSCount = 0;
                double maxPreCastTime = 0; // 当前最大预释放时间

                // 计算预释放角色的数量和最大时间
                foreach (Character c in _hardnessTimes.Keys)
                {
                    if (c.CharacterState == CharacterState.PreCastSuperSkill && c != character)
                    {
                        preCastSSCount++;
                        if (_hardnessTimes[c] > maxPreCastTime)
                        {
                            maxPreCastTime = _hardnessTimes[c];
                        }
                    }
                }

                // 为非预释放角色增加偏移量
                foreach (Character c in _hardnessTimes.Keys)
                {
                    if (c.CharacterState != CharacterState.PreCastSuperSkill)
                    {
                        _hardnessTimes[c] = Calculation.Round2Digits(_hardnessTimes[c] + 0.01);
                    }
                }

                // 计算新角色的硬直时间
                double newHardnessTime = preCastSSCount > 0 ? Calculation.Round2Digits(maxPreCastTime + 0.01) : 0;

                AddCharacter(character, newHardnessTime, false);
                skill.OnSkillCasting(this, character, [], []);
                Effect[] effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                foreach (Effect effect in effects)
                {
                    effect.AfterCharacterStartCasting(character, skill, []);
                }
                OnQueueUpdatedEvent(_queue, character, dp, 0, QueueUpdatedReason.PreCastSuperSkill, "设置角色预释放爆发技的硬直时间。");
            }
        }

        /// <summary>
        /// 设置角色对目标们的非伤害辅助时间
        /// </summary>
        /// <param name="character"></param>
        /// <param name="targets"></param>
        public void SetNotDamageAssistTime(Character character, params IEnumerable<Character> targets)
        {
            foreach (Character target in targets)
            {
                if (character == target || IsTeammate(character, target)) continue;
                Character c = character, t = target;
                if (character.Master != null)
                {
                    c = character.Master;
                }
                if (target.Master != null)
                {
                    t = target.Master;
                }
                if (c == t) continue;
                _assistDetail[c].NotDamageAssistLastTime[t] = TotalTime;
            }
        }

        /// <summary>
        /// 修改角色的硬直时间
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="addValue">加值</param>
        /// <param name="isPercentage">是否是百分比</param>
        /// <param name="isCheckProtected">是否使用插队保护机制</param>
        public void ChangeCharacterHardnessTime(Character character, double addValue, bool isPercentage, bool isCheckProtected)
        {
            if (!_queue.Contains(character))
            {
                return;
            }
            if (!_hardnessTimes.TryGetValue(character, out double hardnessTime))
            {
                hardnessTime = 0;
            }
            if (isPercentage)
            {
                addValue = hardnessTime * addValue;
            }
            hardnessTime = Calculation.Round2Digits(hardnessTime + addValue);
            if (hardnessTime <= 0) hardnessTime = 0;
            AddCharacter(character, hardnessTime, isCheckProtected);
        }

        #endregion

        #region 自动化

        /// <summary>
        /// 设置角色为 AI 控制
        /// </summary>
        /// <param name="bySystem"></param>
        /// <param name="cancel"></param>
        /// <param name="characters"></param>
        public void SetCharactersToAIControl(bool bySystem = true, bool cancel = false, params IEnumerable<Character> characters)
        {
            foreach (Character character in characters)
            {
                if (cancel)
                {
                    if (bySystem)
                    {
                        _charactersInAIBySystem.Remove(character);
                    }
                    else
                    {
                        _charactersInAIByUser.Remove(character);
                    }
                }
                else
                {
                    if (bySystem)
                    {
                        _charactersInAIBySystem.Add(character);
                    }
                    else
                    {
                        _charactersInAIByUser.Add(character);
                    }
                }
            }
        }

        /// <summary>
        /// 设置角色为 AI 控制 [ 玩家手动设置 ]
        /// </summary>
        /// <param name="cancel"></param>
        /// <param name="characters"></param>
        public void SetCharactersToAIControl(bool cancel = false, params IEnumerable<Character> characters)
        {
            SetCharactersToAIControl(false, cancel, characters);
        }

        /// <summary>
        /// 检查角色是否在 AI 控制状态
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool IsCharacterInAIControlling(Character character)
        {
            if (character.Master != null)
            {
                if (_charactersInAIBySystem.Contains(character))
                {
                    return true;
                }
                character = character.Master;
            }
            return CharactersInAI.Contains(character);
        }

        /// <summary>
        /// 检查角色是否在 AI 控制状态 [ 系统控制 ]
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool IsCharacterInAIControllingBySystem(Character character)
        {
            return _charactersInAIBySystem.Contains(character);
        }

        /// <summary>
        /// 检查角色是否在 AI 控制状态 [ 玩家手动设置 ]
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool IsCharacterInAIControllingByUser(Character character)
        {
            return _charactersInAIByUser.Contains(character);
        }

        #endregion

        #region 检定

        /// <summary>
        /// 免疫检定
        /// </summary>
        /// <param name="character"></param>
        /// <param name="targets"></param>
        /// <param name="skill"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public void CheckSkilledImmune(Character character, List<Character> targets, Skill skill, Item? item = null)
        {
            Character[] loop = [.. targets];
            foreach (Character target in loop)
            {
                if (CheckSkilledImmune(character, target, skill, item))
                {
                    targets.Remove(target);
                }
            }
        }

        /// <summary>
        /// 免疫检定
        /// </summary>
        /// <param name="character"></param>
        /// <param name="target"></param>
        /// <param name="skill"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckSkilledImmune(Character character, Character target, Skill skill, Item? item = null)
        {
            bool ignore = false;
            bool isImmune = (skill.IsMagic && (target.ImmuneType & ImmuneType.Magical) == ImmuneType.Magical) ||
                (target.ImmuneType & ImmuneType.Skilled) == ImmuneType.Skilled || (target.ImmuneType & ImmuneType.All) == ImmuneType.All;
            if (isImmune)
            {
                Effect[] effects = [.. skill.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
                foreach (Effect effect in effects)
                {
                    // 自带无视免疫
                    if (effect.IgnoreImmune == ImmuneType.All || effect.IgnoreImmune == ImmuneType.Skilled || (skill.IsMagic && effect.IgnoreImmune == ImmuneType.Magical))
                    {
                        ignore = true;
                    }
                }
                if (!ignore)
                {
                    Character[] characters = [character, target];
                    effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
                    foreach (Effect effect in effects)
                    {
                        // 特效免疫检定不通过可无视免疫
                        if (!effect.OnImmuneCheck(character, target, skill, item))
                        {
                            ignore = true;
                        }
                    }
                }
            }
            if (ignore)
            {
                isImmune = false;
            }
            if (isImmune)
            {
                WriteLine($"[ {character} ] 想要对 [ {target} ] 释放技能 [ {skill.Name} ]，但是被 [ {target} ] 免疫了！");
                OnCharacterImmunedEvent(character, target, skill, item);
            }
            return isImmune;
        }

        /// <summary>
        /// 特效豁免检定
        /// </summary>
        /// <param name="character"></param>
        /// <param name="source"></param>
        /// <param name="effect"></param>
        /// <param name="isEvade">true - 豁免成功等效于闪避</param>
        /// <returns></returns>
        public bool CheckExemption(Character character, Character? source, Effect effect, bool isEvade)
        {
            double exemption = effect.ExemptionType switch
            {
                PrimaryAttribute.STR => character.STRExemption,
                PrimaryAttribute.AGI => character.AGIExemption,
                PrimaryAttribute.INT => character.INTExemption,
                _ => 0
            };
            bool exempted = false;
            bool checkExempted = true;
            double throwingBonus = 0;
            Character[] characters = source != null ? [character, source] : [character];
            Effect[] effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
            foreach (Effect e in effects)
            {
                if (!e.OnExemptionCheck(character, source, effect, isEvade, ref throwingBonus))
                {
                    checkExempted = false;
                }
            }
            if (checkExempted)
            {
                double dice = Random.Shared.NextDouble();
                if (dice < (exemption + throwingBonus))
                {
                    exempted = true;
                }
            }
            if (exempted)
            {
                if (isEvade)
                {
                    WriteLine($"[ {source} ] 想要对 [ {character} ] 施加 [ {effect.Name} ]，但 [ {character} ] 的{CharacterSet.GetPrimaryAttributeName(effect.ExemptionType)}豁免检定通过，免疫了该效果！");
                }
                else
                {
                    string description = "";
                    if (effect.Durative && effect.RemainDuration > 0)
                    {
                        // 随机减小 20% 至 50%
                        double reduce = Random.Shared.Next(2, 6) * 10;
                        reduce = effect.RemainDuration * (reduce / 100);
                        effect.RemainDuration -= reduce;
                        description = $"[ {effect.Name} ] 的持续时间减少了 {reduce:0.##} {GameplayEquilibriumConstant.InGameTime}！";
                    }
                    else if (effect.RemainDurationTurn > 0)
                    {
                        effect.RemainDurationTurn--;
                        description = $"[ {effect.Name} ] 的持续时间减少了 1 回合！";
                        if (effect.RemainDurationTurn <= 0)
                        {
                            effect.RemainDurationTurn = 0;
                            character.Effects.Remove(effect);
                            effect.OnEffectLost(character);
                            description += $"\r\n[ {character} ] 失去了 [ {effect.Name} ] 效果。";
                        }
                    }
                    WriteLine($"[ {character} ] 的{CharacterSet.GetPrimaryAttributeName(effect.ExemptionType)}豁免检定通过！{description}");
                }
                OnCharacterExemptionEvent(character, source, effect.Skill, effect.Skill.Item, isEvade);
            }
            return exempted;
        }

        #endregion

        #region 数据统计

        /// <summary>
        /// 计算角色的数据
        /// </summary>
        public void CalculateCharacterDamageStatistics(Character character, Character characterTaken, double damage, DamageType damageType, double takenDamage = -1)
        {
            if (takenDamage == -1) takenDamage = damage;
            if (character.Master != null)
            {
                character = character.Master;
            }
            if (characterTaken.Master != null)
            {
                characterTaken = characterTaken.Master;
            }
            if (_stats.TryGetValue(character, out CharacterStatistics? stats) && stats != null)
            {
                if (damageType == DamageType.True)
                {
                    stats.TotalTrueDamage += damage;
                }
                else if (damageType == DamageType.Magical)
                {
                    stats.TotalMagicDamage += damage;
                }
                else
                {
                    stats.TotalPhysicalDamage += damage;
                }
                stats.TotalDamage += damage;
            }
            if (_stats.TryGetValue(characterTaken, out CharacterStatistics? statsTaken) && statsTaken != null)
            {
                if (damageType == DamageType.True)
                {
                    statsTaken.TotalTakenTrueDamage = Calculation.Round2Digits(statsTaken.TotalTakenTrueDamage + takenDamage);
                }
                else if (damageType == DamageType.Magical)
                {
                    statsTaken.TotalTakenMagicDamage = Calculation.Round2Digits(statsTaken.TotalTakenMagicDamage + takenDamage);
                }
                else
                {
                    statsTaken.TotalTakenPhysicalDamage = Calculation.Round2Digits(statsTaken.TotalTakenPhysicalDamage + takenDamage);
                }
                statsTaken.TotalTakenDamage = Calculation.Round2Digits(statsTaken.TotalTakenDamage + takenDamage);
            }
            if (LastRound.Damages.TryGetValue(characterTaken, out double damageTotal))
            {
                LastRound.Damages[characterTaken] = damageTotal + damage;
            }
            else
            {
                LastRound.Damages[characterTaken] = damage;
            }
        }

        #endregion

        #region 其他辅助方法

        /// <summary>
        /// 装备物品
        /// </summary>
        /// <param name="character"></param>
        /// <param name="item"></param>
        public void Equip(Character character, Item item)
        {
            if (character.Equip(item))
            {
                EquipSlotType type = item.EquipSlotType;
                WriteLine($"[ {character} ] 装备了 [ {item.Name} ]。" + (type != EquipSlotType.None ? $"（{ItemSet.GetEquipSlotTypeName(type)} 栏位）" : ""));
            }
        }

        /// <summary>
        /// 装备物品到指定栏位，并返回被替换的装备（如果有的话）
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        /// <param name="item"></param>
        /// <param name="previous"></param>
        public void Equip(Character character, EquipSlotType type, Item item, out Item? previous)
        {
            if (character.Equip(item, type, out previous))
            {
                WriteLine($"[ {character} ] 装备了 [ {item.Name} ]。（{ItemSet.GetEquipSlotTypeName(type)} 栏位）");
            }
        }

        /// <summary>
        /// 取消装备，并返回被替换的装备（如果有的话）
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Item? UnEquip(Character character, EquipSlotType type)
        {
            Item? item = character.UnEquip(type);
            if (item != null)
            {
                WriteLine($"[ {character} ] 取消装备了 [ {item.Name} ]。（{ItemSet.GetEquipSlotTypeName(type)} 栏位）");
            }
            return item;
        }

        /// <summary>
        /// 向角色（或控制该角色的玩家）进行询问并取得答复
        /// </summary>
        /// <param name="character"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public InquiryResponse Inquiry(Character character, InquiryOptions options)
        {
            if (!_decisionPoints.TryGetValue(character, out DecisionPoints? dp) || dp is null)
            {
                dp = new();
                _decisionPoints[character] = dp;
            }
            InquiryResponse response = OnCharacterInquiryEvent(character, dp, options);
            Effect[] effects = [.. character.Effects.Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
            foreach (Effect effect in effects)
            {
                effect.OnCharacterInquiry(character, options, response);
            }
            return response;
        }

        #endregion

        #region 事件

        public delegate bool TurnStartEventHandler(GamingQueue queue, Character character, DecisionPoints dp, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items);
        /// <summary>
        /// 回合开始事件
        /// </summary>
        public event TurnStartEventHandler? TurnStartEvent;
        /// <summary>
        /// 回合开始事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="skills"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        protected bool OnTurnStartEvent(Character character, DecisionPoints dp, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items)
        {
            return TurnStartEvent?.Invoke(this, character, dp, enemys, teammates, skills, items) ?? true;
        }

        public delegate void TurnEndEventHandler(GamingQueue queue, Character character, DecisionPoints dp);
        /// <summary>
        /// 回合结束事件
        /// </summary>
        public event TurnEndEventHandler? TurnEndEvent;
        /// <summary>
        /// 回合结束事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        protected void OnTurnEndEvent(Character character, DecisionPoints dp)
        {
            TurnEndEvent?.Invoke(this, character, dp);
        }

        public delegate CharacterActionType DecideActionEventHandler(GamingQueue queue, Character character, DecisionPoints dp, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items);
        /// <summary>
        /// 决定角色的行动事件
        /// </summary>
        public event DecideActionEventHandler? DecideActionEvent;
        /// <summary>
        /// 决定角色的行动事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="skills"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        protected CharacterActionType OnDecideActionEvent(Character character, DecisionPoints dp, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items)
        {
            return DecideActionEvent?.Invoke(this, character, dp, enemys, teammates, skills, items) ?? CharacterActionType.None;
        }

        public delegate Skill? SelectSkillEventHandler(GamingQueue queue, Character character, List<Skill> skills);
        /// <summary>
        /// 角色需要选择一个技能
        /// </summary>
        public event SelectSkillEventHandler? SelectSkillEvent;
        /// <summary>
        /// 角色需要选择一个技能
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skills"></param>
        /// <returns></returns>
        protected Skill? OnSelectSkillEvent(Character character, List<Skill> skills)
        {
            return SelectSkillEvent?.Invoke(this, character, skills) ?? null;
        }

        public delegate Item? SelectItemEventHandler(GamingQueue queue, Character character, List<Item> items);
        /// <summary>
        /// 角色需要选择一个物品
        /// </summary>
        public event SelectItemEventHandler? SelectItemEvent;
        /// <summary>
        /// 角色需要选择一个物品
        /// </summary>
        /// <param name="character"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        protected Item? OnSelectItemEvent(Character character, List<Item> items)
        {
            return SelectItemEvent?.Invoke(this, character, items) ?? null;
        }

        public delegate Grid SelectTargetGridEventHandler(GamingQueue queue, Character character, List<Character> enemys, List<Character> teammates, GameMap map, List<Grid> moveRange);
        /// <summary>
        /// 选取移动目标事件
        /// </summary>
        public event SelectTargetGridEventHandler? SelectTargetGridEvent;
        /// <summary>
        /// 选取移动目标事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="map"></param>
        /// <param name="moveRange"></param>
        /// <returns></returns>
        protected Grid OnSelectTargetGridEvent(Character character, List<Character> enemys, List<Character> teammates, GameMap map, List<Grid> moveRange)
        {
            return SelectTargetGridEvent?.Invoke(this, character, enemys, teammates, map, moveRange) ?? Grid.Empty;
        }

        public delegate List<Character> SelectSkillTargetsEventHandler(GamingQueue queue, Character caster, Skill skill, List<Character> enemys, List<Character> teammates, List<Grid> castRange);
        /// <summary>
        /// 选取技能目标事件
        /// </summary>
        public event SelectSkillTargetsEventHandler? SelectSkillTargetsEvent;
        /// <summary>
        /// 选取技能目标事件
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="castRange"></param>
        /// <returns></returns>
        protected List<Character> OnSelectSkillTargetsEvent(Character caster, Skill skill, List<Character> enemys, List<Character> teammates, List<Grid> castRange)
        {
            return SelectSkillTargetsEvent?.Invoke(this, caster, skill, enemys, teammates, castRange) ?? [];
        }

        public delegate List<Grid> SelectNonDirectionalSkillTargetsEventHandler(GamingQueue queue, Character caster, Skill skill, List<Character> enemys, List<Character> teammates, List<Grid> castRange);
        /// <summary>
        /// 选取非指向性技能目标事件
        /// </summary>
        public event SelectNonDirectionalSkillTargetsEventHandler? SelectNonDirectionalSkillTargetsEvent;
        /// <summary>
        /// 选取非指向性技能目标事件
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="castRange"></param>
        /// <returns></returns>
        protected List<Grid> OnSelectNonDirectionalSkillTargetsEvent(Character caster, Skill skill, List<Character> enemys, List<Character> teammates, List<Grid> castRange)
        {
            return SelectNonDirectionalSkillTargetsEvent?.Invoke(this, caster, skill, enemys, teammates, castRange) ?? [];
        }

        public delegate List<Character> SelectNormalAttackTargetsEventHandler(GamingQueue queue, Character character, NormalAttack attack, List<Character> enemys, List<Character> teammates, List<Grid> attackRange);
        /// <summary>
        /// 选取普通攻击目标事件
        /// </summary>
        public event SelectNormalAttackTargetsEventHandler? SelectNormalAttackTargetsEvent;
        /// <summary>
        /// 选取普通攻击目标事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="attack"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="attackRange"></param>
        /// <returns></returns>
        protected List<Character> OnSelectNormalAttackTargetsEvent(Character character, NormalAttack attack, List<Character> enemys, List<Character> teammates, List<Grid> attackRange)
        {
            return SelectNormalAttackTargetsEvent?.Invoke(this, character, attack, enemys, teammates, attackRange) ?? [];
        }

        public delegate void InterruptCastingEventHandler(GamingQueue queue, Character cast, Skill? skill, Character interrupter);
        /// <summary>
        /// 打断施法事件
        /// </summary>
        public event InterruptCastingEventHandler? InterruptCastingEvent;
        /// <summary>
        /// 打断施法事件
        /// </summary>
        /// <param name="cast"></param>
        /// <param name="skill"></param>
        /// <param name="interrupter"></param>
        /// <returns></returns>
        protected void OnInterruptCastingEvent(Character cast, Skill skill, Character interrupter)
        {
            InterruptCastingEvent?.Invoke(this, cast, skill, interrupter);
        }

        public delegate bool DeathCalculationEventHandler(GamingQueue queue, Character killer, Character death);
        /// <summary>
        /// 死亡结算事件
        /// </summary>
        public event DeathCalculationEventHandler? DeathCalculationEvent;
        /// <summary>
        /// 死亡结算事件
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="death"></param>
        /// <returns></returns>
        protected bool OnDeathCalculationEvent(Character killer, Character death)
        {
            return DeathCalculationEvent?.Invoke(this, killer, death) ?? true;
        }

        public delegate bool DeathCalculationByTeammateEventHandler(GamingQueue queue, Character killer, Character death);
        /// <summary>
        /// 死亡结算（击杀队友）事件
        /// </summary>
        public event DeathCalculationEventHandler? DeathCalculationByTeammateEvent;
        /// <summary>
        /// 死亡结算（击杀队友）事件
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="death"></param>
        /// <returns></returns>
        protected bool OnDeathCalculationByTeammateEvent(Character killer, Character death)
        {
            return DeathCalculationByTeammateEvent?.Invoke(this, killer, death) ?? true;
        }

        public delegate bool CharacterDeathEventHandler(GamingQueue queue, Character death, Character? killer, Character[] assists);
        /// <summary>
        /// 角色死亡事件，此事件位于 <see cref="DeathCalculation"/> 之后
        /// </summary>
        public event CharacterDeathEventHandler? CharacterDeathEvent;
        /// <summary>
        /// 角色死亡事件，此事件位于 <see cref="DeathCalculation"/> 之后
        /// </summary>
        /// <param name="death"></param>
        /// <param name="killer"></param>
        /// <param name="assists"></param>
        /// <returns></returns>
        protected bool OnCharacterDeathEvent(Character death, Character? killer, Character[] assists)
        {
            return CharacterDeathEvent?.Invoke(this, death, killer, assists) ?? true;
        }

        public delegate void HealToTargetEventHandler(GamingQueue queue, Character actor, Character target, double heal, bool isRespawn);
        /// <summary>
        /// 治疗事件
        /// </summary>
        public event HealToTargetEventHandler? HealToTargetEvent;
        /// <summary>
        /// 治疗事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="target"></param>
        /// <param name="heal"></param>
        /// <param name="isRespawn"></param>
        /// <returns></returns>
        protected void OnHealToTargetEvent(Character actor, Character target, double heal, bool isRespawn)
        {
            HealToTargetEvent?.Invoke(this, actor, target, heal, isRespawn);
        }

        public delegate void DamageToEnemyEventHandler(GamingQueue queue, Character actor, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult);
        /// <summary>
        /// 造成伤害事件
        /// </summary>
        public event DamageToEnemyEventHandler? DamageToEnemyEvent;
        /// <summary>
        /// 造成伤害事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <param name="actualDamage"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="damageType"></param>
        /// <param name="magicType"></param>
        /// <param name="damageResult"></param>
        /// <returns></returns>
        protected void OnDamageToEnemyEvent(Character actor, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            DamageToEnemyEvent?.Invoke(this, actor, enemy, damage, actualDamage, isNormalAttack, damageType, magicType, damageResult);
        }

        public delegate void CharacterNormalAttackEventHandler(GamingQueue queue, Character actor, DecisionPoints dp, List<Character> targets);
        /// <summary>
        /// 角色普通攻击事件
        /// </summary>
        public event CharacterNormalAttackEventHandler? CharacterNormalAttackEvent;
        /// <summary>
        /// 角色普通攻击事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="dp"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        protected void OnCharacterNormalAttackEvent(Character actor, DecisionPoints dp, List<Character> targets)
        {
            CharacterNormalAttackEvent?.Invoke(this, actor, dp, targets);
        }

        public delegate void CharacterPreCastSkillEventHandler(GamingQueue queue, Character actor, DecisionPoints dp, SkillTarget skillTarget);
        /// <summary>
        /// 角色吟唱技能事件（包括直接释放战技）
        /// </summary>
        public event CharacterPreCastSkillEventHandler? CharacterPreCastSkillEvent;
        /// <summary>
        /// 角色吟唱技能事件（包括直接释放战技）
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="dp"></param>
        /// <param name="skillTarget"></param>
        /// <returns></returns>
        protected void OnCharacterPreCastSkillEvent(Character actor, DecisionPoints dp, SkillTarget skillTarget)
        {
            CharacterPreCastSkillEvent?.Invoke(this, actor, dp, skillTarget);
        }

        public delegate void CharacterCastSkillEventHandler(GamingQueue queue, Character actor, DecisionPoints dp, SkillTarget skillTarget, double cost);
        /// <summary>
        /// 角色释放技能事件
        /// </summary>
        public event CharacterCastSkillEventHandler? CharacterCastSkillEvent;
        /// <summary>
        /// 角色释放技能事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="dp"></param>
        /// <param name="skillTarget"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        protected void OnCharacterCastSkillEvent(Character actor, DecisionPoints dp, SkillTarget skillTarget, double cost)
        {
            CharacterCastSkillEvent?.Invoke(this, actor, dp, skillTarget, cost);
        }

        public delegate void CharacterUseItemEventHandler(GamingQueue queue, Character actor, DecisionPoints dp, Item item, List<Character> targets);
        /// <summary>
        /// 角色使用物品事件
        /// </summary>
        public event CharacterUseItemEventHandler? CharacterUseItemEvent;
        /// <summary>
        /// 角色使用物品事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="dp"></param>
        /// <param name="item"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        protected void OnCharacterUseItemEvent(Character actor, DecisionPoints dp, Item item, List<Character> targets)
        {
            CharacterUseItemEvent?.Invoke(this, actor, dp, item, targets);
        }

        public delegate void CharacterCastItemSkillEventHandler(GamingQueue queue, Character actor, DecisionPoints dp, Item item, SkillTarget skillTarget, double costMP, double costEP);
        /// <summary>
        /// 角色释放物品的技能事件
        /// </summary>
        public event CharacterCastItemSkillEventHandler? CharacterCastItemSkillEvent;
        /// <summary>
        /// 角色释放物品的技能事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="dp"></param>
        /// <param name="item"></param>
        /// <param name="skillTarget"></param>
        /// <param name="costMP"></param>
        /// <param name="costEP"></param>
        /// <returns></returns>
        protected void OnCharacterCastItemSkillEvent(Character actor, DecisionPoints dp, Item item, SkillTarget skillTarget, double costMP, double costEP)
        {
            CharacterCastItemSkillEvent?.Invoke(this, actor, dp, item, skillTarget, costMP, costEP);
        }

        public delegate void CharacterImmunedEventHandler(GamingQueue queue, Character character, Character immune, ISkill skill, Item? item = null);
        /// <summary>
        /// 角色免疫事件
        /// </summary>
        public event CharacterImmunedEventHandler? CharacterImmunedEvent;
        /// <summary>
        /// 角色免疫事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="immune"></param>
        /// <param name="skill"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        protected void OnCharacterImmunedEvent(Character character, Character immune, ISkill skill, Item? item = null)
        {
            CharacterImmunedEvent?.Invoke(this, character, immune, skill, item);
        }

        public delegate void CharacterExemptionEventHandler(GamingQueue queue, Character character, Character? source, ISkill skill, Item? item = null, bool isEvade = false);
        /// <summary>
        /// 角色豁免事件
        /// </summary>
        public event CharacterExemptionEventHandler? CharacterExemptionEvent;
        /// <summary>
        /// 角色豁免事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="source"></param>
        /// <param name="skill"></param>
        /// <param name="item"></param>
        /// <param name="isEvade"></param>
        /// <returns></returns>
        protected void OnCharacterExemptionEvent(Character character, Character? source, ISkill skill, Item? item = null, bool isEvade = false)
        {
            CharacterExemptionEvent?.Invoke(this, character, source, skill, item, isEvade);
        }

        public delegate void CharacterDoNothingEventHandler(GamingQueue queue, Character actor, DecisionPoints dp);
        /// <summary>
        /// 角色主动结束回合事件（区别于放弃行动，这个是主动的）
        /// </summary>
        public event CharacterDoNothingEventHandler? CharacterDoNothingEvent;
        /// <summary>
        /// 角色主动结束回合事件（区别于放弃行动，这个是主动的）
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        protected void OnCharacterDoNothingEvent(Character actor, DecisionPoints dp)
        {
            CharacterDoNothingEvent?.Invoke(this, actor, dp);
        }

        public delegate void CharacterGiveUpEventHandler(GamingQueue queue, Character actor, DecisionPoints dp);
        /// <summary>
        /// 角色放弃行动事件
        /// </summary>
        public event CharacterGiveUpEventHandler? CharacterGiveUpEvent;
        /// <summary>
        /// 角色放弃行动事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        protected void OnCharacterGiveUpEvent(Character actor, DecisionPoints dp)
        {
            CharacterGiveUpEvent?.Invoke(this, actor, dp);
        }

        public delegate void CharacterMoveEventHandler(GamingQueue queue, Character actor, DecisionPoints dp, Grid grid);
        /// <summary>
        /// 角色移动事件
        /// </summary>
        public event CharacterMoveEventHandler? CharacterMoveEvent;
        /// <summary>
        /// 角色移动事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="dp"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        protected void OnCharacterMoveEvent(Character actor, DecisionPoints dp, Grid grid)
        {
            CharacterMoveEvent?.Invoke(this, actor, dp, grid);
        }

        public delegate void GameStartEventHandler(GamingQueue queue);
        /// <summary>
        /// 游戏开始事件
        /// </summary>
        public event GameStartEventHandler? GameStartEvent;
        /// <summary>
        /// 游戏开始事件
        /// </summary>
        /// <returns></returns>
        protected void OnGameStartEvent()
        {
            GameStartEvent?.Invoke(this);
        }

        public delegate bool GameEndEventHandler(GamingQueue queue, Character winner);
        /// <summary>
        /// 游戏结束事件
        /// </summary>
        public event GameEndEventHandler? GameEndEvent;
        /// <summary>
        /// 游戏结束事件
        /// </summary>
        /// <param name="winner"></param>
        /// <returns></returns>
        protected bool OnGameEndEvent(Character winner)
        {
            return GameEndEvent?.Invoke(this, winner) ?? true;
        }

        public delegate void QueueUpdatedEventHandler(GamingQueue queue, List<Character> characters, Character character, DecisionPoints dp, double hardnessTime, QueueUpdatedReason reason, string msg);
        /// <summary>
        /// 行动顺序表更新事件
        /// </summary>
        public event QueueUpdatedEventHandler? QueueUpdatedEvent;
        /// <summary>
        /// 行动顺序表更新事件
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        /// <param name="hardnessTime"></param>
        /// <param name="reason"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected void OnQueueUpdatedEvent(List<Character> characters, Character character, DecisionPoints dp, double hardnessTime, QueueUpdatedReason reason, string msg = "")
        {
            QueueUpdatedEvent?.Invoke(this, characters, character, dp, hardnessTime, reason, msg);
        }

        public delegate void CharacterActionTakenEventHandler(GamingQueue queue, Character actor, DecisionPoints dp, CharacterActionType type, RoundRecord record);
        /// <summary>
        /// 角色完成行动事件
        /// </summary>
        public event CharacterActionTakenEventHandler? CharacterActionTakenEvent;
        /// <summary>
        /// 角色完成行动事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="dp"></param>
        /// <param name="type"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        protected void OnCharacterActionTakenEvent(Character actor, DecisionPoints dp, CharacterActionType type, RoundRecord record)
        {
            CharacterActionTakenEvent?.Invoke(this, actor, dp, type, record);
        }

        public delegate void CharacterDecisionCompletedEventHandler(GamingQueue queue, Character actor, DecisionPoints dp, RoundRecord record);
        /// <summary>
        /// 角色完成决策事件
        /// </summary>
        public event CharacterDecisionCompletedEventHandler? CharacterDecisionCompletedEvent;
        /// <summary>
        /// 角色完成决策事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="dp"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        protected void OnCharacterDecisionCompletedEvent(Character actor, DecisionPoints dp, RoundRecord record)
        {
            CharacterDecisionCompletedEvent?.Invoke(this, actor, dp, record);
        }

        public delegate InquiryResponse CharacterInquiryEventHandler(GamingQueue queue, Character character, DecisionPoints dp, InquiryOptions options);
        /// <summary>
        /// 角色询问反应事件
        /// </summary>
        public event CharacterInquiryEventHandler? CharacterInquiryEvent;
        /// <summary>
        /// 角色询问反应事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected InquiryResponse OnCharacterInquiryEvent(Character character, DecisionPoints dp, InquiryOptions options)
        {
            return CharacterInquiryEvent?.Invoke(this, character, dp, options) ?? new(options);
        }

        #endregion
    }
}
