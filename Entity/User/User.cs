using System.Data;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class User : BaseEntity
    {
        public override long Id { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public DateTime RegTime { get; set; }
        public DateTime LastTime { get; set; }
        public string Email { get; set; } = "";
        public string NickName { get; set; } = "";
        public bool IsAdmin { get; set; } = false;
        public bool IsOperator { get; set; } = false;
        public bool IsEnable { get; set; } = false;
        public decimal Credits { get; set; } = 0;
        public decimal Materials { get; set; } = 0;
        public decimal GameTime { get; set; } = 0;
        public string AutoKey { get; set; } = "";
        public UserStatistics? Statistics { get; set; } = null;
        public Inventory? Stock { get; set; } = null;

        internal User()
        {

        }

        internal User(DataSet? DataSet, int Index = 0)
        {
            if (DataSet != null && DataSet.Tables[0].Rows.Count > 0)
            {
                DataRow DataRow = DataSet.Tables[0].Rows[Index];
                Id = (long)DataRow[UserQuery.Column_UID];
                Username = (string)DataRow[UserQuery.Column_Username];
                Password = (string)DataRow[UserQuery.Column_Password];
                RegTime = (DateTime)DataRow[UserQuery.Column_RegTime];
                LastTime = (DateTime)DataRow[UserQuery.Column_LastTime];
                Email = (string)DataRow[UserQuery.Column_Email];
                NickName = (string)DataRow[UserQuery.Column_Nickname];
                IsAdmin = Convert.ToInt32(DataRow[UserQuery.Column_IsAdmin]) == 1;
                IsOperator = Convert.ToInt32(DataRow[UserQuery.Column_IsOperator]) == 1;
                IsEnable = Convert.ToInt32(DataRow[UserQuery.Column_IsEnable]) == 1;
                Credits = Convert.ToDecimal(DataRow[UserQuery.Column_Credits]);
                Materials = Convert.ToDecimal(DataRow[UserQuery.Column_Materials]);
                GameTime = Convert.ToDecimal(DataRow[UserQuery.Column_GameTime]);
                AutoKey = (string)DataRow[UserQuery.Column_AutoKey];
            }
        }

        public override bool Equals(IBaseEntity? other)
        {
            if (other == null) return false;
            if (((User)other).Id == Id) return true;
            return false;
        }
    }
}
