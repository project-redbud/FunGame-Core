using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Models.Entity
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
        public int Credits { get; set; } = 0;
        public int Material { get; set; } = 0;
        public UserStatistics? Statistics { get; set; } = null;
        public Stock? Stock { get; set; } = null;

        public User()
        {

        }

        public User(string username)
        {
            Userame = username;
        }

        public User(string username, string password)
        {
            Userame = username;
            Password = password;
        }
    }
}
