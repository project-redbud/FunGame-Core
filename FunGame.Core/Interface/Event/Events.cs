using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Interface
{
    public interface IEvent
    {

    }

    public interface IConnectEvent : IEvent
    {

    }

    public interface IDisconnectEvent : IEvent
    {

    }

    public interface ILoginEvent : IEvent
    {

    }

    public interface ILogoutEvent : IEvent
    {

    }

    public interface IRegEvent : IEvent
    {

    }

    public interface IIntoRoomEvent : IEvent
    {

    }

    public interface ISendTalkEvent : IEvent
    {

    }

    public interface ICreateRoomEvent : IEvent
    {

    }

    public interface IQuitRoomEvent : IEvent
    {

    }

    public interface IChangeRoomSettingEvent : IEvent
    {

    }

    public interface IStartMatchEvent : IEvent
    {

    }

    public interface IStartGameEvent : IEvent
    {

    }

    public interface IChangeProfileEvent : IEvent
    {

    }

    public interface IChangeAccountSettingEvent : IEvent
    {

    }

    public interface IOpenStockEvent : IEvent
    {

    }

    public interface ISignInEvent : IEvent
    {

    }

    public interface IOpenStoreEvent : IEvent
    {

    }

    public interface IBuyItemEvent : IEvent
    {

    }

    public interface IShowRankingEvent : IEvent
    {

    }

    public interface IUseItemEvent : IEvent
    {

    }

    public interface IEndGameEvent : IEvent
    {

    }
}
