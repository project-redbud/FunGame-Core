namespace Milimoe.FunGame.Core.Library.SQLScript.Common
{
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

    public class ForgetVerifyCodes
    {
        public const string TableName = "ForgetVerifyCodes";
        public const string Column_Username = "Username";
        public const string Column_Email = "Email";
        public const string Column_ForgetVerifyCode = "ForgetVerifyCode";
        public const string Column_SendTime = "SendTime";

        public static string Insert_ForgetVerifyCodes(string Username, string Email, string ForgetVerifyCodes)
        {
            return $"{Constant.Command_Insert} {Constant.Command_Into} {TableName} ({Column_Username}, {Column_Email}, {Column_ForgetVerifyCode}, {Column_SendTime}) {Constant.Command_Values} ('{Username}', '{Email}', '{ForgetVerifyCodes}', '{DateTime.Now}')";
        }

        public static string Select_ForgetVerifyCode(string Username, string Email, string ForgetVerifyCode)
        {
            return $"{Constant.Command_Select} {Constant.Command_All} {Constant.Command_From} {TableName} {Constant.Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{Email}' and {Column_ForgetVerifyCode} = '{ForgetVerifyCode}'";
        }

        public static string Select_HasSentForgetVerifyCode(string Username, string Email)
        {
            return $"{Constant.Command_Select} {Constant.Command_All} {Constant.Command_From} {TableName} {Constant.Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{Email}'";
        }

        public static string Delete_ForgetVerifyCode(string Username, string Email)
        {
            return $"{Constant.Command_Delete} {Constant.Command_From} {TableName} {Constant.Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{Email}'";
        }
    }
}
