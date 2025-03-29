using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Common
{
    public class Configs : Constant
    {
        public const string TableName = "Configs";
        public const string Column_Id = "Id";
        public const string Column_Content = "Content";
        public const string Column_Description = "Description";
        public const string Column_UpdateTime = "UpdateTime";

        public static string Insert_Config(SQLHelper SQLHelper, string Id, string Content, string Description = "")
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@Content"] = Content;
            SQLHelper.Parameters["@Description"] = Description;
            SQLHelper.Parameters["@UpdateTime"] = DateTime.Now;
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_Id}, {Column_Content}, {Column_Description}, {Column_UpdateTime}) {Command_Values} (@Id, @Content, @Description, @UpdateTime)";
        }

        public static string Select_GetConfig(SQLHelper SQLHelper, string Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_Id} = @Id";
        }

        public static string Update_Config(SQLHelper SQLHelper, string Id, string Content, string Description)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@Content"] = Content;
            SQLHelper.Parameters["@Description"] = Description;
            SQLHelper.Parameters["@UpdateTime"] = DateTime.Now;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Id} = @Id, {Column_Content} = @Content, {Column_Description} = @Description, {Column_UpdateTime} = @UpdateTime {Command_Where} {Column_Id} = @Id";
        }
    }
}
