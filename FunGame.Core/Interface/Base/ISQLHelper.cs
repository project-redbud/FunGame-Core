using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISQLHelper
    {
        public string Script { get; set; }
        public EntityType EntityType { get; }
        public object Entity { get; }
        public SQLResult Result { get; }
        int UpdateRows { get; }
        public Library.Common.Network.SQLServerInfo ServerInfo { get; }
    }
}
