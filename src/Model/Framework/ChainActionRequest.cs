using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    /// <summary>绑定某一次正常回合的协同行动申请；提交不扣资源。</summary>
    public sealed class ChainActionRequest
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Guid TurnId { get; }
        public int Round { get; }
        public Character TurnActor { get; }
        public Character Actor { get; }
        public CharacterActionType ActionType { get; }
        public Skill? Skill { get; }
        public Item? Item { get; }
        public IReadOnlyList<Character> Targets { get; }
        public IReadOnlyList<Grid> TargetGrids { get; }

        /// <summary>范围方案的合法施法中心，空值表示直接提交影响格。</summary>
        public Grid? CastCenter { get; }

        internal ChainActionRequest(Guid turnId, int round, Character turnActor, Character actor,
            CharacterActionType actionType, Skill? skill, Item? item, IEnumerable<Character> targets, IEnumerable<Grid> grids, Grid? castCenter = null)
        {
            TurnId = turnId;
            Round = round;
            TurnActor = turnActor;
            Actor = actor;
            ActionType = actionType;
            Skill = skill;
            Item = item;
            Targets = Array.AsReadOnly(targets.Distinct().ToArray());
            TargetGrids = Array.AsReadOnly(grids.Distinct().ToArray());
            CastCenter = castCenter;
        }
    }

    /// <summary>确认失败不扣费；成功开始吟唱也视为成功发动。</summary>
    public sealed record ChainActionResult(bool IsSuccess, string FailReason = "", bool IsCasting = false,
        double HardnessTime = 0, ActionRecord? Record = null);

    /// <summary>吟唱期间冻结的原硬直及协同归属。</summary>
    internal sealed record ChainCastContext(ChainActionRequest Request, double OriginalHardnessTime);
}
