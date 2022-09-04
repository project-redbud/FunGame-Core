using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Core.Api.Interface
{
    /**
     * 接口需要在FunGame.Core项目中Implement文件夹创建新的类实现
     * 参考：
     * using FunGame.Core.Interface;

        namespace FunGame.Core.Implement
        {
            public class ClientConnectInterfaceImpl : ClientConnectInterface
            {
                public string RemoteServerIP()
                {
                    // 此处修改连接远程服务器IP
                    string serverIP = "127.0.0.1";
                    string serverPort = "22222";
                    return serverIP + ":" + serverPort;
                }
            }
        }
     */

    public interface ClientConnectInterface
    {
        public string RemoteServerIP();
    }

    public interface ServerInterface
    {
        public string DBConnection();
    }
}
