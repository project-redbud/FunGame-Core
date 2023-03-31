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
        public const string Command_On = "On";
    }
}

namespace Milimoe.FunGame.Core.Library.SQLScript.Common
{
    public class ServerLoginLogs
    {
        public const string TableName = "ServerLoginLogs";
        public const string Column_ServerName = "ServerName";
        public const string Column_ServerKey = "ServerKey";
        public const string Column_LoginTime = "LoginTime";

        public static string Insert_ServerLoginLogs(string ServerName, string ServerKey)
        {
            return $"{Constant.Command_Insert} {Constant.Command_Into} {TableName} ({Column_ServerName}, {Column_ServerKey}, {Column_LoginTime}) {Constant.Command_Values} ('{ServerName}', '{ServerKey}', '{DateTime.Now}')";
        }
    }

    public class RegVerifyCodes
    {
        public const string TableName = "RegVerifyCodes";
        public const string Column_Username = "Username";
        public const string Column_Email = "Email";
        public const string Column_RegVerifyCode = "RegVerifyCode";
        public const string Column_RegTime = "RegTime";

        public static string Insert_RegVerifyCodes(string Username, string Email, string RegVerifyCodes)
        {
            return $"{Constant.Command_Insert} {Constant.Command_Into} {TableName} ({Column_Username}, {Column_Email}, {Column_RegVerifyCode}, {Column_RegTime}) {Constant.Command_Values} ('{Username}', '{Email}', '{RegVerifyCodes}', '{DateTime.Now}')";
        }

        public static string Select_RegVerifyCode(string Username, string Email, string RegVerifyCode)
        {
            return $"{Constant.Command_Select} {Constant.Command_All} {Constant.Command_From} {TableName} {Constant.Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{Email}' and {Column_RegVerifyCode} = '{RegVerifyCode}'";
        }
        
        public static string Select_HasSentRegVerifyCode(string Username, string Email)
        {
            return $"{Constant.Command_Select} {Constant.Command_All} {Constant.Command_From} {TableName} {Constant.Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{Email}'";
        }

        public static string Delete_RegVerifyCode(string Username, string Email)
        {
            return $"{Constant.Command_Delete} {Constant.Command_From} {TableName} {Constant.Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{Email}'";
        }
    }
}
