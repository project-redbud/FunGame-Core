namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class UserQuery
    {
        public const string TableName= "Users";
        public const string Column_UID = "UID";
        public const string Column_Username = "Username";
        public const string Column_Password = "Password";
        public const string Column_RegTime = "RegTime";
        public const string Column_LastTime = "LastTime";
        public const string Column_LastIP = "LastIP";
        public const string Column_Email = "Email";
        public const string Column_Nickname = "Nickname";
        public const string Column_IsAdmin = "IsAdmin";
        public const string Column_IsOperator = "IsOperator";
        public const string Column_IsEnable = "IsEnable";
        public const string Column_Credits = "Credits";
        public const string Column_Materials = "Materials";
        public const string Column_GameTime = "GameTime";
        public const string Column_AutoKey = "AutoKey";
        public const string Select_Users = $"{Constant.Command_Select} {Constant.Command_All} {Constant.Command_From} {TableName}";

        public static string Select_Users_LoginQuery(string Username, string Password)
        {
            return $"{Select_Users} {Constant.Command_Where} {Column_Username} = '{Username}' and {Column_Password} = '{Password}'";
        }
        
        public static string Select_IsExistEmail(string Email)
        {
            return $"{Select_Users} {Constant.Command_Where} {Column_Email} = '{Email}'";
        }
        
        public static string Select_IsExistUsername(string Username)
        {
            return $"{Select_Users} {Constant.Command_Where} {Column_Username} = '{Username}'";
        }
        
        public static string Select_CheckEmailWithUsername(string Username, string email)
        {
            return $"{Select_Users} {Constant.Command_Where} {Column_Username} = '{Username}' and {Column_Email} = '{email}'";
        }

        public static string Select_Users_Where(string Where)
        {
            return $"{Select_Users} {Constant.Command_Where} {Where}'";
        }

        public static string Select_CheckAutoKey(string Username, string AutoKey)
        {
            return $"{Select_Users} {Constant.Command_Where} {Column_Username} = '{Username}' and {Column_AutoKey} = '{AutoKey}'";
        }

        public static string Update_CheckLogin(string Username, string IP)
        {
            return $"{Constant.Command_Update} {TableName} {Constant.Command_Set} {Column_LastTime} = '{DateTime.Now}', {Column_LastIP} = '{IP}' {Constant.Command_Where} {Column_Username} = '{Username}'";
        }
        
        public static string Update_GameTime(string Username, int GameTimeMinutes)
        {
            return $"{Constant.Command_Update} {TableName} {Constant.Command_Set} {Column_GameTime} = {Column_GameTime} + {GameTimeMinutes} {Constant.Command_Where} {Column_Username} = '{Username}'";
        }

        public static string Insert_Register(string Username, string Password, string Email, string IP)
        {
            DateTime Now = DateTime.Now;
            return $"{Constant.Command_Insert} {Constant.Command_Into} {TableName} ({Column_Username}, {Column_Password}, {Column_Email}, {Column_RegTime}, {Column_LastTime}, {Column_LastIP}) {Constant.Command_Values} ('{Username}', '{Password}', '{Email}', '{Now}', '{Now}', '{IP}')";
        }
    }
}
