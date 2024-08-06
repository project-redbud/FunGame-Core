using System.Collections;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;

namespace Milimoe.FunGame.Core.Api.Transmittal
{
    /// <summary>
    /// 需要配合 <see cref="DataRequestType"/> 使用<para/>
    /// 如果是 <see cref="Model.Gaming"/> 的数据请求，则配合 <see cref="GamingType"/> 使用<para/>
    /// 确保已添加对应的枚举
    /// </summary>
    public class DataRequest
    {
        /// <summary>
        /// 数据请求结果
        /// </summary>
        public RequestResult Result => Worker != null ? Worker.Result : (GamingWorker != null ? GamingWorker.Result : RequestResult.Missing);

        /// <summary>
        /// 详细错误信息
        /// </summary>
        public string Error => Worker != null ? Worker.Error : (GamingWorker != null ? GamingWorker.Error : "");

        // 获取ResultData中key值对应的Json字符串
        // -- 此索引器仅返回Json字符串，对象类型请使用反序列化方法GetResult<T>() --
        // -- 当然也可以自己反序列化 --
        // -- 基本类型可能有效，但仍建议使用反序列化方法 --
        public object? this[string key]
        {
            get
            {
                if (Worker != null) return Worker.ResultData[key];
                else if (GamingWorker != null) return GamingWorker.ResultData[key];
                return null;
            }
            set
            {
                AddRequestData(key, value);
            }
        }

        /// <summary>
        /// 私有的实现类
        /// </summary>
        private readonly SocketRequest? Worker;

        /// <summary>
        /// 私有的实现类（这是局内请求的）
        /// </summary>
        private readonly GamingRequest? GamingWorker;

        /// <summary>
        /// 基于本地已连接的 <see cref="Socket"/> 创建新的数据请求<para/>
        /// 使用 <see cref="RunTimeController"/> 中的 <see cref="RunTimeController.NewDataRequest(DataRequestType)"/> 创建一个新的请求
        /// 插件则使用 <see cref="RunTimeController"/> 中的 <see cref="RunTimeController.NewDataRequestForAddon(DataRequestType)"/> 创建一个新的请求<para/>
        /// </summary>
        /// <param name="Socket"></param>
        /// <param name="RequestType"></param>
        /// <param name="IsLongRunning"></param>
        /// <param name="RuntimeType"></param>
        internal DataRequest(Socket Socket, DataRequestType RequestType, bool IsLongRunning = false, SocketRuntimeType RuntimeType = SocketRuntimeType.Client)
        {
            Worker = new(Socket, RequestType, Guid.NewGuid(), IsLongRunning, RuntimeType);
        }

        /// <summary>
        /// 基于本地已连接的 <see cref="HTTPClient"/> 创建新的数据请求<para/>
        /// 使用 <see cref="RunTimeController"/> 中的 <see cref="RunTimeController.NewDataRequest(DataRequestType)"/> 创建一个新的请求<para/>
        /// 插件则使用 <see cref="RunTimeController"/> 中的 <see cref="RunTimeController.NewDataRequestForAddon(DataRequestType)"/> 创建一个新的请求<para/>
        /// 此数据请求只能调用异步方法 <see cref="SendRequestAsync"/> 请求数据
        /// </summary>
        /// <param name="WebSocket"></param>
        /// <param name="RequestType"></param>
        /// <param name="IsLongRunning"></param>
        /// <param name="RuntimeType"></param>
        internal DataRequest(HTTPClient WebSocket, DataRequestType RequestType, bool IsLongRunning = false, SocketRuntimeType RuntimeType = SocketRuntimeType.Client)
        {
            Worker = new(WebSocket, RequestType, Guid.NewGuid(), IsLongRunning, RuntimeType);
        }

        /// <summary>
        /// 基于本地已连接的 <see cref="Socket"/> 创建新的局内（<see cref="Model.Gaming"/>）数据请求<para/>
        /// 使用 <see cref="RunTimeController"/> 中的 <see cref="RunTimeController.NewDataRequestForAddon(GamingType)"/> 创建一个新的请求<para/>
        /// 此构造方法是给 <see cref="Library.Common.Addon.GameModule"/> 提供的
        /// </summary>
        /// <param name="Socket"></param>
        /// <param name="GamingType"></param>
        /// <param name="IsLongRunning"></param>
        /// <param name="RuntimeType"></param>
        internal DataRequest(Socket Socket, GamingType GamingType, bool IsLongRunning = false, SocketRuntimeType RuntimeType = SocketRuntimeType.Client)
        {
            GamingWorker = new(Socket, GamingType, Guid.NewGuid(), IsLongRunning, RuntimeType);
        }

