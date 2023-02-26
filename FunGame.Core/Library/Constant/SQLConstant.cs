namespace Milimoe.FunGame.Core.Library.Constant
{
    public class SQLConstant
    {
        /**
         * Commands
         */
        public const string Command_Select = "Select";
        public const string Command_Update = "Update";
        public const string Command_Delete = "Delete";
        public const string Command_Insert = "Insert";
        public const string Command_Set = "Set";
        public const string Command_Where = "Where";
        public const string Command_From = "From";
        public const string Command_All = "*";
        public const string Command_Into = "Into";
        public const string Command_Values = "Values";
        public const string Command_And = "And";
        public const string Command_Or = "Or";

        /**
         * Tables
         */
        public const string Table_Users = "Users";
        public const string Table_ServerLoginLogs = "ServerLoginLogs";

        /**
         * Select
         */
        public const string Select_Users = $"{Command_Select} {Command_All} {Command_From} {Table_Users}";

        /**
         * Update
         */

        /**
         * Insert
         */
        public static string Insert_ServerLoginLogs(string ServerName, string ServerKey)
        {
            return $"{Command_Insert} {Command_Into} {Table_ServerLoginLogs} (ServerName, ServerKey, LoginTime) {Command_Values} ('{ServerName}', '{ServerKey}', '{DateTime.Now}')";
        }
    }
}
