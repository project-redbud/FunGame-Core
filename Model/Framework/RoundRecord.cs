using System.Text;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    public class RoundRecord(int round)
    {
        public int Round { get; set; } = round;
        public Character Actor { get; set; } = new();
        /// <summary>
        /// 本回合内的全部操作记录（决策点系统允许一回合多次操作，按执行顺序排列，<see cref="ActionRecord.ActionIndex"/> 从 1 开始）
        /// </summary>
        public List<ActionRecord> Actions { get; } = [];
        public HashSet<CharacterActionType> ActionTypes { get; } = [];
        public Dictionary<CharacterActionType, List<Character>> Targets { get; } = [];
        public Dictionary<CharacterActionType, Skill> Skills { get; } = [];
        public Dictionary<Skill, string> SkillsCost { get; set; } = [];
        public Dictionary<CharacterActionType, Item> Items { get; set; } = [];
        public Dictionary<Item, string> ItemsCost { get; set; } = [];
        public bool HasKill { get; set; } = false;
        public List<Character> Assists { get; set; } = [];
        public Dictionary<Character, double> Damages { get; set; } = [];
        /// <summary>
        /// 每个目标按伤害类型分桶的伤害值（物理/魔法/真实；各桶之和与 <see cref="Damages"/> 一致）
        /// </summary>
        public Dictionary<Character, Dictionary<DamageType, double>> DamageDetails { get; set; } = [];
        public Dictionary<Character, bool> IsCritical { get; set; } = [];
        public Dictionary<Character, bool> IsEvaded { get; set; } = [];
        public Dictionary<Character, bool> IsImmune { get; set; } = [];
        public Dictionary<Character, double> Heals { get; set; } = [];
        /// <summary>
        /// 角色 -> 技能。施放技能时由队列写入 [施法者 -> 技能]；
        /// 特效钩子被触发时由框架自动写入（仅当开发者重写了对应钩子方法），key 取特效所在状态栏的角色（<see cref="Character.Effects"/> 归属），施法者/技能持有者未知时回退。
        /// </summary>
        public Dictionary<Character, Skill> Effects { get; set; } = [];
        public Dictionary<Character, List<EffectType>> ApplyEffects { get; set; } = [];
        public List<string> ActorContinuousKilling { get; set; } = [];
        public List<string> DeathContinuousKilling { get; set; } = [];
        public double CastTime { get; set; } = 0;
        public double HardnessTime { get; set; } = 0;
        public Dictionary<Character, double> RespawnCountdowns { get; set; } = [];
        public List<Character> Respawns { get; set; } = [];
        public List<Skill> RoundRewards { get; set; } = [];
        public List<string> OtherMessages { get; set; } = [];

        /// <summary>
        /// 全角色清单（开局时由队列写入所有参与角色，供回放端在开局获取完整角色列表；后续回合由序列化时动态收集出现的角色）
        /// </summary>
        public List<Character> AllCharacters { get; set; } = [];

        /// <summary>
        /// 角色队伍归属映射（角色 Guid -> 队伍名；团队模式开局时由队列写入，非团队模式为空）
        /// </summary>
        public Dictionary<Guid, string> TeamMap { get; set; } = [];

        /// <summary>
        /// 所有角色的最终统计数据（游戏结束时由队列写入所有参与角色的 <see cref="CharacterStatistics"/>，其他回合为空）
        /// </summary>
        public Dictionary<Character, CharacterStatistics> CharacterStatistics { get; set; } = [];

        /// <summary>
        /// 游戏结束信息（胜者与排名条目，按名次顺序排列；游戏结束时由队列写入，非游戏结束回合为空）
        /// </summary>
        public List<RankingEntry> GameResult { get; set; } = [];

        /// <summary>
        /// 回合结束时的游戏总时间（状态推算的时间轴基准）
        /// </summary>
        public double TotalTime { get; set; } = 0;

        /// <summary>
        /// 状态检查点（周期性生成的全角色状态快照列表，每个元素为一个角色状态，作为状态推算的精确基准；非检查点回合为 null）
        /// </summary>
        public List<CharacterStateSnapshot>? Checkpoint { get; set; } = null;

        public void AddApplyEffects(Character character, params IEnumerable<EffectType> types)
        {
            if (ApplyEffects.TryGetValue(character, out List<EffectType>? list) && list != null)
            {
                list.AddRange(types);
            }
            else
            {
                ApplyEffects.TryAdd(character, [.. types]);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine($"=== Round {Round} ===");
            if (RoundRewards.Count > 0)
            {
                builder.AppendLine($"[ {Actor} ] 回合奖励 -> {string.Join(" / ", RoundRewards.Select(s => s.Name)).Trim()}");
            }

            // 操作流：按执行顺序逐条输出
            foreach (ActionRecord action in Actions)
            {
                builder.AppendLine(action.ToString());
            }

            // 回合级杂项消息
            if (OtherMessages.Count > 0) builder.AppendLine(string.Join("\r\n", OtherMessages));

            // 本回合被施加的特效类型
            foreach (KeyValuePair<Character, List<EffectType>> kv in ApplyEffects)
            {
                builder.AppendLine($"[ {kv.Key} ] 被施加了 [ {string.Join(" ] / [ ", kv.Value.Select(t => SkillSet.GetEffectTypeName(t)))} ]");
            }

            if (CastTime > 0)
            {
                builder.AppendLine($"[ {Actor} ] 吟唱持续时间：{CastTime:0.##}");
            }
            else
            {
                builder.AppendLine($"[ {Actor} ] 回合结束，硬直时间：{HardnessTime:0.##}");
            }

            foreach (Character character in RespawnCountdowns.Keys)
            {
                builder.AppendLine($"[ {character} ] 进入复活倒计时：{RespawnCountdowns[character]:0.##}");
            }

            foreach (Character character in Respawns)
            {
                builder.AppendLine($"[ {character} ] 复活了");
            }

            // 游戏结束信息
            if (GameResult.Count > 0)
            {
                builder.AppendLine("");
                builder.AppendLine("=== 游戏结束 ===");
                foreach (RankingEntry entry in GameResult)
                {
                    string name = entry.IsTeam ? $"[ {entry.Team} ] 团队" : $"[ {entry.Character} ]";
                    string stats = entry.IsTeam ? $"得分 {entry.Score} / 击杀 {entry.Kills} / 助攻 {entry.Assists}" :
                        $"击杀 {entry.Kills} / 死亡 {entry.Deaths} / 助攻 {entry.Assists}{(entry.TotalEarnedMoney > 0 ? $" / 赚取 {entry.TotalEarnedMoney} {General.GameplayEquilibriumConstant.InGameCurrency}" : "")}";
                    builder.AppendLine($"{(entry.IsWinner ? "★ " : "")}{entry.Rank} 名：{name}（{stats}）");
                }
            }

            // 角色最终统计数据（游戏结束时写入）
            if (CharacterStatistics.Count > 0)
            {
                builder.AppendLine("");
                builder.AppendLine("=== 角色统计 ===");
                foreach (KeyValuePair<Character, CharacterStatistics> kv in CharacterStatistics)
                {
                    CharacterStatistics s = kv.Value;
                    string teamTag = TeamMap.TryGetValue(kv.Key.Guid, out string? team) && team != "" ? $"[{team}] " : "";
                    builder.AppendLine($"{teamTag}[ {kv.Key} ] 伤害 {s.TotalDamage:0.##} / 治疗 {s.TotalHeal:0.##} / 击杀 {s.Kills} / 死亡 {s.Deaths} / 助攻 {s.Assists} / 金币 {s.TotalEarnedMoney}");
                }
            }

            // 开局（第 1 回合）与游戏结束回合输出全角色状态（装备/物品/技能/状态栏）
            if (Round == 1 || GameResult.Count > 0)
            {
                if (Checkpoint != null && Checkpoint.Count > 0)
                {
                    builder.AppendLine("");
                    builder.AppendLine("=== 角色状态 ===");
                    foreach (CharacterStateSnapshot state in Checkpoint)
                    {
                        string teamTag = TeamMap.TryGetValue(state.Character.Guid, out string? team) && team != "" ? $"[{team}] " : "";
                        builder.Append($"{teamTag}[ {state.Character} ] HP {state.HP:0.##}/{state.MaxHP:0.##} MP {state.MP:0.##}/{state.MaxMP:0.##} EP {state.EP:0.##}");
                        List<string> details = [];
                        if (state.EquipmentsDetail.Count > 0) details.Add("装备: " + string.Join(" / ", state.EquipmentsDetail.Select(e => $"{ItemSet.GetEquipSlotTypeName(e.Slot)}={e.ItemName}")));
                        else if (state.Equipments.Count > 0) details.Add("装备: " + string.Join(" / ", state.Equipments.Select(e => $"{ItemSet.GetEquipSlotTypeName(e.Key)}={e.Value}")));
                        if (state.Items.Count > 0) details.Add("物品: " + string.Join(" / ", state.Items.Select(i => i.ItemName)));
                        if (state.Skills.Count > 0) details.Add("技能: " + string.Join(" / ", state.Skills.Select(s => s.CurrentCD > 0 ? $"{s.SkillName} Lv{s.Level} CD{s.CurrentCD:0.##}" : $"{s.SkillName} Lv{s.Level}")));
                        if (state.Effects.Count > 0) details.Add("状态: " + string.Join(" / ", state.Effects.Select(e => (e.RemainDurationTurn > 0 ? $"{e.EffectName} 剩余{e.RemainDurationTurn}R" : e.RemainDuration > 0 ? $"{e.EffectName} 剩余{e.RemainDuration:0.##}" : e.EffectName) + (e.SourceGuid != Guid.Empty ? $" 来源:{e.SourceGuid}" : ""))));
                        if (state.Attributes.Count > 0) details.Add("属性: " + string.Join(" / ", state.Attributes.Select(kv => $"{kv.Key} {kv.Value}")));
                        if (details.Count > 0) builder.AppendLine("\r\n  " + string.Join("\r\n  ", details));
                        else builder.AppendLine();
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 生成当前记录的结构快照<para/>
        /// 所有集合与字典均为独立副本，但其中的实体引用（<see cref="Character"/> / <see cref="Skill"/> / <see cref="Item"/>）与当前记录共享；<para/>
        /// 用于事件分发与回合归档，保证订阅端看到的是快照时刻的回合状态，不受后续对集合结构的修改（新增、覆盖、删除条目）影响
        /// </summary>
        /// <returns></returns>
        public RoundRecord Snapshot()
        {
            RoundRecord snapshot = new(Round)
            {
                Actor = Actor,
                HasKill = HasKill,
                Assists = [.. Assists],
                Damages = new(Damages),
                DamageDetails = DamageDetails.ToDictionary(kv => kv.Key, kv => new Dictionary<DamageType, double>(kv.Value)),
                IsCritical = new(IsCritical),
                IsEvaded = new(IsEvaded),
                IsImmune = new(IsImmune),
                Heals = new(Heals),
                Effects = new(Effects),
                ApplyEffects = ApplyEffects.ToDictionary(kv => kv.Key, kv => (List<EffectType>)[.. kv.Value]),
                ActorContinuousKilling = [.. ActorContinuousKilling],
                DeathContinuousKilling = [.. DeathContinuousKilling],
                CastTime = CastTime,
                HardnessTime = HardnessTime,
                RespawnCountdowns = new(RespawnCountdowns),
                Respawns = [.. Respawns],
                RoundRewards = [.. RoundRewards],
                OtherMessages = [.. OtherMessages],
                SkillsCost = new(SkillsCost),
                Items = new(Items),
                ItemsCost = new(ItemsCost)
            };
            foreach (CharacterActionType type in ActionTypes)
            {
                snapshot.ActionTypes.Add(type);
            }
            foreach (KeyValuePair<CharacterActionType, List<Character>> kv in Targets)
            {
                snapshot.Targets[kv.Key] = [.. kv.Value];
            }
            foreach (KeyValuePair<CharacterActionType, Skill> kv in Skills)
            {
                snapshot.Skills[kv.Key] = kv.Value;
            }
            foreach (ActionRecord action in Actions)
            {
                snapshot.Actions.Add(action.Snapshot());
            }
            // 检查点列表拷贝（元素共享，防快照后对列表结构的修改）
            if (Checkpoint != null)
            {
                snapshot.Checkpoint = [.. Checkpoint];
            }
            snapshot.AllCharacters = [.. AllCharacters];
            snapshot.TeamMap = new(TeamMap);
            snapshot.GameResult = [.. GameResult];
            snapshot.CharacterStatistics = new(CharacterStatistics);
            snapshot.TotalTime = TotalTime;
            return snapshot;
        }
    }
}
