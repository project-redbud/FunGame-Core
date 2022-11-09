using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Others
{
    public enum InterfaceType
    {
        IClient,
        IServer
    }

    public class InterfaceSet
    {
        public const string IClient = "IClientImpl";
        public const string IServer = "IServerImpl";
    }
}
