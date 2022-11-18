using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Constant
{
    public class InterfaceSet
    {
        public const string IClient = "IClientImpl";
        public const string IServer = "IServerImpl";
    }

    public class SocketSet
    {
        public const string Unknown = "Unknown";
        public const string GetNotice = "GetNotice";
        public const string Login = "Login";
        public const string CheckLogin = "CheckLogin";
        public const string Logout = "Logout";
        public const string Disconnect = "Disconnect";
        public const string HeartBeat = "HeartBeat";
    }
}
