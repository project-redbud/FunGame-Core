using System.Collections;
using Milimoe.FunGame.Core.Library.Common.Event;

// 模组需要实现什么事件就继承什么接口
namespace Milimoe.FunGame.Core.Interface
{
    public interface IGamingConnectEvent
    {
        public void GamingConnectEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingDisconnectEvent
    {
        public void GamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingReconnectEvent
    {
        public void GamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingBanCharacterEvent
    {
        public void GamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingPickCharacterEvent
    {
        public void GamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingRandomEvent
    {
        public void GamingRandomEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingRoundEvent
    {
        public void GamingRoundEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingLevelUpEvent
    {
        public void GamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingMoveEvent
    {
        public void GamingMoveEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingAttackEvent
    {
        public void GamingAttackEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingSkillEvent
    {
        public void GamingSkillEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingItemEvent
    {
        public void GamingItemEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingMagicEvent
    {
        public void GamingMagicEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingBuyEvent
    {
        public void GamingBuyEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingSuperSkillEvent
    {
        public void GamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingPauseEvent
    {
        public void GamingPauseEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingUnpauseEvent
    {
        public void GamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingSurrenderEvent
    {
        public void GamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingUpdateInfoEvent
    {
        public void GamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data);
    }

    public interface IGamingPunishEvent
    {
        public void GamingPunishEvent(object sender, GamingEventArgs e, Hashtable data);
    }
}
