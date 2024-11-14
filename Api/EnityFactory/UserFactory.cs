using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.EntityFactory
{
    internal class UserFactory : IFactory<User>
    {
        public Type EntityType => typeof(User);

        public User Create()
        {
            return General.UnknownUserInstance;
        }

        public static User Create(long Id = 0, string Username = "", DateTime? RegTime = null, DateTime? LastTime = null, string Email = "", string NickName = "", bool IsAdmin = false, bool IsOperator = false, bool IsEnable = true, double GameTime = 0, string AutoKey = "")
        {
            return new(Id, Username, RegTime, LastTime, Email, NickName, IsAdmin, IsOperator, IsEnable, GameTime, AutoKey);
        }

        public static User Create(UserType type)
        {
            return new(type);
        }

        public static User CreateGuest()
        {
            return General.GuestUserInstance;
        }

        public static User CreateLocalUser()
        {
            return General.LocalUserInstance;
        }
    }
}
