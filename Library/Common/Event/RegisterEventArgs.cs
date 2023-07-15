namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class RegisterEventArgs : GeneralEventArgs
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Email { get; set; } = "";

        public RegisterEventArgs(string username = "", string password = "", string email = "")
        {
            if (username.Trim() != "") Username = username;
            if (password.Trim() != "") Password = password;
            if (email.Trim() != "") Email = email;
        }
    }
}
