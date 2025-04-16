/**
 * 此文件保存State（状态）的枚举
 */
namespace Milimoe.FunGame.Core.Library.Constant
{
    public enum EntityState
    {
        Unchanged,
        Added,
        Modified,
        Deleted
    }

    public enum StartMatchState
    {
        Matching,
        Success,
        Enable,
        Cancel
    }

    public enum RoomState
    {
        Created,
        Closed,
        Matching,
        Gaming,
        Complete
    }

    /// <summary>
    /// 角色目前所处的状态
    /// </summary>
    public enum CharacterState
    {
        /// <summary>
        /// 可以行动 [ 战斗相关 ]
        /// </summary>
        Actionable,

        /// <summary>
        /// 完全行动不能 [ 战斗相关 ]
        /// </summary>
        NotActionable,

        /// <summary>
        /// 行动受限 [ 战斗相关 ]
        /// </summary>
        ActionRestricted,

        /// <summary>
        /// 战斗不能 [ 战斗相关 ]
        /// </summary>
        BattleRestricted,

        /// <summary>
        /// 技能受限 [ 战斗相关 ]
        /// </summary>
        SkillRestricted,
        
        /// <summary>
        /// 攻击受限 [ 战斗相关 ]
        /// </summary>
        AttackRestricted,

        /// <summary>
        /// 处于吟唱中 [ 战斗相关 ] [ 技能相关 ]
        /// </summary>
        Casting,

        /// <summary>
        /// 预释放爆发技(插队) [ 战斗相关 ] [ 技能相关 ]
        /// </summary>
        PreCastSuperSkill
    }

    public enum SelectState
    {
        None,
        SelectingOne,
        SelectingMultiple,
        SelectingRange,
        SelectingLine
    }

    public enum OnlineState
    {
        Offline,
        Online,
        Matching,
        InRoom,
        Gaming
    }

    public enum ClientState
    {
        WaitConnect,
        WaitLogin,
        Online,
        InRoom
    }

    public enum QuestState
    {
        NotStarted,
        InProgress,
        Completed,
        Settled,
        Missed
    }

    public enum ActivityState
    {
        Future,
        Upcoming,
        InProgress,
        Ended
    }

    public enum OfferState
    {
        Created,
        Cancelled,
        PendingOfferorConfirmation,
        PendingOffereeConfirmation,
        OfferorConfirmed,
        OffereeConfirmed,
        Sent,
        Negotiating,
        NegotiationAccepted,
        Rejected,
        Completed,
        Expired
    }

    public enum MarketItemState
    {
        Listed,
        Delisted,
        Purchased
    }
}
