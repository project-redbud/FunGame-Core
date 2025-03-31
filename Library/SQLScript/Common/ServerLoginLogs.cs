using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Common
{
    public class ServerLoginLogs : Constant
    {
        public const string TableName = "ServerLoginLogs";
        public const string Column_ServerName = "ServerName";
        public const string Column_ServerKey = "ServerKey";
        public const string Column_LoginTime = "LoginTime";
        public const string Column_LastTime = "LastTime";

        public static string Insert_ServerLoginLog(SQLHelper SQLHelper, string ServerName, string ServerKey)
        {
            SQLHelper.Parameters["@ServerName"] = ServerName;
            SQLHelper.Parameters["@ServerKey"] = ServerKey;
            SQLHelper.Parameters["@LoginTime"] = DateTime.Now;
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_ServerName}, {Column_ServerKey}, {Column_LoginTime}) {Command_Values} (@ServerName, @ServerKey, @LoginTime)";
        }

        public static string Select_GetLastLoginTime()
        {
            return $"{Command_Select} Max({Column_LoginTime}) {Column_LastTime} {Command_From} {TableName}";
        }
    }
}
