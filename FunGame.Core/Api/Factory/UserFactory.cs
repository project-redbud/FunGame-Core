using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class UserFactory
    {
        internal static Milimoe.FunGame.Core.Entity.User GetInstance()
        {
            return new Milimoe.FunGame.Core.Entity.User();
        }

        internal static Milimoe.FunGame.Core.Entity.User GetInstance(string username)
        {
            return new Milimoe.FunGame.Core.Entity.User(username);
        }

        internal static Milimoe.FunGame.Core.Entity.User GetInstance(string username, string password)
        {
            return new Milimoe.FunGame.Core.Entity.User(username, password);
        }
    }
}
