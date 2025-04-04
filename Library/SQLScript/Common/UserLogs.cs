using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Common
{
    public class UserLogs : Constant
    {
        public const string TableName = "UserLogs";
        public const string Column_Id = "Id";
        public const string Column_UserId = "UserId";
        public const string Column_Title = "Title";
        public const string Column_Description = "Description";
        public const string Column_Remark = "Remark";
        public const string Column_CreateTime = "CreateTime";

        public static string Insert_UserLog(SQLHelper SQLHelper, long UserId, string Title, string Description = "", string Remark = "")
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Title"] = Title;
            SQLHelper.Parameters["@Description"] = Description;
            SQLHelper.Parameters["@Remark"] = Remark;
            SQLHelper.Parameters["@CreateTime"] = DateTime.Now;
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_UserId}, {Column_Title}, {Column_Description}, {Column_Remark}, {Column_CreateTime}) {Command_Values} (@UserId, @Title, @Description, @Remark, @CreateTime)";
        }

        public static string Select_GetUserLog(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_Id} = @Id";
        }

        public static string Select_GetUserLogsByUserId(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_UserLog(SQLHelper SQLHelper, long Id, long UserId, string Title, string Description, string Remark)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Title"] = Title;
            SQLHelper.Parameters["@Description"] = Description;
            SQLHelper.Parameters["@Remark"] = Remark;
            SQLHelper.Parameters["@CreateTime"] = DateTime.Now;
            return $"{Command_Update} {TableName} {Command_Set} {Column_UserId} = @UserId, {Column_Title} = @Title, {Column_Description} = @Description, {Column_Remark} = @Remark, {Column_CreateTime} = @CreateTime {Command_Where} {Column_Id} = @Id";
        }
    }
}
