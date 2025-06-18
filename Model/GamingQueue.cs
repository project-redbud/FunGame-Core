using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

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
        /// 参与本次游戏的所有角色列表
        /// </summary>
        public List<Character> AllCharacter => _allCharacter;

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
        public HashSet<Character> CharactersInAI => _charactersInAI;

        /// <summary>
        /// 角色数据
        /// </summary>
        public Dictionary<Character, CharacterStatistics> CharacterStatistics => _stats;

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
        /// 自定义数据
        /// </summary>
        public Dictionary<string, object> CustomData { get; } = [];

        #endregion

        #region 保护变量

        /// <summary>
        /// 参与本次游戏的所有角色列表
        /// </summary>
        protected readonly List<Character> _allCharacter = [];

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
        /// 角色是否在 AI 控制下
        /// </summary>
        protected readonly HashSet<Character> _charactersInAI = [];

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
        /// 当前回合死亡角色
        /// </summary>
        protected readonly List<Character> _roundDeaths = [];

        /// <summary>
        /// 回合奖励
        /// </summary>
        protected readonly Dictionary<int, List<Skill>> _roundRewards = [];

        /// <summary>
        /// 回合奖励的特效工厂
        /// </summary>
        protected Func<long, Dictionary<string, object>> _factoryRoundRewardEffects = id => [];

        /// <summary>
        /// 游戏是否结束
        /// </summary>
        protected bool _isGameEnd = false;

        /// <summary>
        /// 是否在回合内
        /// </summary>
        protected bool _isInRound = false;

        #endregion

        #region 构造函数

        /// <summary>
        /// 新建一个基础回合制游戏队列
        /// </summary>
        /// <param name="writer">用于文本输出</param>
        public GamingQueue(Action<string>? writer = null)
        {
            if (writer != null)
            {
                WriteLine = writer;
            }
            WriteLine ??= new Action<string>(Console.WriteLine);
        }

        /// <summary>
        /// 新建一个基础回合制游戏队列并初始化
        /// </summary>
        /// <param name="characters">参与本次游戏的角色列表</param>
        /// <param name="writer">用于文本输出</param>
        public GamingQueue(List<Character> characters, Action<string>? writer = null)
        {
            if (writer != null)
            {
                WriteLine = writer;
            }
            WriteLine ??= new Action<string>(Console.WriteLine);
            InitCharacterQueue(characters);
        }

        #endregion

        #region 队列框架

        /// <summary>
        /// 初始化基础回合制游戏队列
        /// </summary>
        /// <param name="characters"></param>
        public void InitCharacterQueue(List<Character> characters)
        {
            // 保存原始的角色信息。用于复活时还原状态
            foreach (Character character in characters)
            {
                // 添加角色引用到所有角色列表
                _allCharacter.Add(character);
                // 复制原始角色对象
                Character original = character.Copy();
                original.Guid = Guid.NewGuid();
                character.Guid = original.Guid;
                _original.Add(original.Guid, original);
            }

            // 初始排序：按速度排序
            List<IGrouping<double, Character>> groupedBySpeed = [.. characters
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
                    _assistDetail.Add(character, new AssistDetail(character, characters.Where(c => c != character)));
                    _stats.Add(character, new());
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
                            _assistDetail.Add(selectedCharacter, new AssistDetail(selectedCharacter, characters.Where(c => c != selectedCharacter)));
                            _stats.Add(selectedCharacter, new());
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

            // 插队机制：按硬直时间排序
            int insertIndex = _queue.FindIndex(c => _hardnessTimes[c] > hardnessTime);

            if (isCheckProtected)
            {
                // 查找保护条件 被插队超过此次数便能获得插队补偿 即行动保护
                int countProtected = Math.Max(5, _queue.Count);

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
                    if (insertIndex != -1 && insertIndex <= protectIndex)
                    {
                        // 如果按硬直时间插入的位置在受保护角色之前或相同，则插入到受保护角色的后面一位
                        insertIndex = protectIndex + 1;
                        hardnessTime = lastProtectedHardnessTime;

                        // 列出受保护角色的名单
                        WriteLine($"由于 [ {string.Join(" ]，[ ", list.Select(x => x.Character))} ] 受到行动保护，因此角色 [ {character} ] 将插入至顺序表第 {insertIndex + 1} 位。");
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
        }

        /// <summary>
        /// 清空行动队列
        /// </summary>
        public void ClearQueue()
        {
            FirstKiller = null;
            CustomData.Clear();
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
            _charactersInAI.Clear();
        }

        #endregion

        #region 时间流逝

        /// <summary>
        /// 从行动顺序表取出第一个角色
        /// </summary>
        /// <returns></returns>
        public async Task<Character?> NextCharacterAsync()
        {
            if (_queue.Count == 0) return null;

            // 硬直时间为 0 的角色或预释放爆发技的角色先行动，取第一个
            Character? character = _queue.FirstOrDefault(c => c.CharacterState == CharacterState.PreCastSuperSkill);
            if (character is null)
            {
                Character temp = _queue[0];
                if (_hardnessTimes[temp] == 0)
                {
                    character = temp;
                }
            }
            else
            {
                _hardnessTimes[character] = 0;
            }

            if (character != null)
            {
                _queue.Remove(character);
                _cutCount.Remove(character);

                // 进入下一回合
                TotalRound++;
                LastRound = new(TotalRound);
                Rounds.Add(LastRound);

                return character;
            }
            else
            {
                await TimeLapse();
                return await NextCharacterAsync();
            }
        }

        /// <summary>
        /// 时间进行流逝，减少硬直时间，减少技能冷却时间，角色也会因此回复状态
        /// </summary>
        /// <returns>流逝的时间</returns>
        public async Task<double> TimeLapse()
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
            WriteLine("时间流逝：" + timeToReduce);

            Character[] characters = [.. _queue];
            foreach (Character character in characters)
            {
                // 减少所有角色的硬直时间
                _hardnessTimes[character] = Calculation.Round2Digits(_hardnessTimes[character] - timeToReduce);

                // 统计
                _stats[character].LiveRound += 1;
                _stats[character].LiveTime += timeToReduce;
                _stats[character].DamagePerRound = _stats[character].TotalDamage / _stats[character].LiveRound;
                _stats[character].DamagePerTurn = _stats[character].TotalDamage / _stats[character].ActionTurn;
                _stats[character].DamagePerSecond = _stats[character].TotalDamage / _stats[character].LiveTime;

                // 回血回蓝
                double recoveryHP = character.HR * timeToReduce;
                double recoveryMP = character.MR * timeToReduce;
                double needHP = character.MaxHP - character.HP;
                double needMP = character.MaxMP - character.MP;
                double reallyReHP = needHP >= recoveryHP ? recoveryHP : needHP;
                double reallyReMP = needMP >= recoveryMP ? recoveryMP : needMP;
                if (reallyReHP > 0 && reallyReMP > 0)
                {
                    character.HP += reallyReHP;
                    character.MP += reallyReMP;
                    WriteLine($"角色 {character.Name} 回血：{recoveryHP:0.##} [{character.HP:0.##} / {character.MaxHP:0.##}] / 回蓝：{recoveryMP:0.##} [{character.MP:0.##} / {character.MaxMP:0.##}] / 当前能量：{character.EP:0.##}");
                }
                else
                {
                    if (reallyReHP > 0)
                    {
                        character.HP += reallyReHP;
                        WriteLine($"角色 {character.Name} 回血：{recoveryHP:0.##} [{character.HP:0.##} / {character.MaxHP:0.##}] / 当前能量：{character.EP:0.##}");
                    }
                    if (reallyReMP > 0)
                    {
                        character.MP += reallyReMP;
                        WriteLine($"角色 {character.Name} 回蓝：{recoveryMP:0.##} [{character.MP:0.##} / {character.MaxMP:0.##}] / 当前能量：{character.EP:0.##}");
                    }
                }

                // 减少所有技能的冷却时间
                foreach (Skill skill in character.Skills)
                {
                    skill.CurrentCD -= timeToReduce;
                    if (skill.CurrentCD <= 0)
                    {
                        skill.CurrentCD = 0;
                        skill.Enable = true;
                    }
                }

                // 移除到时间的特效
                List<Effect> effects = [.. character.Effects];
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

                    if (effect.IsBeingTemporaryDispelled)
                    {
                        effect.IsBeingTemporaryDispelled = false;
                        effect.OnEffectGained(character);
                    }

                    // 如果特效具备临时驱散或者持续性驱散的功能
                    if (effect.Source != null && (effect.EffectType == EffectType.WeakDispelling || effect.EffectType == EffectType.StrongDispelling))
                    {
                        effect.Dispel(effect.Source, character, IsTeammate(character, effect.Source));
                    }

                    // 自身被动不会考虑
                    if (effect.EffectType == EffectType.None && effect.Skill.SkillType == SkillType.Passive)
                    {
                        continue;
                    }

                    // 统计控制时长
                    if (effect.Source != null && SkillSet.GetCharacterStateByEffectType(effect.EffectType) != CharacterState.Actionable)
                    {
                        _stats[effect.Source].ControlTime += timeToReduce;
                        _assistDetail[effect.Source][character, TotalTime] += 1;
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
                        }
                        else
                        {
                            effect.RemainDuration -= timeToReduce;
                            effect.OnTimeElapsed(character, timeToReduce);
                        }
                    }
                }
            }

            // 减少复活倒计时
            foreach (Character character in _respawnCountdown.Keys)
            {
                _respawnCountdown[character] = Calculation.Round2Digits(_respawnCountdown[character] - timeToReduce);
                if (_respawnCountdown[character] <= 0)
                {
                    await SetCharacterRespawn(character);
                }
            }

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
        public async Task<bool> ProcessTurnAsync(Character character)
        {
            _isInRound = true;
            LastRound.Actor = character;
            _roundDeaths.Clear();

            if (!await BeforeTurnAsync(character))
            {
                _isInRound = false;
                return _isGameEnd;
            }

            // 获取回合奖励
            List<Skill> rewards = GetRoundRewards(TotalRound, character);

            // 基础硬直时间
            double baseTime = 10;
            bool isCheckProtected = true;

            // 队友列表
            List<Character> allTeammates = GetTeammates(character);
            List<Character> teammates = [.. allTeammates.Where(_queue.Contains)];

            // 敌人列表
            List<Character> allEnemys = [.. _allCharacter.Where(c => c != character && !teammates.Contains(c))];
            List<Character> enemys = [.. allEnemys.Where(c => _queue.Contains(c) && !c.IsUnselectable)];

            // 技能列表
            List<Skill> skills = [.. character.Skills.Where(s => s.Level > 0 && s.SkillType != SkillType.Passive && s.Enable && !s.IsInEffect && s.CurrentCD == 0 &&
                ((s.SkillType == SkillType.SuperSkill || s.SkillType == SkillType.Skill) && s.RealEPCost <= character.EP || s.SkillType == SkillType.Magic && s.RealMPCost <= character.MP))];

            // 物品列表
            List<Item> items = [.. character.Items.Where(i => i.IsActive && i.Skills.Active != null && i.Enable && i.IsInGameItem &&
                i.Skills.Active.SkillType == SkillType.Item && i.Skills.Active.Enable && !i.Skills.Active.IsInEffect && i.Skills.Active.CurrentCD == 0 && i.Skills.Active.RealMPCost <= character.MP && i.Skills.Active.RealEPCost <= character.EP)];

            // 回合开始事件，允许事件返回 false 接管回合操作
            // 如果事件全程接管回合操作，需要注意触发特效
            if (!await OnTurnStartAsync(character, enemys, teammates, skills, items))
            {
                _isInRound = false;
                return _isGameEnd;
            }

            foreach (Skill skillTurnStart in skills)
            {
                skillTurnStart.OnTurnStart(character, enemys, teammates, skills, items);
            }

            List<Effect> effects = [.. character.Effects.Where(e => e.IsInEffect)];
            foreach (Effect effect in effects)
            {
                effect.OnTurnStart(character, enemys, teammates, skills, items);
            }

            // 此变量用于在取消选择时，能够重新行动
            bool decided = false;
            // 最大取消次数
            int cancelTimes = 3;

            // 作出了什么行动
            CharacterActionType type = CharacterActionType.None;

            // 循环条件：
            // AI 控制下：未决策、取消次数大于0
            // 手动控制下：未决策
            bool isAI = _charactersInAI.Contains(character);
            while (!decided && (!isAI || cancelTimes > 0))
            {
                type = CharacterActionType.None;

                // 是否能使用物品和释放技能
                bool canUseItem = items.Count > 0;
                bool canCastSkill = skills.Count > 0;

                // 使用物品和释放技能、使用普通攻击的概率
                double pUseItem = 0.33;
                double pCastSkill = 0.33;
                double pNormalAttack = 0.34;

                cancelTimes--;
                // 不允许在吟唱和预释放状态下，修改角色的行动
                if (character.CharacterState != CharacterState.Casting && character.CharacterState != CharacterState.PreCastSuperSkill)
                {
                    CharacterActionType actionTypeTemp = CharacterActionType.None;
                    effects = [.. character.Effects.Where(e => e.IsInEffect)];
                    foreach (Effect effect in effects)
                    {
                        actionTypeTemp = effect.AlterActionTypeBeforeAction(character, character.CharacterState, ref canUseItem, ref canCastSkill, ref pUseItem, ref pCastSkill, ref pNormalAttack);
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
                            // 战斗不能，只能使用物品
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

                        // 模组可以通过此事件来决定角色的行动
                        type = await OnDecideActionAsync(character, enemys, teammates, skills, items);
                        // 若事件未完成决策，则将通过概率对角色进行自动化决策
                        if (type == CharacterActionType.None)
                        {
                            type = GetActionType(pUseItem, pCastSkill, pNormalAttack);
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

                Dictionary<Character, int> continuousKillingTemp = new(_continuousKilling);
                Dictionary<Character, int> earnedMoneyTemp = new(_earnedMoney);
                effects = [.. character.Effects.Where(e => e.IsInEffect)];
                foreach (Effect effect in effects)
                {
                    effect.AlterSelectListBeforeAction(character, enemys, teammates, skills, continuousKillingTemp, earnedMoneyTemp);
                }

                if (type == CharacterActionType.NormalAttack)
                {
                    if (character.CharacterState == CharacterState.NotActionable ||
                        character.CharacterState == CharacterState.ActionRestricted ||
                        character.CharacterState == CharacterState.BattleRestricted ||
                        character.CharacterState == CharacterState.AttackRestricted)
                    {
                        WriteLine($"角色 [ {character} ] 状态为：{CharacterSet.GetCharacterState(character.CharacterState)}，无法使用普通攻击！");
                    }
                    else
                    {
                        // 使用普通攻击逻辑
                        List<Character> targets = await SelectTargetsAsync(character, character.NormalAttack, enemys, teammates);
                        if (targets.Count > 0)
                        {
                            LastRound.Targets = [.. targets];
                            decided = true;

                            await OnCharacterNormalAttackAsync(character, targets);

                            character.NormalAttack.Attack(this, character, targets);
                            baseTime = character.NormalAttack.RealHardnessTime;
                            effects = [.. character.Effects.Where(e => e.IsInEffect)];
                            foreach (Effect effect in effects)
                            {
                                effect.AlterHardnessTimeAfterNormalAttack(character, ref baseTime, ref isCheckProtected);
                            }
                        }
                    }
                }
                else if (type == CharacterActionType.PreCastSkill)
                {
                    if (character.CharacterState == CharacterState.NotActionable ||
                        character.CharacterState == CharacterState.ActionRestricted ||
                        character.CharacterState == CharacterState.BattleRestricted ||
                        character.CharacterState == CharacterState.SkillRestricted)
                    {
                        WriteLine($"角色 [ {character} ] 状态为：{CharacterSet.GetCharacterState(character.CharacterState)}，无法释放技能！");
                    }
                    else
                    {
                        // 预使用技能，即开始吟唱逻辑
                        Skill? skill = await OnSelectSkillAsync(character, skills);
                        if (skill is null && _charactersInAI.Contains(character) && skills.Count > 0)
                        {
                            skill = skills[Random.Shared.Next(skills.Count)];
                        }
                        if (skill != null)
                        {
                            // 吟唱前需要先选取目标
                            if (skill.SkillType == SkillType.Magic)
                            {
                                List<Character> targets = await SelectTargetsAsync(character, skill, enemys, teammates);
                                if (targets.Count > 0)
                                {
                                    // 免疫检定
                                    await CheckSkilledImmuneAsync(character, targets, skill);

                                    if (targets.Count > 0)
                                    {
                                        LastRound.Targets = [.. targets];
                                        decided = true;

                                        character.CharacterState = CharacterState.Casting;
                                        SkillTarget skillTarget = new(skill, targets);
                                        await OnCharacterPreCastSkillAsync(character, skillTarget);

                                        _castingSkills[character] = skillTarget;
                                        baseTime = skill.RealCastTime;
                                        skill.OnSkillCasting(this, character, targets);
                                    }
                                }
                            }
                            else
                            {
                                // 只有魔法需要吟唱，战技和爆发技直接释放
                                if (CheckCanCast(character, skill, out double cost))
                                {
                                    List<Character> targets = await SelectTargetsAsync(character, skill, enemys, teammates);
                                    if (targets.Count > 0)
                                    {
                                        // 免疫检定
                                        await CheckSkilledImmuneAsync(character, targets, skill);

                                        if (targets.Count > 0)
                                        {
                                            LastRound.Targets = [.. targets];
                                            decided = true;

                                            SkillTarget skillTarget = new(skill, targets);
                                            await OnCharacterPreCastSkillAsync(character, skillTarget);

                                            skill.OnSkillCasting(this, character, targets);
                                            skill.BeforeSkillCasted();

                                            character.EP -= cost;
                                            baseTime = skill.RealHardnessTime;
                                            skill.CurrentCD = skill.RealCD;
                                            skill.Enable = false;
                                            LastRound.SkillCost = $"{-cost:0.##} EP";
                                            WriteLine($"[ {character} ] 消耗了 {cost:0.##} 点能量，释放了{(skill.IsSuperSkill ? "爆发技" : "战技")} [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}");

                                            await OnCharacterCastSkillAsync(character, skillTarget, cost);

                                            skill.OnSkillCasted(this, character, targets);
                                            effects = [.. character.Effects.Where(e => e.IsInEffect)];
                                            foreach (Effect effect in effects)
                                            {
                                                effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                                            }
                                        }
                                    }
                                }
                            }
                            LastRound.Skill = skill;
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
                        List<Character> targets = [.. skillTarget.Targets.Where(c => !c.IsUnselectable)];

                        // 判断是否能够释放技能
                        if (targets.Count > 0 && CheckCanCast(character, skill, out double cost))
                        {
                            // 免疫检定
                            await CheckSkilledImmuneAsync(character, targets, skill);

                            if (targets.Count > 0)
                            {
                                decided = true;
                                LastRound.Targets = [.. targets];
                                LastRound.Skill = skill;
                                _castingSkills.Remove(character);

                                skill.BeforeSkillCasted();

                                character.MP -= cost;
                                baseTime = skill.RealHardnessTime;
                                skill.CurrentCD = skill.RealCD;
                                skill.Enable = false;
                                LastRound.SkillCost = $"{-cost:0.##} MP";
                                WriteLine($"[ {character} ] 消耗了 {cost:0.##} 点魔法值，释放了魔法 [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}");

                                await OnCharacterCastSkillAsync(character, skillTarget, cost);

                                skill.OnSkillCasted(this, character, targets);
                            }
                        }
                        else
                        {
                            WriteLine($"[ {character} ] 放弃释放技能！");
                            // 放弃释放技能会获得3的硬直时间
                            baseTime = 3;
                        }

                        effects = [.. character.Effects.Where(e => e.IsInEffect)];
                        foreach (Effect effect in effects)
                        {
                            effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
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
                    decided = true;
                    // 结束预释放爆发技的状态
                    character.CharacterState = CharacterState.Actionable;
                    character.UpdateCharacterState();
                    Skill skill = _castingSuperSkills[character];
                    LastRound.Skill = skill;
                    _castingSuperSkills.Remove(character);

                    // 判断是否能够释放技能
                    if (CheckCanCast(character, skill, out double cost))
                    {
                        // 预释放的爆发技不可取消
                        List<Character> targets = await SelectTargetsAsync(character, skill, enemys, teammates);
                        // 免疫检定
                        await CheckSkilledImmuneAsync(character, targets, skill);
                        LastRound.Targets = [.. targets];

                        skill.BeforeSkillCasted();

                        character.EP -= cost;
                        baseTime = skill.RealHardnessTime;
                        skill.CurrentCD = skill.RealCD;
                        skill.Enable = false;
                        LastRound.SkillCost = $"{-cost:0.##} EP";
                        WriteLine($"[ {character} ] 消耗了 {cost:0.##} 点能量值，释放了爆发技 [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}");

                        SkillTarget skillTarget = new(skill, targets);
                        await OnCharacterCastSkillAsync(character, skillTarget, cost);

                        skill.OnSkillCasted(this, character, targets);
                    }
                    else
                    {
                        WriteLine($"[ {character} ] 因能量不足放弃释放爆发技！");
                        // 放弃释放技能会获得3的硬直时间
                        baseTime = 3;
                    }

                    effects = [.. character.Effects.Where(e => e.IsInEffect)];
                    foreach (Effect effect in effects)
                    {
                        effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                    }
                }
                else if (type == CharacterActionType.UseItem)
                {
                    // 使用物品逻辑
                    Item? item = await OnSelectItemAsync(character, items);
                    if (item is null && _charactersInAI.Contains(character) && items.Count > 0)
                    {
                        // AI 控制下随机选取一个物品
                        item = items[Random.Shared.Next(items.Count)];
                    }
                    if (item != null && item.Skills.Active != null)
                    {
                        Skill skill = item.Skills.Active;
                        if (await UseItemAsync(item, character, enemys, teammates))
                        {
                            decided = true;
                            LastRound.Item = item;
                            baseTime = skill.RealHardnessTime > 0 ? skill.RealHardnessTime : 5;
                            effects = [.. character.Effects.Where(e => e.IsInEffect)];
                            foreach (Effect effect in effects)
                            {
                                effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                            }
                        }
                    }
                }
                else if (type == CharacterActionType.EndTurn)
                {
                    baseTime = 3;
                    if (character.CharacterState == CharacterState.NotActionable ||
                        character.CharacterState == CharacterState.ActionRestricted ||
                        character.CharacterState == CharacterState.BattleRestricted)
                    {
                        baseTime += 5;
                        WriteLine($"[ {character} ] {CharacterSet.GetCharacterState(character.CharacterState)}，放弃行动将额外获得 5 {GameplayEquilibriumConstant.InGameTime}硬直时间！");
                    }
                    decided = true;
                    WriteLine($"[ {character} ] 结束了回合！");
                    await OnCharacterDoNothingAsync(character);
                }
                else
                {
                    decided = true;
                    WriteLine($"[ {character} ] 完全行动不能！");
                }
            }

            if (type == CharacterActionType.None)
            {
                WriteLine($"[ {character} ] 放弃了行动！");
                await OnCharacterGiveUpAsync(character);
            }

            _stats[character].ActionTurn += 1;
            LastRound.ActionType = type;

            await AfterCharacterAction(character, type);

            // 统一在回合结束时处理角色的死亡
            await ProcessCharacterDeathAsync(character);

            // 移除回合奖励
            RemoveRoundRewards(character, rewards);

            if (_isGameEnd)
            {
                // 回合结束事件
                await OnTurnEndAsync(character);

                await AfterTurnAsync(character);

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
            await OnQueueUpdatedAsync(_queue, character, newHardnessTime, QueueUpdatedReason.Action, "设置角色行动后的硬直时间。");

            effects = [.. character.Effects];
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
                    }
                }
            }

            // 有人想要插队吗？
            await WillPreCastSuperSkill();

            // 回合结束事件
            await OnTurnEndAsync(character);

            await AfterTurnAsync(character);

            WriteLine("");
            _isInRound = false;
            return _isGameEnd;
        }

        /// <summary>
        /// 处理角色死亡
        /// </summary>
        /// <param name="character"></param>
        protected async Task ProcessCharacterDeathAsync(Character character)
        {
            foreach (Character death in _roundDeaths)
            {
                if (!await OnCharacterDeathAsync(character, death))
                {
                    continue;
                }

                // 给所有角色的特效广播角色死亡结算
                List<Effect> effects = [.. _queue.SelectMany(c => c.Effects.Where(e => e.IsInEffect))];
                foreach (Effect effect in effects)
                {
                    effect.AfterDeathCalculation(death, character, _continuousKilling, _earnedMoney);
                }
                // 将死者移出队列
                _queue.Remove(death);

                await AfterDeathCalculation(death, character);
            }
        }

        /// <summary>
        /// 获取某角色的团队成员
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        protected virtual List<Character> GetTeammates(Character character)
        {
            return [];
        }

        /// <summary>
        /// 角色行动后触发
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        protected virtual async Task AfterCharacterAction(Character character, CharacterActionType type)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 死亡结算时触发
        /// </summary>
        /// <param name="death"></param>
        /// <param name="killer"></param>
        protected virtual async Task OnDeathCalculation(Character death, Character killer)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 死亡结算完成后触发
        /// </summary>
        /// <param name="death"></param>
        /// <param name="killer"></param>
        protected virtual async Task AfterDeathCalculation(Character death, Character killer)
        {
            if (!_queue.Where(c => c != killer).Any())
            {
                // 没有其他的角色了，游戏结束
                WriteLine("[ " + killer + " ] 是胜利者。");
                _queue.Remove(killer);
                _eliminated.Add(killer);
                _isGameEnd = true;
                await OnGameEndAsync(killer);
            }
        }

        /// <summary>
        /// 回合开始前触发
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<bool> BeforeTurnAsync(Character character)
        {
            return await Task.FromResult(true);
        }

        /// <summary>
        /// 回合结束后触发
        /// </summary>
        /// <returns></returns>
        protected virtual async Task AfterTurnAsync(Character character)
        {
            await Task.CompletedTask;
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
        public async Task DamageToEnemyAsync(Character actor, Character enemy, double damage, bool isNormalAttack, DamageType damageType = DamageType.Physical, MagicType magicType = MagicType.None, DamageResult damageResult = DamageResult.Normal)
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

            // 真实伤害跳过伤害加成区间
            if (damageType != DamageType.True)
            {
                Dictionary<Effect, double> totalDamageBonus = [];
                effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
                foreach (Effect effect in effects)
                {
                    double damageBonus = effect.AlterActualDamageAfterCalculation(actor, enemy, damage, isNormalAttack, damageType, magicType, damageResult, ref isEvaded, totalDamageBonus);
                    totalDamageBonus[effect] = damageBonus;
                    if (isEvaded)
                    {
                        damageResult = DamageResult.Evaded;
                    }
                }
                damage += totalDamageBonus.Sum(kv => kv.Value);
            }
            double actualDamage = damage;

            // 闪避了就没伤害了
            if (damageResult != DamageResult.Evaded)
            {
                // 开始计算伤害免疫
                bool isImmune = false;
                // 真实伤害跳过免疫
                if (damageType != DamageType.True)
                {
                    // 此变量为是否无视免疫
                    bool ignore = false;
                    // 技能免疫无法免疫普通攻击，但是魔法免疫和物理免疫可以
                    isImmune = (isNormalAttack && (enemy.ImmuneType == ImmuneType.All || enemy.ImmuneType == ImmuneType.Physical || enemy.ImmuneType == ImmuneType.Magical)) ||
                        (!isNormalAttack && (enemy.ImmuneType == ImmuneType.All || enemy.ImmuneType == ImmuneType.Physical || enemy.ImmuneType == ImmuneType.Magical || enemy.ImmuneType == ImmuneType.Skilled));
                    if (isImmune)
                    {
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
                        foreach (Effect effect in effects)
                        {
                            if (isNormalAttack)
                            {
                                if (actor.NormalAttack.IgnoreImmune == ImmuneType.All ||
                                    (damageType == DamageType.Physical && actor.NormalAttack.IgnoreImmune == ImmuneType.Physical) ||
                                    (damageType == DamageType.Magical && actor.NormalAttack.IgnoreImmune == ImmuneType.Magical) ||
                                    !effect.OnDamageImmuneCheck(actor, enemy, isNormalAttack, damageType, magicType, damage))
                                {
                                    ignore = true;
                                }
                            }
                            else
                            {
                                if (!effect.OnDamageImmuneCheck(actor, enemy, isNormalAttack, damageType, magicType, damage))
                                {
                                    ignore = true;
                                }
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

                    // 真实伤害跳过护盾结算
                    if (damageType != DamageType.True)
                    {
                        // 在护盾结算前，特效可以有自己的逻辑
                        bool change = false;
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
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
                            effects = [.. enemy.Shield.ShieldOfEffects.Keys];
                            foreach (Effect effect in effects)
                            {
                                ShieldOfEffect soe = enemy.Shield.ShieldOfEffects[effect];
                                if (soe.IsMagic == (damageType == DamageType.Magical) && (damageType == DamageType.Physical || soe.MagicType == magicType) && soe.Shield > 0)
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
                                        Effect[] effects2 = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
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
                                        Effect[] effects2 = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
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
                                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
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
                                        if (isMagicDamage && enemy.Shield.TotalMagicial <= 0 || !isMagicDamage && enemy.Shield.TotalPhysical <= 0)
                                        {
                                            effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
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
                                if (remain > 0 && enemy.Shield.Mix > 0)
                                {
                                    shield = enemy.Shield.Mix;
                                    shield -= remain;
                                    if (shield > 0)
                                    {
                                        WriteLine($"[ {enemy} ] 的混合护盾抵消了 {remain:0.##} 点{damageTypeString}！");
                                        enemy.Shield.Mix -= remain;
                                        remain = 0;
                                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
                                        foreach (Effect effect in effects)
                                        {
                                            effect.OnShieldNeutralizeDamage(enemy, actor, damageType, magicType, damage, ShieldType.Mix);
                                        }
                                    }
                                    else
                                    {
                                        WriteLine($"[ {enemy} ] 的混合护盾抵消了 {enemy.Shield.Mix:0.##} 点{damageTypeString}并破碎！");
                                        remain -= enemy.Shield.Mix;
                                        enemy.Shield.Mix = 0;
                                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
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
                        if (damage > actualDamage && _stats.TryGetValue(actor, out CharacterStatistics? stats) && stats != null)
                        {
                            stats.TotalShield += damage - actualDamage;
                        }
                    }

                    enemy.HP -= actualDamage;
                    WriteLine($"[ {enemy} ] 受到了 {actualDamage:0.##} 点{damageTypeString}！{shieldMsg}");

                    // 生命偷取，攻击者为全额
                    double steal = damage * actor.Lifesteal;
                    await HealToTargetAsync(actor, actor, steal, false);
                    effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
                    foreach (Effect effect in effects)
                    {
                        effect.AfterLifesteal(actor, enemy, damage, steal);
                    }

                    // 造成伤害和受伤都可以获得能量。攻击者为全额，被攻击者为 actualDamage，护盾抵消的伤害不算
                    double ep = GetEP(damage, GameplayEquilibriumConstant.DamageGetEPFactor, GameplayEquilibriumConstant.DamageGetEPMax);
                    if (ep > 0)
                    {
                        effects = [.. actor.Effects.Where(e => e.IsInEffect)];
                        foreach (Effect effect in effects)
                        {
                            effect.AlterEPAfterDamage(actor, ref ep);
                        }
                        actor.EP += ep;
                    }
                    ep = GetEP(actualDamage, GameplayEquilibriumConstant.TakenDamageGetEPFactor, GameplayEquilibriumConstant.TakenDamageGetEPMax);
                    if (ep > 0)
                    {
                        effects = [.. enemy.Effects.Where(e => e.IsInEffect)];
                        foreach (Effect effect in effects)
                        {
                            effect.AlterEPAfterGetDamage(enemy, ref ep);
                        }
                        enemy.EP += ep;
                    }

                    // 统计伤害
                    CalculateCharacterDamageStatistics(actor, enemy, damage, damageType, actualDamage);

                    // 计算助攻
                    _assistDetail[actor][enemy, TotalTime] += damage;
                }
            }
            else
            {
                LastRound.IsEvaded[enemy] = true;
                actualDamage = 0;
            }

            await OnDamageToEnemyAsync(actor, enemy, damage, actualDamage, isNormalAttack, damageType, magicType, damageResult);

            effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
            foreach (Effect effect in effects)
            {
                effect.AfterDamageCalculation(actor, enemy, damage, actualDamage, isNormalAttack, damageType, magicType, damageResult);
            }

            if (enemy.HP <= 0 && !_eliminated.Contains(enemy) && !_respawnCountdown.ContainsKey(enemy))
            {
                LastRound.HasKill = true;
                _roundDeaths.Add(enemy);
                await DeathCalculationAsync(actor, enemy);
            }
        }

        /// <summary>
        /// 治疗一个目标
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="target"></param>
        /// <param name="heal"></param>
        /// <param name="canRespawn"></param>
        public async Task HealToTargetAsync(Character actor, Character target, double heal, bool canRespawn = false)
        {
            if (target.HP == target.MaxHP)
            {
                return;
            }

            bool isDead = target.HP <= 0;

            Dictionary<Effect, double> totalHealBonus = [];
            List<Effect> effects = [.. actor.Effects.Union(target.Effects).Distinct().Where(e => e.IsInEffect)];
            foreach (Effect effect in effects)
            {
                bool changeCanRespawn = false;
                double healBonus = effect.AlterHealValueBeforeHealToTarget(actor, target, heal, ref changeCanRespawn, totalHealBonus);
                if (changeCanRespawn && !canRespawn)
                {
                    canRespawn = true;
                }
            }
            heal += totalHealBonus.Sum(kv => kv.Value);

            if (heal <= 0)
            {
                return;
            }

            double realHeal = heal;
            if (target.HP > 0 || (isDead && canRespawn))
            {
                // 用于数据统计，不能是全额，溢出的部分需要扣除
                if (target.HP + heal > target.MaxHP)
                {
                    realHeal = target.MaxHP - target.HP;
                }
                target.HP += heal;
                if (!LastRound.Heals.TryAdd(target, heal))
                {
                    LastRound.Heals[target] += heal;
                }
            }

            bool isRespawn = isDead && canRespawn;
            if (isRespawn)
            {
                if (target != actor)
                {
                    WriteLine($"[ {target} ] 被 [ {actor} ] 复苏了，并回复了 {heal:0.##} 点生命值！！");
                }
                else
                {
                    WriteLine($"[ {target} ] 复苏了，并回复了 {heal:0.##} 点生命值！！");
                }
                double hp = target.HP;
                double mp = target.MP;
                await SetCharacterRespawn(target);
                target.HP = hp;
                target.MP = mp;
            }
            else
            {
                WriteLine($"[ {target} ] 回复了 {heal:0.##} 点生命值！");
            }

            // 添加助攻
            SetNotDamageAssistTime(actor, target);

            // 统计数据
            if (_stats.TryGetValue(actor, out CharacterStatistics? stats) && stats != null)
            {
                stats.TotalHeal += realHeal;
            }

            await OnHealToTargetAsync(actor, target, heal, isRespawn);
        }

        /// <summary>
        /// 死亡结算
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="death"></param>
        public async Task DeathCalculationAsync(Character killer, Character death)
        {
            if (!await OnDeathCalculationAsync(killer, death))
            {
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
                    _stats[assist].Assists += 1;
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

            await OnDeathCalculation(death, killer);

            death.EP = 0;

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
                double respawnTime = Calculation.Round2Digits(Math.Min(30, death.Level * 0.15 + times * 0.87 + coefficient));
                _respawnCountdown.TryAdd(death, respawnTime);
                LastRound.RespawnCountdowns.TryAdd(death, respawnTime);
                WriteLine($"[ {death} ] 进入复活倒计时：{respawnTime:0.##} {GameplayEquilibriumConstant.InGameTime}！");
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

        #endregion

        #region 回合内-辅助方法

        /// <summary>
        /// 使用物品实际逻辑
        /// </summary>
        /// <param name="item"></param>
        /// <param name="character"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <returns></returns>
        public async Task<bool> UseItemAsync(Item item, Character character, List<Character> enemys, List<Character> teammates)
        {
            if (CheckCanCast(character, item, out double costMP, out double costEP))
            {
                Skill? skill = item.Skills.Active;
                if (skill != null)
                {
                    List<Character> targets = await SelectTargetsAsync(character, skill, enemys, teammates);
                    if (targets.Count > 0)
                    {
                        // 免疫检定
                        await CheckSkilledImmuneAsync(character, targets, skill, item);

                        if (targets.Count > 0)
                        {
                            LastRound.Targets = [.. targets];

                            WriteLine($"[ {character} ] 使用了物品 [ {item.Name} ]！");
                            item.ReduceTimesAndRemove();
                            if (item.IsReduceTimesAfterUse && item.RemainUseTimes == 0)
                            {
                                character.Items.Remove(item);
                            }
                            await OnCharacterUseItemAsync(character, item, targets);

                            skill.OnSkillCasting(this, character, targets);
                            skill.BeforeSkillCasted();

                            skill.CurrentCD = skill.RealCD;
                            skill.Enable = false;

                            string line = $"[ {character} ] ";
                            if (costMP > 0)
                            {
                                character.MP -= costMP;
                                LastRound.SkillCost = $"{-costMP:0.##} MP";
                                line += $"消耗了 {costMP:0.##} 点魔法值，";
                            }

                            if (costEP > 0)
                            {
                                character.EP -= costEP;
                                if (LastRound.SkillCost != "") LastRound.SkillCost += " / ";
                                LastRound.SkillCost += $"{-costEP:0.##} EP";
                                line += $"消耗了 {costEP:0.##} 点能量，";
                            }

                            line += $"释放了物品技能 [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}";
                            WriteLine(line);

                            SkillTarget skillTarget = new(skill, targets);
                            await OnCharacterCastItemSkillAsync(character, item, skillTarget, costMP, costEP);

                            skill.OnSkillCasted(this, character, targets);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 通过概率计算角色要干嘛
        /// </summary>
        /// <param name="pUseItem"></param>
        /// <param name="pCastSkill"></param>
        /// <param name="pNormalAttack"></param>
        /// <returns></returns>
        public static CharacterActionType GetActionType(double pUseItem, double pCastSkill, double pNormalAttack)
        {
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
        /// 选取技能目标
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <returns></returns>
        public async Task<List<Character>> SelectTargetsAsync(Character caster, Skill skill, List<Character> enemys, List<Character> teammates)
        {
            List<Effect> effects = [.. caster.Effects.Where(e => e.IsInEffect)];
            foreach (Effect effect in effects)
            {
                effect.AlterSelectListBeforeSelection(caster, skill, enemys, teammates);
            }
            List<Character> targets = await OnSelectSkillTargetsAsync(caster, skill, enemys, teammates);
            if (targets.Count == 0 && _charactersInAI.Contains(caster))
            {
                targets = skill.SelectTargets(caster, enemys, teammates);
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
        /// <returns></returns>
        public async Task<List<Character>> SelectTargetsAsync(Character character, NormalAttack attack, List<Character> enemys, List<Character> teammates)
        {
            List<Effect> effects = [.. character.Effects.Where(e => e.IsInEffect)];
            foreach (Effect effect in effects)
            {
                effect.AlterSelectListBeforeSelection(character, attack, enemys, teammates);
            }
            List<Character> targets = await OnSelectNormalAttackTargetsAsync(character, attack, enemys, teammates);
            if (targets.Count == 0 && _charactersInAI.Contains(character))
            {
                targets = character.NormalAttack.GetSelectableTargets(character, enemys, teammates);
                if (targets.Count > 0)
                {
                    targets = [targets[Random.Shared.Next(targets.Count)]];
                }
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
        /// <returns></returns>
        public DamageResult CalculatePhysicalDamage(Character actor, Character enemy, bool isNormalAttack, double expectedDamage, out double finalDamage, ref int changeCount)
        {
            List<Character> characters = [actor, enemy];
            DamageType damageType = DamageType.Physical;
            MagicType magicType = MagicType.None;
            List<Effect> effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
            if (changeCount < 3)
            {
                foreach (Effect effect in effects)
                {
                    effect.AlterDamageTypeBeforeCalculation(actor, enemy, ref isNormalAttack, ref damageType, ref magicType);
                }
                if (damageType == DamageType.Magical)
                {
                    changeCount++;
                    return CalculateMagicalDamage(actor, enemy, isNormalAttack, magicType, expectedDamage, out finalDamage, ref changeCount);
                }
            }

            Dictionary<Effect, double> totalDamageBonus = [];
            effects = [.. actor.Effects.Union(enemy.Effects).Distinct().Where(e => e.IsInEffect)];
            foreach (Effect effect in effects)
            {
                double damageBonus = effect.AlterExpectedDamageBeforeCalculation(actor, enemy, expectedDamage, isNormalAttack, DamageType.Physical, MagicType.None, totalDamageBonus);
                totalDamageBonus[effect] = damageBonus;
            }
            expectedDamage += totalDamageBonus.Sum(kv => kv.Value);

            double dice = Random.Shared.NextDouble();
            double throwingBonus = 0;
            bool checkEvade = true;
            bool checkCritical = true;
            if (isNormalAttack)
            {
                effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
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
                        finalDamage = 0;
                        bool isAlterEvaded = false;
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
                        foreach (Effect effect in effects)
                        {
                            if (effect.OnEvadedTriggered(actor, enemy, dice))
                            {
                                isAlterEvaded = true;
                            }
                        }
                        if (!isAlterEvaded)
                        {
                            WriteLine("此物理攻击被完美闪避了！");
                            return DamageResult.Evaded;
                        }
                    }
                }
            }

            // 物理穿透后的护甲
            double penetratedDEF = (1 - actor.PhysicalPenetration) * enemy.DEF;

            // 物理伤害减免
            double physicalDamageReduction = penetratedDEF / (penetratedDEF + GameplayEquilibriumConstant.DEFReductionFactor);

            // 最终的物理伤害
            finalDamage = expectedDamage * (1 - Calculation.PercentageCheck(physicalDamageReduction + enemy.ExPDR));

            // 暴击检定
            effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
            foreach (Effect effect in effects)
            {
                if (!effect.BeforeCriticalCheck(actor, enemy, ref throwingBonus))
                {
                    checkCritical = false;
                }
            }

            if (checkCritical)
            {
                dice = Random.Shared.NextDouble();
                if (dice < (actor.CritRate + throwingBonus))
                {
                    finalDamage *= actor.CritDMG; // 暴击伤害倍率加成
                    WriteLine("暴击生效！！");
                    effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
                    foreach (Effect effect in effects)
                    {
                        effect.OnCriticalDamageTriggered(actor, enemy, dice);
                    }
                    return DamageResult.Critical;
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
        /// <returns></returns>
        public DamageResult CalculateMagicalDamage(Character actor, Character enemy, bool isNormalAttack, MagicType magicType, double expectedDamage, out double finalDamage, ref int changeCount)
        {
            List<Character> characters = [actor, enemy];
            DamageType damageType = DamageType.Magical;
            List<Effect> effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
            if (changeCount < 3)
            {
                foreach (Effect effect in effects)
                {
                    effect.AlterDamageTypeBeforeCalculation(actor, enemy, ref isNormalAttack, ref damageType, ref magicType);
                }
                if (damageType == DamageType.Physical)
                {
                    changeCount++;
                    return CalculatePhysicalDamage(actor, enemy, isNormalAttack, expectedDamage, out finalDamage, ref changeCount);
                }
            }

            Dictionary<Effect, double> totalDamageBonus = [];
            effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
            foreach (Effect effect in effects)
            {
                double damageBonus = effect.AlterExpectedDamageBeforeCalculation(actor, enemy, expectedDamage, isNormalAttack, DamageType.Magical, magicType, totalDamageBonus);
                totalDamageBonus[effect] = damageBonus;
            }
            expectedDamage += totalDamageBonus.Sum(kv => kv.Value);

            double dice = Random.Shared.NextDouble();
            double throwingBonus = 0;
            bool checkEvade = true;
            bool checkCritical = true;
            if (isNormalAttack)
            {
                effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
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
                        finalDamage = 0;
                        bool isAlterEvaded = false;
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
                        foreach (Effect effect in effects)
                        {
                            if (effect.OnEvadedTriggered(actor, enemy, dice))
                            {
                                isAlterEvaded = true;
                            }
                        }
                        if (!isAlterEvaded)
                        {
                            WriteLine("此魔法攻击被完美闪避了！");
                            return DamageResult.Evaded;
                        }
                    }
                }
            }

            double MDF = enemy.MDF[magicType];

            // 魔法穿透后的魔法抗性
            MDF = (1 - actor.MagicalPenetration) * MDF;

            // 最终的魔法伤害
            finalDamage = expectedDamage * (1 - MDF);

            // 暴击检定
            effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
            foreach (Effect effect in effects)
            {
                if (!effect.BeforeCriticalCheck(actor, enemy, ref throwingBonus))
                {
                    checkCritical = false;
                }
            }

            if (checkCritical)
            {
                dice = Random.Shared.NextDouble();
                if (dice < (actor.CritRate + throwingBonus))
                {
                    finalDamage *= actor.CritDMG; // 暴击伤害倍率加成
                    WriteLine("暴击生效！！");
                    effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
                    foreach (Effect effect in effects)
                    {
                        effect.OnCriticalDamageTriggered(actor, enemy, dice);
                    }
                    return DamageResult.Critical;
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
        /// 判断目标对于某个角色是否是队友
        /// </summary>
        /// <param name="character"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsTeammate(Character character, Character target)
        {
            List<Character> teammates = GetTeammates(character);
            return teammates.Contains(target);
        }

        /// <summary>
        /// 获取目标对于某个角色是否是友方的字典
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
                dict[target] = teammates.Contains(target);
            }
            return dict;
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
                        LastRound.Targets.Add(character);
                        skill.OnSkillCasted(this, character, [character]);
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
        protected async Task WillPreCastSuperSkill()
        {
            // 选取所有 AI 控制角色
            foreach (Character other in _queue.Where(c => c.CharacterState == CharacterState.Actionable && _charactersInAI.Contains(c)).ToList())
            {
                // 有 65% 欲望插队
                if (Random.Shared.NextDouble() < 0.65)
                {
                    List<Skill> skills = [.. other.Skills.Where(s => s.Level > 0 && s.SkillType == SkillType.SuperSkill && s.Enable && !s.IsInEffect && s.CurrentCD == 0 && other.EP >= s.RealEPCost)];
                    if (skills.Count > 0)
                    {
                        Skill skill = skills[Random.Shared.Next(skills.Count)];
                        await SetCharacterPreCastSuperSkill(other, skill);
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
        public async Task InterruptCastingAsync(Character caster, Character interrupter)
        {
            Skill? skill = null;
            if (_castingSkills.TryGetValue(caster, out SkillTarget target))
            {
                skill = target.Skill;
                _castingSkills.Remove(caster);
            }
            if (skill is null && caster.CharacterState == CharacterState.PreCastSuperSkill)
            {
                WriteLine($"因 [ {caster} ] 的预释放爆发技状态不可驱散，[ {interrupter} ] 打断失败！！");
            }
            if (skill != null)
            {
                WriteLine($"[ {caster} ] 的施法被 [ {interrupter} ] 打断了！！");
                List<Effect> effects = [.. caster.Effects.Union(interrupter.Effects).Distinct().Where(e => e.IsInEffect)];
                foreach (Effect effect in effects)
                {
                    effect.OnSkillCastInterrupted(caster, skill, interrupter);
                }
                await OnInterruptCastingAsync(caster, skill, interrupter);
            }
        }

        /// <summary>
        /// 打断施法 [ 用于使敌人目标丢失 ]
        /// </summary>
        /// <param name="interrupter"></param>
        public async Task InterruptCastingAsync(Character interrupter)
        {
            foreach (Character caster in _castingSkills.Keys)
            {
                SkillTarget skillTarget = _castingSkills[caster];
                if (skillTarget.Targets.Contains(interrupter))
                {
                    Skill skill = skillTarget.Skill;
                    WriteLine($"[ {interrupter} ] 打断了 [ {caster} ] 的施法！！");
                    List<Effect> effects = [.. caster.Effects.Union(interrupter.Effects).Distinct().Where(e => e.IsInEffect)];
                    foreach (Effect effect in effects)
                    {
                        effect.OnSkillCastInterrupted(caster, skill, interrupter);
                    }
                    await OnInterruptCastingAsync(caster, skill, interrupter);
                }
            }
        }

        /// <summary>
        /// 设置角色复活
        /// </summary>
        /// <param name="character"></param>
        public async Task SetCharacterRespawn(Character character)
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
            await OnQueueUpdatedAsync(_queue, character, hardnessTime, QueueUpdatedReason.Respawn, "设置角色复活后的硬直时间。");
        }

        /// <summary>
        /// 设置角色将预释放爆发技
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skill"></param>
        public async Task SetCharacterPreCastSuperSkill(Character character, Skill skill)
        {
            if (character.CharacterState == CharacterState.Actionable)
            {
                _castingSuperSkills[character] = skill;
                character.CharacterState = CharacterState.PreCastSuperSkill;
                _queue.Remove(character);
                _cutCount.Remove(character);
                WriteLine("[ " + character + " ] 预释放了爆发技！！");
                int preCastSSCount = 0;
                double baseHardnessTime = 0;
                foreach (Character c in _hardnessTimes.Keys)
                {
                    if (c.CharacterState != CharacterState.PreCastSuperSkill)
                    {
                        _hardnessTimes[c] = Calculation.Round2Digits(_hardnessTimes[c] + 0.01);
                    }
                    else if (c != character)
                    {
                        if (preCastSSCount == 0)
                        {
                            baseHardnessTime = _hardnessTimes[c];
                        }
                        preCastSSCount++;
                    }
                }
                AddCharacter(character, Calculation.Round2Digits(baseHardnessTime + preCastSSCount * 0.01), false);
                skill.OnSkillCasting(this, character, []);
                await OnQueueUpdatedAsync(_queue, character, 0, QueueUpdatedReason.PreCastSuperSkill, "设置角色预释放爆发技的硬直时间。");
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
                if (character == target) continue;
                _assistDetail[character].NotDamageAssistLastTime[target] = TotalTime;
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
            double hardnessTime = _hardnessTimes[character];
            if (isPercentage)
            {
                addValue = hardnessTime * addValue;
            }
            hardnessTime += addValue;
            if (hardnessTime <= 0) hardnessTime = 0;
            AddCharacter(character, hardnessTime, isCheckProtected);
        }

        #endregion

        #region 自动化

        /// <summary>
        /// 设置角色为 AI 控制
        /// </summary>
        /// <param name="cancel"></param>
        /// <param name="characters"></param>
        public void SetCharactersToAIControl(bool cancel = false, params IEnumerable<Character> characters)
        {
            foreach (Character character in characters)
            {
                if (cancel)
                {
                    _charactersInAI.Remove(character);
                }
                else
                {
                    _charactersInAI.Add(character);
                }
            }
        }

        /// <summary>
        /// 检查角色是否在 AI 控制状态
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool IsCharacterInAIControlling(Character character)
        {
            return _charactersInAI.Contains(character);
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
        protected async Task CheckSkilledImmuneAsync(Character character, List<Character> targets, Skill skill, Item? item = null)
        {
            Character[] loop = [.. targets];
            foreach (Character target in loop)
            {
                bool ignore = false;
                bool isImmune = target.ImmuneType == ImmuneType.Magical || target.ImmuneType == ImmuneType.Skilled || target.ImmuneType == ImmuneType.All;
                if (isImmune)
                {
                    Character[] characters = [character, target];
                    Effect[] effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Distinct()];
                    foreach (Effect effect in effects)
                    {
                        // 自带无视免疫或者特效免疫检定不通过可无视免疫
                        if (effect.IgnoreImmune == ImmuneType.All || effect.IgnoreImmune == ImmuneType.Skilled || (skill.IsMagic && effect.IgnoreImmune == ImmuneType.Magical) || !effect.OnImmuneCheck(character, target, skill, item))
                        {
                            ignore = true;
                        }
                    }
                }
                if (ignore)
                {
                    isImmune = false;
                    targets.Remove(target);
                }
                if (isImmune)
                {
                    WriteLine($"[ {target} ] 免疫了此技能！");
                    await OnCharacterImmunedAsync(character, target, skill, item);
                }
            }
            await Task.CompletedTask;
            return;
        }

        #endregion

        #region 数据统计

        /// <summary>
        /// 计算角色的数据
        /// </summary>
        public void CalculateCharacterDamageStatistics(Character character, Character characterTaken, double damage, DamageType damageType, double takenDamage = -1)
        {
            if (takenDamage == -1) takenDamage = damage;
            if (_stats.TryGetValue(character, out CharacterStatistics? stats) && stats != null)
            {
                if (damageType == DamageType.True)
                {
                    stats.TotalTrueDamage += damage;
                }
                if (damageType == DamageType.Magical)
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
                if (damageType == DamageType.Magical)
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

        #endregion

        #region 事件

        public delegate Task<bool> TurnStartEventHandler(GamingQueue queue, Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items);
        /// <summary>
        /// 回合开始事件
        /// </summary>
        public event TurnStartEventHandler? TurnStart;
        /// <summary>
        /// 回合开始事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="skills"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        protected async Task<bool> OnTurnStartAsync(Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items)
        {
            return await (TurnStart?.Invoke(this, character, enemys, teammates, skills, items) ?? Task.FromResult(true));
        }

        public delegate Task TurnEndEventHandler(GamingQueue queue, Character character);
        /// <summary>
        /// 回合结束事件
        /// </summary>
        public event TurnEndEventHandler? TurnEnd;
        /// <summary>
        /// 回合结束事件
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        protected async Task OnTurnEndAsync(Character character)
        {
            await (TurnEnd?.Invoke(this, character) ?? Task.CompletedTask);
        }

        public delegate Task<CharacterActionType> DecideActionEventHandler(GamingQueue queue, Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items);
        /// <summary>
        /// 决定角色的行动事件
        /// </summary>
        public event DecideActionEventHandler? DecideAction;
        /// <summary>
        /// 决定角色的行动事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="skills"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        protected async Task<CharacterActionType> OnDecideActionAsync(Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items)
        {
            return await (DecideAction?.Invoke(this, character, enemys, teammates, skills, items) ?? Task.FromResult(CharacterActionType.None));
        }

        public delegate Task<Skill?> SelectSkillEventHandler(GamingQueue queue, Character character, List<Skill> skills);
        /// <summary>
        /// 角色需要选择一个技能
        /// </summary>
        public event SelectSkillEventHandler? SelectSkill;
        /// <summary>
        /// 角色需要选择一个技能
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skills"></param>
        /// <returns></returns>
        protected async Task<Skill?> OnSelectSkillAsync(Character character, List<Skill> skills)
        {
            return await (SelectSkill?.Invoke(this, character, skills) ?? Task.FromResult<Skill?>(null));
        }

        public delegate Task<Item?> SelectItemEventHandler(GamingQueue queue, Character character, List<Item> items);
        /// <summary>
        /// 角色需要选择一个物品
        /// </summary>
        public event SelectItemEventHandler? SelectItem;
        /// <summary>
        /// 角色需要选择一个物品
        /// </summary>
        /// <param name="character"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        protected async Task<Item?> OnSelectItemAsync(Character character, List<Item> items)
        {
            return await (SelectItem?.Invoke(this, character, items) ?? Task.FromResult<Item?>(null));
        }

        public delegate Task<List<Character>> SelectSkillTargetsEventHandler(GamingQueue queue, Character caster, Skill skill, List<Character> enemys, List<Character> teammates);
        /// <summary>
        /// 选取技能目标事件
        /// </summary>
        public event SelectSkillTargetsEventHandler? SelectSkillTargets;
        /// <summary>
        /// 选取技能目标事件
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <returns></returns>
        protected async Task<List<Character>> OnSelectSkillTargetsAsync(Character caster, Skill skill, List<Character> enemys, List<Character> teammates)
        {
            return await (SelectSkillTargets?.Invoke(this, caster, skill, enemys, teammates) ?? Task.FromResult(new List<Character>()));
        }

        public delegate Task<List<Character>> SelectNormalAttackTargetsEventHandler(GamingQueue queue, Character character, NormalAttack attack, List<Character> enemys, List<Character> teammates);
        /// <summary>
        /// 选取普通攻击目标事件
        /// </summary>
        public event SelectNormalAttackTargetsEventHandler? SelectNormalAttackTargets;
        /// <summary>
        /// 选取普通攻击目标事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="attack"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <returns></returns>
        protected async Task<List<Character>> OnSelectNormalAttackTargetsAsync(Character character, NormalAttack attack, List<Character> enemys, List<Character> teammates)
        {
            return await (SelectNormalAttackTargets?.Invoke(this, character, attack, enemys, teammates) ?? Task.FromResult(new List<Character>()));
        }

        public delegate Task InterruptCastingEventHandler(GamingQueue queue, Character cast, Skill? skill, Character interrupter);
        /// <summary>
        /// 打断施法事件
        /// </summary>
        public event InterruptCastingEventHandler? InterruptCasting;
        /// <summary>
        /// 打断施法事件
        /// </summary>
        /// <param name="cast"></param>
        /// <param name="skill"></param>
        /// <param name="interrupter"></param>
        /// <returns></returns>
        protected async Task OnInterruptCastingAsync(Character cast, Skill skill, Character interrupter)
        {
            await (InterruptCasting?.Invoke(this, cast, skill, interrupter) ?? Task.CompletedTask);
        }

        public delegate Task<bool> DeathCalculationEventHandler(GamingQueue queue, Character killer, Character death);
        /// <summary>
        /// 死亡结算事件
        /// </summary>
        public event DeathCalculationEventHandler? DeathCalculation;
        /// <summary>
        /// 死亡结算事件
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="death"></param>
        /// <returns></returns>
        protected async Task<bool> OnDeathCalculationAsync(Character killer, Character death)
        {
            return await (DeathCalculation?.Invoke(this, killer, death) ?? Task.FromResult(true));
        }

        public delegate Task<bool> CharacterDeathEventHandler(GamingQueue queue, Character current, Character death);
        /// <summary>
        /// 角色死亡事件，此事件位于 <see cref="DeathCalculation"/> 之后
        /// </summary>
        public event CharacterDeathEventHandler? CharacterDeath;
        /// <summary>
        /// 角色死亡事件，此事件位于 <see cref="DeathCalculation"/> 之后
        /// </summary>
        /// <param name="current"></param>
        /// <param name="death"></param>
        /// <returns></returns>
        protected async Task<bool> OnCharacterDeathAsync(Character current, Character death)
        {
            return await (CharacterDeath?.Invoke(this, current, death) ?? Task.FromResult(true));
        }

        public delegate Task HealToTargetEventHandler(GamingQueue queue, Character actor, Character target, double heal, bool isRespawn);
        /// <summary>
        /// 治疗事件
        /// </summary>
        public event HealToTargetEventHandler? HealToTarget;
        /// <summary>
        /// 治疗事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="target"></param>
        /// <param name="heal"></param>
        /// <param name="isRespawn"></param>
        /// <returns></returns>
        protected async Task OnHealToTargetAsync(Character actor, Character target, double heal, bool isRespawn)
        {
            await (HealToTarget?.Invoke(this, actor, target, heal, isRespawn) ?? Task.CompletedTask);
        }

        public delegate Task DamageToEnemyEventHandler(GamingQueue queue, Character actor, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult);
        /// <summary>
        /// 造成伤害事件
        /// </summary>
        public event DamageToEnemyEventHandler? DamageToEnemy;
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
        protected async Task OnDamageToEnemyAsync(Character actor, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            await (DamageToEnemy?.Invoke(this, actor, enemy, damage, actualDamage, isNormalAttack, damageType, magicType, damageResult) ?? Task.CompletedTask);
        }

        public delegate Task CharacterNormalAttackEventHandler(GamingQueue queue, Character actor, List<Character> targets);
        /// <summary>
        /// 角色普通攻击事件
        /// </summary>
        public event CharacterNormalAttackEventHandler? CharacterNormalAttack;
        /// <summary>
        /// 角色普通攻击事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        protected async Task OnCharacterNormalAttackAsync(Character actor, List<Character> targets)
        {
            await (CharacterNormalAttack?.Invoke(this, actor, targets) ?? Task.CompletedTask);
        }

        public delegate Task CharacterPreCastSkillEventHandler(GamingQueue queue, Character actor, SkillTarget skillTarget);
        /// <summary>
        /// 角色吟唱技能事件（包括直接释放战技）
        /// </summary>
        public event CharacterPreCastSkillEventHandler? CharacterPreCastSkill;
        /// <summary>
        /// 角色吟唱技能事件（包括直接释放战技）
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="skillTarget"></param>
        /// <returns></returns>
        protected async Task OnCharacterPreCastSkillAsync(Character actor, SkillTarget skillTarget)
        {
            await (CharacterPreCastSkill?.Invoke(this, actor, skillTarget) ?? Task.CompletedTask);
        }

        public delegate Task CharacterCastSkillEventHandler(GamingQueue queue, Character actor, SkillTarget skillTarget, double cost);
        /// <summary>
        /// 角色释放技能事件
        /// </summary>
        public event CharacterCastSkillEventHandler? CharacterCastSkill;
        /// <summary>
        /// 角色释放技能事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="skillTarget"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        protected async Task OnCharacterCastSkillAsync(Character actor, SkillTarget skillTarget, double cost)
        {
            await (CharacterCastSkill?.Invoke(this, actor, skillTarget, cost) ?? Task.CompletedTask);
        }

        public delegate Task CharacterUseItemEventHandler(GamingQueue queue, Character actor, Item item, List<Character> targets);
        /// <summary>
        /// 角色使用物品事件
        /// </summary>
        public event CharacterUseItemEventHandler? CharacterUseItem;
        /// <summary>
        /// 角色使用物品事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="item"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        protected async Task OnCharacterUseItemAsync(Character actor, Item item, List<Character> targets)
        {
            await (CharacterUseItem?.Invoke(this, actor, item, targets) ?? Task.CompletedTask);
        }

        public delegate Task CharacterCastItemSkillEventHandler(GamingQueue queue, Character actor, Item item, SkillTarget skillTarget, double costMP, double costEP);
        /// <summary>
        /// 角色释放物品的技能事件
        /// </summary>
        public event CharacterCastItemSkillEventHandler? CharacterCastItemSkill;
        /// <summary>
        /// 角色释放物品的技能事件
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="item"></param>
        /// <param name="skillTarget"></param>
        /// <param name="costMP"></param>
        /// <param name="costEP"></param>
        /// <returns></returns>
        protected async Task OnCharacterCastItemSkillAsync(Character actor, Item item, SkillTarget skillTarget, double costMP, double costEP)
        {
            await (CharacterCastItemSkill?.Invoke(this, actor, item, skillTarget, costMP, costEP) ?? Task.CompletedTask);
        }

        public delegate Task CharacterImmunedEventHandler(GamingQueue queue, Character character, Character immune, ISkill skill, Item? item = null);
        /// <summary>
        /// 角色免疫事件
        /// </summary>
        public event CharacterImmunedEventHandler? CharacterImmuned;
        /// <summary>
        /// 角色免疫事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="immune"></param>
        /// <param name="skill"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        protected async Task OnCharacterImmunedAsync(Character character, Character immune, ISkill skill, Item? item = null)
        {
            await (CharacterImmuned?.Invoke(this, character, immune, skill, item) ?? Task.CompletedTask);
        }

        public delegate Task CharacterDoNothingEventHandler(GamingQueue queue, Character actor);
        /// <summary>
        /// 角色主动结束回合事件（区别于放弃行动，这个是主动的）
        /// </summary>
        public event CharacterDoNothingEventHandler? CharacterDoNothing;
        /// <summary>
        /// 角色主动结束回合事件（区别于放弃行动，这个是主动的）
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        protected async Task OnCharacterDoNothingAsync(Character actor)
        {
            await (CharacterDoNothing?.Invoke(this, actor) ?? Task.CompletedTask);
        }

        public delegate Task CharacterGiveUpEventHandler(GamingQueue queue, Character actor);
        /// <summary>
        /// 角色放弃行动事件
        /// </summary>
        public event CharacterGiveUpEventHandler? CharacterGiveUp;
        /// <summary>
        /// 角色放弃行动事件
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        protected async Task OnCharacterGiveUpAsync(Character actor)
        {
            await (CharacterGiveUp?.Invoke(this, actor) ?? Task.CompletedTask);
        }

        public delegate Task<bool> GameEndEventHandler(GamingQueue queue, Character winner);
        /// <summary>
        /// 游戏结束事件
        /// </summary>
        public event GameEndEventHandler? GameEnd;
        /// <summary>
        /// 游戏结束事件
        /// </summary>
        /// <param name="winner"></param>
        /// <returns></returns>
        protected async Task<bool> OnGameEndAsync(Character winner)
        {
            return await (GameEnd?.Invoke(this, winner) ?? Task.FromResult(true));
        }

        public delegate Task QueueUpdatedEventHandler(GamingQueue queue, List<Character> characters, Character character, double hardnessTime, QueueUpdatedReason reason, string msg);
        /// <summary>
        /// 行动顺序表更新事件
        /// </summary>
        public event QueueUpdatedEventHandler? QueueUpdated;
        /// <summary>
        /// 行动顺序表更新事件
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="character"></param>
        /// <param name="hardnessTime"></param>
        /// <param name="reason"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected async Task OnQueueUpdatedAsync(List<Character> characters, Character character, double hardnessTime, QueueUpdatedReason reason, string msg = "")
        {
            await (QueueUpdated?.Invoke(this, characters, character, hardnessTime, reason, msg) ?? Task.CompletedTask);
        }

        #endregion
    }
}
