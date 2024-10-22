﻿namespace Milimoe.FunGame.Core.Library.SQLScript
{
    public class Constant
    {
        /**
         * Commands
         */
        public const string Command_Select = "Select";
        public const string Command_Update = "Update";
        public const string Command_Delete = "Delete";
        public const string Command_Insert = "Insert";
        public const string Command_From = "From";
        public const string Command_Set = "Set";
        public const string Command_Into = "Into";
        public const string Command_Where = "Where";
        public const string Command_All = "*";
        public const string Command_Values = "Values";
        public const string Command_And = "And";
        public const string Command_Or = "Or";
        public const string Command_As = "As";
        public const string Command_LeftJoin = "Left Join";
        public const string Command_InnerJoin = "Inner Join";
        public const string Command_RightJoin = "Right Join";
        public const string Command_CrossJoin = "Cross Join";
        public const string Command_On = "On";
        public const string Command_In = "In";
    }
}

namespace Milimoe.FunGame.Core.Library.SQLScript.Common
{
    public class ServerLoginLogs : Constant
    {
        public const string TableName = "ServerLoginLogs";
        public const string Column_ServerName = "ServerName";
        public const string Column_ServerKey = "ServerKey";
        public const string Column_LoginTime = "LoginTime";

        public static string Insert_ServerLoginLogs(string ServerName, string ServerKey)
        {
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_ServerName}, {Column_ServerKey}, {Column_LoginTime}) {Command_Values} ('{ServerName}', '{ServerKey}', '{DateTime.Now}')";
        }

        public static string Select_GetLastLoginTime()
        {
            return $"{Command_Select} Max({Column_LoginTime}) {Command_From} {TableName}";
        }
    }
}
