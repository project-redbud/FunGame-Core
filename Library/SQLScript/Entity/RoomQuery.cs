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
        public const string Column_GameModule = "GameModule";
        public const string Column_GameMap = "GameMap";
        public const string Column_RoomState = "RoomState";
        public const string Column_IsRank = "IsRank";
        public const string Column_HasPass = "HasPass";
        public const string Column_Password = "Password";
        public const string Column_MaxUsers = "MaxUsers";
        public const string Select_Rooms = $"{Command_Select} {TableName}.{Command_All}, {UserQuery.TableName}.{UserQuery.Column_Username} {Command_As} {Column_RoomMasterName} " +
            $"{Command_From} {TableName} {Command_LeftJoin} {UserQuery.TableName} {Command_On} {UserQuery.TableName}.{UserQuery.Column_UID} = {TableName}.{Column_RoomMaster}";

        public static string Insert_CreateRoom(string roomid, long roomMaster, Library.Constant.RoomType roomType, string gameModule, string gameMap, bool isRank, string password, int maxUsers)
        {
            Library.Constant.RoomState RoomState = Library.Constant.RoomState.Created;
            DateTime NowTime = DateTime.Now;
            bool HasPass = password.Trim() != "";
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_RoomID}, {Column_CreateTime}, {Column_RoomMaster}, {Column_RoomType}, {Column_GameModule}, {Column_GameMap}, {Column_RoomState}, {Column_IsRank}, {Column_HasPass}, {Column_Password}, {Column_MaxUsers})" +
                $" {Command_Values} ('{roomid}', '{NowTime}', {roomMaster}, {(int)roomType}, '{gameModule}', '{gameMap}', {(int)RoomState}, {(isRank ? 1 : 0)}, {(HasPass ? 1 : 0)}, '{password}', {maxUsers})";
        }

        public static string Delete_Rooms(params string[] roomids)
        {
            if (roomids.Length != 0)
            {
                string where = string.Join("', '", roomids);
                return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_RoomID} {Command_In} ('{where}')";
            }
            return $"{Command_Delete} {Command_From} {TableName}";
        }

        public static string Delete_QuitRoom(string roomID, long roomMaster)
        {
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_RoomID} = '{roomID}' {Command_And} {Column_RoomMaster} = {roomMaster}";
        }

        public static string Update_QuitRoom(string roomid, long oldRoomMaster, long newRoomMaster)
        {
            return $"{Command_Update} {TableName} {Command_Set} {Column_RoomMaster} = {newRoomMaster} {Command_Where} {Column_RoomID} = '{roomid}' {Command_And} {Column_RoomMaster} = {oldRoomMaster}";
        }

        public static string Select_IsExistRoom(string roomid)
        {
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_RoomID} = '{roomid}'";
        }
    }
}
