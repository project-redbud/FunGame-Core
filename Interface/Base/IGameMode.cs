namespace Milimoe.FunGame.Core.Interface
{
    public interface IGameMode : IAddon, IGamingConnectEventHandler, IGamingDisconnectEventHandler, IGamingReconnectEventHandler, IGamingBanCharacterEventHandler, IGamingPickCharacterEventHandler,
        IGamingRandomEventHandler, IGamingMoveEventHandler, IGamingAttackEventHandler, IGamingSkillEventHandler, IGamingItemEventHandler, IGamingMagicEventHandler, IGamingBuyEventHandler,
        IGamingSuperSkillEventHandler, IGamingPauseEventHandler, IGamingUnpauseEventHandler, IGamingSurrenderEventHandler, IGamingUpdateInfoEventHandler
    {

    }
}
