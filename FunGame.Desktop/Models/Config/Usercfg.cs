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
        public static bool Match_Mix = false; // 混战模式选项
        public static bool Match_Team = false; // 团队模式选项
        public static bool Match_HasPass = false; // 密码房间选项
    }
}
