using System.Collections;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Controller
{
    /// <summary>
    /// 这个控制器在Base的基础上添加了DataRequest
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AddonController<T> : BaseAddonController<T> where T : IAddon
    {
        /// <summary>
        /// 基于本地已连接的Socket创建新的数据请求
        /// </summary>
        private Func<DataRequestType, DataRequest> MaskMethod_NewDataRequest { get; set; }

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的数据请求
        /// </summary>
        private Func<DataRequestType, DataRequest> MaskMethod_NewLongRunningDataRequest { get; set; }

        /// <summary>
        /// 基于本地已连接的Socket创建新的数据请求
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ModuleServerNewDataRequestException"></exception>
        public DataRequest NewDataRequest(DataRequestType type)
        {
            if (typeof(T).IsAssignableFrom(typeof(IGameModuleServer)))
            {
                throw new ModuleServerNewDataRequestException();
            }
            return MaskMethod_NewDataRequest(type);
        }

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的数据请求
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ModuleServerNewDataRequestException"></exception>
        public DataRequest NewLongRunningDataRequest(DataRequestType type)
        {
            if (typeof(T).IsAssignableFrom(typeof(IGameModuleServer)))
            {
                throw new ModuleServerNewDataRequestException();
            }
            return MaskMethod_NewLongRunningDataRequest(type);
        }

        /// <summary>
        /// 新建一个AddonController
        /// </summary>
        /// <param name="addon"></param>
        /// <param name="delegates"></param>
        public AddonController(IAddon addon, Hashtable delegates) : base(addon, delegates)
        {
            if (delegates.ContainsKey("NewDataRequest")) MaskMethod_NewDataRequest = delegates["NewDataRequest"] != null ? (Func<DataRequestType, DataRequest>)delegates["NewDataRequest"]! : new(DefaultNewDataRequest);
            if (delegates.ContainsKey("NewLongRunningDataRequest")) MaskMethod_NewLongRunningDataRequest = delegates["NewLongRunningDataRequest"] != null ? (Func<DataRequestType, DataRequest>)delegates["NewLongRunningDataRequest"]! : new(DefaultNewDataRequest);
            MaskMethod_NewDataRequest ??= new(DefaultNewDataRequest);
            MaskMethod_NewLongRunningDataRequest ??= new(DefaultNewDataRequest);
        }

        private DataRequest DefaultNewDataRequest(DataRequestType type) => throw new ConnectFailedException();
    }
}
