using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// 行动顺序表
    /// </summary>
    public class ActionQueue
    {
        /// <summary>
        /// 用于文本输出
        /// </summary>
        public Action<string> WriteLine { get; }

        /// <summary>
        /// 当前的行动顺序
        /// </summary>
        protected readonly List<Character> _queue = [];

        /// <summary>
        /// 当前已死亡的角色顺序(第一个是最早死的)
        /// </summary>
        protected readonly List<Character> _eliminated = [];

        /// <summary>
        /// 硬直时间表
        /// </summary>
        protected readonly Dictionary<Character, double> _hardnessTimes = [];

        /// <summary>
        /// 角色正在吟唱的魔法
        /// </summary>
        protected readonly Dictionary<Character, Skill> _castingSkills = [];

        /// <summary>
        /// 角色预释放的爆发技
        /// </summary>
        protected readonly Dictionary<Character, Skill> _castingSuperSkills = [];

        /// <summary>
        /// 角色目前赚取的金钱
        /// </summary>
        protected readonly Dictionary<Character, int> _earnedMoney = [];

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
        /// 游戏是否结束
        /// </summary>
        protected bool _isGameEnd = false;

        /// <summary>
        /// 新建一个行动顺序表
        /// </summary>
        /// <param name="characters">参与本次游戏的角色列表</param>
        /// <param name="writer">用于文本输出</param>
        public ActionQueue(List<Character> characters, Action<string>? writer = null)
        {
            if (writer != null)
            {
                WriteLine = writer;
            }
            WriteLine ??= new Action<string>(Console.WriteLine);

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
            if (!BeforeTurn(character))
            {
                return _isGameEnd;
            }

            foreach (Effect effect in character.Effects.Where(e => e.Level > 0).ToList())
            {
                effect.OnTurnStart(character);
            }

            // 基础硬直时间
            double baseTime = 10;
            bool isCheckProtected = true;

            // 敌人列表
            List<Character> enemys = [.. _queue.Where(c => c != character && !c.IsUnselectable)];

            // 队友列表
            List<Character> teammates = [];

            // 技能列表
            List<Skill> skills = [.. character.Skills.Where(s => s.Level > 0 && s.SkillType != SkillType.Passive && s.Enable && !s.IsInEffect && s.CurrentCD == 0 &&
                (((s.SkillType == SkillType.SuperSkill || s.SkillType == SkillType.Skill) && s.EPCost <= character.EP) || (s.SkillType == SkillType.Magic && s.MPCost <= character.MP)))];

            // 作出了什么行动
            CharacterActionType type = CharacterActionType.None;

            if (character.CharacterState == CharacterState.Actionable)
            {
                if (character.CharacterState == CharacterState.ActionRestricted)
                {
                    // 行动受限，只能使用特殊物品
                    if (character.Items.Count > 0)
                    {
                        type = CharacterActionType.UseItem;
                    }
                }
                else if (character.CharacterState == CharacterState.BattleRestricted)
                {
                    // 战斗不能，只能使用物品
                    if (character.Items.Count > 0)
                    {
                        type = CharacterActionType.UseItem;
                    }
                }
                else if (character.CharacterState == CharacterState.SkillRestricted)
                {
                    // 技能受限，无法使用技能，可以使用物品
                    if (character.Items.Count > 0)
                    {
                        if (new Random().NextDouble() > 0.5)
                        {
                            type = CharacterActionType.UseItem;
                        }
                        else
                        {
                            type = CharacterActionType.NormalAttack;
                        }
                    }
                    else
                    {
                        type = CharacterActionType.NormalAttack;
                    }
                }
                else
                {
                    // 可以任意行动
                    bool canUseItem = character.Items.Count > 0;
                    bool canCastSkill = skills.Count > 0;
                    if (canUseItem && canCastSkill)
                    {
                        double dowhat = new Random().NextDouble();
                        if (dowhat < 0.33)
                        {
                            type = CharacterActionType.UseItem;
                        }
                        else if (dowhat < 0.66)
                        {
                            type = CharacterActionType.PreCastSkill;
                        }
                        else
                        {
                            type = CharacterActionType.NormalAttack;
                        }
                    }
                    else if (canUseItem && !canCastSkill)
                    {
                        if (new Random().NextDouble() > 0.5)
                        {
                            type = CharacterActionType.UseItem;
                        }
                        else
                        {
                            type = CharacterActionType.NormalAttack;
                        }
                    }
                    else if (!canUseItem && canCastSkill)
                    {
                        if (new Random().NextDouble() > 0.5)
                        {
                            type = CharacterActionType.PreCastSkill;
                        }
                        else
                        {
                            type = CharacterActionType.NormalAttack;
                        }
                    }
                    else type = CharacterActionType.NormalAttack;
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
                WriteLine("[ " + character + $" ] 完全行动不能！");
            }

            List<Character> enemysTemp = new(enemys);
            List<Character> teammatesTemp = new(teammates);
            List<Skill> skillsTemp = new(skills);
            Dictionary<Character, int> continuousKillingTemp = new(_continuousKilling);
            Dictionary<Character, int> earnedMoneyTemp = new(_earnedMoney);
            foreach (Effect e in character.Effects.Where(e => e.Level > 0).ToList())
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
                // 获取随机敌人
                if (enemys.Count > 0)
                {
                    Character enemy = enemys[new Random().Next(enemys.Count)];
                    character.NormalAttack.Attack(this, character, enemy);
                    baseTime = character.NormalAttack.HardnessTime;
                    foreach (Effect effect in character.Effects.Where(e => e.Level > 0).ToList())
                    {
                        effect.AlterHardnessTimeAfterNormalAttack(character, ref baseTime, ref isCheckProtected);
                    }
                }
            }
            else if (type == CharacterActionType.PreCastSkill)
            {
                // 预使用技能，即开始吟唱逻辑
                // 注意：FastAuto 模式下，此吟唱逻辑删减了选取目标的逻辑，将选取逻辑放在了实际释放的环节
                // 在正常交互式模式下，吟唱前需要先选取目标
                Skill skill = skills[new Random().Next(skills.Count)];
                if (skill.SkillType == SkillType.Magic)
                {
                    character.CharacterState = CharacterState.Casting;
                    _castingSkills.Add(character, skill);
                    baseTime = skill.CastTime;
                    skill.OnSkillCasting(this, character);
                }
                else
                {
                    if (CheckCanCast(character, skill, out double cost))
                    {
                        skill.OnSkillCasting(this, character);

                        character.EP = Calculation.Round2Digits(character.EP - cost);
                        baseTime = skill.HardnessTime;
                        skill.CurrentCD = Calculation.Round2Digits(Math.Max(1, skill.CD * (1 - character.CDR)));
                        skill.Enable = false;

                        WriteLine("[ " + character + $" ] 消耗了 {cost:f2} 点能量，释放了{(skill.IsSuperSkill ? "爆发技" : "战技")} {skill.Name}！");
                        skill.OnSkillCasted(this, character, enemys, teammates);

                        foreach (Effect effect in character.Effects.Where(e => e.Level > 0).ToList())
                        {
                            effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                        }
                    }
                }
            }
            else if (type == CharacterActionType.CastSkill)
            {
                // 使用技能逻辑，结束吟唱状态
                character.CharacterState = CharacterState.Actionable;
                Skill skill = _castingSkills[character];
                _castingSkills.Remove(character);

                // 判断是否能够释放技能
                if (CheckCanCast(character, skill, out double cost))
                {
                    character.MP = Calculation.Round2Digits(character.MP - cost);
                    baseTime = skill.HardnessTime;
                    skill.CurrentCD = Calculation.Round2Digits(Math.Max(1, skill.CD * (1 - character.CDR)));
                    skill.Enable = false;

                    WriteLine("[ " + character + $" ] 消耗了 {cost:f2} 点魔法值，释放了技能 {skill.Name}！");
                    skill.OnSkillCasted(this, character, enemys, teammates);
                }
                else
                {
                    WriteLine("[ " + character + $" ] 放弃释放技能！");
                    // 放弃释放技能会获得3的硬直时间
                    baseTime = 3;
                }

                foreach (Effect effect in character.Effects.Where(e => e.Level > 0).ToList())
                {
                    effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                }
            }
            else if (type == CharacterActionType.CastSuperSkill)
            {
                // 结束预释放爆发技的状态
                character.CharacterState = CharacterState.Actionable;
                Skill skill = _castingSuperSkills[character];
                _castingSuperSkills.Remove(character);

                // 判断是否能够释放技能
                if (CheckCanCast(character, skill, out double cost))
                {
                    character.EP = Calculation.Round2Digits(character.EP - cost);
                    baseTime = skill.HardnessTime;
                    skill.CurrentCD = Calculation.Round2Digits(Math.Max(1, skill.CD * (1 - character.CDR)));
                    skill.Enable = false;

                    WriteLine("[ " + character + $" ] 消耗了 {cost:f2} 点能量值，释放了爆发技 {skill.Name}！");
                    skill.OnSkillCasted(this, character, enemys, teammates);
                }
                else
                {
                    WriteLine("[ " + character + $" ] 放弃释放技能！");
                    // 放弃释放技能会获得3的硬直时间
                    baseTime = 3;
                }

                foreach (Effect effect in character.Effects.Where(e => e.Level > 0).ToList())
                {
                    effect.AlterHardnessTimeAfterCastSkill(character, skill, ref baseTime, ref isCheckProtected);
                }
            }
            else if (type == CharacterActionType.UseItem)
            {
                // 使用物品逻辑
            }
            else
            {
                WriteLine("[ " + character + $" ] 放弃了行动！");
            }

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
            }
            AddCharacter(character, newHardnessTime, isCheckProtected);

            // 有人想要插队吗？
            WillPreCastSuperSkill(character);

            foreach (Effect effect in character.Effects.Where(e => e.Level > 0).ToList())
            {
                effect.OnTurnEnd(character);

                // 自身被动不会考虑
                if (effect.ControlType == EffectControlType.None && effect.Skill.SkillType == SkillType.Passive)
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

            WriteLine("时间流逝：" + timeToReduce);

            // 减少所有角色的硬直时间
            foreach (Character character in _queue)
            {
                _hardnessTimes[character] = Calculation.Round2Digits(_hardnessTimes[character] - timeToReduce);

                // 回血回蓝
                double recoveryHP = Calculation.Round2Digits(character.HR * timeToReduce);
                double recoveryMP = Calculation.Round2Digits(character.MR * timeToReduce);
                double needHP = Calculation.Round2Digits(character.MaxHP - character.HP);
                double needMP = Calculation.Round2Digits(character.MaxMP - character.MP);
                double reallyReHP = needHP >= recoveryHP ? recoveryHP : needHP;
                double reallyReMP = needMP >= recoveryMP ? recoveryMP : needMP;
                if (reallyReHP > 0 && reallyReMP > 0)
                {
                    character.HP = Calculation.Round2Digits(character.HP + reallyReHP);
                    character.MP = Calculation.Round2Digits(character.MP + reallyReMP);
                    WriteLine("角色 " + character.NickName + " 回血：" + recoveryHP + " / " + "回蓝：" + recoveryMP);
                }
                else
                {
                    if (reallyReHP > 0)
                    {
                        character.HP = Calculation.Round2Digits(character.HP + reallyReHP);
                        WriteLine("角色 " + character.NickName + " 回血：" + recoveryHP);
                    }
                    if (reallyReMP > 0)
                    {
                        character.MP = Calculation.Round2Digits(character.MP + reallyReMP);
                        WriteLine("角色 " + character.NickName + " 回蓝：" + recoveryMP);
                    }
                }

                // 减少所有技能的冷却时间
                foreach (Skill skill in character.Skills)
                {
                    skill.CurrentCD = Calculation.Round2Digits(skill.CurrentCD - timeToReduce);
                    if (skill.CurrentCD <= 0)
                    {
                        skill.CurrentCD = 0;
                        skill.Enable = true;
                    }
                }

                // 移除到时间的特效
                foreach (Effect effect in character.Effects.ToList())
                {
                    if (effect.Level == 0)
                    {
                        character.Effects.Remove(effect);
                        continue;
                    }

                    effect.OnTimeElapsed(character, timeToReduce);

                    // 自身被动不会考虑
                    if (effect.ControlType == EffectControlType.None && effect.Skill.SkillType == SkillType.Passive)
                    {
                        continue;
                    }

                    if (effect.Durative)
                    {
                        effect.RemainDuration = Calculation.Round2Digits(effect.RemainDuration - timeToReduce);
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
            bool isEvaded = damageResult == DamageResult.Evaded;
            foreach (Effect effect in actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0).ToList())
            {
                if (effect.AlterActualDamageAfterCalculation(actor, enemy, ref damage, isNormalAttack, isMagicDamage, magicType, damageResult))
                {
                    isEvaded = true;
                    damageResult = DamageResult.Evaded;
                }
            }

            // 闪避了就没伤害了
            if (!isEvaded)
            {
                if (damage < 0) damage = 0;
                if (isMagicDamage)
                {
                    string dmgType = CharacterSet.GetMagicName(magicType);
                    WriteLine("[ " + enemy + $" ] 受到了 {damage} 点{dmgType}！");
                }
                else WriteLine("[ " + enemy + $" ] 受到了 {damage} 点物理伤害！");
                enemy.HP = Calculation.Round2Digits(enemy.HP - damage);

                // 计算助攻
                _assistDamage[actor][enemy] += damage;

                // 造成伤害和受伤都可以获得能量
                double ep = GetEP(damage, 0.2, 40);
                foreach (Effect effect in actor.Effects)
                {
                    effect.AlterEPAfterDamage(actor, ref ep);
                }
                actor.EP += ep;
                ep = GetEP(damage, 0.1, 20);
                foreach (Effect effect in enemy.Effects.Where(e => e.Level > 0).ToList())
                {
                    effect.AlterEPAfterGetDamage(enemy, ref ep);
                }
                enemy.EP += ep;
            }

            foreach (Effect effect in actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0).ToList())
            {
                effect.AfterDamageCalculation(actor, enemy, damage, isNormalAttack, isMagicDamage, magicType, damageResult);
            }
            if (enemy.HP <= 0 && !_eliminated.Contains(enemy))
            {
                DeathCalculation(actor, enemy);
                // 给所有角色的特效广播角色死亡结算
                foreach (Effect effect in _queue.SelectMany(c => c.Effects.Where(e => e.Level > 0).ToList()))
                {
                    effect.AfterDeathCalculation(enemy, actor, _continuousKilling, _earnedMoney);
                }
                if (_queue.Remove(enemy) && (!_queue.Where(c => c != actor).Any()))
                {
                    // 没有其他的角色了，游戏结束
                    EndGameInfo(actor);
                }
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
            return Calculation.Round2Digits(Math.Min((a + new Random().Next(30)) * b, max));
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
            foreach(Effect effect in actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0).ToList())
            {
                effect.AlterDamageTypeBeforeCalculation(actor, enemy, ref isNormalAttack, ref isMagic, ref magicType);
            }
            if (isMagic)
            {
                return CalculateMagicalDamage(actor, enemy, isNormalAttack, magicType, expectedDamage, out finalDamage);
            }

            foreach (Effect effect in actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0).ToList())
            {
                effect.AlterExpectedDamageBeforeCalculation(actor, enemy, ref expectedDamage, isNormalAttack, false, MagicType.None);
            }

            double dice = new Random().NextDouble();
            if (isNormalAttack)
            {
                // 闪避判定
                if (dice < enemy.EvadeRate)
                {
                    finalDamage = 0;
                    List<Character> characters = [actor, enemy];
                    bool isAlterEvaded = false;
                    foreach (Effect effect in characters.SelectMany(c => c.Effects.Where(e => e.Level > 0).ToList()))
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

            // 物理穿透后的护甲
            double penetratedDEF = Calculation.Round2Digits((1 - actor.PhysicalPenetration) * enemy.DEF);

            // 物理伤害减免
            double physicalDamageReduction = Calculation.Round4Digits(penetratedDEF / (penetratedDEF + 120));

            // 最终的物理伤害
            finalDamage = Calculation.Round2Digits(expectedDamage * (1 - physicalDamageReduction));

            // 暴击判定
            dice = new Random().NextDouble();
            if (dice < actor.CritRate)
            {
                finalDamage = Calculation.Round2Digits(finalDamage * actor.CritDMG); // 暴击伤害倍率加成
                WriteLine("暴击生效！！");
                foreach (Effect effect in actor.Effects.Where(e => e.Level > 0).ToList())
                {
                    effect.OnCriticalDamageTriggered(actor, dice);
                }
                return DamageResult.Critical;
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
            foreach (Effect effect in actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0).ToList())
            {
                effect.AlterDamageTypeBeforeCalculation(actor, enemy, ref isNormalAttack, ref isMagic, ref magicType);
            }
            if (!isMagic)
            {
                return CalculatePhysicalDamage(actor, enemy, isNormalAttack, expectedDamage, out finalDamage);
            }

            foreach (Effect effect in actor.Effects.Union(enemy.Effects).Where(e => e.Level > 0).ToList())
            {
                effect.AlterExpectedDamageBeforeCalculation(actor, enemy, ref expectedDamage, isNormalAttack, true, magicType);
            }

            double dice = new Random().NextDouble();
            if (isNormalAttack)
            {
                // 闪避判定
                if (dice < enemy.EvadeRate)
                {
                    finalDamage = 0;
                    List<Character> characters = [actor, enemy];
                    bool isAlterEvaded = false;
                    foreach (Effect effect in characters.SelectMany(c => c.Effects.Where(e => e.Level > 0).ToList()))
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

            MagicResistance magicResistance = magicType switch
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
            double MDF = Calculation.Round2Digits((1 - actor.MagicalPenetration) * magicResistance.Value);

            // 最终的魔法伤害
            finalDamage = Calculation.Round2Digits(expectedDamage * (1 - MDF));

            // 暴击判定
            dice = new Random().NextDouble();
            if (dice < actor.CritRate)
            {
                finalDamage = Calculation.Round2Digits(finalDamage * actor.CritDMG); // 暴击伤害倍率加成
                WriteLine("暴击生效！！");
                foreach (Effect effect in actor.Effects.Where(e => e.Level > 0).ToList())
                {
                    effect.OnCriticalDamageTriggered(actor, dice);
                }
                return DamageResult.Critical;
            }

            // 是否有效伤害
            return DamageResult.Normal;
        }

        /// <summary>
        /// 死亡结算
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="death"></param>
        public void DeathCalculation(Character killer, Character death)
        {
            if (!_continuousKilling.TryAdd(killer, 1)) _continuousKilling[killer] += 1;
            int money = new Random().Next(250, 350);

            Character[] assists = _assistDamage.Keys.Where(c => c != death && _assistDamage[c].GetPercentage(death) > 0.10).ToArray();
            double totalDamagePercentage = Calculation.Round4Digits(_assistDamage.Keys.Where(assists.Contains).Select(c => _assistDamage[c].GetPercentage(death)).Sum());
            int totalMoney = Math.Min(Convert.ToInt32(money * totalDamagePercentage), 425); // 防止刷伤害设置金钱上限

            // 按伤害比分配金钱 只有造成10%伤害以上才能参与
            foreach (Character assist in assists)
            {
                int cmoney = Convert.ToInt32(_assistDamage[assist].GetPercentage(death) / totalDamagePercentage * totalMoney);
                if (assist != killer)
                {
                    if (!_earnedMoney.TryAdd(assist, cmoney)) _earnedMoney[assist] += cmoney;
                }
                else
                {
                    money = cmoney;
                }
            }

            // 终结击杀的奖励仍然是全额的
            if (_continuousKilling.TryGetValue(death, out int coefficient) && coefficient > 1)
            {
                money += (coefficient + 1) * new Random().Next(100, 200);
                string termination = CharacterSet.GetContinuousKilling(coefficient);
                string msg = $"[ {killer} ] 终结了 [ {death} ]{(termination != "" ? " 的" + termination : "")}，获得 {money} 金钱！";
                if (assists.Length > 1)
                {
                    msg += "助攻：[ " + string.Join(" ] / [ ", assists.Where(c => c != killer)) + " ]";
                }
                WriteLine(msg);
            }
            else
            {
                string msg = $"[ {killer} ] 杀死了 [ {death} ]，获得 {money} 金钱！";
                if (assists.Length > 1)
                {
                    msg += "助攻：[ " + string.Join(" ] / [ ", assists.Where(c => c != killer)) + " ]";
                }
                WriteLine(msg);
            }

            int kills = _continuousKilling[killer];
            string continuousKilling = CharacterSet.GetContinuousKilling(kills);
            if (kills == 2 || kills == 3)
            {
                WriteLine("[ " + killer + " ] 完成了" + continuousKilling + "！");
            }
            else if (kills == 4)
            {
                WriteLine("[ " + killer + " ] 正在" + continuousKilling + "！");
            }
            else if (kills > 4 && kills < 10)
            {
                WriteLine("[ " + killer + " ] 已经" + continuousKilling + "！");
            }
            else if (kills >= 10)
            {
                WriteLine("[ " + killer + " ] 已经" + continuousKilling + "！拜托谁去杀了他吧！！！");
            }
             
            if (!_earnedMoney.TryAdd(killer, money)) _earnedMoney[killer] += money;

            _eliminated.Add(death);
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
                string topCharacter = _eliminated[i].ToString() + (_continuousKilling.TryGetValue(_eliminated[i], out int kills) && kills > 1 ? $" [ {CharacterSet.GetContinuousKilling(kills)} ]" : "") + (_earnedMoney.TryGetValue(_eliminated[i], out int earned) ? $" [ 已赚取 {earned} 金钱 ]" : "");
                if (top == 1)
                {
                    WriteLine("冠军：" + topCharacter);
                }
                else if (top == 2)
                {
                    WriteLine("亚军：" + topCharacter);
                }
                else if (top == 3)
                {
                    WriteLine("季军：" + topCharacter);
                }
                else
                {
                    WriteLine($"第 {top} 名：" + topCharacter);
                }
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
                cost = skill.MPCost;
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
                cost = skill.EPCost;
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
                if (new Random().NextDouble() < 0.65)
                {
                    List<Skill> skills = other.Skills.Where(s => s.Level > 0 && s.SkillType == SkillType.SuperSkill && s.Enable && !s.IsInEffect && s.CurrentCD == 0 && other.EP >= s.EPCost).ToList();
                    if (skills.Count > 0)
                    {
                        Skill skill = skills[new Random().Next(skills.Count)];
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
                        skill.OnSkillCasting(this, other);
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
            if (_castingSkills.TryGetValue(caster, out Skill? cast))
            {
                _castingSkills.Remove(caster);
            }
            else if (_castingSuperSkills.TryGetValue(caster, out cast))
            {
                _castingSuperSkills.Remove(caster);
            }
            if (cast != null)
            {
                WriteLine($"[ {caster} ] 的施法被 [ {interrupter} ] 打断了！！");
                foreach (Effect e in cast.Effects.Where(e => e.Level > 0).ToList())
                {
                    e.OnSkillCastInterrupted(caster, cast, interrupter);
                }
            }
        }
    }
}
