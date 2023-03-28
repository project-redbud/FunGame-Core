namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class LoginEventArgs : GeneralEventArgs
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string AutoKey { get; set; } = "";

        public LoginEventArgs(params object[]? objs)
        {
            if (objs != null)
            {
                if (objs.Length > 0) Username = (string)objs[0];
                if (objs.Length > 1) Password = (string)objs[1];
                if (objs.Length > 2) AutoKey = (string)objs[2];
            }
        }
    }
}
