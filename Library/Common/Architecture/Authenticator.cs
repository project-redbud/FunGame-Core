using System.Data;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;

namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    public class Authenticator(SQLHelper SQLHelper)
    {
        private readonly SQLHelper SQLHelper = SQLHelper;

        public virtual bool BeforeAuthenticator(AuthenticationType type, params object[] args)
        {
            return true;
        }

        public virtual bool AfterAuthenticator(AuthenticationType type, params object[] args)
        {
            return true;
        }

        public bool Authenticate(string script)
        {
            if (!BeforeAuthenticator(AuthenticationType.ScriptOnly, script)) return false;
            SQLHelper.ExecuteDataSet(script);
            if (SQLHelper.Success && AfterAuthenticator(AuthenticationType.ScriptOnly, script))
            {
                return true;
            }
            return false;
        }

        public bool Authenticate<T>(string script, string column, T keyword)
        {
            if (keyword is null) return false;
            if (!BeforeAuthenticator(AuthenticationType.Column, script, column, keyword)) return false;
            SQLHelper.ExecuteDataSet(script);
            if (SQLHelper.Success)
            {
                DataSet ds = SQLHelper.DataSet;
                if (ds.Tables[0].Columns.Contains(column) &&
                    ds.Tables[0].AsEnumerable().Any(dr => dr.Field<T>(column)?.Equals(keyword) ?? false) &&
                    AfterAuthenticator(AuthenticationType.Column, script, column, keyword))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Authenticate(string username, string password)
        {
            if (!BeforeAuthenticator(AuthenticationType.Username, username, password)) return false;
            SQLHelper.ExecuteDataSet(UserQuery.Select_Users_LoginQuery(SQLHelper, username, password));
            if (SQLHelper.Success && AfterAuthenticator(AuthenticationType.Username, username, password))
            {
                return true;
            }
            return false;
        }
    }
}
