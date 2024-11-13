using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Controller
{
    /// <summary>
    /// 这个控制器在 Base 的基础上添加了 DataRequest
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
        /// 基于本地已连接的Socket创建新的局内数据请求
        /// </summary>
        private Func<GamingType, DataRequest> MaskMethod_NewGamingRequest { get; set; }

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的局内数据请求
        /// </summary>
        private Func<GamingType, DataRequest> MaskMethod_NewLongRunningGamingRequest { get; set; }

        /// <summary>
        /// 基于本地已连接的Socket创建新的数据请求<para/>
        /// 此方法只允许插件调用，如果是模组和模组服务器调用此方法将抛出异常
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNewDataRequestException"></exception>
        public DataRequest NewDataRequest(DataRequestType type)
        {
            if (typeof(IGameModule).IsAssignableFrom(typeof(T)) || typeof(IGameModuleServer).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidNewDataRequestException();
            }
            return MaskMethod_NewDataRequest(type);
        }

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的数据请求<para/>
        /// 此方法只允许插件调用，如果是模组和模组服务器调用此方法将抛出异常
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNewDataRequestException"></exception>
        public DataRequest NewLongRunningDataRequest(DataRequestType type)
        {
            if (typeof(IGameModule).IsAssignableFrom(typeof(T)) || typeof(IGameModuleServer).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidNewDataRequestException();
            }
            return MaskMethod_NewLongRunningDataRequest(type);
        }

        /// <summary>
        /// 基于本地已连接的Socket创建新的局内（<see cref="Model.Gaming"/>）数据请求<para/>
        /// 此方法是给 <see cref="Library.Common.Addon.GameModule"/> 提供的，但是 <see cref="Library.Common.Addon.Plugin"/> 也能调用<para/>
        /// 模组服务器调用此方法将抛出异常
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ConnectFailedException"></exception>
        /// <exception cref="InvalidNewDataRequestException"></exception>
        public DataRequest NewDataRequest(GamingType type)
        {
            if (typeof(IGameModuleServer).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidNewDataRequestException();
            }
            return MaskMethod_NewGamingRequest(type);
        }

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的局内（<see cref="Model.Gaming"/>）数据请求<para/>
        /// 此方法是给 <see cref="Library.Common.Addon.GameModule"/> 提供的，但是 <see cref="Library.Common.Addon.Plugin"/> 也能调用<para/>
        /// 模组服务器调用此方法将抛出异常
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ConnectFailedException"></exception>
        /// <exception cref="InvalidNewDataRequestException"></exception>
        public DataRequest NewLongRunningDataRequest(GamingType type)
        {
            if (typeof(IGameModuleServer).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidNewDataRequestException();
            }
            return MaskMethod_NewLongRunningGamingRequest(type);
        }

        /// <summary>
        /// 新建一个AddonController
        /// </summary>
        /// <param name="addon"></param>
        /// <param name="delegates"></param>
        public AddonController(IAddon addon, Dictionary<string, object> delegates) : base(addon, delegates)
        {
            if (delegates.TryGetValue("NewDataRequest", out object? value)) MaskMethod_NewDataRequest = value != null ? (Func<DataRequestType, DataRequest>)value : new(DefaultNewDataRequest);
            if (delegates.TryGetValue("NewLongRunningDataRequest", out value)) MaskMethod_NewLongRunningDataRequest = value != null ? (Func<DataRequestType, DataRequest>)value : new(DefaultNewDataRequest);
            if (delegates.TryGetValue("NewGamingRequest", out value)) MaskMethod_NewGamingRequest = value != null ? (Func<GamingType, DataRequest>)value : new(DefaultNewDataRequest);
            if (delegates.TryGetValue("NewLongRunningGamingRequest", out value)) MaskMethod_NewLongRunningGamingRequest = value != null ? (Func<GamingType, DataRequest>)value : new(DefaultNewDataRequest);
            MaskMethod_NewDataRequest ??= new(DefaultNewDataRequest);
            MaskMethod_NewLongRunningDataRequest ??= new(DefaultNewDataRequest);
            MaskMethod_NewGamingRequest ??= new(DefaultNewDataRequest);
            MaskMethod_NewLongRunningGamingRequest ??= new(DefaultNewDataRequest);
        }

        private DataRequest DefaultNewDataRequest(DataRequestType type) => throw new InvalidNewDataRequestException();

        private DataRequest DefaultNewDataRequest(GamingType type) => throw new InvalidNewDataRequestException();
    }
}
