using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;

namespace FunGame.Core.Model.Queue
{
    /// <remark>
    /// <see cref="GamingQueue"/> 的回合奖励：双表（回合绑定 / 角色绑定）+ 1000 窗口滚动生成 + 发放回收 + 查询增删夺取
    /// <para/>规则书前提：回合奖励仅在该回合内有效，回合结束立即丢弃。本实现在此基础上扩展：
    /// 被动奖励在「回合以吟唱（<see cref="CharacterState.Casting"/>）或预释放爆发技（<see cref="CharacterState.PreCastSuperSkill"/>）结束」时
    /// 顺延到该吟唱的结算回合，并在结算回合结束时一并清除。
    /// </remark>
    public partial class GamingQueue
    {
        #region 回合奖励：字段与状态

        /// <summary>
        /// 回合奖励的窗口大小：每张表一次物化的键数量，窗口用尽后按同一随机源惰性物化下一窗口
        /// </summary>
        public const int RoundRewardWindowSize = 1000;

        /// <summary>
        /// 回合绑定的奖励表：键 = 全局回合（<see cref="TotalRound"/>）
        /// </summary>
        protected readonly Dictionary<int, List<Skill>> _roundRewards = [];

        /// <summary>
        /// 角色绑定的奖励表：键 = 角色 → 该角色的行动回合序号（可排序），默认不生成、不启用
        /// </summary>
        protected readonly Dictionary<Character, SortedDictionary<int, List<Skill>>> _characterRoundRewards = [];

        /// <summary>
        /// 角色的行动回合计数：作为角色绑定表的查询键与 offset 换算基准（在角色回合开始处自增）
        /// </summary>
        protected readonly Dictionary<Character, int> _characterActionTurns = [];

        /// <summary>
        /// 顺延表：本回合以吟唱结束的被动奖励，顺延到该吟唱的结算回合结束时移除
        /// </summary>
        protected readonly Dictionary<Character, List<Skill>> _deferredRoundRewards = [];

        /// <summary>
        /// 本局奖励特效池：key = 特效数字标识符，value = 是否为主动技能特效
        /// </summary>
        protected Dictionary<long, bool> _roundRewardEffects = [];

        /// <summary>
        /// 是否启用角色绑定（默认关闭，关闭时角色表不生成、不查询）
        /// </summary>
        protected bool _bindRoundRewardsToCharacter = false;

        private readonly RewardTableCursor _roundRewardCursor = new();
        private readonly Dictionary<Character, RewardTableCursor> _characterRoundRewardCursors = [];
        private Dictionary<int, IReadOnlyList<Skill>>? _roundRewardsView;
        private Dictionary<Character, IReadOnlyDictionary<int, IReadOnlyList<Skill>>>? _characterRoundRewardsView;

        /// <summary>
        /// 单张奖励表的生成游标：稀疏生成（沿用 <c>Random.Next(1, 9)</c> 步进）+ 窗口物化
        /// </summary>
        private sealed class RewardTableCursor
        {
            /// <summary>已物化到的键上界</summary>
            public int MaterializedThrough { get; set; }

            /// <summary>最后一次落点的键</summary>
            public int LastKey { get; set; }

            /// <summary>已抽但尚未物化的下一个键（0 表示未抽）；跨窗口时保留，避免浪费一次随机</summary>
            public int PendingKey { get; set; }
        }

        /// <summary>
        /// 回合绑定的奖励表
        /// </summary>
        public IReadOnlyDictionary<int, IReadOnlyList<Skill>> RoundRewards
        {
            get
            {
                if (_roundRewardsView is null)
                {
                    _roundRewardsView = [];
                    foreach (KeyValuePair<int, List<Skill>> kv in _roundRewards)
                    {
                        _roundRewardsView[kv.Key] = kv.Value;
                    }
                }
                return _roundRewardsView;
            }
        }

        /// <summary>
        /// 角色绑定的奖励表
        /// </summary>
        public IReadOnlyDictionary<Character, IReadOnlyDictionary<int, IReadOnlyList<Skill>>> CharacterRoundRewards
        {
            get
            {
                if (_characterRoundRewardsView is null)
                {
                    _characterRoundRewardsView = [];
                    foreach (KeyValuePair<Character, SortedDictionary<int, List<Skill>>> kv in _characterRoundRewards)
                    {
                        Dictionary<int, IReadOnlyList<Skill>> inner = [];
                        foreach (KeyValuePair<int, List<Skill>> item in kv.Value)
                        {
                            inner[item.Key] = item.Value;
                        }
                        _characterRoundRewardsView[kv.Key] = inner;
                    }
                }
                return _characterRoundRewardsView;
            }
        }

