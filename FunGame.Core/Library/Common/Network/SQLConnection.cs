namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class SQLServerInfo
    {
        public string SQLServerName { get; } = "";
        public string SQLServerIP { get; } = "";
        public string SQLServerPort { get; } = "";
        public string SQLServerUser { get; } = "";
        public string SQLServerPassword { get; } = "";

        internal SQLServerInfo(InfoBuilder builder)
        {
            SQLServerName = builder.SQLServerName;
            SQLServerIP = builder.SQLServerIP;
            SQLServerPort = builder.SQLServerPort;
            SQLServerUser = builder.SQLServerUser;
            SQLServerPassword = builder.SQLServerPassword;
        }

        public static SQLServerInfo Create(string name = "", string ip = "", string port = "", string user = "", string password = "")
        {
            return new SQLServerInfo(new InfoBuilder(name, ip, port, user, password));
        }

        internal class InfoBuilder
        {
            internal string SQLServerName { get; } = "";
            internal string SQLServerIP { get; } = "";
            internal string SQLServerPort { get; } = "";
            internal string SQLServerUser { get; } = "";
            internal string SQLServerPassword { get; } = "";

            internal InfoBuilder(string name, string ip, string port, string user, string password)
            {
                SQLServerName = name;
                SQLServerIP = ip;
                SQLServerPort = port;
                SQLServerUser = user;
                SQLServerPassword = password;
            }
        }
    }
}
