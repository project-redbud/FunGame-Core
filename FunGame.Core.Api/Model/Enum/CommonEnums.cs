using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Core.Api.Model.Enum
{
    /// <summary>
    /// 这里存放框架实现相关的State Type Result Method
    /// 添加FunGame.Core.Api接口和实现时，需要在这里同步添加：InterfaceType、InterfaceMethod
    /// </summary>
    public static class CommonEnums
    {
        #region State

        public enum StartMatch_State
        {
            Matching,
            Success,
            Enable,
            Cancel
        }

        public enum CreateRoom_State
        {
            Creating,
            Success
        }

        public enum RoomState
        {
            Created,
            Gaming,
            Close,
            Complete
        }

        public enum OnlineState
        {
            Offline,
            Online,
            Matching,
            InRoom,
            Gaming
        }

        #endregion

        #region Type

        public enum RoomType
        {
            Mix,
            Team,
            MixHasPass,
            TeamHasPass
        }

        public enum MessageButtonType
        {
            OK,
            OKCancel,
            YesNo,
            RetryCancel
        }

        public enum InterfaceType
        {
            ClientConnectInterface,
            ServerInterface
        }

        public enum LightType
        {
            Green,
            Yellow,
            Red
        }

        public enum SocketType
        {
            Unknown,
            GetNotice,
            Login,
            CheckLogin,
            Logout,
            HeartBeat
        }

        #endregion

        #region Result

        public enum MessageResult
        {
            OK,
            Cancel,
            Yes,
            No,
            Retry
        }

        #endregion

        #region Method

        public enum WebHelperMethod
        {
            CreateSocket,
            CloseSocket,
            StartWebHelper,
            Login
        }

        public enum InterfaceMethod
        {
            RemoteServerIP,
            DBConnection,
            GetServerSettings
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 获取实现类类名
        /// </summary>
        /// <param name="Interface">接口代号</param>
        /// <returns></returns>
        public static string GetImplementClassName(int Interface)
        {
            foreach (string str in System.Enum.GetNames(typeof(InterfaceType)))
            {
                InterfaceType temp = (InterfaceType)System.Enum.Parse(typeof(InterfaceType), Interface.ToString(), true);
                if (temp.ToString() == str)
                    return temp + "Impl";
            }
            return "";
        }

        /// <summary>
        /// 获取实现类的方法名
        /// </summary>
        /// <param name="Method">方法代号</param>
        /// <returns></returns>
        public static string GetImplementMethodName(int Method)
        {
            foreach (string str in System.Enum.GetNames(typeof(InterfaceMethod)))
            {
                InterfaceMethod temp = (InterfaceMethod)System.Enum.Parse(typeof(InterfaceMethod), Method.ToString(), true);
                if (temp.ToString() == str)
                    return temp.ToString();
            }
            return "";
        }

        /// <summary>
        /// 获取Socket枚举名
        /// </summary>
        /// <param name="SocketType">Socket枚举</param>
        /// <returns></returns>
        public static string GetSocketTypeName(int SocketType)
        {
            foreach (string str in System.Enum.GetNames(typeof(SocketType)))
            {
                SocketType temp = (SocketType)System.Enum.Parse(typeof(SocketType), SocketType.ToString(), true);
                if (temp.ToString() == str)
                    return temp.ToString();
            }
            return "";
        }

        #endregion
    }
}
