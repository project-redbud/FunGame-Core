using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Constant
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
