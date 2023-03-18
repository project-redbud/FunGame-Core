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
    }
}
