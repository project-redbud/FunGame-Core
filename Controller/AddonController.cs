using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Controller
{
    public class AddonController
    {
        private IAddon Addon { get; }

        /// <summary>
        /// 输出系统消息
        /// </summary>
        public Action<string> WriteLine { get; set; } = new(msg => Console.Write("\r" + msg + "\n\r> "));

        /// <summary>
        /// 基于本地已连接的Socket创建新的数据请求
        /// </summary>
        public Func<DataRequestType, DataRequest> NewDataRequest { get; set; }

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的数据请求
        /// </summary>
        public Func<DataRequestType, DataRequest> NewLongRunningDataRequest { get; set; }

        /// <summary>
        /// 新建一个AddonController
        /// </summary>
        /// <param name="Addon"></param>
        /// <param name="delegates"></param>
        public AddonController(IAddon Addon, Delegate[] delegates)
        {
            this.Addon = Addon;
            if (delegates.Length > 0) WriteLine = (Action<string>)delegates[0];
            if (delegates.Length > 1) NewDataRequest = (Func<DataRequestType, DataRequest>)delegates[1];
            if (delegates.Length > 2) NewLongRunningDataRequest = (Func<DataRequestType, DataRequest>)delegates[2];
            if (NewDataRequest is null)
            {
                NewDataRequest = new(DefaultNewDataRequest);
            }
            if (NewLongRunningDataRequest is null)
            {
                NewLongRunningDataRequest = new(DefaultNewDataRequest);
            }
        }

        private DataRequest DefaultNewDataRequest(DataRequestType type)
        {
            if (Addon is IGameModeServer) throw new NotSupportedException("请勿在GameModeServer类中调用此方法");
            else throw new ConnectFailedException();
        }
    }
}
