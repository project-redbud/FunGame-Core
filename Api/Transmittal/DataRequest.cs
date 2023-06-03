using System.Collections;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;

namespace Milimoe.FunGame.Core.Api.Transmittal
{
    public class DataRequest
    {
        public RequestResult Result => Worker.Result;
        public string Error => Worker.Error;
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

        private readonly Request Worker;

        public DataRequest(Socket Socket, DataRequestType RequestType)
        {
            Worker = new(Socket, RequestType);
        }

        public void AddRequestData(string key, object? value)
        {
            if (Worker.RequestData.ContainsKey(key)) Worker.RequestData[key] = value;
            else Worker.RequestData.Add(key, value);
        }

        public RequestResult SendRequest()
        {
            Worker.SendRequest();
            return Result;
        }

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
            public Hashtable ResultData => _ResultData;
            public RequestResult Result => _Result;
            public string Error => _Error;

            private readonly Socket? Socket;
            private readonly DataRequestType RequestType;

            private bool _Finish = false;
            private RequestResult _Result = RequestResult.Missing;
            private string _Error = "";
            private Hashtable _ResultData = new();

            public void SendRequest()
            {
                try
                {
                    if (Socket?.Send(SocketMessageType.DataRequest, RequestType, RequestData) == SocketResult.Success)
                    {
                        while (true)
                        {
                            if (_Finish) break;
                            Thread.Sleep(100);
                        }
                    }
                    else throw new ConnectFailedException();
                }
                catch (Exception e)
                {
                    _Finish = true;
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
                        DataRequestType type = SocketObject.GetParam<DataRequestType>(0);
                        if (type == RequestType)
                        {
                            Dispose();
                            _ResultData = SocketObject.GetParam<Hashtable>(1) ?? new();
                            _Finish = true;
                            _Result = RequestResult.Success;
                        }
                    }
                }
                catch (Exception e)
                {
                    _Finish = true;
                    _Result = RequestResult.Fail;
                    _Error = e.GetErrorInfo();
                }
            }
        }
    }
}
