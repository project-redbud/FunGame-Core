namespace Milimoe.FunGame.Core.Library.SQLScript.Common
{
    public class RegVerifyCodes : Constant
    {
        public const string TableName = "RegVerifyCodes";
        public const string Column_Username = "Username";
        public const string Column_Email = "Email";
        public const string Column_RegVerifyCode = "RegVerifyCode";
        public const string Column_RegTime = "RegTime";

        public static string Insert_RegVerifyCode(string Username, string Email, string RegVerifyCodes)
        {
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_Username}, {Column_Email}, {Column_RegVerifyCode}, {Column_RegTime}) {Command_Values} ('{Username}', '{Email}', '{RegVerifyCodes}', '{DateTime.Now}')";
        }

        public static string Select_RegVerifyCode(string Username, string Email, string RegVerifyCode)
        {
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{Email}' and {Column_RegVerifyCode} = '{RegVerifyCode}'";
        }

        public static string Select_HasSentRegVerifyCode(string Username, string Email)
        {
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{Email}'";
        }

        public static string Delete_RegVerifyCode(string Username, string Email)
        {
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{Email}'";
        }
    }

    public class ForgetVerifyCodes : Constant
    {
        public const string TableName = "ForgetVerifyCodes";
        public const string Column_Username = "Username";
        public const string Column_Email = "Email";
        public const string Column_ForgetVerifyCode = "ForgetVerifyCode";
        public const string Column_SendTime = "SendTime";

        public static string Insert_ForgetVerifyCode(string Username, string Email, string ForgetVerifyCodes)
        {
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_Username}, {Column_Email}, {Column_ForgetVerifyCode}, {Column_SendTime}) {Command_Values} ('{Username}', '{Email}', '{ForgetVerifyCodes}', '{DateTime.Now}')";
        }

        public static string Select_ForgetVerifyCode(string Username, string Email, string ForgetVerifyCode)
        {
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{Email}' and {Column_ForgetVerifyCode} = '{ForgetVerifyCode}'";
        }

        public static string Select_HasSentForgetVerifyCode(string Username, string Email)
        {
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{Email}'";
        }

        public static string Delete_ForgetVerifyCode(string Username, string Email)
        {
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{Email}'";
        }
    }
}
