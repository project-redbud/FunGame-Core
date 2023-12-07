using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Interface
{
    public interface IPlugin : IAddon, IAddonController, IConnectEventHandler, IDisconnectEventHandler, ILoginEventHandler, ILogoutEventHandler, IRegEventHandler, IIntoRoomEventHandler, ISendTalkEventHandler,
        ICreateRoomEventHandler, IQuitRoomEventHandler, IChangeRoomSettingEventHandler, IStartMatchEventHandler, IStartGameEventHandler, IChangeProfileEventHandler, IChangeAccountSettingEventHandler,
        IOpenInventoryEventHandler, ISignInEventHandler, IOpenStoreEventHandler, IBuyItemEventHandler, IShowRankingEventHandler, IUseItemEventHandler, IEndGameEventHandler
    {

    }
}
