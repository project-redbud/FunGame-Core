namespace Milimoe.FunGame.Core.Model
{
    public class SQLServerInfo
    {
        public string SQLServerName { get; } = "";
        public string SQLServerIP { get; } = "";
        public string SQLServerPort { get; } = "";
        public string SQLServerDataBase { get; } = "";
        public string SQLServerUser { get; } = "";
        public string SQLServerPassword { get; } = "";

        internal SQLServerInfo(InfoBuilder builder)
        {
            SQLServerName = builder.SQLServerName;
            SQLServerIP = builder.SQLServerIP;
            SQLServerPort = builder.SQLServerPort;
            SQLServerDataBase = builder.SQLServerDataBase;
            SQLServerUser = builder.SQLServerUser;
            SQLServerPassword = builder.SQLServerPassword;
        }

        public static SQLServerInfo Create(string name = "", string ip = "", string port = "", string database = "", string user = "", string password = "")
        {
            return new SQLServerInfo(new InfoBuilder(name, ip, port, database, user, password));
        }

        internal class InfoBuilder
        {
            internal string SQLServerName { get; } = "";
            internal string SQLServerIP { get; } = "";
            internal string SQLServerPort { get; } = "";
            internal string SQLServerDataBase { get; } = "";
            internal string SQLServerUser { get; } = "";
            internal string SQLServerPassword { get; } = "";

            internal InfoBuilder(string name, string ip, string port, string database, string user, string password)
            {
                SQLServerName = name;
                SQLServerIP = ip;
                SQLServerPort = port;
                SQLServerDataBase = database;
                SQLServerUser = user;
                SQLServerPassword = password;
            }
        }
    }
}
