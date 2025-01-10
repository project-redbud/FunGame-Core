using System.Data;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Constant;
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

        public abstract bool BeforeAuthenticator(AuthenticationType type, params object[] args);

        public abstract bool AfterAuthenticator(AuthenticationType type, params object[] args);

        public bool Authenticate(string script)
        {
            if (!BeforeAuthenticator(AuthenticationType.ScriptOnly, script)) return false;
            SQLHelper.ExecuteDataSet(script);
            if (SQLHelper.Success)
            {
                DataSet ds = SQLHelper.DataSet;
                if (ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    if (!AfterAuthenticator(AuthenticationType.ScriptOnly, script)) return false;
                    return true;
                }
            }
            return false;
        }

        public bool Authenticate<T>(string script, string column, T keyword)
        {
            if (!BeforeAuthenticator(AuthenticationType.Column, script, column, keyword?.ToString() ?? "")) return false;
            SQLHelper.ExecuteDataSet(script);
            if (SQLHelper.Success)
            {
                DataSet ds = SQLHelper.DataSet;
                if (ds.Tables.Count > 0 &&
                    ds.Tables[0].Columns.Contains(column) &&
                    ds.Tables[0].Rows.Count > 0 &&
                    ds.Tables[0].AsEnumerable().Where(row => row.Field<T>(column)?.Equals(keyword) ?? false).Any())
                {
                    if (!AfterAuthenticator(AuthenticationType.Column, script, column, keyword?.ToString() ?? "")) return false;
                    return true;
                }
            }
            return false;
        }

        public bool Authenticate(string username, string password)
        {
            if (!BeforeAuthenticator(AuthenticationType.Username, username, password)) return false;
            SQLHelper.ExecuteDataSet(UserQuery.Select_Users_LoginQuery(SQLHelper, username, password));
            if (SQLHelper.Success)
            {
                DataSet ds = SQLHelper.DataSet;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (!AfterAuthenticator(AuthenticationType.Username, username, password)) return false;
                    return true;
                }
            }
            return false;
        }
    }
}
