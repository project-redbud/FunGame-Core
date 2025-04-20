using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class UserQuery : Constant
    {
        public const string TableName = "Users";
        public const string Column_Id = "Id";
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
        public const string Column_GameTime = "GameTime";
        public const string Column_AutoKey = "AutoKey";
        public const string Select_Users = $"{Command_Select} {Command_All} {Command_From} {TableName}";

        public static string Select_Users_LoginQuery(SQLHelper SQLHelper, string Username, string Password)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Password"] = Password;
            return $"{Select_Users} {Command_Where} {Column_Username} = @Username and {Column_Password} = @Password";
        }

        public static string Select_UserById(SQLHelper SQLHelper, long id)
        {
            SQLHelper.Parameters["@Id"] = id;
            return $"{Select_Users} {Command_Where} {Column_Id} = @Id";
        }
        
        public static string Select_IsExistEmail(SQLHelper SQLHelper, string Email)
        {
            SQLHelper.Parameters["@Email"] = Email;
            return $"{Select_Users} {Command_Where} {Column_Email} = @Email";
        }

        public static string Select_IsExistUsername(SQLHelper SQLHelper, string Username)
        {
            SQLHelper.Parameters["@Username"] = Username;
            return $"{Select_Users} {Command_Where} {Column_Username} = @Username";
        }

        public static string Select_CheckEmailWithUsername(SQLHelper SQLHelper, string Username, string email)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Email"] = email;
            return $"{Select_Users} {Command_Where} {Column_Username} = @Username and {Column_Email} = @Email";
        }

        public static string Select_Users_Where(SQLHelper SQLHelper, string Where)
        {
            SQLHelper.Parameters["@Where"] = Where;
            return $"{Select_Users} {Command_Where} @Where";
        }

        public static string Select_CheckAutoKey(SQLHelper SQLHelper, string Username, string AutoKey)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@AutoKey"] = AutoKey;
            return $"{Select_Users} {Command_Where} {Column_Username} = @Username and {Column_AutoKey} = @AutoKey";
        }

        public static string Update_CheckLogin(SQLHelper SQLHelper, string Username, string IP)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@LastTime"] = DateTime.Now;
            SQLHelper.Parameters["@LastIP"] = IP;
            return $"{Command_Update} {TableName} {Command_Set} {Column_LastTime} = @LastTime, {Column_LastIP} = @LastIP {Command_Where} {Column_Username} = @Username";
        }

        public static string Update_Password(SQLHelper SQLHelper, string Username, string Password)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Password"] = Password;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Password} = @Password {Command_Where} {Column_Username} = @Username";
        }

        public static string Update_GameTime(SQLHelper SQLHelper, string Username, int GameTimeMinutes)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@GameTimeMinutes"] = GameTimeMinutes;
            return $"{Command_Update} {TableName} {Command_Set} {Column_GameTime} = {Column_GameTime} + @GameTimeMinutes {Command_Where} {Column_Username} = @Username";
        }

        public static string Update_User(SQLHelper SQLHelper, string Username, string NickName, string Email, bool IsAdmin, bool IsOperator, bool IsEnable, string AutoKey)
        {
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Nickname"] = NickName;
            SQLHelper.Parameters["@Email"] = Email;
            SQLHelper.Parameters["@IsAdmin"] = IsAdmin ? 1 : 0;
            SQLHelper.Parameters["@IsOperator"] = IsOperator ? 1 : 0;
            SQLHelper.Parameters["@IsEnable"] = IsEnable ? 1 : 0;
            SQLHelper.Parameters["@AutoKey"] = AutoKey;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Nickname} = @Nickname, {Column_Email} = @Email, {Column_IsAdmin} = @IsAdmin, {Column_IsOperator} = @IsOperator, {Column_IsEnable} = @IsEnable, {Column_AutoKey} = @AutoKey WHERE {Column_Username} = @Username";
        }

        public static string Insert_Register(SQLHelper SQLHelper, string Username, string Password, string Email, string IP, string AutoKey = "")
        {
            DateTime now = DateTime.Now;
            SQLHelper.Parameters["@Username"] = Username;
            SQLHelper.Parameters["@Nickname"] = Username;
            SQLHelper.Parameters["@Password"] = Password;
            SQLHelper.Parameters["@Email"] = Email;
            SQLHelper.Parameters["@RegTime"] = now;
            SQLHelper.Parameters["@LastTime"] = now;
            SQLHelper.Parameters["@LastIP"] = IP;
            SQLHelper.Parameters["@AutoKey"] = AutoKey;
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_Username}, {Column_Nickname}, {Column_Password}, {Column_Email}, {Column_RegTime}, {Column_LastTime}, {Column_LastIP}, {Column_AutoKey}) {Command_Values} (@Username, @Nickname, @Password, @Email, @RegTime, @LastTime, @LastIP, @AutoKey)";
        }
    }
}
