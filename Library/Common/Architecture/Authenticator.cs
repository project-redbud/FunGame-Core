using System.Data;
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

        public abstract bool BeforeAuthenticator();

        public abstract bool AfterAuthenticator();

        public bool Authenticate(string script)
        {
            if (!BeforeAuthenticator()) return false;
            SQLHelper.ExecuteDataSet(script);
            if (SQLHelper.Success)
            {
                DataSet ds = SQLHelper.DataSet;
                if (ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    if (!AfterAuthenticator()) return false;
                    return true;
                }
            }
            return false;
        }

        public bool Authenticate<T>(string script, string column, T keyword)
        {
            if (!BeforeAuthenticator()) return false;
            SQLHelper.ExecuteDataSet(script);
            if (SQLHelper.Success)
            {
                DataSet ds = SQLHelper.DataSet;
                if (ds.Tables.Count > 0 &&
                    ds.Tables[0].Columns.Contains(column) &&
                    ds.Tables[0].Rows.Count > 0 &&
                    ds.Tables[0].AsEnumerable().Where(row => row.Field<T>(column)!.Equals(keyword)).Any())
                {
                    if (!AfterAuthenticator()) return false;
                    return true;
                }
            }
            return false;
        }
        
        public bool Authenticate(string username, string password)
        {
            if (!BeforeAuthenticator()) return false;
            SQLHelper.ExecuteDataSet(UserQuery.Select_Users_LoginQuery(username, password));
            if (SQLHelper.Success)
            {
                DataSet ds = SQLHelper.DataSet;
                if (ds.Tables.Count > 0 &&
                    ds.Tables[0].Columns.Contains(UserQuery.Column_Username) &&
                    ds.Tables[0].Columns.Contains(UserQuery.Column_Password) &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    if (!AfterAuthenticator()) return false;
                    return true;
                }
            }
            return false;
        }
    }
}
