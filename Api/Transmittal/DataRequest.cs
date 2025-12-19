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
    public class DataRequest : IDisposable
    {
        /// <summary>
        /// 数据请求结果
        /// </summary>
        public RequestResult Result => _worker != null ? _worker.Result : (_gamingWorker != null ? _gamingWorker.Result : RequestResult.Missing);

        /// <summary>
        /// 详细错误信息
        /// </summary>
        public string Error => _worker != null ? _worker.Error : (_gamingWorker != null ? _gamingWorker.Error : "");

        /// <summary>
        /// 是否已经关闭
        /// </summary>
        public bool IsDisposed => _isDisposed;

        // 获取ResultData中key值对应的Json字符串
        // -- 此索引器仅返回Json字符串，对象类型请使用反序列化方法GetResult<T>() --
        // -- 当然也可以自己反序列化 --
        // -- 基本类型可能有效，但仍建议使用反序列化方法 --
        public object? this[string key]
        {
            get
            {
                if (_worker != null) return _worker.ResultData[key];
                else if (_gamingWorker != null) return _gamingWorker.ResultData[key];
                return null;
            }
            set
            {
                if (value != null) AddRequestData(key, value);
            }
        }

        /// <summary>
        /// 私有的实现类
        /// </summary>
        private readonly SocketRequest? _worker;

        /// <summary>
        /// 私有的实现类（这是局内请求的）
        /// </summary>
        private readonly GamingRequest? _gamingWorker;

        /// <summary>
        /// 指示关闭的变量
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// 基于本地已连接的 <see cref="Socket"/> 创建新的数据请求<para/>
        /// 使用 <see cref="RunTimeController"/> 中的 <see cref="RunTimeController.NewDataRequest(DataRequestType)"/> 创建一个新的请求
        /// 插件则使用 <see cref="RunTimeController"/> 中的 <see cref="RunTimeController.NewDataRequestForAddon(DataRequestType)"/> 创建一个新的请求<para/>
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="type"></param>
        /// <param name="longRunning"></param>
        /// <param name="runtime"></param>
        internal DataRequest(Socket socket, DataRequestType type, bool longRunning = false, SocketRuntimeType runtime = SocketRuntimeType.Client)
        {
            _worker = new(socket, type, Guid.NewGuid(), longRunning, runtime);
        }

        /// <summary>
        /// 基于本地已连接的 <see cref="HTTPClient"/> 创建新的数据请求<para/>
        /// 使用 <see cref="RunTimeController"/> 中的 <see cref="RunTimeController.NewDataRequest(DataRequestType)"/> 创建一个新的请求<para/>
        /// 插件则使用 <see cref="RunTimeController"/> 中的 <see cref="RunTimeController.NewDataRequestForAddon(DataRequestType)"/> 创建一个新的请求<para/>
        /// 此数据请求只能调用异步方法 <see cref="SendRequestAsync"/> 请求数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="type"></param>
        /// <param name="longRunning"></param>
        /// <param name="runtime"></param>
        internal DataRequest(HTTPClient client, DataRequestType type, bool longRunning = false, SocketRuntimeType runtime = SocketRuntimeType.Client)
        {
            _worker = new(client, type, Guid.NewGuid(), longRunning, runtime);
        }

        /// <summary>
        /// 基于本地已连接的 <see cref="Socket"/> 创建新的局内（<see cref="Model.Gaming"/>）数据请求<para/>
        /// 使用 <see cref="RunTimeController"/> 中的 <see cref="RunTimeController.NewDataRequestForAddon(GamingType)"/> 创建一个新的请求<para/>
        /// 此构造方法是给 <see cref="Library.Common.Addon.GameModule"/> 提供的
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="type"></param>
        /// <param name="longRunning"></param>
        /// <param name="runtime"></param>
        internal DataRequest(Socket socket, GamingType type, bool longRunning = false, SocketRuntimeType runtime = SocketRuntimeType.Client)
        {
            _gamingWorker = new(socket, type, Guid.NewGuid(), longRunning, runtime);
        }

        /// <summary>
        /// 基于本地已连接的 <see cref="HTTPClient"/> 创建新的局内（<see cref="Model.Gaming"/>）数据请求<para/>
        /// 使用 <see cref="RunTimeController"/> 中的 <see cref="RunTimeController.NewDataRequestForAddon(GamingType)"/> 创建一个新的请求<para/>
        /// 此构造方法是给 <see cref="Library.Common.Addon.GameModule"/> 提供的<para/>
        /// 此数据请求只能调用异步方法 <see cref="SendRequestAsync"/> 请求数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="type"></param>
        /// <param name="longRunning"></param>
        /// <param name="runtime"></param>
        internal DataRequest(HTTPClient client, GamingType type, bool longRunning = false, SocketRuntimeType runtime = SocketRuntimeType.Client)
        {
            _gamingWorker = new(client, type, Guid.NewGuid(), longRunning, runtime);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddRequestData(string key, object value)
        {
            if (_worker != null)
            {
                if (!_worker.RequestData.TryAdd(key, value)) _worker.RequestData[key] = value;
            }
            else if (_gamingWorker != null)
            {
                if (!_gamingWorker.RequestData.TryAdd(key, value)) _gamingWorker.RequestData[key] = value;
            }
        }

        /// <summary>
        /// 长时间运行的数据请求需要在使用完毕后自行关闭
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 关闭时
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _worker?.Dispose();
                    _gamingWorker?.Dispose();
                }
            }
            _isDisposed = true;
        }

        /// <summary>
        /// 向服务器发送数据请求
        /// <para/>警告：<see cref="HTTPClient"/> 调用此方法将抛出异常。请调用并等待 <see cref="SendRequestAsync"/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AsyncSendException"></exception>
        public RequestResult SendRequest()
        {
            _worker?.SendRequest();
            _gamingWorker?.SendRequest();
            return Result;
        }

        /// <summary>
        /// 异步向服务器发送数据请求
        /// </summary>
        /// <returns></returns>
        public async Task<RequestResult> SendRequestAsync()
        {
            if (_worker != null)
            {
                await _worker.SendRequestAsync();
            }
            else if (_gamingWorker != null)
            {
                await _gamingWorker.SendRequestAsync();
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
            if (_worker != null)
            {
                return GetDictionaryJsonObject<T>(_worker.ResultData, key);
            }
            else if (_gamingWorker != null)
            {
                return GetDictionaryJsonObject<T>(_gamingWorker.ResultData, key);
            }
            return default;
        }

        /// <summary>
        /// 常规数据请求
        /// </summary>
        private class SocketRequest : SocketHandlerController
        {
            public Dictionary<string, object> RequestData { get; } = [];
            public Dictionary<string, object> ResultData => _resultData;
            public RequestResult Result => _result;
            public string Error => _error;

            private readonly Socket? _socket = null;
            private readonly HTTPClient? _httpClient = null;
            private readonly DataRequestType _requestType = DataRequestType.UnKnown;
            private readonly Guid _requestID = Guid.Empty;
            private readonly bool _isLongRunning = false;
            private readonly SocketRuntimeType _runtimeType = SocketRuntimeType.Client;
            private Dictionary<string, object> _resultData = [];
            private RequestResult _result = RequestResult.Missing;
            private string _error = "";

            public SocketRequest(Socket? socket, DataRequestType type, Guid requestId, bool longRunning = false, SocketRuntimeType runtime = SocketRuntimeType.Client) : base(socket)
            {
                _socket = socket;
                _requestType = type;
                _requestID = requestId;
                _isLongRunning = longRunning;
                _runtimeType = runtime;
            }

            public SocketRequest(HTTPClient? client, DataRequestType type, Guid requestId, bool longRunning = false, SocketRuntimeType runtime = SocketRuntimeType.Client) : base(client)
            {
                _httpClient = client;
                _requestType = type;
                _requestID = requestId;
                _isLongRunning = longRunning;
                _runtimeType = runtime;
            }

            public void SendRequest()
            {
                try
                {
                    SetWorking();
                    if (_runtimeType == SocketRuntimeType.Addon)
                    {
                        if (RequestData.ContainsKey(SocketSet.Plugins_Mark)) RequestData[SocketSet.Plugins_Mark] = "true";
                        else RequestData.Add(SocketSet.Plugins_Mark, true);
                    }
                    else RequestData.Remove(SocketSet.Plugins_Mark);
                    if (_socket != null && _socket.Send(SocketMessageType.DataRequest, _requestType, _requestID, RequestData) == SocketResult.Success)
                    {
                        WaitForWorkDone();
                    }
                    else if (_httpClient != null)
                    {
                        throw new AsyncSendException();
                    }
                    else throw new ConnectFailedException();
                }
                catch (Exception e)
                {
                    Working = false;
                    _result = RequestResult.Fail;
                    _error = e.GetErrorInfo();
                }
            }

            public async Task SendRequestAsync()
            {
                try
                {
                    SetWorking();
                    if (_runtimeType == SocketRuntimeType.Addon)
                    {
                        if (RequestData.ContainsKey(SocketSet.Plugins_Mark)) RequestData[SocketSet.Plugins_Mark] = "true";
                        else RequestData.Add(SocketSet.Plugins_Mark, true);
                    }
                    else RequestData.Remove(SocketSet.Plugins_Mark);
                    if (_socket != null && _socket.Send(SocketMessageType.DataRequest, _requestType, _requestID, RequestData) == SocketResult.Success)
                    {
                        await WaitForWorkDoneAsync();
                    }
                    else if (_httpClient != null && await _httpClient.Send(SocketMessageType.DataRequest, _requestType, _requestID, RequestData) == SocketResult.Success)
                    {
                        await WaitForWorkDoneAsync();
                    }
                    else throw new ConnectFailedException();
                }
                catch (Exception e)
                {
                    Working = false;
                    _result = RequestResult.Fail;
                    _error = e.GetErrorInfo();
                }
            }

            public override void SocketHandler(SocketObject obj)
            {
                try
                {
                    if (obj.SocketType == SocketMessageType.DataRequest)
                    {
                        DataRequestType type = obj.GetParam<DataRequestType>(0);
                        Guid id = obj.GetParam<Guid>(1);
                        if (type == _requestType && id == _requestID)
                        {
                            if (!_isLongRunning) Dispose();
                            ReceivedObject = obj;
                            Working = false;
                            _resultData = obj.GetParam<Dictionary<string, object>>(2) ?? [];
                            _result = RequestResult.Success;
                        }
                    }
                }
                catch (Exception e)
                {
                    Working = false;
                    _result = RequestResult.Fail;
                    _error = e.GetErrorInfo();
                }
            }
        }

        /// <summary>
        /// 游戏局内请求
        /// </summary>
        private class GamingRequest : SocketHandlerController
        {
            public Dictionary<string, object> RequestData { get; } = [];
            public Dictionary<string, object> ResultData => _resultData;
            public RequestResult Result => _result;
            public string Error => _error;

            private readonly Socket? _socket = null;
            private readonly HTTPClient? _httpClient = null;
            private readonly GamingType _gamingType = GamingType.None;
            private readonly Guid _requestID = Guid.Empty;
            private readonly bool _isLongRunning = false;
            private readonly SocketRuntimeType _runtimeType = SocketRuntimeType.Client;
            private Dictionary<string, object> _resultData = [];
            private RequestResult _result = RequestResult.Missing;
            private string _error = "";

            public GamingRequest(Socket? socket, GamingType type, Guid requestId, bool longRunning = false, SocketRuntimeType runtime = SocketRuntimeType.Client) : base(socket)
            {
                _socket = socket;
                _gamingType = type;
                _requestID = requestId;
                _isLongRunning = longRunning;
                _runtimeType = runtime;
            }

            public GamingRequest(HTTPClient? client, GamingType type, Guid requestId, bool longRunning = false, SocketRuntimeType runtime = SocketRuntimeType.Client) : base(client)
            {
                _httpClient = client;
                _gamingType = type;
                _requestID = requestId;
                _isLongRunning = longRunning;
                _runtimeType = runtime;
            }

            public void SendRequest()
            {
                try
                {
                    SetWorking();
                    if (_runtimeType == SocketRuntimeType.Addon)
                    {
                        if (RequestData.ContainsKey(SocketSet.Plugins_Mark)) RequestData[SocketSet.Plugins_Mark] = "true";
                        else RequestData.Add(SocketSet.Plugins_Mark, true);
                    }
                    else RequestData.Remove(SocketSet.Plugins_Mark);
                    if (_socket != null && _socket.Send(SocketMessageType.GamingRequest, _gamingType, _requestID, RequestData) == SocketResult.Success)
                    {
                        WaitForWorkDone();
                    }
                    else if (_httpClient != null)
                    {
                        throw new AsyncSendException();
                    }
                    else throw new ConnectFailedException();
                }
                catch (Exception e)
                {
                    Working = false;
                    _result = RequestResult.Fail;
                    _error = e.GetErrorInfo();
                }
            }

            public async Task SendRequestAsync()
            {
                try
                {
                    SetWorking();
                    if (_runtimeType == SocketRuntimeType.Addon)
                    {
                        if (RequestData.ContainsKey(SocketSet.Plugins_Mark)) RequestData[SocketSet.Plugins_Mark] = "true";
                        else RequestData.Add(SocketSet.Plugins_Mark, true);
                    }
                    else RequestData.Remove(SocketSet.Plugins_Mark);
                    if (_socket != null && _socket.Send(SocketMessageType.GamingRequest, _gamingType, _requestID, RequestData) == SocketResult.Success)
                    {
                        await WaitForWorkDoneAsync();
                    }
                    else if (_httpClient != null && await _httpClient.Send(SocketMessageType.GamingRequest, _gamingType, _requestID, RequestData) == SocketResult.Success)
                    {
                        await WaitForWorkDoneAsync();
                    }
                    else throw new ConnectFailedException();
                }
                catch (Exception e)
                {
                    Working = false;
                    _result = RequestResult.Fail;
                    _error = e.GetErrorInfo();
                }
            }

            public override void SocketHandler(SocketObject obj)
            {
                try
                {
                    if (obj.SocketType == SocketMessageType.GamingRequest)
                    {
                        GamingType type = obj.GetParam<GamingType>(0);
                        Guid id = obj.GetParam<Guid>(1);
                        if (type == _gamingType && id == _requestID)
                        {
                            if (!_isLongRunning) Dispose();
                            ReceivedObject = obj;
                            Working = false;
                            _resultData = obj.GetParam<Dictionary<string, object>>(2) ?? [];
                            _result = RequestResult.Success;
                        }
                    }
                }
                catch (Exception e)
                {
                    Working = false;
                    _result = RequestResult.Fail;
                    _error = e.GetErrorInfo();
                }
            }
        }

        /// <summary>
        /// 反序列化Dictionary中的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? GetDictionaryJsonObject<T>(Dictionary<string, object> dict, string key)
        {
            return Service.JsonManager.GetObject<T>(dict, key);
        }
    }
}
