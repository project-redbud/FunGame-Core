using System.Net;
using System.Net.WebSockets;
using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;

namespace Milimoe.FunGame.Core.Service
{
    internal class HTTPManager
    {
        internal static HttpListener? HttpListener => _HttpListener;
        private static HttpListener? _HttpListener = null;

        internal static HttpListener StartListening(string address = "*", int port = 22223, string subUrl = "ws", bool ssl = false)
        {
            _HttpListener = new();
            _HttpListener.Prefixes.Add((ssl ? "https://" : "http://") + address + ":" + port + "/" + subUrl.Trim('/') + "/");
            _HttpListener.Start();
            return _HttpListener;
        }

        internal static async Task<System.Net.WebSockets.ClientWebSocket?> Connect(Uri uri)
        {
            System.Net.WebSockets.ClientWebSocket socket = new();
            await socket.ConnectAsync(uri, CancellationToken.None);
            if (socket.State == WebSocketState.Open)
            {
                return socket;
            }
            return null;
        }

        internal static async Task<SocketResult> Send(System.Net.WebSockets.ClientWebSocket socket, SocketObject obj)
        {
            if (socket != null)
            {
                try
                {
                    await socket.SendAsync(new ArraySegment<byte>(General.DefaultEncoding.GetBytes(JsonManager.GetString(obj))), WebSocketMessageType.Text, true, CancellationToken.None);
                    return SocketResult.Success;
                }
                catch (Exception e)
                {
                    TXTHelper.AppendErrorLog(e.GetErrorInfo());
                    return SocketResult.Fail;
                }
            }
            return SocketResult.NotSent;
        }

        internal static async Task<SocketResult> Send(WebSocket socket, SocketObject obj)
        {
            if (socket != null)
            {
                try
                {
                    await socket.SendAsync(new ArraySegment<byte>(General.DefaultEncoding.GetBytes(JsonManager.GetString(obj))), WebSocketMessageType.Text, true, CancellationToken.None);
                    return SocketResult.Success;
                }
                catch (Exception e)
                {
                    TXTHelper.AppendErrorLog(e.GetErrorInfo());
                    return SocketResult.Fail;
                }
            }
            return SocketResult.NotSent;
        }

        internal static async Task<object[]> Accept()
        {
            if (HttpListener is null) return [];
            try
            {
                HttpListenerContext context = await HttpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext socketContext = await context.AcceptWebSocketAsync(null);
                    WebSocket socket = socketContext.WebSocket;
                    string ip = context.Request.UserHostAddress;
                    return [ip, socket];
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
            catch
            {
                HttpListener?.Close();
            }
            return [];
        }

        internal static async Task<SocketObject[]> Receive(WebSocket socket)
        {
            try
            {
                List<SocketObject> objs = [];
                if (socket != null)
                {
                    byte[] buffer = new byte[General.SocketByteSize];
                    WebSocketReceiveResult result;
                    StringBuilder builder = new();

                    do
                    {
                        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        builder.Append(General.DefaultEncoding.GetString(buffer, 0, result.Count).Replace("\0", "").Trim());
                    }
                    while (!result.EndOfMessage);

                    string msg = builder.ToString();

                    if (JsonManager.IsCompleteJson<SocketObject>(msg))
                    {
                        foreach (SocketObject obj in JsonManager.GetObjects<SocketObject>(msg))
                        {
                            objs.Add(obj);
                        }
                    }
                }
                return [.. objs];
            }
            catch (Exception e)
            {
                TXTHelper.AppendErrorLog(e.GetErrorInfo());
                return [];
            }
        }

        internal static async Task<bool> ReceiveMessage(HTTPClient client)
        {
            if (client.Instance is null) return false;

            byte[] buffer = new byte[General.SocketByteSize];
            WebSocketReceiveResult result = await client.Instance.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string msg = General.DefaultEncoding.GetString(buffer).Replace("\0", "").Trim();
            SocketObject[] objs = await GetSocketObjects(client.Instance, result, msg);

            foreach (SocketObject obj in objs)
            {
                SocketObject sendobject = client.SocketObject_Handler(obj);
                SocketManager.OnSocketReceive(obj);
                if (obj.SocketType == SocketMessageType.Connect)
                {
                    return true;
                }
                else if (obj.SocketType == SocketMessageType.Disconnect)
                {
                    await client.Instance.CloseAsync(result.CloseStatus ?? WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, CancellationToken.None);
                    return true;
                }
                await Send(client.Instance, sendobject);
            }

            return true;
        }

        private static async Task<SocketObject[]> GetSocketObjects(WebSocket socket, WebSocketReceiveResult result, string msg)
        {
            List<SocketObject> objs = [];

            if (JsonManager.IsCompleteJson<SocketObject>(msg))
            {
                foreach (SocketObject obj in JsonManager.GetObjects<SocketObject>(msg))
                {
                    objs.Add(obj);
                }
                return [.. objs];
            }
            else
            {
                await Task.Delay(20);
                while (true)
                {
                    if (!result.CloseStatus.HasValue)
                    {
                        byte[] buffer = new byte[General.SocketByteSize];
                        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        msg += General.DefaultEncoding.GetString(buffer).Replace("\0", "").Trim();
                        if (JsonManager.IsCompleteJson<SocketObject>(msg))
                        {
                            break;
                        }
                        await Task.Delay(20);
                    }
                    else break;
                }
                foreach (SocketObject obj in JsonManager.GetObjects<SocketObject>(msg))
                {
                    objs.Add(obj);
                }
            }

            return [.. objs];
        }
    }
}
