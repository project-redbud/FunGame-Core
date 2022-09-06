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
            Matching = 1,
            Success = 2,
            Enable = 3,
            Cancel = 4
        }

        public enum CreateRoom_State
        {
            Creating = 1,
            Success = 2
        }

        public enum RoomState
        {
            Created = 1,
            Gaming = 2,
            Close = 3,
            Complete = 4
        }

        public enum OnlineState
        {
            Offline = 1,
            Online = 2,
            Matching = 3,
            InRoom = 4,
            Gaming = 5
        }

        #endregion

        #region Type

        public enum RoomType
        {
            Mix = 1,
            Team = 2,
            MixHasPass = 3,
            TeamHasPass = 4
        }

        public enum InterfaceType
        {
            ClientConnectInterface = 1,
            ServerInterface = 2
        }

        public enum LightType
        {
            Green = 1,
            Yellow = 2,
            Red = 3
        }

        public enum SocketType
        {
            Unknown = 0,
            GetNotice = 1,
            Login = 2,
            CheckLogin = 3,
            Logout = 4,
            HeartBeat = 5
        }

        #endregion

        #region Result

        public enum MessageResult
        {
            OK = 1,
            Cancel = 2,
            Yes = 3,
            No = 4,
            Retry = 5
        }

        #endregion

        #region Method

        public enum WebHelperMethod
        {
            CreateSocket = 1,
            CloseSocket = 2,
            StartWebHelper = 3,
        }

        public enum InterfaceMethod
        {
            RemoteServerIP = 1,
            DBConnection = 2,
            GetServerSettings = 3
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
