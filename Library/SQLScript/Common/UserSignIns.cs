using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Common
{
    public class UserSignIns : Constant
    {
        public const string TableName = "UserSignIns";
        public const string Column_UserId = "UserId";
        public const string Column_LastTime = "LastTime";
        public const string Column_Days = "Days";
        public const string Column_IsSigned = "IsSigned";

        public static string Insert_NewUserSignIn(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@LastTime"] = DBNull.Value;
            SQLHelper.Parameters["@Days"] = 0;
            SQLHelper.Parameters["@IsSigned"] = 0;
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_UserId}, {Column_LastTime}, {Column_Days}, {Column_IsSigned}) {Command_Values} (@UserId, @LastTime, @Days, @IsSigned)";
        }

        public static string Select_GetUserSignIn(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_UserSignIn(SQLHelper SQLHelper, long UserId, int Days)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@LastTime"] = DateTime.Now;
            SQLHelper.Parameters["@Days"] = Days;
            SQLHelper.Parameters["@IsSigned"] = 1;
            return $"{Command_Update} {TableName} {Command_Set} {Column_LastTime} = @LastTime, {Column_Days} = @Days, {Column_IsSigned} = @IsSigned {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_ResetStatus(SQLHelper SQLHelper)
        {
            SQLHelper.Parameters["@IsSigned"] = 0;
            return $"{Command_Update} {TableName} {Command_Set} {Column_IsSigned} = @IsSigned";
        }
    }
}
