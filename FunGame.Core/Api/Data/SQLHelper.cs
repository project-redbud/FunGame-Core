using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Data
{
    /// <summary>
    /// 需要在Server中继承此类实现。
    /// Milimoe.FunGame.Core.Service.SQLManager也是
    /// </summary>
    public abstract class SQLHelper : ISQLHelper
    {
        public string Script { get; set; } = "";

        public EntityType EntityType { get; }

        public object Entity { get; } = General.EntityInstance;

        public SQLResult Result { get; }

        public abstract SQLServerInfo ServerInfo { get; }

        public int UpdateRows { get; }

        public virtual SQLResult Execute()
        {
            return SQLResult.NotFound;
        }
    }
}
