using System.Collections;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;

namespace Milimoe.FunGame.Core.Api.Transmittal
{
    /// <summary>
    /// 需要配合Milimoe.FunGame.Core.Library.Constant.DataRequestType使用
    /// 确保已添加对应的枚举
    /// </summary>
    public class DataRequest
    {
        /// <summary>
        /// 数据请求结果
        /// </summary>
        public RequestResult Result => Worker.Result;

        /// <summary>
        /// 详细错误信息
        /// </summary>
        public string Error => Worker.Error;

        /// <summary>
        /// 获取ResultData中key值对应的Json字符串
        /// -- 此索引器仅返回Json字符串，对象类型请使用反序列化方法GetResult<T>() --
        /// -- 当然也可以自己反序列化 --
        /// -- 基本类型可能有效，但仍建议使用反序列化方法 --
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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
        private readonly Request Worker;

        /// <summary>
        /// 基于本地已连接的Socket创建新的数据请求
        /// 使用RunTimeModel中的NewDataRequest创建一个新的请求
        /// </summary>
        /// <param name="Socket"></param>
        /// <param name="RequestType"></param>
        internal DataRequest(Socket Socket, DataRequestType RequestType)
        {
            Worker = new(Socket, RequestType);
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
        public RequestResult SendRequest()
        {
            Worker.SendRequest();
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

        private class Request : BaseModel
        {
            public Hashtable RequestData { get; } = new();
            public Hashtable ResultData => _ResultData;
            public RequestResult Result => _Result;
            public string Error => _Error;

            private readonly Socket? Socket;
            private readonly DataRequestType RequestType;

            private RequestResult _Result = RequestResult.Missing;
            private string _Error = "";
            private Hashtable _ResultData = new();

            public void SendRequest()
            {
                try
                {
                    SetWorking();
                    if (Socket?.Send(SocketMessageType.DataRequest, RequestType, RequestData) == SocketResult.Success)
                    {
                        WaitForWorkDone();
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
                    if (Socket?.Send(SocketMessageType.DataRequest, RequestType, RequestData) == SocketResult.Success)
                    {
                        await Task.Factory.StartNew(WaitForWorkDone);
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

            public Request(Socket? socket, DataRequestType requestType) : base(socket)
            {
                Socket = socket;
                RequestType = requestType;
            }

            public override void SocketHandler(SocketObject SocketObject)
            {
                try
                {
                    if (SocketObject.SocketType == SocketMessageType.DataRequest)
                    {
                        Dispose();
                        Work = SocketObject;
                        Working = false;
                        DataRequestType type = SocketObject.GetParam<DataRequestType>(0);
                        if (type == RequestType)
                        {
                            _ResultData = SocketObject.GetParam<Hashtable>(1) ?? new();
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
        /// 获取Type的等效字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeString(DataRequestType type)
        {
            return type switch
            {
                DataRequestType.Main_GetNotice => DataRequestSet.Main_GetNotice,
                DataRequestType.Main_CreateRoom => DataRequestSet.Main_CreateRoom,
                DataRequestType.Main_UpdateRoom => DataRequestSet.Main_UpdateRoom,
                DataRequestType.Reg_GetRegVerifyCode => DataRequestSet.Reg_GetRegVerifyCode,
                DataRequestType.Login_GetFindPasswordVerifyCode => DataRequestSet.Login_GetFindPasswordVerifyCode,
                DataRequestType.Login_UpdatePassword => DataRequestSet.Login_UpdatePassword,
                DataRequestType.Room_GetRoomSettings => DataRequestSet.Room_GetRoomSettings,
                DataRequestType.Room_GetRoomPlayerCount => DataRequestSet.Room_GetRoomPlayerCount,
                _ => DataRequestSet.UnKnown
            };
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
            return Utility.NetworkUtility.JsonDeserializeFromHashtable<T>(hashtable, key);
        }
    }
}
