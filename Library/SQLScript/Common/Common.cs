using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript
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
        public const string Column_LastTime = "LastTime";

        public static string Insert_ServerLoginLogs(SQLHelper SQLHelper, string ServerName, string ServerKey)
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

namespace Milimoe.FunGame.Core.Library.SQLScript.Common
{
    public class ApiTokens : Constant
    {
        public const string TableName = "ApiTokens";
        public const string Column_TokenID = "TokenID";
        public const string Column_SecretKey = "SecretKey";
        public const string Column_Reference1 = "Reference1";
        public const string Column_Reference2 = "Reference2";

        public static string Insert_APITokens(SQLHelper SQLHelper, string TokenID, string SecretKey = "", string Reference1 = "", string Reference2 = "")
        {
            SQLHelper.Parameters["@TokenID"] = TokenID;
            SQLHelper.Parameters["@SecretKey"] = SecretKey;
            SQLHelper.Parameters["@Reference1"] = Reference1;
            SQLHelper.Parameters["@Reference2"] = Reference2;
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_TokenID}, {Column_SecretKey}, {Column_Reference1}, {Column_Reference2}) {Command_Values} (@TokenID, @SecretKey, @Reference1, @Reference2)";
        }

        public static string Select_GetAPIToken(SQLHelper SQLHelper, string TokenID)
        {
            SQLHelper.Parameters["@TokenID"] = TokenID;
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_TokenID} = @TokenID";
        }

        public static string Update_GetAPIToken(SQLHelper SQLHelper, string TokenID, string SecretKey, string Reference1 = "", string Reference2 = "")
        {
            SQLHelper.Parameters["@TokenID"] = TokenID;
            SQLHelper.Parameters["@SecretKey"] = SecretKey;
            SQLHelper.Parameters["@Reference1"] = Reference1;
            SQLHelper.Parameters["@Reference2"] = Reference2;
            return $"{Command_Update} {TableName} {Command_Set} {Column_TokenID} = @TokenID, {Column_SecretKey} = @SecretKey, {Column_Reference1} = @Reference1, {Column_Reference2} = @Reference2 {Command_Where} {Column_TokenID} = @TokenID";
        }
    }
}