        /// <summary>
        /// 失效只读投影缓存（仅在表结构变化时调用）
        /// </summary>
        private void InvalidateRoundRewardViews()
        {
            _roundRewardsView = null;
            _characterRoundRewardsView = null;
        }

        #endregion

        #region 回合奖励：初始化与窗口物化

        /// <summary>
        /// 初始化回合奖励<para/>
        /// 每张表在每个命中的键上固定生成 1 个奖励（不再需要 maxRound / maxRewardsInRound），
        /// 命中位置沿用稀疏步进，以 <see cref="RoundRewardWindowSize"/> 为窗口滚动生成。
        /// </summary>
        /// <param name="effects">key: 特效的数字标识符；value: 是否是主动技能的特效</param>
        /// <param name="bindToCharacter">是否启用角色绑定（默认关闭）：开启后为每个参战角色生成其行动回合维度的奖励表</param>
        /// <param name="factoryEffects">通过数字标识符来获取构造特效的参数；为空时沿用队列上已设置的工厂</param>
        public void InitRoundRewards(Dictionary<long, bool> effects, bool bindToCharacter = false, Func<long, Dictionary<string, object>>? factoryEffects = null)
        {
            _roundRewardEffects = effects ?? [];
            if (factoryEffects != null)
            {
                _factoryRoundRewardEffects = factoryEffects;
            }

            _roundRewards.Clear();
            _characterRoundRewards.Clear();
            _characterRoundRewardCursors.Clear();
            _deferredRoundRewards.Clear();
            _characterActionTurns.Clear();
            _roundRewardCursor.MaterializedThrough = 0;
            _roundRewardCursor.LastKey = 0;
            _roundRewardCursor.PendingKey = 0;
            _bindRoundRewardsToCharacter = bindToCharacter;
            InvalidateRoundRewardViews();

            // 全局表：物化第一个窗口
            EnsureRoundRewardsMaterialized(1);

            // 角色表：为每个参战角色（不含召唤物，召唤物统一折算到其 Master）物化第一个窗口
            if (_bindRoundRewardsToCharacter)
            {
                foreach (Character character in _allCharacters)
                {
                    if (character.Master is not null) continue;
                    _characterActionTurns[character] = 0;
                    EnsureCharacterRoundRewardsMaterialized(character, 1);
                }
            }
        }

        /// <summary>
        /// 确保全局奖励表已物化到 <paramref name="requiredKey"/>：不足时按 <see cref="RoundRewardWindowSize"/> 继续物化
        /// </summary>
        private void EnsureRoundRewardsMaterialized(int requiredKey)
        {
            while (_roundRewardCursor.MaterializedThrough < requiredKey)
            {
                MaterializeRewardTable(_roundRewards, _roundRewardCursor, _roundRewardCursor.MaterializedThrough + RoundRewardWindowSize);
            }
        }

        /// <summary>
        /// 确保某角色的奖励表已物化到 <paramref name="requiredTurn"/>（首次访问时惰性建表）
        /// </summary>
        private void EnsureCharacterRoundRewardsMaterialized(Character owner, int requiredTurn)
        {
            if (!_characterRoundRewardCursors.TryGetValue(owner, out RewardTableCursor? cursor) || cursor is null)
            {
                cursor = new();
                _characterRoundRewardCursors[owner] = cursor;
                _characterRoundRewards[owner] = [];
                if (!_characterActionTurns.ContainsKey(owner))
                {
                    _characterActionTurns[owner] = 0;
                }
            }
            if (!_characterRoundRewards.TryGetValue(owner, out SortedDictionary<int, List<Skill>>? table) || table is null)
            {
                table = [];
                _characterRoundRewards[owner] = table;
            }
            bool changed = false;
            while (cursor.MaterializedThrough < requiredTurn)
            {
                MaterializeRewardTable(table, cursor, cursor.MaterializedThrough + RoundRewardWindowSize);
                changed = true;
            }
            if (changed)
            {
                InvalidateRoundRewardViews();
            }
        }

