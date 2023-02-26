using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Server;
using System.Data;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISQLHelper
    {
        public string Script { get; set; }
        public CommandType CommandType { get; set; }
        public SQLResult Result { get; }
        public SQLServerInfo ServerInfo { get; }
        public int UpdateRows { get; }
        public DataSet DataSet { get; }
    }
}
