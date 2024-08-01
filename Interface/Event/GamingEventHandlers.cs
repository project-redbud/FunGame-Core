using System.Collections;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Common.Event;

namespace Milimoe.FunGame.Core.Interface
{
    /// <summary>
    /// 局内事件的接口，与 <see cref="GameModule"/> 配套使用
    /// </summary>
    public interface IGamingEventHandler
    {
        public delegate void BeforeEventHandler(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public delegate void AfterEventHandler(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public delegate void SucceedEventHandler(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public delegate void FailedEventHandler(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingConnectEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingConnect;
        public event AfterEventHandler? AfterGamingConnect;
        public event SucceedEventHandler? SucceedGamingConnect;
        public event FailedEventHandler? FailedGamingConnect;

        public void OnBeforeGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingDisconnectEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingDisconnect;
        public event AfterEventHandler? AfterGamingDisconnect;
        public event SucceedEventHandler? SucceedGamingDisconnect;
        public event FailedEventHandler? FailedGamingDisconnect;

        public void OnBeforeGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingReconnectEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingReconnect;
        public event AfterEventHandler? AfterGamingReconnect;
        public event SucceedEventHandler? SucceedGamingReconnect;
        public event FailedEventHandler? FailedGamingReconnect;

        public void OnBeforeGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingBanCharacterEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingBanCharacter;
        public event AfterEventHandler? AfterGamingBanCharacter;
        public event SucceedEventHandler? SucceedGamingBanCharacter;
        public event FailedEventHandler? FailedGamingBanCharacter;

        public void OnBeforeGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingPickCharacterEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingPickCharacter;
        public event AfterEventHandler? AfterGamingPickCharacter;
        public event SucceedEventHandler? SucceedGamingPickCharacter;
        public event FailedEventHandler? FailedGamingPickCharacter;

        public void OnBeforeGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingRandomEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingRandom;
        public event AfterEventHandler? AfterGamingRandom;
        public event SucceedEventHandler? SucceedGamingRandom;
        public event FailedEventHandler? FailedGamingRandom;

        public void OnBeforeGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingRoundEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingRound;
        public event AfterEventHandler? AfterGamingRound;
        public event SucceedEventHandler? SucceedGamingRound;
        public event FailedEventHandler? FailedGamingRound;

        public void OnBeforeGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingLevelUpEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingLevelUp;
        public event AfterEventHandler? AfterGamingLevelUp;
        public event SucceedEventHandler? SucceedGamingLevelUp;
        public event FailedEventHandler? FailedGamingLevelUp;

        public void OnBeforeGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingMoveEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingMove;
        public event AfterEventHandler? AfterGamingMove;
        public event SucceedEventHandler? SucceedGamingMove;
        public event FailedEventHandler? FailedGamingMove;

        public void OnBeforeGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingAttackEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingAttack;
        public event AfterEventHandler? AfterGamingAttack;
        public event SucceedEventHandler? SucceedGamingAttack;
        public event FailedEventHandler? FailedGamingAttack;

        public void OnBeforeGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingSkillEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingSkill;
        public event AfterEventHandler? AfterGamingSkill;
        public event SucceedEventHandler? SucceedGamingSkill;
        public event FailedEventHandler? FailedGamingSkill;

        public void OnBeforeGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingItemEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingItem;
        public event AfterEventHandler? AfterGamingItem;
        public event SucceedEventHandler? SucceedGamingItem;
        public event FailedEventHandler? FailedGamingItem;

        public void OnBeforeGamingItemEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingItemEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingItemEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingItemEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingMagicEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingMagic;
        public event AfterEventHandler? AfterGamingMagic;
        public event SucceedEventHandler? SucceedGamingMagic;
        public event FailedEventHandler? FailedGamingMagic;

        public void OnBeforeGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingBuyEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingBuy;
        public event AfterEventHandler? AfterGamingBuy;
        public event SucceedEventHandler? SucceedGamingBuy;
        public event FailedEventHandler? FailedGamingBuy;

        public void OnBeforeGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingSuperSkillEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingSuperSkill;
        public event AfterEventHandler? AfterGamingSuperSkill;
        public event SucceedEventHandler? SucceedGamingSuperSkill;
        public event FailedEventHandler? FailedGamingSuperSkill;

        public void OnBeforeGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingPauseEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingPause;
        public event AfterEventHandler? AfterGamingPause;
        public event SucceedEventHandler? SucceedGamingPause;
        public event FailedEventHandler? FailedGamingPause;

        public void OnBeforeGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingUnpauseEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingUnpause;
        public event AfterEventHandler? AfterGamingUnpause;
        public event SucceedEventHandler? SucceedGamingUnpause;
        public event FailedEventHandler? FailedGamingUnpause;

        public void OnBeforeGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingSurrenderEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingSurrender;
        public event AfterEventHandler? AfterGamingSurrender;
        public event SucceedEventHandler? SucceedGamingSurrender;
        public event FailedEventHandler? FailedGamingSurrender;

        public void OnBeforeGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingUpdateInfoEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingUpdateInfo;
        public event AfterEventHandler? AfterGamingUpdateInfo;
        public event SucceedEventHandler? SucceedGamingUpdateInfo;
        public event FailedEventHandler? FailedGamingUpdateInfo;

        public void OnBeforeGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }

    public interface IGamingPunishEventHandler : IGamingEventHandler
    {
        public event BeforeEventHandler? BeforeGamingPunish;
        public event AfterEventHandler? AfterGamingPunish;
        public event SucceedEventHandler? SucceedGamingPunish;
        public event FailedEventHandler? FailedGamingPunish;

        public void OnBeforeGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnAfterGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnSucceedGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
        public void OnFailedGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result);
    }
}
