namespace Milimoe.FunGame.Core.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Userame { get; set; } = "";
        public string Password { get; set; } = "";
        public DateTime RegTime { get; set; }
        public DateTime LastTime { get; set; }
        public string Email { get; set; } = "";
        public string NickName { get; set; } = "";
        public bool IsAdmin { get; set; } = false;
        public bool IsOperator { get; set; } = false;
        public bool IsEnable { get; set; } = false;
        public int OnlineState { get; set; } = 0;
        public string Roomid { get; set; } = "";
        public decimal Credits { get; set; } = 0;
        public decimal Materials { get; set; } = 0;
        public decimal GameTime { get; set; } = 0;
        public UserStatistics? Statistics { get; set; } = null;
        public Inventory? Stock { get; set; } = null;

        internal User()
        {

        }

        internal User(string username)
        {
            Userame = username;
        }

        internal User(string username, string password)
        {
            Userame = username;
            Password = password;
        }
    }
}
