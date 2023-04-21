using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    public class Session
    {
        public static Guid Guid_Socket { get; set; } = Guid.Empty; // SocketToken
        public static Guid Guid_LoginKey { get; set; } = Guid.Empty; // LoginKey
        public static User LoginUser { get; set; } = General.UnknownUserInstance; // 已登录的用户
        public static string LoginUserName { get; set; } = ""; // 已登录用户名
        public static Room InRoom { get; set; } = General.HallInstance; // 所处的房间
    }
}
