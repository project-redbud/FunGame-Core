using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    public class Session
    {
        public Guid SocketToken { get; set; } = Guid.Empty; // SocketToken
        public Guid LoginKey { get; set; } = Guid.Empty; // LoginKey
        public User LoginUser { get; set; } = General.UnknownUserInstance; // 已登录的用户
        public string LoginUserName { get; set; } = ""; // 已登录用户名
        public Room InRoom { get; set; } = General.HallInstance; // 所处的房间
    }
}
