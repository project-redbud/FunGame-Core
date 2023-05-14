using System.Collections;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Transmittal
{
    public class DataRequest
    {
        private readonly Request Worker;

        public DataRequest(Socket Socket, DataRequestType RequestType)
        {
            Worker = new(Socket, RequestType);
        }

        public void AddRequestData(string key, object value)
        {
            Worker.RequestData.Add(key, value);
        }

        public async Task SendRequest()
        {
            await Worker.SendRequest();
        }

        public object? this[string key] => Worker.ResultData[key];

        public T? GetResult<T>(string key)
        {
            object? obj = this[key];
            if (obj != null)
            {
                return (T)obj;
            }
            return default;
        }

        private class Request : BaseModel
        {
            public Hashtable RequestData { get; } = new();
            public Hashtable ResultData => _Result;

            private bool JobFinish = false;
            private readonly Socket? Socket;
            private readonly DataRequestType RequestType;
            private Hashtable _Result = new();

            public async Task<RequestResult> SendRequest()
            {
                try
                {
                    if (Socket?.Send(SocketMessageType.DataRequest, RequestData) == SocketResult.Success)
                    {
                        await Task.Run(() =>
                        {
                            while (true)
                            {
                                if (JobFinish) break;
                                Thread.Sleep(100);
                            }
                        });
                    }
                }
                catch
                {
                    return RequestResult.Fail;
                }
                return RequestResult.Success;
            }

            public Request(Socket? socket, DataRequestType requestType) : base(socket)
            {
                Socket = socket;
                RequestType = requestType;
            }

            public override void SocketHandler(SocketObject SocketObject)
            {
                if (SocketObject.SocketType == SocketMessageType.DataRequest)
                {
                    DataRequestType type = SocketObject.GetParam<DataRequestType>(0);
                    if (type == RequestType)
                    {
                        Dispose();
                        _Result = SocketObject.GetParam<Hashtable>(1) ?? new();
                        JobFinish = true;
                    }
                }
            }
        }
    }
}