        /// <summary>
        /// 把一张奖励表物化到 <paramref name="limit"/>：沿用稀疏步进（<c>baseline + Random.Next(1, 9)</c>），
        /// 每个命中键固定 1 个奖励。落点超过窗口上界时保留游标待下一窗口，不浪费随机。
        /// </summary>
        private void MaterializeRewardTable(IDictionary<int, List<Skill>> table, RewardTableCursor cursor, int limit)
        {
            if (_roundRewardEffects.Count == 0)
            {
                cursor.MaterializedThrough = limit;
                return;
            }
            while (true)
            {
                if (cursor.PendingKey <= 0)
                {
                    int baseline = cursor.LastKey > 0 ? cursor.LastKey : 1;
                    cursor.PendingKey = baseline + Random.Next(1, 9);
                }
                if (cursor.PendingKey > limit)
                {
                    break;
                }
                int key = cursor.PendingKey;
                cursor.PendingKey = 0;
                cursor.LastKey = key;
                if (!table.ContainsKey(key))
                {
                    table[key] = [CreateRoundRewardSkill()];
                }
            }
            cursor.MaterializedThrough = limit;
        }

        /// <summary>
        /// 按奖励特效池随机构造 1 个回合奖励技能
        /// </summary>
        private Skill CreateRoundRewardSkill()
        {
            long[] effectIDs = [.. _roundRewardEffects.Keys];
            long effectID = effectIDs[Random.Next(effectIDs.Length)];
            Dictionary<string, object> args = [];
            if (_roundRewardEffects[effectID])
            {
                args.Add("active", true);
                args.Add("self", true);
                args.Add("enemy", false);
            }
            Skill skill = Factory.OpenFactory.GetInstance<Skill>(effectID, "", args);
            Dictionary<string, object> effectArgs = _factoryRoundRewardEffects != null ? _factoryRoundRewardEffects(effectID) : [];
            Effect effect = Factory.OpenFactory.GetInstance(effectID, "", skill, effectArgs);
            skill.Effects.Add(effect);
            skill.Name = $"[R] {effect.Name}";
            return skill;
        }

        #endregion

        #region 回合奖励：发放与回收

        /// <summary>
        /// 角色行动回合计数自增（在角色回合开始处调用）<para/>
        /// 仅启用角色绑定时维护；召唤物（<see cref="Character.Master"/> 非空）不单独计数，统一折算到 Master
        /// </summary>
        /// <param name="character">当前行动角色</param>
        private void CountCharacterActionTurn(Character character)
        {
            if (!_bindRoundRewardsToCharacter || character.Master is not null)
            {
                return;
            }
            _characterActionTurns[character] = _characterActionTurns.TryGetValue(character, out int turn) ? turn + 1 : 1;
        }

        /// <summary>
        /// 回合开始发放回合奖励：汇总「回合绑定表中该全局回合的项」+「角色绑定表中该角色当前行动回合的项」，两表命中即叠加
        /// </summary>
        /// <param name="character">当前行动角色</param>
        /// <returns>本回合实际发放的奖励（供回合结束时回收）</returns>
        protected List<Skill> GrantRoundRewards(Character character)
        {
            List<Skill> granted = [];

            // 回合绑定：按全局回合
            EnsureRoundRewardsMaterialized(TotalRound);
            if (_roundRewards.TryGetValue(TotalRound, out List<Skill>? roundList) && roundList.Count > 0)
            {
                List<Skill> items = BindAndRelease(character, roundList);
                if (items.Count > 0)
                {
                    FireRoundRewardGained(character, RoundRewardBinding.Round, TotalRound, items);
                    granted.AddRange(items);
                }
            }

            // 角色绑定：按该角色的行动回合（召唤物折算到 Master）
            if (_bindRoundRewardsToCharacter)
            {
                Character owner = ResolveRoundRewardOwner(character);
                int actionTurn = _characterActionTurns.TryGetValue(owner, out int turn) ? turn : 0;
                if (actionTurn > 0)
                {
                    // 行动回合跨过窗口末尾时惰性物化下一窗口
                    EnsureCharacterRoundRewardsMaterialized(owner, actionTurn);
                    if (_characterRoundRewards.TryGetValue(owner, out SortedDictionary<int, List<Skill>>? table)
                        && table.TryGetValue(actionTurn, out List<Skill>? charList)
                        && charList.Count > 0)
                    {
                        List<Skill> items = BindAndRelease(character, charList);
                        if (items.Count > 0)
                        {
                            FireRoundRewardGained(character, RoundRewardBinding.Character, actionTurn, items);
                            granted.AddRange(items);
                        }
                    }
                }
            }

            return granted;
        }

