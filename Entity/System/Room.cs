﻿using System.Data;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class Room : BaseEntity
    {
        public override long Id { get => base.Id ; set => base.Id = value; }
        public string Roomid { get; set; } = "";
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public Dictionary<string, User> Players { get; set; } = new();
        public User? RoomMaster { get; set; }
        public RoomType RoomType { get; set; }
        public RoomState RoomState { get; set; }
        public bool HasPass => Password.Trim() != "";
        public string Password { get; set; } = "";
        public GameStatistics? Statistics { get; set; } = null;

        internal Room(DataSet? DsRoom, DataSet? DsUser, int Index = 0)
        {
            if (DsRoom != null && DsRoom.Tables[0].Rows.Count > 0)
            {
                DataRow DrRoom = DsRoom.Tables[0].Rows[Index];
                Id = (long)DrRoom[RoomQuery.Column_ID];
                Roomid = (string)DrRoom[RoomQuery.Column_RoomID];
                CreateTime = (DateTime)DrRoom[RoomQuery.Column_CreateTime];
                RoomMaster = Api.Utility.Factory.GetUser(DsUser, GetUserRowIndex(DsUser, (long)DrRoom[RoomQuery.Column_RoomMaster]));
                RoomType = (RoomType)Convert.ToInt32(DrRoom[RoomQuery.Column_RoomType]);
                RoomState = (RoomState)Convert.ToInt32(DrRoom[RoomQuery.Column_RoomState]);
                Password = (string)DrRoom[RoomQuery.Column_Password];
            }
            else
            {
                Id = 0;
                Roomid = "-1";
                CreateTime = DateTime.MinValue;
                RoomMaster = null;
                RoomType = RoomType.None;
                RoomState = RoomState.Created;
                Password = "";
            }
        }

        public bool Equals(Room other)
        {
            return Equals(other);
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other?.Id == Id;
        }

        private static int GetUserRowIndex(DataSet? DsUser, long UID)
        {
            if (DsUser != null)
            {
                for (int i = 0; i < DsUser.Tables[0].Rows.Count; i++)
                {
                    if (DsUser.Tables[0].Rows[i][UserQuery.Column_UID].Equals(UID))
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
    }
}
