namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class RoomQuery
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
        public const string Select_Rooms = $"{Constant.Command_Select} {TableName}.{Constant.Command_All}, {UserQuery.TableName}.{UserQuery.Column_Username} {Constant.Command_As} {Column_RoomMasterName} " +
            $"{Constant.Command_From} {TableName} {Constant.Command_LeftJoin} {UserQuery.TableName} {Constant.Command_On} {UserQuery.TableName}.{UserQuery.Column_UID} = {TableName}.{Column_RoomMaster}";

        public static string Insert_CreateRoom(string RoomID, long RoomMaster, Library.Constant.RoomType RoomType, string Password)
        {
            Library.Constant.RoomState RoomState = Library.Constant.RoomState.Created;
            DateTime NowTime = DateTime.Now;
            bool HasPass = false;
            if (Password.Trim() != "")
            {
                HasPass = true;
            }
            return $"{Constant.Command_Insert} {Constant.Command_Into} {TableName} ({Column_RoomID}, {Column_CreateTime}, {Column_RoomMaster}, {Column_RoomType}, {Column_RoomState}, {Column_HasPass}, {Column_Password})" +
                $" {Constant.Command_Values} ('{RoomID}', '{NowTime}', {RoomMaster}, {(int)RoomType}, {(int)RoomState}, {(HasPass ? 1 : 0)}, '{Password}')";
        }

        public static string Delete_QuitRoom(string RoomID, long RoomMaster)
        {
            return $"{Constant.Command_Delete} {Constant.Command_From} {TableName} {Constant.Command_Where} {Column_RoomID} = '{RoomID}' {Constant.Command_And} {Column_RoomMaster} = {RoomMaster}";
        }
        
        public static string Update_QuitRoom(string RoomID, long OldRoomMaster, long NewRoomMaster)
        {
            return $"{Constant.Command_Update} {TableName} {Constant.Command_Set} {Column_RoomMaster} = {NewRoomMaster} {Constant.Command_Where} {Column_RoomID} = '{RoomID}' {Constant.Command_And} {Column_RoomMaster} = {OldRoomMaster}";
        }
        
        public static string Select_IsExistRoom(string RoomID)
        {
            return $"{Constant.Command_Select} {Constant.Command_All} {Constant.Command_From} {TableName} {Constant.Command_Where} {Column_RoomID} = '{RoomID}'";
        }
    }
}
