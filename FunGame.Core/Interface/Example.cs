namespace Milimoe.FunGame.Core.Interface
{
    /**
     * 接口需要在FunGame.Core项目中创建新的类实现
     * namespace必须是Milimoe.FunGame.Core.Implement，文件夹位置随意
     * 参考：
     * using Milimoe.FunGame.Core.Interface;

        namespace Milimoe.FunGame.Core.Implement
        {
            public class IClientImpl : IClient
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
}
