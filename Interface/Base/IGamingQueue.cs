using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Interface.Base
{
    /// <summary>
    /// 回合制游戏的基础队列
    /// </summary>
    public interface IGamingQueue
    {
        /// <summary>
        /// 使用的游戏平衡常数
        /// </summary>
        public EquilibriumConstant GameplayEquilibriumConstant { get; }

        /// <summary>
        /// 用于文本输出
        /// </summary>
        public Action<string> WriteLine { get; }

        /// <summary>
        /// 原始的角色字典
        /// </summary>
        public Dictionary<Guid, Character> Original { get; }

        /// <summary>
        /// 参与本次游戏的所有角色列表
        /// </summary>
        public List<Character> AllCharacters { get; }

        /// <summary>
        /// 当前的行动顺序
        /// </summary>
        public List<Character> Queue { get; }

        /// <summary>
        /// 上回合记录
        /// </summary>
        public RoundRecord LastRound { get; set; }

        /// <summary>
        /// 所有回合的记录
        /// </summary>
        public List<RoundRecord> Rounds { get; }

        /// <summary>
        /// 当前已死亡的角色顺序(第一个是最早死的)
        /// </summary>
        public List<Character> Eliminated { get; }

        /// <summary>
        /// 角色数据
        /// </summary>
        public Dictionary<Character, CharacterStatistics> CharacterStatistics { get; }

        /// <summary>
        /// 角色的决策点
        /// </summary>
        public Dictionary<Character, DecisionPoints> CharacterDecisionPoints { get; }

        /// <summary>
        /// 游戏运行的时间
        /// </summary>
        public double TotalTime { get; }

        /// <summary>
        /// 游戏运行的回合
        /// 对于某角色而言，在其行动的回合叫 Turn，而所有角色行动的回合，都称之为 Round。
        /// </summary>
        public int TotalRound { get; }

        /// <summary>
        /// 使用的地图
        /// </summary>
        public GameMap? Map { get; }

        /// <summary>
        /// 显示队列信息
        /// </summary>
        public void DisplayQueue();

        /// <summary>
        /// 处理回合动作
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public Task<bool> ProcessTurnAsync(Character character);

        /// <summary>
        /// 造成伤害
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="damageType"></param>
        /// <param name="magicType"></param>
        /// <param name="damageResult"></param>
        public Task DamageToEnemyAsync(Character actor, Character enemy, double damage, bool isNormalAttack, DamageType damageType = DamageType.Physical, MagicType magicType = MagicType.None, DamageResult damageResult = DamageResult.Normal);

        /// <summary>
        /// 治疗一个目标
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="target"></param>
        /// <param name="heal"></param>
        /// <param name="canRespawn"></param>
        public Task HealToTargetAsync(Character actor, Character target, double heal, bool canRespawn = false);

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
        public DamageResult CalculatePhysicalDamage(Character actor, Character enemy, bool isNormalAttack, double expectedDamage, out double finalDamage, ref int changeCount);

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
        public DamageResult CalculateMagicalDamage(Character actor, Character enemy, bool isNormalAttack, MagicType magicType, double expectedDamage, out double finalDamage, ref int changeCount);

        /// <summary>
        /// 死亡结算
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="death"></param>
        public Task DeathCalculationAsync(Character killer, Character death);

        /// <summary>
        /// 打断施法
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="interrupter"></param>
        public Task InterruptCastingAsync(Character caster, Character interrupter);

        /// <summary>
        /// 打断施法 [ 用于使敌人目标丢失 ]
        /// </summary>
        /// <param name="interrupter"></param>
        public Task InterruptCastingAsync(Character interrupter);

        /// <summary>
        /// 使用物品
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
        public Task<bool> UseItemAsync(Item item, Character character, DecisionPoints dp, List<Character> enemys, List<Character> teammates, List<Grid> castRange, List<Character> allEnemys, List<Character> allTeammates, AIDecision? aiDecision = null);

        /// <summary>
        /// 角色移动
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        /// <param name="target"></param>
        /// <param name="startGrid"></param>
        /// <returns></returns>
        public Task<bool> CharacterMoveAsync(Character character, DecisionPoints dp, Grid target, Grid? startGrid);

        /// <summary>
        /// 选取移动目标
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="map"></param>
        /// <param name="moveRange"></param>
        /// <returns></returns>
        public Task<Grid> SelectTargetGridAsync(Character character, List<Character> enemys, List<Character> teammates, GameMap map, List<Grid> moveRange);

        /// <summary>
        /// 选取技能目标
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="castRange"></param>
        /// <returns></returns>
        public Task<List<Character>> SelectTargetsAsync(Character caster, Skill skill, List<Character> enemys, List<Character> teammates, List<Grid> castRange);

        /// <summary>
        /// 选取普通攻击目标
        /// </summary>
        /// <param name="character"></param>
        /// <param name="attack"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="attackRange"></param>
        /// <returns></returns>
        public Task<List<Character>> SelectTargetsAsync(Character character, NormalAttack attack, List<Character> enemys, List<Character> teammates, List<Grid> attackRange);

        /// <summary>
        /// 判断目标对于某个角色是否是队友
        /// </summary>
        /// <param name="character"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsTeammate(Character character, Character target);

        /// <summary>
        /// 获取目标对于某个角色是否是队友的字典
        /// </summary>
        /// <param name="character"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public Dictionary<Character, bool> GetIsTeammateDictionary(Character character, IEnumerable<Character> targets);

        /// <summary>
        /// 设置角色为 AI 控制
        /// </summary>
        /// <returns></returns>
        public void SetCharactersToAIControl(bool bySystem = true, bool cancel = false, params IEnumerable<Character> characters);

        /// <summary>
        /// 检查角色是否在 AI 控制状态
        /// </summary>
        /// <returns></returns>
        public bool IsCharacterInAIControlling(Character character);

        /// <summary>
        /// 修改角色的硬直时间
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="addValue">加值</param>
        /// <param name="isPercentage">是否是百分比</param>
        /// <param name="isCheckProtected">是否使用插队保护机制</param>
        public void ChangeCharacterHardnessTime(Character character, double addValue, bool isPercentage, bool isCheckProtected);

        /// <summary>
        /// 计算角色的数据
        /// </summary>
        /// <param name="character"></param>
        /// <param name="characterTaken"></param>
        /// <param name="damage"></param>
        /// <param name="damageType"></param>
        /// <param name="takenDamage"></param>
        public void CalculateCharacterDamageStatistics(Character character, Character characterTaken, double damage, DamageType damageType, double takenDamage = -1);
    }
}
