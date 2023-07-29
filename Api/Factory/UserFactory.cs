using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class UserFactory : IFactory<User>
    {
        public Type EntityType => typeof(User);

        public User Create()
        {
            return General.UnknownUserInstance;
        }

        public static User Create(long Id = 0, string Username = "", DateTime? RegTime = null, DateTime? LastTime = null, string Email = "", string NickName = "", bool IsAdmin = false, bool IsOperator = false, bool IsEnable = true, decimal Credits = 0, decimal Materials = 0, decimal GameTime = 0, string AutoKey = "")
        {
            return new User(Id, Username, RegTime, LastTime, Email, NickName, IsAdmin, IsOperator, IsEnable, Credits, Materials, GameTime, AutoKey);
        }
    }
}
