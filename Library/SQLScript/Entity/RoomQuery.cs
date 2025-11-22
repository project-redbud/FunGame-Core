using System.Text;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class RoomQuery : Constant
    {
        public const string TableName = "Rooms";
        public const string Column_ID = "Id";
        public const string Column_Roomid = "Roomid";
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
            $"{Command_From} {TableName} {Command_LeftJoin} {UserQuery.TableName} {Command_On} {UserQuery.TableName}.{UserQuery.Column_Id} = {TableName}.{Column_RoomMaster}";

        public static string Select_RoomByRoomId(SQLHelper SQLHelper, string Roomid)
        {
            SQLHelper.Parameters["@Roomid"] = Roomid;
            return $"{Command_Select} {Command_All} {Command_From} {TableName} {Command_Where} {Column_Roomid} = @Roomid";
        }

        public static string Select_RoomsByRoomState(SQLHelper SQLHelper, params RoomState[] States)
        {
            RoomState[] states = [.. States.Distinct()];
            if (states.Length == 0)
            {
                return $"{Select_Rooms} {Command_Where} 1 = 0";
            }
            StringBuilder builder = new();
            builder.Append($" {Command_Where} {Column_RoomState} {Command_In} (");
            for (int i = 0; i < states.Length; i++)
            {
                if (i > 0) builder.Append(", ");
                builder.Append($"@s{i}");
                SQLHelper.Parameters[$"@s{i}"] = states[i];
            }
            builder.Append(')');
            return $"{Select_Rooms} {Command_Where} {builder}";
        }

        public static string Select_RoomsByGameModuleAndRoomState(SQLHelper SQLHelper, string GameModule = "", params RoomState[] States)
        {
            string sql = Select_Rooms;
            string whereClause = "";

            if (!string.IsNullOrEmpty(GameModule))
            {
                SQLHelper.Parameters["@GameModule"] = GameModule;
                whereClause += $"{Command_And} {Column_GameModule} = @GameModule\r\n";
            }

            RoomState[] states = [.. States.Distinct()];
            if (states.Length > 0)
            {
                StringBuilder builder = new();
                builder.Append($"{Command_And} {Column_RoomState} {Command_In} (");
                for (int i = 0; i < states.Length; i++)
                {
                    if (i > 0) builder.Append(", ");
                    builder.Append($"@s{i}");
                    SQLHelper.Parameters[$"@s{i}"] = states[i];
                }
                builder.AppendLine(")");
            }

            if (!string.IsNullOrEmpty(whereClause))
            {
                sql += $" {Command_Where} {whereClause[Command_And.Length..]}";
            }

            return sql.Trim();
        }

        public static string Insert_CreateRoom(SQLHelper SQLHelper, string Roomid, long RoomMaster, RoomType RoomType, string GameModule, string GameMap, bool IsRank, string Password, int MaxUsers)
        {
            RoomState RoomState = RoomState.Created;
            DateTime NowTime = DateTime.Now;
            bool HasPass = Password.Trim() != "";

            SQLHelper.Parameters["@Roomid"] = Roomid;
            SQLHelper.Parameters["@CreateTime"] = NowTime;
            SQLHelper.Parameters["@RoomMaster"] = RoomMaster;
            SQLHelper.Parameters["@RoomType"] = (int)RoomType;
            SQLHelper.Parameters["@GameModule"] = GameModule;
            SQLHelper.Parameters["@GameMap"] = GameMap;
            SQLHelper.Parameters["@RoomState"] = (int)RoomState;
            SQLHelper.Parameters["@IsRank"] = IsRank ? 1 : 0;
            SQLHelper.Parameters["@HasPass"] = HasPass ? 1 : 0;
            SQLHelper.Parameters["@Password"] = Password;
            SQLHelper.Parameters["@MaxUsers"] = MaxUsers;

            return $"{Command_Insert} {Command_Into} {TableName} ({Column_Roomid}, {Column_CreateTime}, {Column_RoomMaster}, {Column_RoomType}, {Column_GameModule}, {Column_GameMap}, {Column_RoomState}, {Column_IsRank}, {Column_HasPass}, {Column_Password}, {Column_MaxUsers})" +
                $" {Command_Values} (@Roomid, @CreateTime, @RoomMaster, @RoomType, @GameModule, @GameMap, @RoomState, @IsRank, @HasPass, @Password, @MaxUsers)";
        }

        public static string Delete_Rooms(SQLHelper SQLHelper, params string[] Roomids)
        {
            if (Roomids.Length != 0)
            {
                Roomids = [.. Roomids.Distinct()];
                if (Roomids.Length > 0)
                {
                    StringBuilder builder = new();
                    builder.Append($"{Command_Where} {Column_RoomState} {Command_In} (");
                    for (int i = 0; i < Roomids.Length; i++)
                    {
                        if (i > 0) builder.Append(", ");
                        builder.Append($"@room{i}");
                        SQLHelper.Parameters[$"@room{i}"] = Roomids[i];
                    }
                    builder.AppendLine(")");
                    return $"{Command_Delete} {Command_From} {TableName} {builder}";
                }
            }
            return $"{Command_Delete} {Command_From} {TableName}";
        }

        public static string Delete_QuitRoom(SQLHelper SQLHelper, string Roomid, long RoomMaster)
        {
            SQLHelper.Parameters["@Roomid"] = Roomid;
            SQLHelper.Parameters["@RoomMaster"] = RoomMaster;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Roomid} = @Roomid {Command_And} {Column_RoomMaster} = @RoomMaster";
        }

        public static string Update_QuitRoom(SQLHelper SQLHelper, string Roomid, long OldRoomMaster, long NewRoomMaster)
        {
            SQLHelper.Parameters["@Roomid"] = Roomid;
            SQLHelper.Parameters["@OldRoomMaster"] = OldRoomMaster;
            SQLHelper.Parameters["@NewRoomMaster"] = NewRoomMaster;
            return $"{Command_Update} {TableName} {Command_Set} {Column_RoomMaster} = @NewRoomMaster {Command_Where} {Column_Roomid} = @Roomid {Command_And} {Column_RoomMaster} = @OldRoomMaster";
        }

        public static string Update_UpdateRoomMaster(SQLHelper SQLHelper, string Roomid, long NewRoomMaster)
        {
            SQLHelper.Parameters["@Roomid"] = Roomid;
            SQLHelper.Parameters["@NewRoomMaster"] = NewRoomMaster;
            return $"{Command_Update} {TableName} {Command_Set} {Column_RoomMaster} = @NewRoomMaster {Command_Where} {Column_Roomid} = @Roomid";
        }
    }
}
