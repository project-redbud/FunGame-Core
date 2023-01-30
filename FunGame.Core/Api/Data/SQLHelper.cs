using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Data
{
    public class SQLHelper : ISQLHelper
    {
        public string Script { get; set; } = "";

        public EntityType EntityType { get; }

        public object Entity { get; } = General.EntityInstance;

        public SQLResult Result { get; }

        public SQLServerInfo ServerInfo { get; }

        public int UpdateRows { get; }

        public static SQLHelper GetHelper(EntityType type)
        {
            return new SQLHelper(type);
        }

        private SQLHelper(EntityType type = EntityType.Empty, SQLServerInfo? info = null, SQLResult result = SQLResult.Success, int rows = 0)
        {
            this.EntityType = type;
            if (info == null) this.ServerInfo = SQLServerInfo.Create();
            else this.ServerInfo = info;
            this.Result = result;
            this.UpdateRows = rows;
        }

        public SQLResult Execute()
        {
            switch (EntityType)
            {
                case EntityType.NotEntity:
                    SQLManager SQLManager = new(this);
                    return SQLManager.Execute(Script);
                default:
                    break;
            }
            return Result;
        }
    }
}