        /// <summary>
        /// 基于本地已连接的 <see cref="HTTPClient"/> 创建新的局内（<see cref="Model.Gaming"/>）数据请求<para/>
        /// 使用 <see cref="RunTimeController"/> 中的 <see cref="RunTimeController.NewDataRequestForAddon(GamingType)"/> 创建一个新的请求<para/>
        /// 此构造方法是给 <see cref="Library.Common.Addon.GameModule"/> 提供的<para/>
        /// 此数据请求只能调用异步方法 <see cref="SendRequestAsync"/> 请求数据
        /// </summary>
        /// <param name="WebSocket"></param>
        /// <param name="GamingType"></param>
        /// <param name="IsLongRunning"></param>
        /// <param name="RuntimeType"></param>
        internal DataRequest(HTTPClient WebSocket, GamingType GamingType, bool IsLongRunning = false, SocketRuntimeType RuntimeType = SocketRuntimeType.Client)
        {
            GamingWorker = new(WebSocket, GamingType, Guid.NewGuid(), IsLongRunning, RuntimeType);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddRequestData(string key, object? value)
        {
            if (Worker != null)
            {
                if (Worker.RequestData.ContainsKey(key)) Worker.RequestData[key] = value;
                else Worker.RequestData.Add(key, value);
            }
            else if (GamingWorker != null)
            {
                if (GamingWorker.RequestData.ContainsKey(key)) GamingWorker.RequestData[key] = value;
                else GamingWorker.RequestData.Add(key, value);
            }
        }

        /// <summary>
        /// 长时间运行的数据请求需要在使用完毕后自行关闭
        /// </summary>
        public void Dispose()
        {
            Worker?.Dispose();
            GamingWorker?.Dispose();
        }

        /// <summary>
        /// 向服务器发送数据请求
        /// <para/>警告：<see cref="HTTPClient"/> 调用此方法将抛出异常。请调用并等待 <see cref="SendRequestAsync"/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AsyncRequestException"></exception>
        public RequestResult SendRequest()
        {
            Worker?.SendRequest();
            GamingWorker?.SendRequest();
            return Result;
        }

        /// <summary>
        /// 异步向服务器发送数据请求
        /// </summary>
        /// <returns></returns>
        public async Task<RequestResult> SendRequestAsync()
        {
            if (Worker != null)
            {
                await Worker.SendRequestAsync();
            }
            else if (GamingWorker != null)
            {
                await GamingWorker.SendRequestAsync();
            }
            return Result;
        }

        /// <summary>
        /// 获取指定key对应的反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? GetResult<T>(string key)
        {
            if (Worker != null)
            {
                return GetHashtableJsonObject<T>(Worker.ResultData, key);
            }
            else if (GamingWorker != null)
            {
                return GetHashtableJsonObject<T>(GamingWorker.ResultData, key);
            }
            return default;
        }

        /// <summary>
        /// 常规数据请求
        /// </summary>
        private class SocketRequest : SocketHandlerController
        {
            public Hashtable RequestData { get; } = [];
            public Hashtable ResultData => _ResultData;
            public RequestResult Result => _Result;
            public string Error => _Error;

            private readonly Socket? Socket = null;
            private readonly HTTPClient? WebSocket = null;
            private readonly DataRequestType RequestType = DataRequestType.UnKnown;
            private readonly Guid RequestID = Guid.Empty;
            private readonly bool IsLongRunning = false;
            private readonly SocketRuntimeType RuntimeType = SocketRuntimeType.Client;
            private Hashtable _ResultData = [];
            private RequestResult _Result = RequestResult.Missing;
            private string _Error = "";

            public SocketRequest(Socket? Socket, DataRequestType RequestType, Guid RequestID, bool IsLongRunning = false, SocketRuntimeType RuntimeType = SocketRuntimeType.Client) : base(Socket)
            {
                this.Socket = Socket;
                this.RequestType = RequestType;
                this.RequestID = RequestID;
                this.IsLongRunning = IsLongRunning;
                this.RuntimeType = RuntimeType;
            }

            public SocketRequest(HTTPClient? WebSocket, DataRequestType RequestType, Guid RequestID, bool IsLongRunning = false, SocketRuntimeType RuntimeType = SocketRuntimeType.Client) : base(WebSocket)
            {
                this.WebSocket = WebSocket;
                this.RequestType = RequestType;
                this.RequestID = RequestID;
                this.IsLongRunning = IsLongRunning;
                this.RuntimeType = RuntimeType;
            }

            public void SendRequest()
            {
                try
                {
                    SetWorking();
                    if (RuntimeType == SocketRuntimeType.Addon || RuntimeType == SocketRuntimeType.Addon)
                    {
                        if (RequestData.ContainsKey(SocketSet.Plugins_Mark)) RequestData[SocketSet.Plugins_Mark] = "true";
                        else RequestData.Add(SocketSet.Plugins_Mark, true);
                    }
                    else RequestData.Remove(SocketSet.Plugins_Mark);
                    if (Socket != null && Socket.Send(SocketMessageType.DataRequest, RequestType, RequestID, RequestData) == SocketResult.Success)
                    {
                        WaitForWorkDone();
                    }
                    else if (WebSocket != null)
                    {
                        throw new AsyncRequestException();
                    }
                    else throw new ConnectFailedException();
                }
                catch (Exception e)
                {
                    Working = false;
                    _Result = RequestResult.Fail;
                    _Error = e.GetErrorInfo();
                }
            }

            public async Task SendRequestAsync()
            {
                try
                {
                    SetWorking();
                    if (RuntimeType == SocketRuntimeType.Addon || RuntimeType == SocketRuntimeType.Addon)
                    {
                        if (RequestData.ContainsKey(SocketSet.Plugins_Mark)) RequestData[SocketSet.Plugins_Mark] = "true";
                        else RequestData.Add(SocketSet.Plugins_Mark, true);
                    }
                    else RequestData.Remove(SocketSet.Plugins_Mark);
                    if (Socket != null && Socket.Send(SocketMessageType.DataRequest, RequestType, RequestID, RequestData) == SocketResult.Success)
                    {
                        await WaitForWorkDoneAsync();
                    }
                    else if (WebSocket != null && await WebSocket.Send(SocketMessageType.DataRequest, RequestType, RequestID, RequestData) == SocketResult.Success)
                    {
                        await WaitForWorkDoneAsync();
                    }
                    else throw new ConnectFailedException();
                }
                catch (Exception e)
                {
                    Working = false;
                    _Result = RequestResult.Fail;
                    _Error = e.GetErrorInfo();
                }
            }

            public override void SocketHandler(SocketObject SocketObject)
            {
                try
                {
                    if (SocketObject.SocketType == SocketMessageType.DataRequest)
                    {
                        DataRequestType type = SocketObject.GetParam<DataRequestType>(0);
                        Guid id = SocketObject.GetParam<Guid>(1);
                        if (type == RequestType && id == RequestID)
                        {
                            if (!IsLongRunning) Dispose();
                            Work = SocketObject;
                            Working = false;
                            _ResultData = SocketObject.GetParam<Hashtable>(2) ?? [];
                            _Result = RequestResult.Success;
                        }
                    }
                }
                catch (Exception e)
                {
                    Working = false;
                    _Result = RequestResult.Fail;
                    _Error = e.GetErrorInfo();
                }
            }
        }

        /// <summary>
        /// 游戏局内请求
        /// </summary>
        private class GamingRequest : SocketHandlerController
        {
            public Hashtable RequestData { get; } = [];
            public Hashtable ResultData => _ResultData;
            public RequestResult Result => _Result;
            public string Error => _Error;

            private readonly Socket? Socket = null;
            private readonly HTTPClient? WebSocket = null;
            private readonly GamingType GamingType = GamingType.None;
            private readonly Guid RequestID = Guid.Empty;
            private readonly bool IsLongRunning = false;
            private readonly SocketRuntimeType RuntimeType = SocketRuntimeType.Client;
            private Hashtable _ResultData = [];
            private RequestResult _Result = RequestResult.Missing;
            private string _Error = "";

            public GamingRequest(Socket? Socket, GamingType GamingType, Guid RequestID, bool IsLongRunning = false, SocketRuntimeType RuntimeType = SocketRuntimeType.Client) : base(Socket)
            {
                this.Socket = Socket;
                this.GamingType = GamingType;
                this.RequestID = RequestID;
                this.IsLongRunning = IsLongRunning;
                this.RuntimeType = RuntimeType;
            }

            public GamingRequest(HTTPClient? WebSocket, GamingType GamingType, Guid RequestID, bool IsLongRunning = false, SocketRuntimeType RuntimeType = SocketRuntimeType.Client) : base(WebSocket)
            {
                this.WebSocket = WebSocket;
                this.GamingType = GamingType;
                this.RequestID = RequestID;
                this.IsLongRunning = IsLongRunning;
                this.RuntimeType = RuntimeType;
            }

            public void SendRequest()
            {
                try
                {
                    SetWorking();
                    if (RuntimeType == SocketRuntimeType.Addon || RuntimeType == SocketRuntimeType.Addon)
                    {
                        if (RequestData.ContainsKey(SocketSet.Plugins_Mark)) RequestData[SocketSet.Plugins_Mark] = "true";
                        else RequestData.Add(SocketSet.Plugins_Mark, true);
                    }
                    else RequestData.Remove(SocketSet.Plugins_Mark);
                    if (Socket != null && Socket.Send(SocketMessageType.DataRequest, GamingType, RequestID, RequestData) == SocketResult.Success)
                    {
                        WaitForWorkDone();
                    }
                    else if (WebSocket != null)
                    {
                        throw new AsyncRequestException();
                    }
                    else throw new ConnectFailedException();
                }
                catch (Exception e)
                {
                    Working = false;
                    _Result = RequestResult.Fail;
                    _Error = e.GetErrorInfo();
                }
            }

            public async Task SendRequestAsync()
            {
                try
                {
                    SetWorking();
                    if (RuntimeType == SocketRuntimeType.Addon || RuntimeType == SocketRuntimeType.Addon)
                    {
                        if (RequestData.ContainsKey(SocketSet.Plugins_Mark)) RequestData[SocketSet.Plugins_Mark] = "true";
                        else RequestData.Add(SocketSet.Plugins_Mark, true);
                    }
                    else RequestData.Remove(SocketSet.Plugins_Mark);
                    if (Socket != null && Socket.Send(SocketMessageType.DataRequest, GamingType, RequestID, RequestData) == SocketResult.Success)
                    {
                        await WaitForWorkDoneAsync();
                    }
                    else if (WebSocket != null && await WebSocket.Send(SocketMessageType.DataRequest, GamingType, RequestID, RequestData) == SocketResult.Success)
                    {
                        await WaitForWorkDoneAsync();
                    }
                    else throw new ConnectFailedException();
                }
                catch (Exception e)
                {
                    Working = false;
                    _Result = RequestResult.Fail;
                    _Error = e.GetErrorInfo();
                }
            }

            public override void SocketHandler(SocketObject SocketObject)
            {
                try
                {
                    if (SocketObject.SocketType == SocketMessageType.DataRequest)
                    {
                        GamingType type = SocketObject.GetParam<GamingType>(0);
                        Guid id = SocketObject.GetParam<Guid>(1);
                        if (type == GamingType && id == RequestID)
                        {
                            if (!IsLongRunning) Dispose();
                            Work = SocketObject;
                            Working = false;
                            _ResultData = SocketObject.GetParam<Hashtable>(2) ?? [];
                            _Result = RequestResult.Success;
                        }
                    }
                }
                catch (Exception e)
                {
                    Working = false;
                    _Result = RequestResult.Fail;
                    _Error = e.GetErrorInfo();
                }
            }
        }

        /// <summary>
        /// 反序列化Hashtable中的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashtable"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? GetHashtableJsonObject<T>(Hashtable hashtable, string key)
        {
            return Service.JsonManager.GetObject<T>(hashtable, key);
        }
    }
}
