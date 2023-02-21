namespace Milimoe.FunGame
{
    public class SystemError : System.Exception
    {
        public override string Message => "系统错误 (#00001)";
    }

    public class CanNotConnectException : System.Exception
    {
        public override string Message => "无法连接至服务器，请检查网络并重启游戏再试 (#00002)";
    }

    public class TimeOutException : System.Exception
    {
        public override string Message => "连接超时 (#00003)";
    }

    public class UnknownException : System.Exception
    {
        public override string Message => "未知错误 (#00004)";
    }

    public class ReadConfigException : System.Exception
    {
        public override string Message => "读取配置文件出错，参数格式不正确 (#00005)";
    }

    public class SingletonAddException : System.Exception
    {
        public override string Message => "添加单例到单例表时遇到错误 (#00006)";
    }

    public class SingletonGetException : System.Exception
    {
        public override string Message => "不能从单例表中获取到指定的单例 (#00007)";
    }

    public class SocketWrongInfoException : System.Exception
    {
        public override string Message => "收到错误的返回信息 (#00008)";
    }

    public class SocketCreateListenException : System.Exception
    {
        public override string Message => "无法创建监听，请重新启动服务器再试 (#00009)";
    }

    public class SocketGetClientException : System.Exception
    {
        public override string Message => "无法获取客户端信息 (#00010)";
    }

    public class ListeningSocketCanNotSendException : System.Exception
    {
        public override string Message => "监听Socket不能用于发送信息 (#00011)";
    }

    public class ConnectFailedException : System.Exception
    {
        public override string Message => "连接到服务器失败 (#00012)";
    }

    public class LostConnectException : System.Exception
    {
        public override string Message => "与服务器连接中断 (#00013)";
    }

    public class FindServerFailedException : System.Exception
    {
        public override string Message => "查找可用的服务器失败，请重启FunGame (#00014)";
    }

    public class FormHasBeenOpenedException : System.Exception
    {
        public override string Message => "目标窗口可能已处于打开状态 (#00015)";
    }

    public class FormCanNotOpenException : System.Exception
    {
        public override string Message => "无法打开指定窗口 (#00016)";
    }
}