        /// <summary>
        /// 把奖励表项绑定到角色并发放：主动奖励立即释放（获得即释放），被动奖励挂载特效到角色状态栏
        /// </summary>
        private List<Skill> BindAndRelease(Character character, List<Skill> source)
        {
            List<Skill> granted = [];
            foreach (Skill skill in source)
            {
                skill.GamingQueue = this;
                skill.Character = character;
                skill.Level = 1;
                skill.Source = SkillSource.Reward;
                LastRound.RoundRewards.Add(skill);
                WriteLine($"[ {character} ] 获得了回合奖励！{skill.Description}".Trim());
                if (skill.IsActive)
                {
                    skill.OnSkillCasted(this, character, [character], []);
                }
                else
                {
                    // 被动奖励必须把特效挂到角色状态栏，否则永远不可观测（挂载/移除必须对称）
                    foreach (Effect effect in skill.Effects)
                    {
                        effect.AddToCharacter(character);
                    }
                    character.Skills.Add(skill);
                }
                granted.Add(skill);
            }
            return granted;
        }

        /// <summary>
        /// 触发「获得回合奖励」：前事件 → 特效钩子 → 后事件（同一份上下文流经三者）
        /// </summary>
        private void FireRoundRewardGained(Character character, RoundRewardBinding binding, int turnKey, List<Skill> skills)
        {
            RoundRewardContext ctx = new(this, character, binding, turnKey) { Skills = skills };
            OnRoundRewardGainedBeforeEvent(ctx);
            TriggerOnRoundRewardGained(character, ctx);
            OnRoundRewardGainedAfterEvent(ctx);
        }

        /// <summary>
        /// 回合结束回收回合奖励<para/>
        /// 1. 上一吟唱回合顺延过来的被动奖励：无论本回合如何结束，都在此一并移除（对应「结算回合全部清除」）；
        /// 2. 本回合发放的奖励：若本回合以吟唱（<see cref="CharacterState.Casting"/>）或预释放爆发技
        /// （<see cref="CharacterState.PreCastSuperSkill"/>）结束，则被动奖励顺延到结算回合，主动奖励照常移除；否则全部移除。
        /// </summary>
        /// <param name="character">当前行动角色</param>
        /// <param name="granted">本回合实际发放的奖励</param>
        protected void SettleRoundRewards(Character character, List<Skill> granted)
        {
            // 1) 清除顺延来的奖励（结算回合结束）
            if (_deferredRoundRewards.TryGetValue(character, out List<Skill>? deferred) && deferred.Count > 0)
            {
                _deferredRoundRewards.Remove(character);
                RemoveRoundRewardSkills(character, deferred, true);
            }
            if (granted.Count == 0)
            {
                return;
            }

            // 2) 本回合发放的奖励
            if (IsInCastingState(character))
            {
                List<Skill> carryOver = [.. granted.Where(s => !s.IsActive)];
                List<Skill> immediately = [.. granted.Where(s => s.IsActive)];
                RemoveRoundRewardSkills(character, immediately, false);
                if (carryOver.Count > 0)
                {
                    _deferredRoundRewards[character] = carryOver;
                }
            }
            else
            {
                RemoveRoundRewardSkills(character, granted, false);
            }
        }

        /// <summary>
        /// 角色是否处于「回合因吟唱/预释放爆发技而结束」的两种状态之一
        /// </summary>
        private static bool IsInCastingState(Character character)
        {
            return character.CharacterState == CharacterState.Casting || character.CharacterState == CharacterState.PreCastSuperSkill;
        }

        /// <summary>
        /// 立即移除某角色已顺延的被动奖励（吟唱被打断、施法者死亡时调用，避免泄漏）
        /// </summary>
        private void ClearDeferredRoundRewards(Character character)
        {
            if (_deferredRoundRewards.TryGetValue(character, out List<Skill>? deferred) && deferred.Count > 0)
            {
                _deferredRoundRewards.Remove(character);
                RemoveRoundRewardSkills(character, deferred, true);
            }
        }

