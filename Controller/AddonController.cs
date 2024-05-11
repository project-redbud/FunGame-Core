using System.Collections;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Controller
{
    public class AddonController
    {
        private IAddon Addon { get; }

        /// <summary>
        /// 输出系统消息
        /// </summary>
        private Action<string> MaskMethod_WriteLine { get; set; }

        /// <summary>
        /// 输出错误消息
        /// </summary>
        private Action<Exception> MaskMethod_Error { get; set; }

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
        /// 输出错误消息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public void Error(Exception e) => MaskMethod_Error(e);

        /// <summary>
        /// 基于本地已连接的Socket创建新的数据请求
        /// <para>请勿在 <see cref="Library.Common.Addon.GameModeServer"/> 中调用此方法</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataRequest NewDataRequest(DataRequestType type) => MaskMethod_NewDataRequest(type);

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的数据请求
        /// <para>请勿在 <see cref="Library.Common.Addon.GameModeServer"/> 中调用此方法</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataRequest NewLongRunningDataRequest(DataRequestType type) => MaskMethod_NewLongRunningDataRequest(type);

        /// <summary>
        /// 新建一个AddonController
        /// </summary>
        /// <param name="Addon"></param>
        /// <param name="delegates"></param>
        public AddonController(IAddon Addon, Hashtable delegates)
        {
            this.Addon = Addon;
            if (delegates.ContainsKey("WriteLine")) MaskMethod_WriteLine = delegates["WriteLine"] != null ? (Action<string>)delegates["WriteLine"]! : new(DefaultPrint);
            if (delegates.ContainsKey("Error")) MaskMethod_Error = delegates["Error"] != null ? (Action<Exception>)delegates["Error"]! : new(DefaultPrint);
            if (delegates.ContainsKey("NewDataRequest")) MaskMethod_NewDataRequest = delegates["NewDataRequest"] != null ? (Func<DataRequestType, DataRequest>)delegates["NewDataRequest"]! : new(DefaultNewDataRequest);
            if (delegates.ContainsKey("NewLongRunningDataRequest")) MaskMethod_NewLongRunningDataRequest = delegates["NewLongRunningDataRequest"] != null ? (Func<DataRequestType, DataRequest>)delegates["NewLongRunningDataRequest"]! : new(DefaultNewDataRequest);
            MaskMethod_WriteLine ??= new(DefaultPrint);
            MaskMethod_Error ??= new(DefaultPrint);
            MaskMethod_NewDataRequest ??= new(DefaultNewDataRequest);
            MaskMethod_NewLongRunningDataRequest ??= new(DefaultNewDataRequest);
        }

        private void DefaultPrint(string msg) => Console.Write("\r" + msg + "\n\r> ");

        private void DefaultPrint(Exception e) => DefaultPrint(e.ToString());

        private DataRequest DefaultNewDataRequest(DataRequestType type)
        {
            if (Addon is IGameModeServer) throw new NotSupportedException("请勿在GameModeServer类中调用此方法");
            else throw new ConnectFailedException();
        }
    }
}
