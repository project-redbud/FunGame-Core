using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Model
{
    public class MainModel : BaseModel
    {
        private readonly Main Main;
        private string LastSendTalkMessage = "";

        public MainModel(Main main) : base(RunTime.Socket)
        {
            Main = main;
        }

        #region 公开方法
        
        public bool LogOut()
        {
            try
            {
                // 需要当时登录给的Key发回去，确定是账号本人在操作才允许登出
                if (Config.Guid_LoginKey != Guid.Empty)
                {
                    if (RunTime.Socket?.Send(SocketMessageType.Logout, Config.Guid_LoginKey) == SocketResult.Success)
                    {
                        return true;
                    }
                }
                else throw new CanNotLogOutException();
            }
            catch (Exception e)
            {
                ShowMessage.ErrorMessage("无法登出您的账号，请联系服务器管理员。", "登出失败", 5);
                Main.GetMessage(e.GetErrorInfo());
                Main.OnFailedLogoutEvent(new GeneralEventArgs());
                Main.OnAfterLogoutEvent(new GeneralEventArgs());
            }
            return false;
        }

        public bool IntoRoom(string roomid)
        {
            try
            {
                if (RunTime.Socket?.Send(SocketMessageType.IntoRoom, roomid) == SocketResult.Success)
                    return true;
                else throw new CanNotIntoRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                RoomEventArgs args = new(roomid);
                Main.OnFailedIntoRoomEvent(args);
                Main.OnAfterIntoRoomEvent(args);
                return false;
            }
        }
        
        public bool UpdateRoom()
        {
            try
            {
                if (RunTime.Socket?.Send(SocketMessageType.UpdateRoom) == SocketResult.Success)
                    return true;
                else throw new GetRoomListException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                return false;
            }
        }
        
        public bool QuitRoom(string roomid)
        {
            try
            {
                if (RunTime.Socket?.Send(SocketMessageType.QuitRoom, roomid) == SocketResult.Success)
                    return true;
                else throw new QuitRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                RoomEventArgs args = new(roomid);
                Main.OnFailedQuitRoomEvent(args);
                Main.OnAfterQuitRoomEvent(args);
                return false;
            }
        }
        
        public bool CreateRoom()
        {
            try
            {
                if (RunTime.Socket?.Send(SocketMessageType.CreateRoom) == SocketResult.Success)
                    return true;
                else throw new QuitRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                RoomEventArgs args = new();
                Main.OnFailedCreateRoomEvent(args);
                Main.OnAfterCreateRoomEvent(args);
                return false;
            }
        }

        public bool Chat(string msg)
        {
            LastSendTalkMessage = msg;
            try
            {
                if (RunTime.Socket?.Send(SocketMessageType.Chat, msg) == SocketResult.Success)
                    return true;
                else throw new CanNotSendTalkException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                SendTalkEventArgs SendTalkEventArgs = new(LastSendTalkMessage);
                Main.OnFailedSendTalkEvent(SendTalkEventArgs);
                Main.OnAfterSendTalkEvent(SendTalkEventArgs);
                return false;
            }
        }

        public override void SocketHandler(SocketObject SocketObject)
        {
            try
            {
                switch (SocketObject.SocketType)
                {
                    case SocketMessageType.GetNotice:
                        SocketHandler_GetNotice(SocketObject);
                        break;

                    case SocketMessageType.Logout:
                        SocketHandler_LogOut(SocketObject);
                        break;

                    case SocketMessageType.HeartBeat:
                        if ((RunTime.Socket?.Connected ?? false) && Usercfg.LoginUser != null)
                            Main.UpdateUI(MainInvokeType.SetGreenAndPing);
                        break;

                    case SocketMessageType.IntoRoom:
                        SocketHandler_IntoRoom(SocketObject);
                        break;
                        
                    case SocketMessageType.QuitRoom:
                        break;
                        
                    case SocketMessageType.Chat:
                        SocketHandler_Chat(SocketObject);
                        break;
                        
                    case SocketMessageType.UpdateRoom:
                        SocketHandler_UpdateRoom(SocketObject);
                        break;

                    case SocketMessageType.Unknown:
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                RunTime.Connector?.Error(e);
            }
        }

        #endregion

        #region SocketHandler

        private void SocketHandler_GetNotice(SocketObject SocketObject)
        {
            if (SocketObject.Length > 0) Config.FunGame_Notice = SocketObject.GetParam<string>(0)!;
        }

        private void SocketHandler_LogOut(SocketObject SocketObject)
        {
            Guid key = Guid.Empty;
            string? msg = "";
            // 返回一个Key，如果这个Key是空的，登出失败
            if (SocketObject.Parameters != null && SocketObject.Length > 0) key = SocketObject.GetParam<Guid>(0);
            if (SocketObject.Parameters != null && SocketObject.Length > 1) msg = SocketObject.GetParam<string>(1);
            if (key != Guid.Empty)
            {
                Config.Guid_LoginKey = Guid.Empty;
                Main.UpdateUI(MainInvokeType.LogOut, msg ?? "");
                Main.OnSucceedLogoutEvent(new GeneralEventArgs());
            }
            else
            {
                ShowMessage.ErrorMessage("无法登出您的账号，请联系服务器管理员。", "登出失败", 5);
                Main.OnFailedLogoutEvent(new GeneralEventArgs());
            }
            Main.OnAfterLogoutEvent(new GeneralEventArgs());
        }
        
        private void SocketHandler_IntoRoom(SocketObject SocketObject)
        {
            string roomid = "";
            if (SocketObject.Length > 0) roomid = SocketObject.GetParam<string>(0)!;
            if (roomid.Trim() != "" && roomid == "-1")
            {
                Main.GetMessage($"已连接至公共聊天室。");
            }
            else
            {
                Config.FunGame_Roomid = roomid;
            }
            RoomEventArgs args = new(roomid);
            Main.OnSucceedIntoRoomEvent(args);
            Main.OnAfterIntoRoomEvent(args);
        }
        
        private void SocketHandler_UpdateRoom(SocketObject SocketObject)
        {
            List<string>? list = SocketObject.GetParam<List<string>>(0);
            list ??= new List<string>();
            Main.UpdateUI(MainInvokeType.UpdateRoom, list);
        }
        
        private void SocketHandler_Chat(SocketObject SocketObject)
        {
            SendTalkEventArgs SendTalkEventArgs = new(LastSendTalkMessage);
            if (SocketObject.Parameters != null && SocketObject.Length > 1)
            {
                string user = SocketObject.GetParam<string>(0)!;
                string msg = SocketObject.GetParam<string>(1)!;
                if (user != Usercfg.LoginUserName)
                {
                    Main.GetMessage(msg, TimeType.None);
                }
                Main.OnSucceedSendTalkEvent(SendTalkEventArgs);
                Main.OnAfterSendTalkEvent(SendTalkEventArgs);
                return;
            }
            Main.OnFailedSendTalkEvent(SendTalkEventArgs);
            Main.OnAfterSendTalkEvent(SendTalkEventArgs);
        }
        
        #endregion
    }
}
