namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class RoomQuery : Constant
    {
        public const string TableName = "Rooms";
        public const string Column_ID = "Id";
        public const string Column_RoomID = "Roomid";
        public const string Column_CreateTime = "CreateTime";
        public const string Column_RoomMaster = "RoomMaster";
        public const string Column_RoomMasterName = "RoomMasterName";
        public const string Column_RoomType = "RoomType";
        public const string Column_RoomState = "RoomState";
        public const string Column_HasPass = "HasPass";
        public const string Column_Password = "Password";
        public const string Select_Rooms = $"{Command_Select} {TableName}.{Command_All}, {UserQuery.TableName}.{UserQuery.Column_Username} {Command_As} {Column_RoomMasterName} " +
            $"{Command_From} {TableName} {Command_LeftJoin} {UserQuery.TableName} {Command_On} {UserQuery.TableName}.{UserQuery.Column_UID} = {TableName}.{Column_RoomMaster}";

        public static string Insert_CreateRoom(string RoomID, long RoomMaster, Library.Constant.RoomType RoomType, string Password)
        {
            Library.Constant.RoomState RoomState = Library.Constant.RoomState.Created;
            DateTime NowTime = DateTime.Now;
            bool HasPass = false;
            if (Password.Trim() != "")
            {
                HasPass = true;
            }
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_RoomID}, {Column_CreateTime}, {Column_RoomMaster}, {Column_RoomType}, {Column_RoomState}, {Column_HasPass}, {Column_Password})" +
                $" {Command_Values} ('{RoomID}', '{NowTime}', {RoomMaster}, {(int)RoomType}, {(int)RoomState}, {(HasPass ? 1 : 0)}, '{Password}')";
        }

        public static string Delete_Rooms(params string[] roomids)
        {
            if (roomids.Length == 0)
            {
                string where = string.Join("', '", roomids);
                return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_RoomID} {Command_In} ('{where}')";
            }
            return $"{Command_Delete} {Command_From} {TableName}";
        }
        
        public static string Delete_QuitRoom(string RoomID, long RoomMaster)
        {
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_RoomID} = '{RoomID}' {Command_And} {Column_RoomMaster} = {RoomMaster}";
        }

        public static string Update_QuitRoom(string RoomID, long OldRoomMaster, long NewRoomMaster)
        {
            return $"{Command_Update} {TableName} {Command_Set} {Column_RoomMaster} = {NewRoomMaster} {Command_Where} {Column_RoomID} = '{RoomID}' {Command_And} {Column_RoomMaster} = {OldRoomMaster}";
        }

        public static string Select_IsExistRoom(string RoomID)
        {
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_RoomID} = '{RoomID}'";
        }
    }
}
