using FunGame.Core.Library.Common.Event;

namespace FunGame.Core.Interface
{
    /// <summary>
    /// 局内事件的接口，与 <see cref="IGameEventHandler"/> 配套使用
    /// </summary>
    public interface IGamingEventHandler
    {
        public delegate void GamingEventHandler(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingConnectEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingConnect;

        public void OnGamingConnectEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingDisconnectEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingDisconnect;

        public void OnGamingDisconnectEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingReconnectEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingReconnect;

        public void OnGamingReconnectEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingBanCharacterEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingBanCharacter;

        public void OnGamingBanCharacterEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingPickCharacterEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingPickCharacter;

        public void OnGamingPickCharacterEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingRandomEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingRandom;

        public void OnGamingRandomEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingRoundEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingRound;

        public void OnGamingRoundEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingLevelUpEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingLevelUp;

        public void OnGamingLevelUpEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingMoveEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingMove;

        public void OnGamingMoveEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingAttackEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingAttack;

        public void OnGamingAttackEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingSkillEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingSkill;

        public void OnGamingSkillEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingItemEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingItem;

        public void OnGamingItemEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingMagicEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingMagic;

        public void OnGamingMagicEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingBuyEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingBuy;

        public void OnGamingBuyEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingSuperSkillEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingSuperSkill;

        public void OnGamingSuperSkillEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingPauseEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingPause;

        public void OnGamingPauseEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingUnpauseEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingUnpause;

        public void OnGamingUnpauseEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingSurrenderEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingSurrender;

        public void OnGamingSurrenderEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingUpdateInfoEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingUpdateInfo;

        public void OnGamingUpdateInfoEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    public interface IGamingPunishEventHandler : IGamingEventHandler
    {
        public event GamingEventHandler? GamingPunish;

        public void OnGamingPunishEvent(object sender, GamingEventArgs e, Dictionary<string, object> data);
    }

    /// <summary>
    /// 局内事件处理器聚合接口<para/>
    /// 实现此接口即可接收所有局内事件(GamingHandler 的分发目标)<para/>
    /// v3.0 起替代旧 <c>GameModule</c> 的事件部分
    /// </summary>
    public interface IGameEventHandler : IGamingConnectEventHandler, IGamingDisconnectEventHandler, IGamingReconnectEventHandler, IGamingBanCharacterEventHandler, IGamingPickCharacterEventHandler,
        IGamingRandomEventHandler, IGamingRoundEventHandler, IGamingLevelUpEventHandler, IGamingMoveEventHandler, IGamingAttackEventHandler, IGamingSkillEventHandler, IGamingItemEventHandler, IGamingMagicEventHandler,
        IGamingBuyEventHandler, IGamingSuperSkillEventHandler, IGamingPauseEventHandler, IGamingUnpauseEventHandler, IGamingSurrenderEventHandler, IGamingUpdateInfoEventHandler, IGamingPunishEventHandler
    {
    }
}