        /// <summary>
        /// 从角色身上移除奖励并触发失去事件与钩子
        /// </summary>
        private void RemoveRoundRewardSkills(Character character, IEnumerable<Skill> skills, bool isCarryOver)
        {
            List<Skill> list = [.. skills];
            if (list.Count == 0)
            {
                return;
            }
            RoundRewardContext ctx = new(this, character, RoundRewardBinding.Round, TotalRound)
            {
                Skills = list,
                IsCarryOver = isCarryOver
            };
            OnRoundRewardLostBeforeEvent(ctx);
            foreach (Skill skill in list)
            {
                foreach (Effect effect in skill.Effects)
                {
                    effect.RemoveFromCharacter(character);
                }
                character.Skills.Remove(skill);
            }
            TriggerOnRoundRewardLost(character, ctx);
            OnRoundRewardLostAfterEvent(ctx);
        }

        #endregion

        #region 回合奖励：查询 / 增加 / 移除 / 夺取（对外统一 offset）

        /// <summary>
        /// 奖励归属角色：召唤物（<see cref="Character.Master"/> 非空）统一折算到其 Master，
        /// 即召唤物不享受角色绑定的回合奖励，任何追加/夺取都记在 Master 名下
        /// </summary>
        private static Character ResolveRoundRewardOwner(Character character)
        {
            return character.Master ?? character;
        }

        /// <summary>
        /// offset 夹取：相对各自当前行动回合，最小为 1
        /// </summary>
        private static int ClampRoundRewardOffset(int actionTurnOffset)
        {
            return actionTurnOffset < 1 ? 1 : actionTurnOffset;
        }

        /// <summary>
        /// 取某角色当前行动回合序号（召唤物折算到 Master）
        /// </summary>
        private int CurrentActionTurnOf(Character character)
        {
            Character owner = ResolveRoundRewardOwner(character);
            return _characterActionTurns.TryGetValue(owner, out int turn) ? turn : 0;
        }

        /// <summary>
        /// 查询某角色（召唤物折算到 Master）未来第 <paramref name="actionTurnOffset"/> 个行动回合的奖励
        /// </summary>
        /// <param name="character">目标角色</param>
        /// <param name="actionTurnOffset">相对其当前行动回合的偏移，最小为 1</param>
        public IReadOnlyList<Skill> QueryRoundRewards(Character character, int actionTurnOffset)
        {
            if (!_bindRoundRewardsToCharacter)
            {
                return [];
            }
            Character owner = ResolveRoundRewardOwner(character);
            int key = CurrentActionTurnOf(owner) + ClampRoundRewardOffset(actionTurnOffset);
            // 查询跨过窗口末尾时同样惰性物化，避免把「未生成」误报成「无奖励」
            EnsureCharacterRoundRewardsMaterialized(owner, key);
            if (_characterRoundRewards.TryGetValue(owner, out SortedDictionary<int, List<Skill>>? table)
                && table.TryGetValue(key, out List<Skill>? list)
                && list.Count > 0)
            {
                return [.. list];
            }
            return [];
        }

        /// <summary>
        /// 为某角色（召唤物折算到 Master）追加一条未来行动回合的回合奖励
        /// </summary>
        /// <param name="character">目标角色</param>
        /// <param name="actionTurnOffset">相对其当前行动回合的偏移，最小为 1</param>
        /// <param name="skill">奖励技能，不可为 null</param>
        public bool AddRoundReward(Character character, int actionTurnOffset, Skill skill)
        {
            ArgumentNullException.ThrowIfNull(skill);
            if (!_bindRoundRewardsToCharacter)
            {
                return false;
            }
            Character owner = ResolveRoundRewardOwner(character);
            int key = CurrentActionTurnOf(owner) + ClampRoundRewardOffset(actionTurnOffset);
            EnsureCharacterRoundRewardsMaterialized(owner, key);
            if (!_characterRoundRewards.TryGetValue(owner, out SortedDictionary<int, List<Skill>>? table) || table is null)
            {
                return false;
            }
            List<Skill> list;
            if (table.TryGetValue(key, out List<Skill>? existing) && existing is not null)
            {
                if (existing.Contains(skill))
                {
                    return false;
                }
                list = existing;
            }
            else
            {
                list = [];
                table[key] = list;
            }
            list.Add(skill);
            skill.GamingQueue = this;
            InvalidateRoundRewardViews();
            FireRoundRewardGained(owner, RoundRewardBinding.Character, key, [skill]);
            return true;
        }

