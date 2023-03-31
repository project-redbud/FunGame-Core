using System.Data;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class UserFactory
    {
        internal static User GetInstance(DataSet? DataSet, int Index = 0)
        {
            return new User(DataSet, Index);
        }
    }
}
