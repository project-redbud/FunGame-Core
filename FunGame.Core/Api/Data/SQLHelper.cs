using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Data
{
    public class SQLHelper : ISQLHelper
    {
        public string Script { get; set; } = "";

        public EntityType EntityType { get; private set; } =  EntityType.Empty;

        public object Entity { get; private set; } = General.EntityInstance;

        public SQLResult Result { get; private set; } = SQLResult.Success;

        public SQLServerInfo ServerInfo { get; private set; } = new SQLServerInfo();

        public int UpdateRows { get; private set; } = 0;

        public SQLHelper Instance { get; private set; } = new SQLHelper();

        public static SQLHelper GetHelper(EntityType type)
        {
            return new SQLHelper(type);
        }

        private SQLHelper(EntityType type)
        {
            this.EntityType = type;
        }

        private SQLHelper()
        {
            
        }
    }
}
