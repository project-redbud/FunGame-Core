namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class LoginEventArgs : GeneralEventArgs
    {
        public string Username;
        public string Password;

        public LoginEventArgs(string username = "", string password = "")
        {
            Username = username;
            Password = password;
        }
    }
}
