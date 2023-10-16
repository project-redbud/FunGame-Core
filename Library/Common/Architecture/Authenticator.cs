using System.Data;
using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    public abstract class Authenticator
    {
        private readonly SQLHelper SQLHelper;

        public Authenticator(SQLHelper SQLHelper)
        {
            this.SQLHelper = SQLHelper;
        }

        public bool Authenticate<T>(string script, string column, T keyword)
        {
            SQLHelper.ExecuteDataSet(script);
            if (SQLHelper.Success)
            {
                DataSet ds = SQLHelper.DataSet;
                if (ds.Tables.Count > 0 &&
                    ds.Tables[0].Columns.Contains(column) &&
                    ds.Tables[0].Rows.Count > 0 &&
                    ds.Tables[0].AsEnumerable().Where(row => row.Field<T>(column)!.Equals(keyword)).Any())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
