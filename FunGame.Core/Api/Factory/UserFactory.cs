using System.Data;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class UserFactory
    {
        internal static User GetInstance(DataRow? DataRow)
        {
            return new User(DataRow);
        }
    }
}
