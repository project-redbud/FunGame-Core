using System.Collections;
using Milimoe.FunGame.Core.Library.Common.Event;

// 模组需要实现什么事件就继承什么接口
namespace Milimoe.FunGame.Core.Interface
{
    public interface IGamingConnectEvent
    {
        public void BeforeGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingDisconnectEvent
    {
        public void BeforeGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingReconnectEvent
    {
        public void BeforeGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingBanCharacterEvent
    {
        public void BeforeGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingPickCharacterEvent
    {
        public void BeforeGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingRandomEvent
    {
        public void BeforeGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingRoundEvent
    {
        public void BeforeGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingLevelUpEvent
    {
        public void BeforeGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingMoveEvent
    {
        public void BeforeGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingAttackEvent
    {
        public void BeforeGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingSkillEvent
    {
        public void BeforeGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingItemEvent
    {
        public void BeforeGamingItemEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingItemEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingItemEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingItemEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingMagicEvent
    {
        public void BeforeGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingBuyEvent
    {
        public void BeforeGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingSuperSkillEvent
    {
        public void BeforeGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingPauseEvent
    {
        public void BeforeGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingUnpauseEvent
    {
        public void BeforeGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingSurrenderEvent
    {
        public void BeforeGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingUpdateInfoEvent
    {
        public void BeforeGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingPunishEvent
    {
        public void BeforeGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void AfterGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void SucceedGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void FailedGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }
}
