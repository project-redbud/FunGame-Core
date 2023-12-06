using System.Collections;
using Milimoe.FunGame.Core.Library.Common.Event;

// 模组需要实现什么事件就继承什么接口
namespace Milimoe.FunGame.Core.Interface
{
    public interface IGamingConnectEvent
    {
        public void BeforeGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingDisconnectEvent
    {
        public void BeforeGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingReconnectEvent
    {
        public void BeforeGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingBanCharacterEvent
    {
        public void BeforeGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingPickCharacterEvent
    {
        public void BeforeGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingRandomEvent
    {
        public void BeforeGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingLevelUpEvent
    {
        public void BeforeGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingRoundEvent
    {
        public void BeforeGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingMoveEvent
    {
        public void BeforeGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingAttackEvent
    {
        public void BeforeGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingSkillEvent
    {
        public void BeforeGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingItemEvent
    {
        public void BeforeGamingItemEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingItemEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingItemEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingItemEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingMagicEvent
    {
        public void BeforeGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingBuyEvent
    {
        public void BeforeGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingSuperSkillEvent
    {
        public void BeforeGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingPauseEvent
    {
        public void BeforeGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingUnpauseEvent
    {
        public void BeforeGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingSurrenderEvent
    {
        public void BeforeGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingUpdateInfoEvent
    {
        public void BeforeGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingPunishEvent
    {
        public void BeforeGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data);
        public void AfterGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data);
        public void SucceedGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data);
        public void FailedGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data);
    }
}
