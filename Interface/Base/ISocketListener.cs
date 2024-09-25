using Milimoe.FunGame.Core.Api.Utility;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISocketListener<T> where T : ISocketMessageProcessor
    {
        /// <summary>
        /// 已连接的客户端列表
        /// </summary>
        public ConcurrentModelList<IServerModel> ClientList { get; }

        /// <summary>
        ///  已登录的用户列表
        /// </summary>
        public ConcurrentModelList<IServerModel> UserList { get; }

        /// <summary>
        /// 黑名单IP地址列表
        /// </summary>
        public List<string> BannedList { get; }

        /// <summary>
        /// 关闭监听
        /// </summary>
        public void Close();
    }
}
