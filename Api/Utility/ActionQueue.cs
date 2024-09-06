using System.Text;
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

        protected readonly List<Character> _queue = [];
        protected readonly Dictionary<Character, double> _hardnessTimes = [];
        protected readonly Dictionary<Character, Skill> _castingSkills = [];
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

            // 排序时，时间会流逝
            int nowTime = 1;

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
                    AddCharacter(group.First(), _queue.Count + nowTime);
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

                            nowTime++;

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
                            AddCharacter(selectedCharacter, characters.Count + nowTime);
                            WriteLine("decided: " + selectedCharacter.Name + "\r\n\r\n");
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
        public void AddCharacter(Character character, double hardnessTime)
        {
            // 插队机制：按硬直时间排序
            int insertIndex = _queue.FindIndex(c => _hardnessTimes[c] > hardnessTime);
            if (insertIndex == -1)
            {
                _queue.Add(character);
            }
            else
            {
                _queue.Insert(insertIndex, character);
            }
            _hardnessTimes[character] = hardnessTime;
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
                return character;
            }

            return null;
        }

        /// <summary>
        /// 显示当前所有角色的状态和硬直时间
        /// </summary>
        public void DisplayQueue()
        {
            StringBuilder text = new();

            text.AppendLine("==== 角色状态 ====");
            foreach (Character c in _queue)
            {
                text.AppendLine("角色 [ " + c + " ]");
                text.AppendLine($"生命值：{c.HP} / {c.MaxHP}" + (c.ExHP + c.ExHP2 > 0 ? $" [{c.BaseHP} + {c.ExHP + c.ExHP2}]" : ""));
                text.AppendLine($"魔法值：{c.MP} / {c.MaxMP}" + (c.ExMP + c.ExMP2 > 0 ? $" [{c.BaseMP} + {c.ExMP + c.ExMP2}]" : ""));
                if (c.CharacterState != CharacterState.Actionable)
                {
                    string state = c.CharacterState switch
                    {
                        CharacterState.Casting => "角色正在吟唱魔法",
                        CharacterState.ActionRestricted => "角色现在行动受限",
                        CharacterState.BattleRestricted => "角色现在战斗不能",
                        CharacterState.SkillRestricted => "角色现在技能受限",
                        CharacterState.Neutral => "角色现在是无敌的",
                        _ => "角色现在完全行动不能"
                    };
                    text.AppendLine(state);
                }
                text.AppendLine($"硬直时间：{_hardnessTimes[c]}");
            }

            WriteLine(text.ToString());
        }

        /// <summary>
        /// 回合开始前触发，在 <see cref="NextCharacter"/> 前触发
        /// </summary>
        /// <returns></returns>
        public virtual void BeforeTurn()
        {

        }

        /// <summary>
        /// 角色 <paramref name="character"/> 的回合进行中
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool ProcessTurn(Character character)
        {
            BeforeTurn();

            // 假设基础硬直时间
            double baseTime = new Random().Next(10, 30);

            // 敌人列表
            List<Character> enemys = [.. _queue.Where(c => c != character && c.CharacterState != CharacterState.Neutral)];

            // 队友列表
            List<Character> teammates = [];

            // 技能列表
            List<Skill> skills = [.. character.Skills.Values.Where(s => s.Enable && s.CurrentCD == 0 && ((s.IsMagic && s.MPCost <= character.MP) || (!s.IsMagic && s.EPCost <= character.EP)))];

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
            else
            {
                // 完全行动不能会获得10时间硬直
                baseTime = 10;
                WriteLine("[ " + character + $" ] 完全行动不能！");
            }

            if (type == CharacterActionType.NormalAttack)
            {
                // 使用普通攻击逻辑
                // 获取随机敌人
                if (enemys.Count > 0)
                {
                    Character enemy = enemys[new Random().Next(enemys.Count)];
                    character.NormalAttack.Attack(this, character, enemy);
                    // 普通攻击的默认硬直时间为7
                    baseTime = 7;
                }
            }
            else if (type == CharacterActionType.PreCastSkill)
            {
                // 预使用技能，即开始吟唱逻辑
                // 注意：FastAuto 模式下，此吟唱逻辑删减了选取目标的逻辑，将选取逻辑放在了实际释放的环节
                // 在正常交互式模式下，吟唱前需要先选取目标
                Skill skill = skills[new Random().Next(skills.Count)];
                character.CharacterState = CharacterState.Casting;
                _castingSkills.Add(character, skill);
                baseTime = skill.CastTime;
            }
            else if (type == CharacterActionType.CastSkill)
            {
                // 使用技能逻辑，结束吟唱状态
                character.CharacterState = CharacterState.Actionable;
                _castingSkills.Remove(character);

                // 敌人数量需大于0
                bool isCast = false;
                if (enemys.Count > 0)
                {
                    double cost = 0;

                    Skill skill = skills[new Random().Next(skills.Count)];
                    if (skill.IsMagic)
                    {
                        cost = skill.MPCost;
                        if (cost > 0 && cost <= character.MP)
                        {
                            isCast = true;
                        }
                        else
                        {
                            WriteLine("[ " + character + $" ] 魔法不足！");
                        }
                    }
                    else
                    {
                        cost = skill.EPCost;
                        if (cost > 0 && cost <= character.EP)
                        {
                            isCast = true;
                        }
                        else
                        {
                            WriteLine("[ " + character + $" ] 能量不足！");
                        }
                    }

                    if (isCast)
                    {
                        character.MP = Calculation.Round2Digits(character.MP - cost);
                        baseTime = skill.HardnessTime;
                        skill.CurrentCD = Calculation.Round2Digits(Math.Max(1, skill.CD * (1 - character.CDR)));
                        skill.Enable = false;

                        WriteLine("[ " + character + $" ] 消耗了 {cost:f2} 点魔法值，释放了技能 {skill.Name}！");

                        Dictionary<string, object> args = [];
                        args.TryAdd(CharacterActionSet.ActionQueue, this);
                        args.TryAdd(CharacterActionSet.Actor, character);
                        args.TryAdd(CharacterActionSet.CastSkill, skill);
                        args.TryAdd(CharacterActionSet.Enemys, enemys);
                        args.TryAdd(CharacterActionSet.Teammates, teammates);
                        skill.Trigger(args);
                    }
                }

                if (!isCast)
                {
                    WriteLine("[ " + character + $" ] 放弃释放技能！");
                    // 放弃释放技能会获得3的硬直时间
                    baseTime = 3;
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
                return true;
            }

            // 减少硬直时间
            double newHardnessTime = baseTime;
            if (character.CharacterState != CharacterState.Casting)
            {
                newHardnessTime = Math.Max(1, Calculation.Round2Digits(baseTime * (1 - character.ActionCoefficient)));
                WriteLine("[ " + character + " ] 回合结束，获得硬直时间: " + newHardnessTime + "\r\n");
            }
            else
            {
                newHardnessTime = Math.Max(1, Calculation.Round2Digits(baseTime * (1 - character.AccelerationCoefficient)));
                WriteLine("[ " + character + " ] 进行吟唱，持续时间: " + newHardnessTime + "\r\n");
            }
            AddCharacter(character, newHardnessTime);

            AfterTurn(character);

            return false;
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
                    WriteLine("角色 " + character.Name + " 回血：" + recoveryHP + " / " + "回蓝：" + recoveryMP);
                }
                else
                {
                    if (reallyReHP > 0)
                    {
                        character.HP = Calculation.Round2Digits(character.HP + reallyReHP);
                        WriteLine("角色 " + character.Name + " 回血：" + recoveryHP);
                    }
                    if (reallyReMP > 0)
                    {
                        character.MP = Calculation.Round2Digits(character.MP + reallyReMP);
                        WriteLine("角色 " + character.Name + "回蓝：" + recoveryMP);
                    }
                }

                // 减少所有技能的冷却时间
                foreach (Skill skill in character.Skills.Values)
                {
                    skill.CurrentCD = Calculation.Round2Digits(skill.CurrentCD - timeToReduce);
                    if (skill.CurrentCD <= 0)
                    {
                        skill.CurrentCD = 0;
                        skill.Enable = true;
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
        /// <param name="isMagicDamage"></param>
        /// <param name="magicType"></param>
        /// <returns>输出日志</returns>
        public void DamageToEnemy(Character actor, Character enemy, double damage, bool isMagicDamage = false, MagicType magicType = MagicType.None)
        {
            if (damage < 0) damage = 0;
            if (isMagicDamage)
            {
                string dmgType = MagicSet.GetMagicName(magicType);
                WriteLine("[ " + enemy + $" ] 受到了 {damage} 点{dmgType}！");
            }
            else WriteLine("[ " + enemy + $" ] 受到了 {damage} 点物理伤害！");
            enemy.HP = Calculation.Round2Digits(enemy.HP - damage);
            if (enemy.HP < 0)
            {
                WriteLine("[ " + actor + " ] 杀死了 [ " + enemy + $" ]，获得 {new Random().Next(200, 400)} 金钱！");
                if (_queue.Remove(enemy) && (!_queue.Where(c => c != actor).Any()))
                {
                    WriteLine("[ " + actor + " ] 是胜利者。");
                    _isGameEnd = true;
                }
            }
        }

        /// <summary>
        /// 计算物理伤害
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="target"></param>
        /// <param name="expectedDamage"></param>
        /// <param name="finalDamage"></param>
        /// <returns></returns>
        public DamageResult CalculatePhysicalDamage(Character actor, Character target, double expectedDamage, out double finalDamage)
        {
            // 闪避判定
            if (new Random().NextDouble() < target.EvadeRate)
            {
                finalDamage = 0;
                WriteLine("此物理攻击被完美闪避了！");
                return DamageResult.Evaded;
            }

            // 物理穿透后的护甲
            double penetratedDEF = Calculation.Round2Digits((1 - actor.PhysicalPenetration) * target.DEF);

            // 物理伤害减免
            double physicalDamageReduction = Calculation.Round4Digits(penetratedDEF / (penetratedDEF + 120));

            // 最终的物理伤害
            finalDamage = Calculation.Round2Digits(expectedDamage * (1 - physicalDamageReduction));

            // 暴击判定
            if (new Random().NextDouble() < actor.CritRate)
            {
                finalDamage = Calculation.Round2Digits(finalDamage * actor.CritDMG); // 暴击伤害倍率加成
                WriteLine("暴击生效！！");
                return DamageResult.Critical;
            }

            // 是否有效伤害
            return DamageResult.Normal;
        }

        /// <summary>
        /// 计算魔法伤害
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="target"></param>
        /// <param name="magicType"></param>
        /// <param name="expectedDamage"></param>
        /// <param name="finalDamage"></param>
        /// <returns></returns>
        public DamageResult CalculateMagicalDamage(Character actor, Character target, MagicType magicType, double expectedDamage, out double finalDamage)
        {
            MagicResistance magicResistance = magicType switch
            {
                MagicType.Starmark => target.MDF.Starmark,
                MagicType.PurityNatural => target.MDF.PurityNatural,
                MagicType.PurityContemporary => target.MDF.PurityContemporary,
                MagicType.Bright => target.MDF.Bright,
                MagicType.Shadow => target.MDF.Shadow,
                MagicType.Element => target.MDF.Element,
                MagicType.Fleabane => target.MDF.Fleabane,
                MagicType.Particle => target.MDF.Particle,
                _ => target.MDF.None
            };

            // 魔法穿透后的魔法抗性
            double MDF = Calculation.Round2Digits((1 - actor.MagicalPenetration) * magicResistance.Value);

            // 最终的魔法伤害
            finalDamage = Calculation.Round2Digits(expectedDamage * (1 - MDF));

            // 暴击判定
            if (new Random().NextDouble() < actor.CritRate)
            {
                finalDamage = Calculation.Round2Digits(finalDamage * actor.CritDMG); // 暴击伤害倍率加成
                WriteLine("暴击生效！！");
                return DamageResult.Critical;
            }

            // 是否有效伤害
            return DamageResult.Normal;
        }
    }
}
