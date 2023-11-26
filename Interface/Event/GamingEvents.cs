using Milimoe.FunGame.Core.Library.Common.Event;

// 模组需要实现什么事件就继承什么接口
namespace Milimoe.FunGame.Core.Interface
{
    public interface IGamingConnectEvent
    {
        public void BeforeGamingConnectEvent(object sender, GamingEventArgs e);
        public void AfterGamingConnectEvent(object sender, GamingEventArgs e);
        public void SucceedGamingConnectEvent(object sender, GamingEventArgs e);
        public void FailedGamingConnectEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingDisconnectEvent
    {
        public void BeforeGamingDisconnectEvent(object sender, GamingEventArgs e);
        public void AfterGamingDisconnectEvent(object sender, GamingEventArgs e);
        public void SucceedGamingDisconnectEvent(object sender, GamingEventArgs e);
        public void FailedGamingDisconnectEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingReconnectEvent
    {
        public void BeforeGamingReconnectEvent(object sender, GamingEventArgs e);
        public void AfterGamingReconnectEvent(object sender, GamingEventArgs e);
        public void SucceedGamingReconnectEvent(object sender, GamingEventArgs e);
        public void FailedGamingReconnectEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingBanCharacterEvent
    {
        public void BeforeGamingBanCharacterEvent(object sender, GamingEventArgs e);
        public void AfterGamingBanCharacterEvent(object sender, GamingEventArgs e);
        public void SucceedGamingBanCharacterEvent(object sender, GamingEventArgs e);
        public void FailedGamingBanCharacterEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingPickCharacterEvent
    {
        public void BeforeGamingPickCharacterEvent(object sender, GamingEventArgs e);
        public void AfterGamingPickCharacterEvent(object sender, GamingEventArgs e);
        public void SucceedGamingPickCharacterEvent(object sender, GamingEventArgs e);
        public void FailedGamingPickCharacterEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingRandomEvent
    {
        public void BeforeGamingRandomEvent(object sender, GamingEventArgs e);
        public void AfterGamingRandomEvent(object sender, GamingEventArgs e);
        public void SucceedGamingRandomEvent(object sender, GamingEventArgs e);
        public void FailedGamingRandomEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingMoveEvent
    {
        public void BeforeGamingMoveEvent(object sender, GamingEventArgs e);
        public void AfterGamingMoveEvent(object sender, GamingEventArgs e);
        public void SucceedGamingMoveEvent(object sender, GamingEventArgs e);
        public void FailedGamingMoveEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingAttackEvent
    {
        public void BeforeGamingAttackEvent(object sender, GamingEventArgs e);
        public void AfterGamingAttackEvent(object sender, GamingEventArgs e);
        public void SucceedGamingAttackEvent(object sender, GamingEventArgs e);
        public void FailedGamingAttackEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingSkillEvent
    {
        public void BeforeGamingSkillEvent(object sender, GamingEventArgs e);
        public void AfterGamingSkillEvent(object sender, GamingEventArgs e);
        public void SucceedGamingSkillEvent(object sender, GamingEventArgs e);
        public void FailedGamingSkillEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingItemEvent
    {
        public void BeforeGamingItemEvent(object sender, GamingEventArgs e);
        public void AfterGamingItemEvent(object sender, GamingEventArgs e);
        public void SucceedGamingItemEvent(object sender, GamingEventArgs e);
        public void FailedGamingItemEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingMagicEvent
    {
        public void BeforeGamingMagicEvent(object sender, GamingEventArgs e);
        public void AfterGamingMagicEvent(object sender, GamingEventArgs e);
        public void SucceedGamingMagicEvent(object sender, GamingEventArgs e);
        public void FailedGamingMagicEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingBuyEvent
    {
        public void BeforeGamingBuyEvent(object sender, GamingEventArgs e);
        public void AfterGamingBuyEvent(object sender, GamingEventArgs e);
        public void SucceedGamingBuyEvent(object sender, GamingEventArgs e);
        public void FailedGamingBuyEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingSuperSkillEvent
    {
        public void BeforeGamingSuperSkillEvent(object sender, GamingEventArgs e);
        public void AfterGamingSuperSkillEvent(object sender, GamingEventArgs e);
        public void SucceedGamingSuperSkillEvent(object sender, GamingEventArgs e);
        public void FailedGamingSuperSkillEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingPauseEvent
    {
        public void BeforeGamingPauseEvent(object sender, GamingEventArgs e);
        public void AfterGamingPauseEvent(object sender, GamingEventArgs e);
        public void SucceedGamingPauseEvent(object sender, GamingEventArgs e);
        public void FailedGamingPauseEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingUnpauseEvent
    {
        public void BeforeGamingUnpauseEvent(object sender, GamingEventArgs e);
        public void AfterGamingUnpauseEvent(object sender, GamingEventArgs e);
        public void SucceedGamingUnpauseEvent(object sender, GamingEventArgs e);
        public void FailedGamingUnpauseEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingSurrenderEvent
    {
        public void BeforeGamingSurrenderEvent(object sender, GamingEventArgs e);
        public void AfterGamingSurrenderEvent(object sender, GamingEventArgs e);
        public void SucceedGamingSurrenderEvent(object sender, GamingEventArgs e);
        public void FailedGamingSurrenderEvent(object sender, GamingEventArgs e);
    }

    public interface IGamingUpdateInfoEvent
    {
        public void BeforeGamingUpdateInfoEvent(object sender, GamingEventArgs e);
        public void AfterGamingUpdateInfoEvent(object sender, GamingEventArgs e);
        public void SucceedGamingUpdateInfoEvent(object sender, GamingEventArgs e);
        public void FailedGamingUpdateInfoEvent(object sender, GamingEventArgs e);
    }
}
