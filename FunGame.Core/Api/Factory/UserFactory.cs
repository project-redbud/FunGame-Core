using System.Data;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class UserFactory
    {
        internal static User GetInstance()
        {
            return new User();
        }

        internal static User GetInstance(DataSet? ds)
        {
            return new User(ds);
        }
    }
}
