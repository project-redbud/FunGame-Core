using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Enum
{
    public enum SocketHelperMethod
    {
        CreateSocket,
        CloseSocket,
        StartSocketHelper,
        Login,
        Logout,
        Disconnect
    }

    public enum InterfaceMethod
    {
        RemoteServerIP,
        DBConnection,
        GetServerSettings
    }
}
