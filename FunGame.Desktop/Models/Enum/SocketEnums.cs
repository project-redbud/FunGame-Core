using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Desktop.Models.Enum
{
    public static class SocketEnums
    {
        public enum Type
        {
            GetNotice = 1,
            Login = 2,
            CheckLogin = 3,
            Logout = 4,
            HeartBeat = 5
        }

        public const string TYPE_UNKNOWN = "Unknown Type";
        public const string TYPE_GetNotice = "GetNotice";
        public const string TYPE_Login = "Login";
        public const string TYPE_CheckLogin = "CheckLogin";
        public const string TYPE_Logout = "Logout";
        public const string TYPE_HeartBeat = "HeartBeat";
    }
}
