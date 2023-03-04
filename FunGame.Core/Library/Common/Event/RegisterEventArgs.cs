namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class RegisterEventArgs : GeneralEventArgs
    {
        public string Username;
        public string Password;
        public string Email;

        public RegisterEventArgs(string username = "", string password = "", string email = "")
        {
            Username = username;
            Password = password;
            Email = email;
        }
    }
}
