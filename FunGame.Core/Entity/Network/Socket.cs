using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Entity.Network
{
    public class Socket
    {
        public System.Net.Sockets.Socket Instance { get; }
        
        private Socket(System.Net.Sockets.Socket Instance)
        {
            this.Instance = Instance;
        }

        public static Socket Connect(string IP, int Port = 22222)
        {
            System.Net.Sockets.Socket? socket = SocketManager.Connect(IP, Port);
            if (socket != null) return new Socket(socket);
            else throw new Milimoe.FunGame.Core.Entity.Exception.SystemError("创建Socket失败。");
        }
    }
}
