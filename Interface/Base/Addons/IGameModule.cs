using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Interface.Addons
{
    public interface IGameModule : IAddon, IAddonController<IGameModule>, IGamingConnectEventHandler, IGamingDisconnectEventHandler, IGamingReconnectEventHandler, IGamingBanCharacterEventHandler, IGamingPickCharacterEventHandler,
        IGamingRandomEventHandler, IGamingRoundEventHandler, IGamingLevelUpEventHandler, IGamingMoveEventHandler, IGamingAttackEventHandler, IGamingSkillEventHandler, IGamingItemEventHandler, IGamingMagicEventHandler,
        IGamingBuyEventHandler, IGamingSuperSkillEventHandler, IGamingPauseEventHandler, IGamingUnpauseEventHandler, IGamingSurrenderEventHandler, IGamingUpdateInfoEventHandler, IGamingPunishEventHandler, IGameModuleDepend
    {
        public bool HideMain { get; }
        public void StartGame(Gaming instance, params object[] args);
        public void StartUI(params object[] args);
    }
}
