using Milimoe.FunGame.Core.Api.Utility;
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
        /// 当前已死亡的角色顺序(第一个是最早死的)
        /// </summary>
        public List<Character> Eliminated => _eliminated;

        /// <summary>
        /// 当前团灭的团队顺序(第一个是最早死的)
        /// </summary>
        public List<Team> EliminatedTeams => _eliminatedTeams;

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
        /// 硬直时间表
        /// </summary>
        protected readonly Dictionary<Character, double> _hardnessTimes = [];

        /// <summary>
        /// 角色正在吟唱的魔法
        /// </summary>
        protected readonly Dictionary<Character, SkillTarget> _castingSkills = [];

        /// <summary>
        /// 角色预释放的爆发技
        /// </summary>
        protected readonly Dictionary<Character, Skill> _castingSuperSkills = [];

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
            _original.Clear();
            _queue.Clear();
            _hardnessTimes.Clear();
            _assistDamage.Clear();
            _stats.Clear();
            _cutCount.Clear();
            _castingSkills.Clear();
            _castingSuperSkills.Clear();
            _maxContinuousKilling.Clear();
            _continuousKilling.Clear();
            _earnedMoney.Clear();
            _eliminated.Clear();
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
                int countProtected = _queue.Count;

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
        public virtual bool BeforeTurn(Character character)
        {
            return true;
        }

        /// <summary>
        /// 角色 <paramref name="character"/> 的回合进行中
        /// </summary>
        /// <param name="character"></param>
        /// <returns>是否结束游戏</returns>
        public bool ProcessTurn(Character character)
        {
            LastRound.Actor = character;
            _roundDeaths.Clear();

            if (!BeforeTurn(character))
            {
                return _isGameEnd;
            }

            List<Effect> effects = character.Effects.Where(e => e.Level > 0).ToList();
            foreach (Effect effect in effects)
            {
                effect.OnTurnStart(character);
            }

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

            // 此变量用于在取消选择时，能够重新行动
            bool decided = false;
            // 最大取消次数
            int cancelTimes = 3;

            // 作出了什么行动
            CharacterActionType type = CharacterActionType.None;

            while (!decided && cancelTimes > 0)
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
                    effects = character.Effects.Where(e => e.Level > 0).ToList();
                    foreach (Effect e in effects)
                    {
                        actionTypeTemp = e.AlterActionTypeBeforeAction(character, character.CharacterState, ref canUseItem, ref canCastSkill, ref pUseItem, ref pCastSkill, ref pNormalAttack);
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
                            // 技能受限，无法使用技能，可以使用物品
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
                        type = GetActionType(pUseItem, pCastSkill, pNormalAttack);
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
                        decided = true;
                        WriteLine("[ " + character + $" ] 完全行动不能！");
                        type = CharacterActionType.None;
                    }
                }

                List<Character> enemysTemp = new(enemys);
                List<Character> teammatesTemp = new(teammates);
                List<Skill> skillsTemp = new(skills);
                Dictionary<Character, int> continuousKillingTemp = new(_continuousKilling);
                Dictionary<Character, int> earnedMoneyTemp = new(_earnedMoney);
                effects = character.Effects.Where(e => e.Level > 0).ToList();
                foreach (Effect e in effects)
                {
                    if (e.AlterEnemyListBeforeAction(character, enemysTemp, teammatesTemp, skillsTemp, continuousKillingTemp, earnedMoneyTemp))
                    {
                        enemys = enemysTemp.Distinct().ToList();
                        teammates = teammatesTemp.Distinct().ToList();
                        skills = skillsTemp.Distinct().ToList();
                    }
                }

                if (type == CharacterActionType.NormalAttack)
                {
                    // 使用普通攻击逻辑
                    Character[] targets = [.. SelectTargets(character, character.NormalAttack, enemys, teammates, out bool cancel)];
                    LastRound.Targets = [.. targets];
                    if (!cancel && targets.Length > 0)
                    {
                        decided = true;
                        character.NormalAttack.Attack(this, character, targets);
                        baseTime = character.NormalAttack.HardnessTime;
                        effects = character.Effects.Where(e => e.Level > 0).ToList();
                        foreach (Effect effect in effects)
                        {
                            effect.AlterHardnessTimeAfterNormalAttack(character, ref baseTime, ref isCheckProtected);
                        }
                    }
                }
                else if (type == CharacterActionType.PreCastSkill)
                {
                    // 预使用技能，即开始吟唱逻辑
                    // 吟唱前需要先选取目标
                    Skill skill = skills[Random.Shared.Next(skills.Count)];
                    if (skill.SkillType == SkillType.Magic)
                    {
                        List<Character> targets = SelectTargets(character, skill, enemys, teammates, out bool cancel);
                        LastRound.Targets = [.. targets];
                        if (!cancel)
                        {
                            decided = true;
                            character.CharacterState = CharacterState.Casting;
                            _castingSkills.Add(character, new(skill, targets));
                            baseTime = skill.CastTime;
                            skill.OnSkillCasting(this, character, targets);
                        }
                    }
                    else
                    {
                        if (CheckCanCast(character, skill, out double cost))
                        {
                            List<Character> targets = SelectTargets(character, skill, enemys, teammates, out bool cancel);
                            LastRound.Targets = [.. targets];
                            if (!cancel)
                            {
                                decided = true;
                                skill.OnSkillCasting(this, character, targets);
                                skill.BeforeSkillCasted();

                                character.EP -= cost;
                                baseTime = skill.HardnessTime;
                                skill.CurrentCD = skill.RealCD;
                                skill.Enable = false;
                                LastRound.SkillCost = $"{-cost:0.##} EP";

                                WriteLine("[ " + character + $" ] 消耗了 {cost:0.##} 点能量，释放了{(skill.IsSuperSkill ? "爆发技" : "战技")} [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}");
                                skill.OnSkillCasted(this, character, targets);
                                effects = character.Effects.Where(e => e.Level > 0).ToList();
                                foreach (Effect effect in effects)
                                {
                                    effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                                }
                            }
                        }
                    }
                    LastRound.Skill = skill;
                }
                else if (type == CharacterActionType.CastSkill)
                {
                    decided = true;
                    // 使用技能逻辑，结束吟唱状态
                    character.CharacterState = CharacterState.Actionable;
                    SkillTarget skillTarget = _castingSkills[character];
                    Skill skill = skillTarget.Skill;
                    List<Character> targets = skillTarget.Targets;
                    LastRound.Targets = [.. targets];
                    LastRound.Skill = skill;
                    _castingSkills.Remove(character);

                    // 判断是否能够释放技能
                    if (CheckCanCast(character, skill, out double cost))
                    {
                        skill.BeforeSkillCasted();

                        character.MP -= cost;
                        baseTime = skill.HardnessTime;
                        skill.CurrentCD = skill.RealCD;
                        skill.Enable = false;
                        LastRound.SkillCost = $"{-cost:0.##} MP";

                        WriteLine("[ " + character + $" ] 消耗了 {cost:0.##} 点魔法值，释放了魔法 [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}");
                        skill.OnSkillCasted(this, character, targets);
                    }
                    else
                    {
                        WriteLine("[ " + character + $" ] 放弃释放技能！");
                        // 放弃释放技能会获得3的硬直时间
                        baseTime = 3;
                    }

                    effects = character.Effects.Where(e => e.Level > 0).ToList();
                    foreach (Effect effect in effects)
                    {
                        effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
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
                        List<Character> targets = SelectTargets(character, skill, enemys, teammates, out _);
                        LastRound.Targets = [.. targets];

                        skill.BeforeSkillCasted();

                        character.EP -= cost;
                        baseTime = skill.HardnessTime;
                        skill.CurrentCD = skill.RealCD;
                        skill.Enable = false;
                        LastRound.SkillCost = $"{-cost:0.##} EP";

                        WriteLine("[ " + character + $" ] 消耗了 {cost:0.##} 点能量值，释放了爆发技 [ {skill.Name} ]！{(skill.Slogan != "" ? skill.Slogan : "")}");
                        skill.OnSkillCasted(this, character, targets);
                    }
                    else
                    {
                        WriteLine("[ " + character + $" ] 因能量不足放弃释放爆发技！");
                        // 放弃释放技能会获得3的硬直时间
                        baseTime = 3;
                    }

                    effects = character.Effects.Where(e => e.Level > 0).ToList();
                    foreach (Effect effect in effects)
                    {
                        effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                    }
                }
                else if (type == CharacterActionType.UseItem)
                {
                    // 使用物品逻辑
                }
            }

            if (!decided || type == CharacterActionType.None)
            {
                WriteLine("[ " + character + $" ] 放弃了行动！");
            }

            LastRound.ActionType = type;

            // 统一在回合结束时处理角色的死亡
            ProcessCharacterDeath(character);

            if (_isGameEnd)
            {
                return _isGameEnd;
            }

            // 减少硬直时间
            double newHardnessTime = baseTime;
            if (character.CharacterState != CharacterState.Casting)
            {
                newHardnessTime = Math.Max(0, Calculation.Round2Digits(baseTime * (1 - character.ActionCoefficient)));
                WriteLine("[ " + character + " ] 回合结束，获得硬直时间: " + newHardnessTime);
            }
            else
            {
                newHardnessTime = Math.Max(0, Calculation.Round2Digits(baseTime * (1 - character.AccelerationCoefficient)));
                WriteLine("[ " + character + " ] 进行吟唱，持续时间: " + newHardnessTime);
                LastRound.CastTime = newHardnessTime;
            }
            AddCharacter(character, newHardnessTime, isCheckProtected);
            LastRound.HardnessTime = newHardnessTime;

            // 有人想要插队吗？
            WillPreCastSuperSkill(character);

            effects = character.Effects.Where(e => e.Level > 0).ToList();
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

            AfterTurn(character);

            WriteLine("");
            return _isGameEnd;
        }

        /// <summary>
        /// 回合结束后触发
        /// </summary>
        /// <returns></returns>
        public virtual void AfterTurn(Character character)
        {

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
            WriteLine("时间流逝：" + timeToReduce);

            // 减少复活倒计时
            foreach (Character character in _respawnCountdown.Keys)
            {
                _respawnCountdown[character] = Calculation.Round2Digits(_respawnCountdown[character] - timeToReduce);
                if (_respawnCountdown[character] <= 0)
                {
                    double hardnessTime = 5;
                    character.Respawn(_original[character.Guid]);
                    WriteLine($"[ {character} ] 已复活！获得 {hardnessTime} 硬直时间。");
                    AddCharacter(character, hardnessTime, false);
                    LastRound.Respawns.Add(character);
                    _respawnCountdown.Remove(character);
                    if (!_respawnTimes.TryAdd(character, 1))
                    {
                        _respawnTimes[character] += 1;
                    }
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
                    WriteLine($"角色 {character.NickName} 回血：{recoveryHP:0.##} / 回蓝：{recoveryMP:0.##}");
                }
                else
                {
                    if (reallyReHP > 0)
                    {
                        character.HP += reallyReHP;
                        WriteLine($"角色 {character.NickName} 回血：{recoveryHP:0.##}");
                    }
                    if (reallyReMP > 0)
                    {
                        character.MP += reallyReMP;
                        WriteLine($"角色 {character.NickName} 回蓝：{recoveryMP:0.##}");
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
                List<Effect> effects = character.Effects.Where(e => e.Level > 0).ToList();
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
        public void DamageToEnemy(Character actor, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage = false, MagicType magicType = MagicType.None, DamageResult damageResult = DamageResult.Normal)
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
            List<Effect> effects = actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0).ToList();
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
                double ep = GetEP(damage, 0.03, 30);
                effects = [.. actor.Effects];
                foreach (Effect effect in effects)
                {
                    effect.AlterEPAfterDamage(actor, ref ep);
                }
                actor.EP += ep;
                ep = GetEP(damage, 0.015, 15);
                effects = enemy.Effects.Where(e => e.Level > 0).ToList();
                foreach (Effect effect in effects)
                {
                    effect.AlterEPAfterGetDamage(enemy, ref ep);
                }
                enemy.EP += ep;
            }

            effects = actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0).ToList();
            foreach (Effect effect in effects)
            {
                effect.AfterDamageCalculation(actor, enemy, damage, isNormalAttack, isMagicDamage, magicType, damageResult);
            }

            if (enemy.HP <= 0 && !_eliminated.Contains(enemy) && !_respawnCountdown.ContainsKey(enemy))
            {
                LastRound.HasKill = true;
                _roundDeaths.Add(enemy);
                DeathCalculation(actor, enemy);
            }
        }

        /// <summary>
        /// 治疗一个目标
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="target"></param>
        /// <param name="heal"></param>
        /// <param name="canRespawn"></param>
        public void HealToTarget(Character actor, Character target, double heal, bool canRespawn = false)
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

            if (isDead && canRespawn)
            {
                if (target != actor)
                {
                    WriteLine($"[ {target} ] 被 [ {actor} ] 复苏了，并回复了 {heal:0.##} 点生命值！！");
                }
                else
                {
                    WriteLine($"[ {target} ] 复苏了，并回复了 {heal:0.##} 点生命值！！");
                }
            }
            else
            {
                WriteLine($"[ {target} ] 回复了 {heal:0.##} 点生命值！");
            }
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
            bool isMagic = false;
            MagicType magicType = MagicType.None;
            List<Effect> effects = actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0).ToList();
            foreach (Effect effect in effects)
            {
                effect.AlterDamageTypeBeforeCalculation(actor, enemy, ref isNormalAttack, ref isMagic, ref magicType);
            }
            if (isMagic)
            {
                return CalculateMagicalDamage(actor, enemy, isNormalAttack, magicType, expectedDamage, out finalDamage);
            }

            Dictionary<Effect, double> totalDamageBonus = [];
            effects = actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0).ToList();
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
                effects = actor.Effects.Where(e => e.Level > 0).ToList();
                foreach (Effect effect in effects)
                {
                    checkEvade = effect.BeforeEvadeCheck(actor, ref throwingBonus);
                }

                if (checkEvade)
                {
                    // 闪避判定
                    if (dice < (enemy.EvadeRate + throwingBonus))
                    {
                        finalDamage = 0;
                        List<Character> characters = [actor, enemy];
                        bool isAlterEvaded = false;
                        effects = characters.SelectMany(c => c.Effects.Where(e => e.Level > 0)).ToList();
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
            double physicalDamageReduction = penetratedDEF / (penetratedDEF + General.GameplayEquilibriumConstant.DEFReductionFactor);

            // 最终的物理伤害
            finalDamage = expectedDamage * (1 - Calculation.PercentageCheck(physicalDamageReduction + enemy.ExPDR));

            // 暴击判定
            effects = actor.Effects.Where(e => e.Level > 0).ToList();
            foreach (Effect effect in effects)
            {
                checkCritical = effect.BeforeCriticalCheck(actor, ref throwingBonus);
            }

            if (checkCritical)
            {
                dice = Random.Shared.NextDouble();
                if (dice < (actor.CritRate + throwingBonus))
                {
                    finalDamage *= actor.CritDMG; // 暴击伤害倍率加成
                    WriteLine("暴击生效！！");
                    effects = actor.Effects.Where(e => e.Level > 0).ToList();
                    foreach (Effect effect in effects)
                    {
                        effect.OnCriticalDamageTriggered(actor, dice);
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
            bool isMagic = true;
            List<Effect> effects = actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0).ToList();
            foreach (Effect effect in effects)
            {
                effect.AlterDamageTypeBeforeCalculation(actor, enemy, ref isNormalAttack, ref isMagic, ref magicType);
            }
            if (!isMagic)
            {
                return CalculatePhysicalDamage(actor, enemy, isNormalAttack, expectedDamage, out finalDamage);
            }

            Dictionary<Effect, double> totalDamageBonus = [];
            effects = actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0).ToList();
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
                effects = actor.Effects.Where(e => e.Level > 0).ToList();
                foreach (Effect effect in effects)
                {
                    checkEvade = effect.BeforeEvadeCheck(actor, ref throwingBonus);
                }

                if (checkEvade)
                {
                    // 闪避判定
                    if (dice < (enemy.EvadeRate + throwingBonus))
                    {
                        finalDamage = 0;
                        List<Character> characters = [actor, enemy];
                        bool isAlterEvaded = false;
                        effects = characters.SelectMany(c => c.Effects.Where(e => e.Level > 0)).ToList();
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
            effects = actor.Effects.Where(e => e.Level > 0).ToList();
            foreach (Effect effect in effects)
            {
                checkCritical = effect.BeforeCriticalCheck(actor, ref throwingBonus);
            }

            if (checkCritical)
            {
                dice = Random.Shared.NextDouble();
                if (dice < (actor.CritRate + throwingBonus))
                {
                    finalDamage *= actor.CritDMG; // 暴击伤害倍率加成
                    WriteLine("暴击生效！！");
                    effects = actor.Effects.Where(e => e.Level > 0).ToList();
                    foreach (Effect effect in effects)
                    {
                        effect.OnCriticalDamageTriggered(actor, dice);
                    }
                    return DamageResult.Critical;
                }
            }

            // 是否有效伤害
            return DamageResult.Normal;
        }

        public void ProcessCharacterDeath(Character character)
        {
            foreach (Character enemy in _roundDeaths)
            {
                // 给所有角色的特效广播角色死亡结算
                List<Effect> effects = _queue.SelectMany(c => c.Effects.Where(e => e.Level > 0)).ToList();
                foreach (Effect effect in effects)
                {
                    effect.AfterDeathCalculation(enemy, character, _continuousKilling, _earnedMoney);
                }
                // 将死者移出队列
                _queue.Remove(enemy);
                if (_isTeamMode)
                {
                    Team? killTeam = GetTeam(character);
                    Team? deathTeam = GetTeam(enemy);

                    if (MaxRespawnTimes != 0)
                    {
                        string[] teamActive = Teams.OrderByDescending(kv => kv.Value.Score).Select(kv => {
                            int activeCount = kv.Value.GetActiveCharacters(this).Count;
                            if (kv.Value == killTeam)
                            {
                                activeCount += 1;
                            }
                            return kv.Key + "：" + kv.Value.Score + "（剩余存活人数：" + activeCount + "）";
                        }).ToArray();
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
                else
                {
                    if (!_queue.Where(c => c != character).Any())
                    {
                        // 没有其他的角色了，游戏结束
                        EndGameInfo(character);
                    }
                }
            }
        }

        /// <summary>
        /// 死亡结算
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="death"></param>
        public void DeathCalculation(Character killer, Character death)
        {
            if (!_continuousKilling.TryAdd(killer, 1)) _continuousKilling[killer] += 1;
            if (!_maxContinuousKilling.TryAdd(killer, 1) && _continuousKilling[killer] > _maxContinuousKilling[killer])
            {
                _maxContinuousKilling[killer] = _continuousKilling[killer];
            }
            _stats[killer].Kills += 1;
            _stats[death].Deaths += 1;
            int money = Random.Shared.Next(250, 350);

            Character[] assists = _assistDamage.Keys.Where(c => c != death && _assistDamage[c].GetPercentage(death) > 0.10).ToArray();
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
                string msg = $"[ {killer} ] 终结了 [ {death} ]{(termination != "" ? " 的" + termination : "")}，获得 {money} {General.GameplayEquilibriumConstant.InGameCurrency}！";
                LastRound.DeathContinuousKilling.Add(msg);
                if (assists.Length > 1)
                {
                    msg += "助攻：[ " + string.Join(" ] / [ ", assists.Where(c => c != killer)) + " ]";
                }
                WriteLine(msg);
            }
            else
            {
                string msg = $"[ {killer} ] 杀死了 [ {death} ]，获得 {money} {General.GameplayEquilibriumConstant.InGameCurrency}！";
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
                WriteLine($"[ {killer} ] 拿下了第一滴血！额外奖励 200 {General.GameplayEquilibriumConstant.InGameCurrency}！！");
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
                WriteLine($"[ {death} ] 进入复活倒计时：{respawnTime:0.##} 时间！");
            }

            // 移除死者的施法
            _castingSkills.Remove(death);
            _castingSuperSkills.Remove(death);

            // 因丢失目标而中断施法
            List<Character> castingSkills = [.. _castingSkills.Keys];
            foreach (Character caster in castingSkills)
            {
                SkillTarget st = _castingSkills[caster];
                if (st.Targets.Remove(death))
                {
                    _castingSkills.Remove(caster);
                    if (caster.CharacterState == CharacterState.Casting)
                    {
                        caster.CharacterState = CharacterState.Actionable;
                    }
                    WriteLine($"[ {caster} ] 终止了 [ {st.Skill.Name} ] 的施法" + (_hardnessTimes[caster] > 3 ? "，并获得了 3 硬直时间的补偿。" : "。"));
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
        public void EndGameInfo(Character winner)
        {
            WriteLine("[ " + winner + " ] 是胜利者。");
            _queue.Remove(winner);
            _eliminated.Add(winner);
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
                    (_earnedMoney.TryGetValue(ec, out int earned) ? $" [ 已赚取 {earned} {General.GameplayEquilibriumConstant.InGameCurrency} ]" : "");
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
        public void EndGameInfo(Team winner)
        {
            WriteLine("[ " + winner + " ] 是胜利者。");

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
                        respawning = "[ " + (_respawnCountdown.TryGetValue(ec, out double time) && time > 0 ? $"{time:0.##} 时间后复活" : "阵亡") + " ] ";
                    }

                    string topCharacter = respawning + ec.ToString() +
                        (statistics.FirstKills > 0 ? " [ 第一滴血 ]" : "") +
                        (_maxContinuousKilling.TryGetValue(ec, out int kills) && kills > 1 ? $" [ {CharacterSet.GetContinuousKilling(kills)} ]" : "") +
                        (_earnedMoney.TryGetValue(ec, out int earned) ? $" [ 已赚取 {earned} {General.GameplayEquilibriumConstant.InGameCurrency} ]" : "") +
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
        /// 是否在回合外释放爆发技插队
        /// </summary>
        /// <param name="character">当前正在行动的角色</param>
        /// <returns></returns>
        public void WillPreCastSuperSkill(Character character)
        {
            // 选取在顺序表一半之后的角色
            foreach (Character other in _queue.Where(c => c != character && c.CharacterState == CharacterState.Actionable && _queue.IndexOf(c) >= _queue.Count / 2).ToList())
            {
                // 有 65% 欲望插队
                if (Random.Shared.NextDouble() < 0.65)
                {
                    List<Skill> skills = other.Skills.Where(s => s.Level > 0 && s.SkillType == SkillType.SuperSkill && s.Enable && !s.IsInEffect && s.CurrentCD == 0 && other.EP >= s.RealEPCost).ToList();
                    if (skills.Count > 0)
                    {
                        Skill skill = skills[Random.Shared.Next(skills.Count)];
                        _castingSuperSkills.Add(other, skill);
                        other.CharacterState = CharacterState.PreCastSuperSkill;
                        _queue.Remove(other);
                        _cutCount.Remove(character);
                        AddCharacter(other, 0, false);
                        WriteLine("[ " + other + " ] 预释放了爆发技！！");
                        foreach (Character c in _hardnessTimes.Keys)
                        {
                            if (_hardnessTimes[c] != 0)
                            {
                                _hardnessTimes[c] = Calculation.Round2Digits(_hardnessTimes[c] + 0.01);
                            }
                        }
                        skill.OnSkillCasting(this, other, []);
                    }
                }
            }
        }

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
                _castingSkills.Remove(caster);
            }
            else if (_castingSuperSkills.TryGetValue(caster, out skill))
            {
                _castingSuperSkills.Remove(caster);
            }
            if (skill != null)
            {
                WriteLine($"[ {caster} ] 的施法被 [ {interrupter} ] 打断了！！");
                List<Effect> effects = skill.Effects.Where(e => e.Level > 0).ToList();
                foreach (Effect e in effects)
                {
                    e.OnSkillCastInterrupted(caster, skill, interrupter);
                }
            }
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
        /// 选取技能目标
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public virtual List<Character> SelectTargets(Character caster, Skill skill, List<Character> enemys, List<Character> teammates, out bool cancel)
        {
            cancel = false;
            if (skill.SkillType == SkillType.SuperSkill) cancel = false;
            List<Character> targets = skill.SelectTargets(caster, enemys, teammates);
            if (targets.Count == 0)
            {
                cancel = true;
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
        /// <param name="cancel"></param>
        /// <returns></returns>
        public virtual List<Character> SelectTargets(Character character, NormalAttack attack, List<Character> enemys, List<Character> teammates, out bool cancel)
        {
            cancel = false;
            if (enemys.Count > 0)
            {
                List<Character> targets = [enemys[Random.Shared.Next(enemys.Count)]];
                return targets;
            }
            return [];
        }
    }
}
