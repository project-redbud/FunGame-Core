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
        /// <summary>
        /// 实际的 <see cref="System.Net.HttpListener"/> 监听实例 [ 单例 ]
        /// </summary>
        internal static HttpListener? HttpListener => _HttpListener;
        private static HttpListener? _HttpListener = null;

        /// <summary>
        /// 开始监听
        /// 当 <paramref name="address"/> = "*" 时，需要管理员权限
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="subUrl"></param>
        /// <param name="ssl"></param>
        /// <returns></returns>
        internal static HttpListener StartListening(string address = "*", int port = 22223, string subUrl = "ws", bool ssl = false)
        {
            _HttpListener = new();
            _HttpListener.Prefixes.Add((ssl ? "https://" : "http://") + address + ":" + port + "/" + subUrl.Trim('/') + "/");
            _HttpListener.Start();
            return _HttpListener;
        }

        /// <summary>
        /// 客户端连接远程 WebSocket 服务器
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        internal static async Task<ClientWebSocket?> Connect(Uri uri)
        {
            ClientWebSocket socket = new();
            await socket.ConnectAsync(uri, CancellationToken.None);
            if (socket.State == WebSocketState.Open)
            {
                return socket;
            }
            return null;
        }

        /// <summary>
        /// 客户端向服务器发送消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static async Task<SocketResult> Send(ClientWebSocket socket, SocketObject obj)
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

        /// <summary>
        /// 服务器向客户端发送消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 服务器接受一个 HTTP 的 WebSocket 升级请求
        /// </summary>
        /// <returns>[0]客户端IP地址；[1]客户端的WebSocket实例</returns>
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
                    string ip = context.Request.RemoteEndPoint.ToString();
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

        /// <summary>
        /// 服务器接收客户端消息
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 客户端接收服务器消息
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        internal static async Task<bool> ReceiveMessage(HTTPClient client)
        {
            if (client.WebSocket is null) return false;

            byte[] buffer = new byte[General.SocketByteSize];
            WebSocketReceiveResult result = await client.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string msg = General.DefaultEncoding.GetString(buffer).Replace("\0", "").Trim();
            SocketObject[] objs = await GetSocketObjects(client.WebSocket, result, msg);

            foreach (SocketObject obj in objs)
            {
                SocketManager.OnSocketReceive(obj);
                if (obj.SocketType == SocketMessageType.Connect)
                {
                    return true;
                }
                else if (obj.SocketType == SocketMessageType.Disconnect)
                {
                    await client.WebSocket.CloseAsync(result.CloseStatus ?? WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, CancellationToken.None);
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// 将收到的消息反序列为 <see cref="SocketObject"/>
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="result"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
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
