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
        /// 助攻记录
        /// </summary>
        public Dictionary<Character, AssistDetail> AssistDetails { get; }

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
        public bool ProcessTurn(Character character);

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
        /// <param name="options"></param>
        public void DamageToEnemy(Character actor, Character enemy, double damage, bool isNormalAttack, DamageType damageType = DamageType.Physical, MagicType magicType = MagicType.None, DamageResult damageResult = DamageResult.Normal, DamageCalculationOptions? options = null);

        /// <summary>
        /// 治疗一个目标
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="target"></param>
        /// <param name="heal"></param>
        /// <param name="canRespawn"></param>
        /// <param name="triggerEffects"></param>
        public void HealToTarget(Character actor, Character target, double heal, bool canRespawn = false, bool triggerEffects = true);

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
        public DamageResult CalculatePhysicalDamage(Character actor, Character enemy, bool isNormalAttack, double expectedDamage, out double finalDamage, ref int changeCount, DamageCalculationOptions? options = null);

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
        public DamageResult CalculateMagicalDamage(Character actor, Character enemy, bool isNormalAttack, MagicType magicType, double expectedDamage, out double finalDamage, ref int changeCount, DamageCalculationOptions? options = null);

        /// <summary>
        /// 死亡结算
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="death"></param>
        public void DeathCalculation(Character killer, Character death);

        /// <summary>
        /// 打断施法
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="interrupter"></param>
        public void InterruptCasting(Character caster, Character interrupter);

        /// <summary>
        /// 打断施法 [ 用于使敌人目标丢失 ]
        /// </summary>
        /// <param name="interrupter"></param>
        public void InterruptCasting(Character interrupter);

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
        public bool UseItem(Item item, Character character, DecisionPoints dp, List<Character> enemys, List<Character> teammates, List<Grid> castRange, List<Character> allEnemys, List<Character> allTeammates, AIDecision? aiDecision = null);

        /// <summary>
        /// 角色移动
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        /// <param name="target"></param>
        /// <param name="startGrid"></param>
        /// <returns></returns>
        public bool CharacterMove(Character character, DecisionPoints dp, Grid target, Grid? startGrid);

        /// <summary>
        /// 选取移动目标
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="map"></param>
        /// <param name="moveRange"></param>
        /// <returns></returns>
        public Grid SelectTargetGrid(Character character, List<Character> enemys, List<Character> teammates, GameMap map, List<Grid> moveRange);

        /// <summary>
        /// 选取技能目标
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="castRange"></param>
        /// <returns></returns>
        public List<Character> SelectTargets(Character caster, Skill skill, List<Character> enemys, List<Character> teammates, List<Grid> castRange);

        /// <summary>
        /// 选取普通攻击目标
        /// </summary>
        /// <param name="character"></param>
        /// <param name="attack"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="attackRange"></param>
        /// <returns></returns>
        public List<Character> SelectTargets(Character character, NormalAttack attack, List<Character> enemys, List<Character> teammates, List<Grid> attackRange);

        /// <summary>
        /// 获取某角色的敌人列表
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public List<Character> GetEnemies(Character character);

        /// <summary>
        /// 获取某角色的队友列表
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public List<Character> GetTeammates(Character character);

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

        /// <summary>
        /// 免疫检定
        /// </summary>
        /// <param name="character"></param>
        /// <param name="target"></param>
        /// <param name="skill"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckSkilledImmune(Character character, Character target, Skill skill, Item? item = null);

        /// <summary>
        /// 技能豁免检定
        /// </summary>
        /// <param name="character"></param>
        /// <param name="source"></param>
        /// <param name="effect"></param>
        /// <param name="isEvade">true - 豁免成功等效于闪避</param>
        /// <returns></returns>
        public bool CheckExemption(Character character, Character? source, Effect effect, bool isEvade);

        /// <summary>
        /// 向角色（或控制该角色的玩家）进行询问并取得答复
        /// </summary>
        /// <param name="character"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public InquiryResponse Inquiry(Character character, InquiryOptions options);
    }
}
