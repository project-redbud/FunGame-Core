using System.Collections;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Milimoe.FunGame.Core.Api.Transmittal
{
    public class DataRequest
    {
        public bool Success => Worker.Success;
        public string Error => Worker.Error;
        public object? this[string key] => Worker.ResultData[key];

        private readonly Request Worker;

        public DataRequest(Socket Socket, DataRequestType RequestType)
        {
            Worker = new(Socket, RequestType);
        }

        public void AddRequestData(string key, object value)
        {
            Worker.RequestData.Add(key, value);
        }

        public void SendRequest()
        {
            Worker.SendRequest();
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
            public Hashtable ResultData => _Result;
            public bool Success => _Success;
            public string Error => _Error;

            private readonly Socket? Socket;
            private readonly DataRequestType RequestType;

            private bool _Finish = false;
            private bool _Success = false;
            private string _Error = "";
            private Hashtable _Result = new();

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
                }
                catch (Exception e)
                {
                    _Finish = true;
                    _Success = false;
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
                            _Result = SocketObject.GetParam<Hashtable>(1) ?? new();
                            _Finish = true;
                            _Success = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    _Finish = true;
                    _Success = false;
                    _Error = e.GetErrorInfo();
                }
            }
        }
    }
}
