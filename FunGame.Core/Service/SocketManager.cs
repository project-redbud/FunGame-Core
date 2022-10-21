using Milimoe.FunGame.Core.Interface.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Service
{
    internal class SocketManager : ISocket
    {
        int ISocket.Read()
        {
            throw new NotImplementedException();
        }

        int ISocket.Send()
        {
            throw new NotImplementedException();
        }
    }
}
