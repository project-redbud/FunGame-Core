using FunGame.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Desktop.Models.Config
{
    public class Usercfg
    {
        /**
         * 玩家设定内容
         */
        public static User? LoginUser = null; // 已登录的用户
        public static string LoginUserName = ""; // 已登录用户名
    }
}
