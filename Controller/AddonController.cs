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
        private Action<string> MaskMethod_WriteLine { get; set; } = new(msg => Console.Write("\r" + msg + "\n\r> "));

        /// <summary>
        /// 基于本地已连接的Socket创建新的数据请求
        /// </summary>
        private Func<DataRequestType, DataRequest> MaskMethod_NewDataRequest { get; set; }

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的数据请求
        /// </summary>
        private Func<DataRequestType, DataRequest> MaskMethod_NewLongRunningDataRequest { get; set; }

        /// <summary>
        /// 输出系统消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void WriteLine(string msg) => MaskMethod_WriteLine(msg);

        /// <summary>
        /// 基于本地已连接的Socket创建新的数据请求
        /// <para>请勿在 <see cref="GameModeServer"/> 中调用此方法</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataRequest NewDataRequest(DataRequestType type) => MaskMethod_NewDataRequest(type);

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的数据请求
        /// <para>请勿在 <see cref="GameModeServer"/> 中调用此方法</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataRequest NewLongRunningDataRequest(DataRequestType type) => MaskMethod_NewLongRunningDataRequest(type);
        
        /// <summary>
        /// 新建一个AddonController
        /// </summary>
        /// <param name="Addon"></param>
        /// <param name="delegates"></param>
        public AddonController(IAddon Addon, Delegate[] delegates)
        {
            this.Addon = Addon;
            if (delegates.Length > 0) MaskMethod_WriteLine = (Action<string>)delegates[0];
            if (delegates.Length > 1) MaskMethod_NewDataRequest = (Func<DataRequestType, DataRequest>)delegates[1];
            if (delegates.Length > 2) MaskMethod_NewLongRunningDataRequest = (Func<DataRequestType, DataRequest>)delegates[2];
            MaskMethod_NewDataRequest ??= new(DefaultNewDataRequest);
            MaskMethod_NewLongRunningDataRequest ??= new(DefaultNewDataRequest);
        }

        private DataRequest DefaultNewDataRequest(DataRequestType type)
        {
            if (Addon is IGameModeServer) throw new NotSupportedException("请勿在GameModeServer类中调用此方法");
            else throw new ConnectFailedException();
        }
    }
}
