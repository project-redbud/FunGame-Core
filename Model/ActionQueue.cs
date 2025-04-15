﻿using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    /// <summary>
    /// 行动顺序表
    /// </summary>
    public class ActionQueue : IGamingQueue
    {
        /// <summary>
        /// 使用的游戏平衡常数
        /// </summary>
        public EquilibriumConstant GameplayEquilibriumConstant { get; set; } = General.GameplayEquilibriumConstant;

        /// <summary>
        /// 用于文本输出
        /// </summary>
        public Action<string> WriteLine { get; }

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
        /// 当前团灭的团队顺序(第一个是最早死的)
        /// </summary>
        public List<Team> EliminatedTeams => _eliminatedTeams;

        /// <summary>
        /// 角色是否在 AI 控制下
        /// </summary>
        public HashSet<Character> CharactersInAI => _charactersInAI;

        /// <summary>
        /// 角色数据
        /// </summary>
        public Dictionary<Character, CharacterStatistics> CharacterStatistics => _stats;

        /// <summary>
        /// 团队及其成员
        /// </summary>
        public Dictionary<string, Team> Teams => _teams;

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
        /// 最大复活次数
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
        /// 最大获胜积分 [ 适用于团队模式 ]
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
        /// 当前团灭的团队顺序(第一个是最早死的)
        /// </summary>
        protected readonly List<Team> _eliminatedTeams = [];

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
        protected readonly Dictionary<Character, AssistDetail> _assistDamage = [];

        /// <summary>
        /// 角色数据
        /// </summary>
        protected readonly Dictionary<Character, CharacterStatistics> _stats = [];

        /// <summary>
        /// 团队及其成员
        /// </summary>
        protected readonly Dictionary<string, Team> _teams = [];

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
        /// 是否是团队模式
        /// </summary>
        protected bool _isTeamMode = false;

        /// <summary>
        /// 游戏是否结束
        /// </summary>
        protected bool _isGameEnd = false;

        /// <summary>
        /// 新建一个行动顺序表
        /// </summary>
        /// <param name="isTeamMdoe">是否是团队模式</param>
        /// <param name="writer">用于文本输出</param>
        public ActionQueue(bool isTeamMdoe = false, Action<string>? writer = null)
        {
            _isTeamMode = isTeamMdoe;
            if (writer != null)
            {
                WriteLine = writer;
            }
            WriteLine ??= new Action<string>(Console.WriteLine);
        }

        /// <summary>
        /// 新建一个行动顺序表并初始化
        /// </summary>
        /// <param name="characters">参与本次游戏的角色列表</param>
        /// <param name="isTeamMdoe">是否是团队模式</param>
        /// <param name="writer">用于文本输出</param>
        public ActionQueue(List<Character> characters, bool isTeamMdoe = false, Action<string>? writer = null)
        {
            _isTeamMode = isTeamMdoe;
            if (writer != null)
            {
                WriteLine = writer;
            }
            WriteLine ??= new Action<string>(Console.WriteLine);
            InitCharacterQueue(characters);
        }

        /// <summary>
        /// 初始化行动顺序表
        /// </summary>
        /// <param name="characters"></param>
        public void InitCharacterQueue(List<Character> characters)
        {
            // 保存原始的角色信息。用于复活时还原状态
            foreach (Character character in characters)
            {
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
                    _assistDamage.Add(character, new AssistDetail(character, characters.Where(c => c != character)));
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

                        // 将决定好的角色加入顺序表
                        if (selectedCharacter != null)
                        {
                            AddCharacter(selectedCharacter, Calculation.Round2Digits(_queue.Count * 0.1), false);
                            _assistDamage.Add(selectedCharacter, new AssistDetail(selectedCharacter, characters.Where(c => c != selectedCharacter)));
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
        /// 清空行动队列
        /// </summary>
        public void ClearQueue()
        {
            FirstKiller = null;
            CustomData.Clear();
            _original.Clear();
            _queue.Clear();
            _hardnessTimes.Clear();
            _assistDamage.Clear();
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

        /// <summary>
        /// 添加一个团队
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="characters"></param>
        public void AddTeam(string teamName, IEnumerable<Character> characters)
        {
            if (teamName != "" && characters.Any())
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
        public List<Character> GetTeammates(Character character)
        {
            foreach (string team in _teams.Keys)
            {
                if (_teams[team].IsOnThisTeam(character))
                {
                    return _teams[team].GetTeammates(character);
                }
            }
            return [];
        }

        /// <summary>
        /// 将角色加入行动顺序表
        /// </summary>
        /// <param name="character"></param>
        /// <param name="hardnessTime"></param>
        /// <param name="isCheckProtected"></param>
        public void AddCharacter(Character character, double hardnessTime, bool isCheckProtected = true)
        {
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
        /// 从行动顺序表取出第一个角色
        /// </summary>
        /// <returns></returns>
        public Character? NextCharacter()
        {
            if (_queue.Count == 0) return null;

            // 硬直时间为0的角色将执行行动
            Character? character = _queue.FirstOrDefault(c => _hardnessTimes[c] == 0);
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

            return null;
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

        /// <summary>
        /// 回合开始前触发
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> BeforeTurnAsync(Character character)
        {
            return await Task.FromResult(true);
        }

        /// <summary>
        /// 角色 <paramref name="character"/> 的回合进行中
        /// </summary>
        /// <param name="character"></param>
        /// <returns>是否结束游戏</returns>
        public async Task<bool> ProcessTurnAsync(Character character)
        {
            LastRound.Actor = character;
            _roundDeaths.Clear();

            if (!await BeforeTurnAsync(character))
            {
                return _isGameEnd;
            }

            // 获取回合奖励
            List<Skill> rewards = GetRoundRewards(TotalRound, character);

            // 基础硬直时间
            double baseTime = 10;
            bool isCheckProtected = true;

            // 队友列表
            List<Character> teammates = [.. GetTeammates(character).Where(_queue.Contains)];

            // 敌人列表
            List<Character> enemys = [.. _queue.Where(c => c != character && !c.IsUnselectable && !teammates.Contains(c))];

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
                return _isGameEnd;
            }

            foreach (Skill skillTurnStart in skills)
            {
                skillTurnStart.OnTurnStart(character, enemys, teammates, skills, items);
            }

            List<Effect> effects = [.. character.Effects.Where(e => e.Level > 0)];
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
                    effects = [.. character.Effects.Where(e => e.Level > 0)];
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
                            // 行动受限，只能使用特殊物品
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

                        // 模组可以通过此事件来决定角色的行动
                        type = await OnDecideActionAsync(character, enemys, teammates, skills, items);
                        // 若事件未完成决策，则将通过概率对角色进行自动化决策
                        if (type == CharacterActionType.None)
                        {
                            type = GetActionType(pUseItem, pCastSkill, pNormalAttack);
                        }

                        _stats[character].ActionTurn += 1;
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
                effects = [.. character.Effects.Where(e => e.Level > 0)];
                foreach (Effect effect in effects)
                {
                    effect.AlterSelectListBeforeAction(character, enemys, teammates, skills, continuousKillingTemp, earnedMoneyTemp);
                }

                if (type == CharacterActionType.NormalAttack)
                {
                    // 使用普通攻击逻辑
                    List<Character> targets = await SelectTargetsAsync(character, character.NormalAttack, enemys, teammates);
                    if (targets.Count == 0 && _charactersInAI.Contains(character))
                    {
                        // 如果没有选取目标，且角色在 AI 控制下，则随机选取目标
                        if (enemys.Count > character.NormalAttack.CanSelectTargetCount)
                            targets = [.. enemys.OrderBy(o => Random.Shared.Next(enemys.Count)).Take(character.NormalAttack.CanSelectTargetCount)];
                        else
                            targets = [.. enemys];
                    }
                    if (targets.Count > 0)
                    {
                        LastRound.Targets = [.. targets];
                        decided = true;

                        await OnCharacterNormalAttackAsync(character, targets);

                        character.NormalAttack.Attack(this, character, targets);
                        baseTime = character.NormalAttack.HardnessTime;
                        effects = [.. character.Effects.Where(e => e.Level > 0)];
                        foreach (Effect effect in effects)
                        {
                            effect.AlterHardnessTimeAfterNormalAttack(character, ref baseTime, ref isCheckProtected);
                        }
                    }
                }
                else if (type == CharacterActionType.PreCastSkill)
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
                            if (targets.Count == 0 && _charactersInAI.Contains(character) && enemys.Count > 0)
                            {
                                // 如果没有选取目标，且角色在 AI 控制下，则随机选取一个目标
                                targets = [enemys[Random.Shared.Next(enemys.Count)]];
                            }
                            if (targets.Count > 0)
                            {
                                LastRound.Targets = [.. targets];
                                decided = true;

                                character.CharacterState = CharacterState.Casting;
                                SkillTarget skillTarget = new(skill, targets);
                                await OnCharacterPreCastSkillAsync(character, skillTarget);

                                _castingSkills[character] = skillTarget;
                                baseTime = skill.CastTime;
                                skill.OnSkillCasting(this, character, targets);
                            }
                        }
                        else
                        {
                            // 只有魔法需要吟唱，战技和爆发技直接释放
                            if (CheckCanCast(character, skill, out double cost))
                            {
                                List<Character> targets = await SelectTargetsAsync(character, skill, enemys, teammates);
                                if (targets.Count == 0 && _charactersInAI.Contains(character) && enemys.Count > 0)
                                {
                                    // 如果没有选取目标，且角色在 AI 控制下，则随机选取一个目标
                                    targets = [enemys[Random.Shared.Next(enemys.Count)]];
                                }
                                if (targets.Count > 0)
                                {
                                    LastRound.Targets = [.. targets];
                                    decided = true;

                                    SkillTarget skillTarget = new(skill, targets);
                                    await OnCharacterPreCastSkillAsync(character, skillTarget);

                                    skill.OnSkillCasting(this, character, targets);
                                    skill.BeforeSkillCasted();

                                    character.EP -= cost;
                                    baseTime = skill.HardnessTime;
                                    skill.CurrentCD = skill.RealCD;
                                    skill.Enable = false;
                                    LastRound.SkillCost = $"{-cost:0.##} EP";
                                    WriteLine($"[ {character} ] 消耗了 {cost:0.##} 点能量，释放了{(skill.IsSuperSkill ? "爆发技" : "战技")} [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}");

                                    await OnCharacterCastSkillAsync(character, skillTarget, cost);

                                    skill.OnSkillCasted(this, character, targets);
                                    effects = [.. character.Effects.Where(e => e.Level > 0)];
                                    foreach (Effect effect in effects)
                                    {
                                        effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                                    }
                                }
                            }
                        }
                        LastRound.Skill = skill;
                    }
                }
                else if (type == CharacterActionType.CastSkill)
                {
                    if (_castingSkills.TryGetValue(character, out SkillTarget skillTarget))
                    {
                        // 使用技能逻辑，结束吟唱状态
                        character.CharacterState = CharacterState.Actionable;
                        Skill skill = skillTarget.Skill;
                        List<Character> targets = [.. skillTarget.Targets.Where(c => !c.IsUnselectable)];

                        // 判断是否能够释放技能
                        if (targets.Count > 0 && CheckCanCast(character, skill, out double cost))
                        {
                            decided = true;
                            LastRound.Targets = [.. targets];
                            LastRound.Skill = skill;
                            _castingSkills.Remove(character);

                            skill.BeforeSkillCasted();

                            character.MP -= cost;
                            baseTime = skill.HardnessTime;
                            skill.CurrentCD = skill.RealCD;
                            skill.Enable = false;
                            LastRound.SkillCost = $"{-cost:0.##} MP";
                            WriteLine($"[ {character} ] 消耗了 {cost:0.##} 点魔法值，释放了魔法 [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}");

                            await OnCharacterCastSkillAsync(character, skillTarget, cost);

                            skill.OnSkillCasted(this, character, targets);
                        }
                        else
                        {
                            WriteLine($"[ {character} ] 放弃释放技能！");
                            // 放弃释放技能会获得3的硬直时间
                            baseTime = 3;
                        }

                        effects = [.. character.Effects.Where(e => e.Level > 0)];
                        foreach (Effect effect in effects)
                        {
                            effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                        }
                    }
                    else
                    {
                        // 原吟唱的技能丢失（被打断或者被取消），允许角色再次决策
                        character.CharacterState = CharacterState.Actionable;
                    }
                }
                else if (type == CharacterActionType.CastSuperSkill)
                {
                    decided = true;
                    // 结束预释放爆发技的状态
                    character.CharacterState = CharacterState.Actionable;
                    Skill skill = _castingSuperSkills[character];
                    LastRound.Skill = skill;
                    _castingSuperSkills.Remove(character);

                    // 判断是否能够释放技能
                    if (CheckCanCast(character, skill, out double cost))
                    {
                        // 预释放的爆发技不可取消
                        List<Character> targets = await SelectTargetsAsync(character, skill, enemys, teammates);
                        LastRound.Targets = [.. targets];

                        skill.BeforeSkillCasted();

                        character.EP -= cost;
                        baseTime = skill.HardnessTime;
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

                    effects = [.. character.Effects.Where(e => e.Level > 0)];
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
                            baseTime = skill.HardnessTime;
                            effects = [.. character.Effects.Where(e => e.Level > 0)];
                            foreach (Effect effect in effects)
                            {
                                effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                            }
                        }
                    }
                }
                else if (type == CharacterActionType.EndTurn)
                {
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

            LastRound.ActionType = type;

            // 统一在回合结束时处理角色的死亡
            await ProcessCharacterDeathAsync(character);

            if (_isGameEnd)
            {
                return _isGameEnd;
            }

            // 减少硬直时间
            double newHardnessTime = baseTime;
            if (character.CharacterState != CharacterState.Casting)
            {
                newHardnessTime = Math.Max(0, Calculation.Round2Digits(baseTime * (1 - character.ActionCoefficient)));
                WriteLine($"[ {character} ] 回合结束，获得硬直时间：{newHardnessTime} {GameplayEquilibriumConstant.InGameTime}");
            }
            else
            {
                newHardnessTime = Math.Max(0, Calculation.Round2Digits(baseTime * (1 - character.AccelerationCoefficient)));
                WriteLine($"[ {character} ] 进行吟唱，持续时间：{newHardnessTime} {GameplayEquilibriumConstant.InGameTime}");
                LastRound.CastTime = newHardnessTime;
            }
            AddCharacter(character, newHardnessTime, isCheckProtected);
            await OnQueueUpdatedAsync(_queue, character, newHardnessTime, QueueUpdatedReason.Action, "设置角色行动后的硬直时间。");
            LastRound.HardnessTime = newHardnessTime;

            effects = [.. character.Effects.Where(e => e.Level > 0)];
            foreach (Effect effect in effects)
            {
                effect.OnTurnEnd(character);

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

            // 移除回合奖励
            RemoveRoundRewards(TotalRound, character, rewards);

            // 有人想要插队吗？
            await WillPreCastSuperSkill();

            // 回合结束事件
            await OnTurnEndAsync(character);

            await AfterTurnAsync(character);

            WriteLine("");
            return _isGameEnd;
        }

        /// <summary>
        /// 回合结束后触发
        /// </summary>
        /// <returns></returns>
        public virtual async Task AfterTurnAsync(Character character)
        {
            await Task.CompletedTask;
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

            // 减少复活倒计时
            foreach (Character character in _respawnCountdown.Keys)
            {
                _respawnCountdown[character] = Calculation.Round2Digits(_respawnCountdown[character] - timeToReduce);
                if (_respawnCountdown[character] <= 0)
                {
                    await SetCharacterRespawn(character);
                }
            }

            foreach (Character character in _queue)
            {
                // 减少所有角色的硬直时间
                _hardnessTimes[character] = Calculation.Round2Digits(_hardnessTimes[character] - timeToReduce);

                // 统计
                _stats[character].LiveRound += 1;
                _stats[character].LiveTime = Calculation.Round2Digits(_stats[character].LiveTime + timeToReduce);
                _stats[character].DamagePerRound = Calculation.Round2Digits(_stats[character].TotalDamage / _stats[character].LiveRound);
                _stats[character].DamagePerTurn = Calculation.Round2Digits(_stats[character].TotalDamage / _stats[character].ActionTurn);
                _stats[character].DamagePerSecond = Calculation.Round2Digits(_stats[character].TotalDamage / _stats[character].LiveTime);

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
                List<Effect> effects = [.. character.Effects.Where(e => e.Level > 0)];
                foreach (Effect effect in effects)
                {
                    if (effect.Level == 0)
                    {
                        character.Effects.Remove(effect);
                        continue;
                    }

                    effect.OnTimeElapsed(character, timeToReduce);

                    // 自身被动不会考虑
                    if (effect.EffectType == EffectType.None && effect.Skill.SkillType == SkillType.Passive)
                    {
                        continue;
                    }

                    if (effect.Durative)
                    {
                        effect.RemainDuration -= timeToReduce;
                        if (effect.RemainDuration <= 0)
                        {
                            effect.RemainDuration = 0;
                            character.Effects.Remove(effect);
                            effect.OnEffectLost(character);
                        }
                    }
                }
            }

            WriteLine("\r\n");

            return timeToReduce;
        }

        /// <summary>
        /// 对敌人造成伤害
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="isMagicDamage"></param>
        /// <param name="magicType"></param>
        /// <param name="damageResult"></param>
        public async Task DamageToEnemyAsync(Character actor, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage = false, MagicType magicType = MagicType.None, DamageResult damageResult = DamageResult.Normal)
        {
            // 如果敌人在结算伤害之前就已经死亡，将不会继续下去
            if (enemy.HP <= 0)
            {
                return;
            }

            if (!LastRound.IsCritical.TryAdd(enemy, damageResult == DamageResult.Critical) && damageResult == DamageResult.Critical)
            {
                LastRound.IsCritical[enemy] = true;
            }

            bool isEvaded = damageResult == DamageResult.Evaded;
            Dictionary<Effect, double> totalDamageBonus = [];
            List<Effect> effects = [.. actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0)];
            foreach (Effect effect in effects)
            {
                double damageBonus = effect.AlterActualDamageAfterCalculation(actor, enemy, damage, isNormalAttack, isMagicDamage, magicType, damageResult, ref isEvaded, totalDamageBonus);
                totalDamageBonus[effect] = damageBonus;
                if (isEvaded)
                {
                    damageResult = DamageResult.Evaded;
                }
            }
            damage += totalDamageBonus.Sum(kv => kv.Value);

            // 闪避了就没伤害了
            if (!isEvaded)
            {
                if (damage < 0) damage = 0;
                if (isMagicDamage)
                {
                    string dmgType = CharacterSet.GetMagicDamageName(magicType);
                    WriteLine("[ " + enemy + $" ] 受到了 {damage:0.##} 点{dmgType}！");
                }
                else WriteLine("[ " + enemy + $" ] 受到了 {damage:0.##} 点物理伤害！");
                enemy.HP -= damage;

                // 统计伤害
                CalculateCharacterDamageStatistics(actor, enemy, damage, isMagicDamage);

                // 计算助攻
                _assistDamage[actor][enemy] += damage;

                // 造成伤害和受伤都可以获得能量
                double ep = GetEP(damage, GameplayEquilibriumConstant.DamageGetEPFactor, GameplayEquilibriumConstant.DamageGetEPMax);
                effects = [.. actor.Effects];
                foreach (Effect effect in effects)
                {
                    effect.AlterEPAfterDamage(actor, ref ep);
                }
                actor.EP += ep;
                ep = GetEP(damage, GameplayEquilibriumConstant.TakenDamageGetEPFactor, GameplayEquilibriumConstant.TakenDamageGetEPMax);
                effects = [.. enemy.Effects.Where(e => e.Level > 0)];
                foreach (Effect effect in effects)
                {
                    effect.AlterEPAfterGetDamage(enemy, ref ep);
                }
                enemy.EP += ep;
            }

            await OnDamageToEnemyAsync(actor, enemy, damage, isNormalAttack, isMagicDamage, magicType, damageResult);

            effects = [.. actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0)];
            foreach (Effect effect in effects)
            {
                effect.AfterDamageCalculation(actor, enemy, damage, isNormalAttack, isMagicDamage, magicType, damageResult);
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

            if (heal < 0) heal = 0;
            if (target.HP > 0 || (isDead && canRespawn))
            {
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
               await SetCharacterRespawn(target);
            }
            else
            {
                WriteLine($"[ {target} ] 回复了 {heal:0.##} 点生命值！");
            }

            await OnHealToTargetAsync(actor, target, heal, isRespawn);
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
        /// 计算物理伤害
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="expectedDamage"></param>
        /// <param name="finalDamage"></param>
        /// <returns></returns>
        public DamageResult CalculatePhysicalDamage(Character actor, Character enemy, bool isNormalAttack, double expectedDamage, out double finalDamage)
        {
            List<Character> characters = [actor, enemy];
            bool isMagic = false;
            MagicType magicType = MagicType.None;
            List<Effect> effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.Level > 0))];
            foreach (Effect effect in effects)
            {
                effect.AlterDamageTypeBeforeCalculation(actor, enemy, ref isNormalAttack, ref isMagic, ref magicType);
            }
            if (isMagic)
            {
                return CalculateMagicalDamage(actor, enemy, isNormalAttack, magicType, expectedDamage, out finalDamage);
            }

            Dictionary<Effect, double> totalDamageBonus = [];
            effects = [.. actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0)];
            foreach (Effect effect in effects)
            {
                double damageBonus = effect.AlterExpectedDamageBeforeCalculation(actor, enemy, expectedDamage, isNormalAttack, false, MagicType.None, totalDamageBonus);
                totalDamageBonus[effect] = damageBonus;
            }
            expectedDamage += totalDamageBonus.Sum(kv => kv.Value);

            double dice = Random.Shared.NextDouble();
            double throwingBonus = 0;
            bool checkEvade = true;
            bool checkCritical = true;
            if (isNormalAttack)
            {
                effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.Level > 0))];
                foreach (Effect effect in effects)
                {
                    checkEvade = effect.BeforeEvadeCheck(actor, enemy, ref throwingBonus);
                }

                if (checkEvade)
                {
                    // 闪避判定
                    if (dice < (enemy.EvadeRate + throwingBonus))
                    {
                        finalDamage = 0;
                        bool isAlterEvaded = false;
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.Level > 0))];
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

            // 暴击判定
            effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.Level > 0))];
            foreach (Effect effect in effects)
            {
                checkCritical = effect.BeforeCriticalCheck(actor, enemy, ref throwingBonus);
            }

            if (checkCritical)
            {
                dice = Random.Shared.NextDouble();
                if (dice < (actor.CritRate + throwingBonus))
                {
                    finalDamage *= actor.CritDMG; // 暴击伤害倍率加成
                    WriteLine("暴击生效！！");
                    effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.Level > 0))];
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
        /// <returns></returns>
        public DamageResult CalculateMagicalDamage(Character actor, Character enemy, bool isNormalAttack, MagicType magicType, double expectedDamage, out double finalDamage)
        {
            List<Character> characters = [actor, enemy];
            bool isMagic = true;
            List<Effect> effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.Level > 0))];
            foreach (Effect effect in effects)
            {
                effect.AlterDamageTypeBeforeCalculation(actor, enemy, ref isNormalAttack, ref isMagic, ref magicType);
            }
            if (!isMagic)
            {
                return CalculatePhysicalDamage(actor, enemy, isNormalAttack, expectedDamage, out finalDamage);
            }

            Dictionary<Effect, double> totalDamageBonus = [];
            effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.Level > 0))];
            foreach (Effect effect in effects)
            {
                double damageBonus = effect.AlterExpectedDamageBeforeCalculation(actor, enemy, expectedDamage, isNormalAttack, true, magicType, totalDamageBonus);
                totalDamageBonus[effect] = damageBonus;
            }
            expectedDamage += totalDamageBonus.Sum(kv => kv.Value);

            double dice = Random.Shared.NextDouble();
            double throwingBonus = 0;
            bool checkEvade = true;
            bool checkCritical = true;
            if (isNormalAttack)
            {
                effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.Level > 0))];
                foreach (Effect effect in effects)
                {
                    checkEvade = effect.BeforeEvadeCheck(actor, enemy, ref throwingBonus);
                }

                if (checkEvade)
                {
                    // 闪避判定
                    if (dice < (enemy.EvadeRate + throwingBonus))
                    {
                        finalDamage = 0;
                        bool isAlterEvaded = false;
                        effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.Level > 0))];
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

            double MDF = magicType switch
            {
                MagicType.Starmark => enemy.MDF.Starmark,
                MagicType.PurityNatural => enemy.MDF.PurityNatural,
                MagicType.PurityContemporary => enemy.MDF.PurityContemporary,
                MagicType.Bright => enemy.MDF.Bright,
                MagicType.Shadow => enemy.MDF.Shadow,
                MagicType.Element => enemy.MDF.Element,
                MagicType.Fleabane => enemy.MDF.Fleabane,
                MagicType.Particle => enemy.MDF.Particle,
                _ => enemy.MDF.None
            };

            // 魔法穿透后的魔法抗性
            MDF = (1 - actor.MagicalPenetration) * MDF;

            // 最终的魔法伤害
            finalDamage = expectedDamage * (1 - MDF);

            // 暴击判定
            effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.Level > 0))];
            foreach (Effect effect in effects)
            {
                checkCritical = effect.BeforeCriticalCheck(actor, enemy, ref throwingBonus);
            }

            if (checkCritical)
            {
                dice = Random.Shared.NextDouble();
                if (dice < (actor.CritRate + throwingBonus))
                {
                    finalDamage *= actor.CritDMG; // 暴击伤害倍率加成
                    WriteLine("暴击生效！！");
                    effects = [.. characters.SelectMany(c => c.Effects.Where(e => e.Level > 0))];
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
        /// 处理角色死亡
        /// </summary>
        /// <param name="character"></param>
        public async Task ProcessCharacterDeathAsync(Character character)
        {
            foreach (Character death in _roundDeaths)
            {
                if(!await OnCharacterDeathAsync(character, death))
                {
                    continue;
                }

                // 给所有角色的特效广播角色死亡结算
                List<Effect> effects = [.. _queue.SelectMany(c => c.Effects.Where(e => e.Level > 0))];
                foreach (Effect effect in effects)
                {
                    effect.AfterDeathCalculation(death, character, _continuousKilling, _earnedMoney);
                }
                // 将死者移出队列
                _queue.Remove(death);
                if (_isTeamMode)
                {
                    Team? killTeam = GetTeam(character);
                    Team? deathTeam = GetTeam(death);

                    if (MaxRespawnTimes != 0)
                    {
                        string[] teamActive = [.. Teams.OrderByDescending(kv => kv.Value.Score).Select(kv =>
                        {
                            int activeCount = kv.Value.GetActiveCharacters(this).Count;
                            if (kv.Value == killTeam)
                            {
                                activeCount += 1;
                            }
                            return kv.Key + "：" + kv.Value.Score + "（剩余存活人数：" + activeCount + "）";
                        })];
                        WriteLine($"\r\n=== 当前死亡竞赛比分 ===\r\n{string.Join("\r\n", teamActive)}");
                    }

                    if (deathTeam != null)
                    {
                        List<Character> remain = deathTeam.GetActiveCharacters(this);
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
                        List<Character> actives = killTeam.GetActiveCharacters(this);
                        actives.Add(character);
                        int remainCount = actives.Count;
                        if (remainCount > 0 && MaxRespawnTimes == 0)
                        {
                            WriteLine($"[ {killTeam} ] 剩余成员：[ {string.Join(" ] / [ ", actives)} ]（{remainCount} 人）");
                        }
                        if (!_teams.Keys.Where(str => str != killTeam.Name).Any())
                        {
                            // 没有其他的团队了，游戏结束
                            await EndGameInfo(killTeam);
                            return;
                        }
                        if (MaxScoreToWin > 0 && killTeam.Score >= MaxScoreToWin)
                        {
                            List<Team> combinedTeams = [.. _eliminatedTeams, .. _teams.Values];
                            combinedTeams.Remove(killTeam);
                            _eliminatedTeams.Clear();
                            _eliminatedTeams.AddRange(combinedTeams.OrderByDescending(t => t.Score));
                            await EndGameInfo(killTeam);
                            return;
                        }
                    }
                }
                else
                {
                    if (!_queue.Where(c => c != character).Any())
                    {
                        // 没有其他的角色了，游戏结束
                        await EndGameInfo(character);
                    }
                }
            }
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
            int money = Random.Shared.Next(250, 350);

            Character[] assists = [.. _assistDamage.Keys.Where(c => c != death && _assistDamage[c].GetPercentage(death) > 0.10)];
            double totalDamagePercentage = _assistDamage.Keys.Where(assists.Contains).Select(c => _assistDamage[c].GetPercentage(death)).Sum();
            int totalMoney = Math.Min(Convert.ToInt32(money * totalDamagePercentage), 425); // 防止刷伤害设置金钱上限

            // 按伤害比分配金钱 只有造成10%伤害以上才能参与
            foreach (Character assist in assists)
            {
                int cmoney = Convert.ToInt32(_assistDamage[assist].GetPercentage(death) / totalDamagePercentage * totalMoney);
                if (assist != killer)
                {
                    if (!_earnedMoney.TryAdd(assist, cmoney)) _earnedMoney[assist] += cmoney;
                    _stats[assist].Assists += 1;
                }
                else
                {
                    money = cmoney;
                }
            }

            // 终结击杀的奖励仍然是全额的
            if (_continuousKilling.TryGetValue(death, out int coefficient) && coefficient > 1)
            {
                money += (coefficient + 1) * Random.Shared.Next(50, 100);
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
                WriteLine($"[ {killer} ] 拿下了第一滴血！额外奖励 200 {GameplayEquilibriumConstant.InGameCurrency}！！");
            }

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
                actorContinuousKilling = "[ " + killer + " ] 已经" + continuousKilling + "！拜托谁去杀了他吧！！！";
            }
            if (actorContinuousKilling != "")
            {
                LastRound.ActorContinuousKilling.Add(actorContinuousKilling);
                WriteLine(actorContinuousKilling);
            }

            if (!_earnedMoney.TryAdd(killer, money)) _earnedMoney[killer] += money;

            if (_isTeamMode)
            {
                Team? team = GetTeam(killer);
                if (team != null)
                {
                    team.Score++;
                }
            }

            death.EP = 0;

            // 清除对死者的助攻数据
            List<AssistDetail> ads = [.. _assistDamage.Values.Where(ad => ad.Character != death)];
            foreach (AssistDetail ad in ads)
            {
                ad[death] = 0;
            }

            _continuousKilling.Remove(death);
            if (MaxRespawnTimes == 0)
            {
                _eliminated.Add(death);
            }
            else if (_respawnTimes.TryGetValue(death, out int times) && MaxRespawnTimes != -1 && times > MaxRespawnTimes)
            {
                WriteLine($"[ {death} ] 已达到复活次数上限，将不能再复活！！");
                _eliminated.Add(death);
            }
            else
            {
                // 进入复活倒计时
                double respawnTime = Calculation.Round2Digits(Math.Min(90, death.Level * 0.15 + times * 2.77 + coefficient * Random.Shared.Next(1, 3)));
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
                    WriteLine($"[ {caster} ] 终止了 [ {st.Skill.Name} ] 的施法" + (_hardnessTimes[caster] > 3 ? $"，并获得了 3 {GameplayEquilibriumConstant.InGameTime}的硬直时间的补偿。" : "。"));
                    if (_hardnessTimes[caster] > 3)
                    {
                        _hardnessTimes[caster] = 3;
                    }
                }
            }
        }

        /// <summary>
        /// 游戏结束信息
        /// </summary>
        public async Task EndGameInfo(Character winner)
        {
            WriteLine("[ " + winner + " ] 是胜利者。");
            _queue.Remove(winner);
            _eliminated.Add(winner);

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
            _isGameEnd = true;
        }

        /// <summary>
        /// 游戏结束信息 [ 团队版 ] 
        /// </summary>
        public async Task EndGameInfo(Team winner)
        {
            WriteLine("[ " + winner + " ] 是胜利者。");

            if (!await OnGameEndTeamAsync(winner))
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
                if (cost > 0 && cost <= caster.MP)
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
                if (cost > 0 && cost <= caster.EP)
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
            if (costMP > 0 && costMP <= caster.MP)
            {
                isMPOk = true;
            }
            else
            {
                WriteLine("[ " + caster + $" ] 魔法不足！");
            }
            costEP = skill.RealEPCost;
            if (costEP > 0 && costEP <= caster.EP)
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
        /// 是否在回合外释放爆发技插队（仅自动化，手动设置请调用：<see cref="SetCharacterPreCastSuperSkill"/>）
        /// </summary>
        /// <returns></returns>
        public async Task WillPreCastSuperSkill()
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
            else if (_castingSuperSkills.TryGetValue(caster, out skill))
            {
                _castingSuperSkills.Remove(caster);
            }
            if (skill != null)
            {
                WriteLine($"[ {caster} ] 的施法被 [ {interrupter} ] 打断了！！");
                List<Effect> effects = [.. caster.Effects.Where(e => e.Level > 0)];
                foreach (Effect effect in effects)
                {
                    effect.OnSkillCastInterrupted(caster, skill, interrupter);
                }
                effects = [.. interrupter.Effects.Where(e => e.Level > 0)];
                foreach (Effect effect in effects)
                {
                    effect.OnSkillCastInterrupted(caster, skill, interrupter);
                }
            }
            await OnInterruptCastingAsync(caster, skill, interrupter);
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
                return CharacterActionType.None;
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

            return CharacterActionType.None;
        }

        /// <summary>
        /// 计算角色的数据
        /// </summary>
        public void CalculateCharacterDamageStatistics(Character character, Character characterTaken, double damage, bool isMagic)
        {
            if (_stats.TryGetValue(character, out CharacterStatistics? stats) && stats != null)
            {
                if (isMagic)
                {
                    stats.TotalMagicDamage = Calculation.Round2Digits(stats.TotalMagicDamage + damage);
                }
                else
                {
                    stats.TotalPhysicalDamage = Calculation.Round2Digits(stats.TotalPhysicalDamage + damage);
                }
                stats.TotalDamage = Calculation.Round2Digits(stats.TotalDamage + damage);
            }
            if (_stats.TryGetValue(characterTaken, out CharacterStatistics? statsTaken) && statsTaken != null)
            {
                if (isMagic)
                {
                    statsTaken.TotalTakenMagicDamage = Calculation.Round2Digits(statsTaken.TotalTakenMagicDamage + damage);
                }
                else
                {
                    statsTaken.TotalTakenPhysicalDamage = Calculation.Round2Digits(statsTaken.TotalTakenPhysicalDamage + damage);
                }
                statsTaken.TotalTakenDamage = Calculation.Round2Digits(statsTaken.TotalTakenDamage + damage);
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
                    if (targets.Count == 0 && _charactersInAI.Contains(character) && enemys.Count > 0)
                    {
                        // 如果没有选取目标，且角色在 AI 控制下，则随机选取一个目标
                        targets = [enemys[Random.Shared.Next(enemys.Count)]];
                    }
                    if (targets.Count > 0)
                    {
                        LastRound.Targets = [.. targets];

                        await OnCharacterUseItemAsync(character, item, targets);

                        string line = $"[ {character} ] 使用了物品 [ {item.Name} ]！\r\n[ {character} ] ";

                        skill.OnSkillCasting(this, character, targets);
                        skill.BeforeSkillCasted();

                        skill.CurrentCD = skill.RealCD;
                        skill.Enable = false;

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
            return false;
        }

        /// <summary>
        /// 设置角色复活
        /// </summary>
        /// <param name="character"></param>
        public async Task SetCharacterRespawn(Character character)
        {
            double hardnessTime = 5;
            character.Respawn(_original[character.Guid]);
            WriteLine($"[ {character} ] 已复活！获得 {hardnessTime} {GameplayEquilibriumConstant.InGameTime}的硬直时间。");
            AddCharacter(character, hardnessTime, false);
            await OnQueueUpdatedAsync(_queue, character, hardnessTime, QueueUpdatedReason.Respawn, "设置角色复活后的硬直时间。");
            LastRound.Respawns.Add(character);
            _respawnCountdown.Remove(character);
            if (!_respawnTimes.TryAdd(character, 1))
            {
                _respawnTimes[character] += 1;
            }
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
                AddCharacter(character, 0, false);
                await OnQueueUpdatedAsync(_queue, character, 0, QueueUpdatedReason.PreCastSuperSkill, "设置角色预释放爆发技的硬直时间。");
                WriteLine("[ " + character + " ] 预释放了爆发技！！");
                foreach (Character c in _hardnessTimes.Keys)
                {
                    if (_hardnessTimes[c] != 0)
                    {
                        _hardnessTimes[c] = Calculation.Round2Digits(_hardnessTimes[c] + 0.01);
                    }
                }
                skill.OnSkillCasting(this, character, []);
            }
        }

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

        /// <summary>
        /// 初始化回合奖励
        /// </summary>
        /// <param name="maxRound">最大回合数</param>
        /// <param name="maxRewardsInRound">每个奖励回合生成多少技能</param>
        /// <param name="effects">key: 特效的数字标识符；value: 是否是主动技能的特效</param>
        /// <param name="factoryEffects">通过数字标识符来获取构造特效的参数</param>
        /// <returns></returns>
        public virtual void InitRoundRewards(int maxRound, int maxRewardsInRound, Dictionary<long, bool> effects, Func<long, Dictionary<string, object>>? factoryEffects = null)
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
        public virtual List<Skill> GetRoundRewards(int round, Character character)
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
        /// <param name="round"></param>
        /// <param name="character"></param>
        /// <param name="skills"></param>
        public virtual void RemoveRoundRewards(int round, Character character, List<Skill> skills)
        {
            foreach (Skill skill in skills)
            {
                foreach (Effect e in skill.Effects)
                {
                    e.OnEffectLost(character);
                    character.Effects.Remove(e);
                }
                character.Skills.Remove(skill);
                skill.Character = null;
            }
        }

        /// <summary>
        /// 选取技能目标
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <returns></returns>
        public virtual async Task<List<Character>> SelectTargetsAsync(Character caster, Skill skill, List<Character> enemys, List<Character> teammates)
        {
            List<Effect> effects = [.. caster.Effects.Where(e => e.Level > 0)];
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
        public virtual async Task<List<Character>> SelectTargetsAsync(Character character, NormalAttack attack, List<Character> enemys, List<Character> teammates)
        {
            List<Effect> effects = [.. character.Effects.Where(e => e.Level > 0)];
            foreach (Effect effect in effects)
            {
                effect.AlterSelectListBeforeSelection(character, attack, enemys, teammates);
            }
            List<Character> targets = await OnSelectNormalAttackTargetsAsync(character, attack, enemys, teammates);
            return targets;
        }

        #region 事件

        public delegate Task<bool> TurnStartEventHandler(ActionQueue queue, Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items);
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

        public delegate Task TurnEndEventHandler(ActionQueue queue, Character character);
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

        public delegate Task<CharacterActionType> DecideActionEventHandler(ActionQueue queue, Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items);
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

        public delegate Task<Skill?> SelectSkillEventHandler(ActionQueue queue, Character character, List<Skill> skills);
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

        public delegate Task<Item?> SelectItemEventHandler(ActionQueue queue, Character character, List<Item> items);
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

        public delegate Task<List<Character>> SelectSkillTargetsEventHandler(ActionQueue queue, Character caster, Skill skill, List<Character> enemys, List<Character> teammates);
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

        public delegate Task<List<Character>> SelectNormalAttackTargetsEventHandler(ActionQueue queue, Character character, NormalAttack attack, List<Character> enemys, List<Character> teammates);
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

        public delegate Task InterruptCastingEventHandler(ActionQueue queue, Character cast, Skill? skill, Character interrupter);
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
        protected async Task OnInterruptCastingAsync(Character cast, Skill? skill, Character interrupter)
        {
            await (InterruptCasting?.Invoke(this, cast, skill, interrupter) ?? Task.CompletedTask);
        }

        public delegate Task<bool> DeathCalculationEventHandler(ActionQueue queue, Character killer, Character death);
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
        
        public delegate Task<bool> CharacterDeathEventHandler(ActionQueue queue, Character current, Character death);
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

        public delegate Task HealToTargetEventHandler(ActionQueue queue, Character actor, Character target, double heal, bool isRespawn);
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

        public delegate Task DamageToEnemyEventHandler(ActionQueue queue, Character actor, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult);
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
        /// <param name="isNormalAttack"></param>
        /// <param name="isMagicDamage"></param>
        /// <param name="magicType"></param>
        /// <param name="damageResult"></param>
        /// <returns></returns>
        protected async Task OnDamageToEnemyAsync(Character actor, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            await (DamageToEnemy?.Invoke(this, actor, enemy, damage, isNormalAttack, isMagicDamage, magicType, damageResult) ?? Task.CompletedTask);
        }

        public delegate Task CharacterNormalAttackEventHandler(ActionQueue queue, Character actor, List<Character> targets);
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
        
        public delegate Task CharacterPreCastSkillEventHandler(ActionQueue queue, Character actor, SkillTarget skillTarget);
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
        
        public delegate Task CharacterCastSkillEventHandler(ActionQueue queue, Character actor, SkillTarget skillTarget, double cost);
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
        
        public delegate Task CharacterUseItemEventHandler(ActionQueue queue, Character actor, Item item, List<Character> targets);
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
        
        public delegate Task CharacterCastItemSkillEventHandler(ActionQueue queue, Character actor, Item item, SkillTarget skillTarget, double costMP, double costEP);
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
        
        public delegate Task CharacterDoNothingEventHandler(ActionQueue queue, Character actor);
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
        
        public delegate Task CharacterGiveUpEventHandler(ActionQueue queue, Character actor);
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

        public delegate Task<bool> GameEndEventHandler(ActionQueue queue, Character winner);
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
        
        public delegate Task<bool> GameEndTeamEventHandler(ActionQueue queue, Team winner);
        /// <summary>
        /// 游戏结束事件（团队版）
        /// </summary>
        public event GameEndTeamEventHandler? GameEndTeam;
        /// <summary>
        /// 游戏结束事件（团队版）
        /// </summary>
        /// <param name="winner"></param>
        /// <returns></returns>
        protected async Task<bool> OnGameEndTeamAsync(Team winner)
        {
            return await (GameEndTeam?.Invoke(this, winner) ?? Task.FromResult(true));
        }

        public delegate Task QueueUpdatedEventHandler(ActionQueue queue, List<Character> characters, Character character, double hardnessTime, QueueUpdatedReason reason, string msg);
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
