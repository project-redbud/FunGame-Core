namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class LoginEventArgs : GeneralEventArgs
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string AutoKey { get; set; } = "";

        public LoginEventArgs(string username = "", string password = "", string autokey = "")
        {
            if (username.Trim() != "") Username = username;
            if (password.Trim() != "") Password = password;
            if (autokey.Trim() != "") AutoKey = autokey;
        }
    }
}
