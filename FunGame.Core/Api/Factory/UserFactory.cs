using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Api.Factory
{
    public class UserFactory
    {
        public static Milimoe.FunGame.Core.Entity.General.User GetInstance()
        {
            return new Milimoe.FunGame.Core.Entity.General.User();
        }

        public static Milimoe.FunGame.Core.Entity.General.User GetInstance(string username)
        {
            return new Milimoe.FunGame.Core.Entity.General.User(username);
        }

        public static Milimoe.FunGame.Core.Entity.General.User GetInstance(string username, string password)
        {
            return new Milimoe.FunGame.Core.Entity.General.User(username, password);
        }
    }
}
