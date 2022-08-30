using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Desktop.Models.Enum
{
    /// <summary>
    /// 这里存放框架实现相关的State Type Result Method
    /// 添加FunGame.Core接口和实现时，需要在这里同步添加：InterfaceType和InterfaceMethod
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
            ServerInterface = 1
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
            GetServerIP = 1
        }

        #endregion

    }
}