        /// <summary>
        /// 移除某角色（召唤物折算到 Master）未来第 <paramref name="actionTurnOffset"/> 个行动回合中的一条奖励
        /// </summary>
        /// <param name="character">目标角色</param>
        /// <param name="actionTurnOffset">相对其当前行动回合的偏移，最小为 1</param>
        /// <param name="skill">奖励技能，不可为 null</param>
        /// <param name="removed">被移除的奖励</param>
        public bool RemoveRoundReward(Character character, int actionTurnOffset, Skill skill, out Skill? removed)
        {
            ArgumentNullException.ThrowIfNull(skill);
            removed = null;
            if (!_bindRoundRewardsToCharacter)
            {
                return false;
            }
            Character owner = ResolveRoundRewardOwner(character);
            int key = CurrentActionTurnOf(owner) + ClampRoundRewardOffset(actionTurnOffset);
            if (!_characterRoundRewards.TryGetValue(owner, out SortedDictionary<int, List<Skill>>? table)
                || table is null
                || !table.TryGetValue(key, out List<Skill>? list)
                || list is null
                || !list.Remove(skill))
            {
                return false;
            }
            removed = skill;
            if (list.Count == 0)
            {
                table.Remove(key);
            }
            InvalidateRoundRewardViews();
            RoundRewardContext ctx = new(this, owner, RoundRewardBinding.Character, key) { Skills = [skill] };
            OnRoundRewardLostBeforeEvent(ctx);
            TriggerOnRoundRewardLost(owner, ctx);
            OnRoundRewardLostAfterEvent(ctx);
            return true;
        }

        /// <summary>
        /// 夺取目标角色（召唤物折算到 Master）未来第 <paramref name="fromOffset"/> 个行动回合的全部奖励，
        /// 并入夺取者（召唤物折算到 Master）未来第 <paramref name="toOffset"/> 个行动回合的奖励<para/>
        /// 语义：引用转移 + 字典移除；目标键位多条奖励时一次性全部夺取（仅角色绑定表）
        /// </summary>
        /// <param name="target">被夺取者</param>
        /// <param name="fromOffset">被夺取者侧的偏移，最小为 1</param>
        /// <param name="thief">夺取者</param>
        /// <param name="toOffset">夺取者侧的偏移，最小为 1</param>
        /// <param name="stolen">全部被夺取的奖励</param>
        public bool StealRoundReward(Character target, int fromOffset, Character thief, int toOffset, out List<Skill> stolen)
        {
            stolen = [];
            if (!_bindRoundRewardsToCharacter)
            {
                return false;
            }
            Character fromOwner = ResolveRoundRewardOwner(target);
            Character toOwner = ResolveRoundRewardOwner(thief);
            int fromKey = CurrentActionTurnOf(fromOwner) + ClampRoundRewardOffset(fromOffset);
            int toKey = CurrentActionTurnOf(toOwner) + ClampRoundRewardOffset(toOffset);
            if (fromOwner == toOwner && fromKey == toKey)
            {
                return false;
            }
            if (!_characterRoundRewards.TryGetValue(fromOwner, out SortedDictionary<int, List<Skill>>? fromTable)
                || fromTable is null
                || !fromTable.TryGetValue(fromKey, out List<Skill>? fromList)
                || fromList is null
                || fromList.Count == 0)
            {
                return false;
            }

            EnsureCharacterRoundRewardsMaterialized(toOwner, toKey);
            if (!_characterRoundRewards.TryGetValue(toOwner, out SortedDictionary<int, List<Skill>>? toTable) || toTable is null)
            {
                return false;
            }

            List<Skill> moved = [.. fromList];
            fromTable.Remove(fromKey);
            if (toTable.TryGetValue(toKey, out List<Skill>? toList) && toList is not null)
            {
                toList.AddRange(moved);
            }
            else
            {
                toTable[toKey] = moved;
            }
            foreach (Skill skill in moved)
            {
                skill.GamingQueue = this;
                skill.Character = toOwner;
            }
            stolen = moved;
            InvalidateRoundRewardViews();

            RoundRewardContext ctx = new(this, fromOwner, RoundRewardBinding.Character, fromKey)
            {
                Skills = moved,
                Thief = toOwner,
                From = fromOwner
            };
            OnRoundRewardStolenBeforeEvent(ctx);
            TriggerOnRoundRewardStolen(fromOwner, toOwner, ctx);
            OnRoundRewardStolenAfterEvent(ctx);
            return true;
        }

        #endregion
    }
}
