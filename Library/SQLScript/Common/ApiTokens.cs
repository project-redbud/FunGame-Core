using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Common
{
    public class ApiTokens : Constant
    {
        public const string TableName = "ApiTokens";
        public const string Column_TokenID = "TokenID";
        public const string Column_SecretKey = "SecretKey";
        public const string Column_Reference1 = "Reference1";
        public const string Column_Reference2 = "Reference2";

        public static string Insert_APIToken(SQLHelper SQLHelper, string TokenID, string SecretKey = "", string Reference1 = "", string Reference2 = "")
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

        public static string Select_GetAPISecretKey(SQLHelper SQLHelper, string SecretKey)
        {
            SQLHelper.Parameters["@SecretKey"] = SecretKey;
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_SecretKey} = @SecretKey";
        }

        public static string Update_APIToken(SQLHelper SQLHelper, string TokenID, string SecretKey, string Reference1 = "", string Reference2 = "")
        {
            SQLHelper.Parameters["@TokenID"] = TokenID;
            SQLHelper.Parameters["@SecretKey"] = SecretKey;
            SQLHelper.Parameters["@Reference1"] = Reference1;
            SQLHelper.Parameters["@Reference2"] = Reference2;
            return $"{Command_Update} {TableName} {Command_Set} {Column_TokenID} = @TokenID, {Column_SecretKey} = @SecretKey, {Column_Reference1} = @Reference1, {Column_Reference2} = @Reference2 {Command_Where} {Column_TokenID} = @TokenID";
        }
    }
}
