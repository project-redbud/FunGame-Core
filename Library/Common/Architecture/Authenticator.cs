using System.Data;
using System.Data.Common;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;

namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    public abstract class Authenticator
    {
        private readonly SQLHelper SQLHelper;

        public Authenticator(SQLHelper SQLHelper)
        {
            this.SQLHelper = SQLHelper;
        }

        public bool Authenticate(string script)
        {
            SQLHelper.ExecuteDataSet(script);
            if (SQLHelper.Success)
            {
                DataSet ds = SQLHelper.DataSet;
                if (ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
            }
            return false;
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
        
        public bool Authenticate(string script, string username, string password)
        {
            SQLHelper.ExecuteDataSet(script);
            if (SQLHelper.Success)
            {
                DataSet ds = SQLHelper.DataSet;
                if (ds.Tables.Count > 0 &&
                    ds.Tables[0].Columns.Contains(UserQuery.Column_Username) &&
                    ds.Tables[0].Columns.Contains(UserQuery.Column_Password) &&
                    ds.Tables[0].Rows.Count > 0 &&
                    ds.Tables[0].AsEnumerable().Where(row => (string)row[UserQuery.Column_Username] == username && (string)row[UserQuery.Column_Password] == password).Any())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
