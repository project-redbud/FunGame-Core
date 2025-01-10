using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Common
{
    public class RegVerifyCodes : Constant
    {
        public const string TableName = "RegVerifyCodes";
        public const string Column_Username = "Username";
        public const string Column_Email = "Email";
        public const string Column_RegVerifyCode = "RegVerifyCode";
        public const string Column_RegTime = "RegTime";

        public static string Insert_RegVerifyCode(SQLHelper SQLHelper, string Username, string Email, string RegVerifyCodes)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Email"] = Email;
            SQLHelper.Parameters["@RegVerifyCodes"] = RegVerifyCodes;
            SQLHelper.Parameters["@RegTime"] = DateTime.Now;
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_Username}, {Column_Email}, {Column_RegVerifyCode}, {Column_RegTime}) {Command_Values} (@Username, @Email, @RegVerifyCodes, @RegTime)";
        }

        public static string Select_RegVerifyCode(SQLHelper SQLHelper, string Username, string Email, string RegVerifyCode)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Email"] = Email;
            SQLHelper.Parameters["@RegVerifyCode"] = RegVerifyCode;
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_Username} = @Username and {Column_Email} = @Email and {Column_RegVerifyCode} = @RegVerifyCode";
        }

        public static string Select_HasSentRegVerifyCode(SQLHelper SQLHelper, string Username, string Email)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Email"] = Email;
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_Username} = @Username and {Column_Email} = @Email";
        }

        public static string Delete_RegVerifyCode(SQLHelper SQLHelper, string Username, string Email)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Email"] = Email;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Username} = @Username and {Column_Email} = @Email";
        }
    }

    public class ForgetVerifyCodes : Constant
    {
        public const string TableName = "ForgetVerifyCodes";
        public const string Column_Username = "Username";
        public const string Column_Email = "Email";
        public const string Column_ForgetVerifyCode = "ForgetVerifyCode";
        public const string Column_SendTime = "SendTime";

        public static string Insert_ForgetVerifyCode(SQLHelper SQLHelper, string Username, string Email, string ForgetVerifyCodes)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Email"] = Email;
            SQLHelper.Parameters["@ForgetVerifyCodes"] = ForgetVerifyCodes;
            SQLHelper.Parameters["@SendTime"] = DateTime.Now;
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_Username}, {Column_Email}, {Column_ForgetVerifyCode}, {Column_SendTime}) {Command_Values} (@Username, @Email, @ForgetVerifyCodes, @SendTime)";
        }

        public static string Select_ForgetVerifyCode(SQLHelper SQLHelper, string Username, string Email, string ForgetVerifyCode)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Email"] = Email;
            SQLHelper.Parameters["@ForgetVerifyCode"] = ForgetVerifyCode;
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_Username} = @Username and {Column_Email} = @Email and {Column_ForgetVerifyCode} = @ForgetVerifyCode";
        }

        public static string Select_HasSentForgetVerifyCode(SQLHelper SQLHelper, string Username, string Email)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Email"] = Email;
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_Username} = @Username and {Column_Email} = @Email";
        }

        public static string Delete_ForgetVerifyCode(SQLHelper SQLHelper, string Username, string Email)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Email"] = Email;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Username} = @Username and {Column_Email} = @Email";
        }
    }
}
