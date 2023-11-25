namespace Milimoe.FunGame.Core.Interface
{
    public interface IPlugin : IAddon, IConnectEventHandler, IDisconnectEventHandler, ILoginEventHandler, ILogoutEventHandler, IRegEventHandler, IIntoRoomEventHandler, ISendTalkEventHandler,
        ICreateRoomEventHandler, IQuitRoomEventHandler, IChangeRoomSettingEventHandler, IStartMatchEventHandler, IStartGameEventHandler, IChangeProfileEventHandler, IChangeAccountSettingEventHandler,
        IOpenInventoryEventHandler, ISignInEventHandler, IOpenStoreEventHandler, IBuyItemEventHandler, IShowRankingEventHandler, IUseItemEventHandler, IEndGameEventHandler
    {

    }
}
