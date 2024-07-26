using System.Collections;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;

namespace Milimoe.FunGame.Core.Api.Transmittal
{
    public class WebDataRequest
    {
        /// <summary>
        /// 数据请求结果
        /// </summary>
        public RequestResult Result => Worker.Result;

        /// <summary>
        /// 详细错误信息
        /// </summary>
        public string Error => Worker.Error;

        // 获取ResultData中key值对应的Json字符串
        // -- 此索引器仅返回Json字符串，对象类型请使用反序列化方法GetResult<T>() --
        // -- 当然也可以自己反序列化 --
        // -- 基本类型可能有效，但仍建议使用反序列化方法 --
        public object? this[string key]
        {
            get
            {
                return Worker.ResultData[key];
            }
            set
            {
                AddRequestData(key, value);
            }
        }

        /// <summary>
        /// 私有的实现类
        /// </summary>
        private readonly WebSocketRequest Worker;

        /// <summary>
        /// 基于本地已连接的 <see cref="HTTPClient"/> 创建新的数据请求<para/>
        /// </summary>
        /// <param name="Socket"></param>
        /// <param name="RequestType"></param>
        internal WebDataRequest(HTTPClient Socket, DataRequestType RequestType)
        {
            Worker = new(Socket, RequestType, Guid.NewGuid());
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddRequestData(string key, object? value)
        {
            if (Worker.RequestData.ContainsKey(key)) Worker.RequestData[key] = value;
            else Worker.RequestData.Add(key, value);
        }

        /// <summary>
        /// 向服务器发送数据请求
        /// </summary>
        /// <returns></returns>
        public async Task<RequestResult> SendRequest()
        {
            await Worker.SendRequestAsync();
            return Result;
        }

        /// <summary>
        /// 异步向服务器发送数据请求
        /// </summary>
        /// <returns></returns>
        public async Task<RequestResult> SendRequestAsync()
        {
            await Worker.SendRequestAsync();
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
            return GetHashtableJsonObject<T>(Worker.ResultData, key);
        }

        private class WebSocketRequest(HTTPClient? Socket, DataRequestType RequestType, Guid RequestID)
        {
            public Hashtable RequestData { get; } = [];
            public Hashtable ResultData => _ResultData;
            public RequestResult Result => _Result;
            public string Error => _Error;

            private readonly HTTPClient? Socket = Socket;
            private readonly DataRequestType RequestType = RequestType;
            private readonly Guid RequestID = RequestID;
            private readonly Hashtable _ResultData = [];
            private RequestResult _Result = RequestResult.Missing;
            private string _Error = "";

            public async Task SendRequestAsync()
            {
                try
                {
                    if (Socket != null)
                    {
                        await Socket.Send(SocketMessageType.DataRequest, RequestType, RequestID, RequestData);
                    }
                    else throw new ConnectFailedException();
                }
                catch (Exception e)
                {
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
