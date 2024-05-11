using System.Net;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Network;

namespace Milimoe.FunGame.Core.Interface.HTTP
{
    public interface IHTTPListener : IBaseSocket
    {
        public HttpListener Instance { get; }
        public SocketObject SocketObject_Handler(SocketObject objs);
    }
}
