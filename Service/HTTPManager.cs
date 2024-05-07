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
        internal static HttpListener? ServerSocket => _ServerSocket;
        private static HttpListener? _ServerSocket = null;

        internal static HttpListener StartListening(int Port = 22227, bool SSL = false)
        {
            HttpListener listener = new();
            listener.Prefixes.Add((SSL ? "https://" : "http://") + "localhost:" + Port + "/");
            listener.Start();
            return listener;
        }

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

        internal static async Task Receiving(HTTPListener listener)
        {
            if (ServerSocket != null)
            {
                try
                {
                    while (true)
                    {
                        HttpListenerContext context = await ServerSocket.GetContextAsync();
                        if (context.Request.IsWebSocketRequest)
                        {
                            await AddClientWebSocket(listener, context);
                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                            context.Response.Close();
                        }
                    }
                }
                catch
                {
                    _ServerSocket = null;
                }
            }
        }

        internal static async Task<bool> ReceiveMessage(HTTPClient client)
        {
            if (client.Instance is null) return false;

            byte[] buffer = new byte[General.SocketByteSize];
            WebSocketReceiveResult result = await client.Instance.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string msg = Encoding.UTF8.GetString(buffer).Replace("\0", "").Trim();
            SocketObject[] objs = await GetSocketObjects(client.Instance, result, msg);

            foreach (SocketObject obj in objs)
            {
                SocketObject sendobject = client.SocketObject_Handler(obj);
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

        private static async Task AddClientWebSocket(HTTPListener listener, HttpListenerContext context)
        {
            HttpListenerWebSocketContext socketContext = await context.AcceptWebSocketAsync(null);
            WebSocket socket = socketContext.WebSocket;

            byte[] buffer = new byte[General.SocketByteSize];
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string msg = Encoding.UTF8.GetString(buffer).Replace("\0", "").Trim();

            SocketObject sendobject = new(SocketMessageType.Unknown, Guid.Empty);
            SocketObject[] objs = await GetSocketObjects(socket, result, msg);
            bool isConnect = false;

            foreach (SocketObject obj in objs)
            {
                if (obj.SocketType == SocketMessageType.Connect)
                {
                    isConnect = listener.CheckClientConnection(obj);
                }
                else if (listener.ClientSockets.ContainsKey(obj.Token))
                {
                    sendobject = listener.SocketObject_Handler(obj);
                    isConnect = true;
                }
            }

            if (isConnect)
            {
                Guid token = Guid.NewGuid();
                listener.ClientSockets.TryAdd(token, socket);
                await Send(socket, sendobject);

                while (!result.CloseStatus.HasValue)
                {
                    try
                    {
                        buffer = new byte[General.SocketByteSize];
                        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        msg = Encoding.UTF8.GetString(buffer).Replace("\0", "").Trim();
                        objs = await GetSocketObjects(socket, result, msg);
                        foreach (SocketObject obj in objs)
                        {
                            sendobject = listener.SocketObject_Handler(obj);
                            if (obj.SocketType == SocketMessageType.Disconnect)
                            {
                                await socket.CloseAsync(result.CloseStatus ?? WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, CancellationToken.None);
                                return;
                            }
                            await Send(socket, sendobject);
                        }
                    }
                    catch (Exception e)
                    {
                        TXTHelper.AppendErrorLog(e.GetErrorInfo());
                        await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, result.CloseStatusDescription, CancellationToken.None);
                    }
                }
            }
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
