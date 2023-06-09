using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class UserFactory : IFactory<User>
    {
        public Type EntityType => typeof(User);

        public User Create()
        {
            return new User();
        }
    }
}
