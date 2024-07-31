using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Interface.Addons
{
    public interface IGameModule : IAddon, IAddonController<IGameModule>, IGamingConnectEventHandler, IGamingDisconnectEventHandler, IGamingReconnectEventHandler, IGamingBanCharacterEventHandler, IGamingPickCharacterEventHandler,
        IGamingRandomEventHandler, IGamingRoundEventHandler, IGamingLevelUpEventHandler, IGamingMoveEventHandler, IGamingAttackEventHandler, IGamingSkillEventHandler, IGamingItemEventHandler, IGamingMagicEventHandler,
        IGamingBuyEventHandler, IGamingSuperSkillEventHandler, IGamingPauseEventHandler, IGamingUnpauseEventHandler, IGamingSurrenderEventHandler, IGamingUpdateInfoEventHandler, IGamingPunishEventHandler
    {
        public abstract void StartGame(Gaming instance, params object[] args);
        public abstract void StartUI(params object[] args);
    }
}
