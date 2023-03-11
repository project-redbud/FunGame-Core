namespace Milimoe.FunGame
{
    public class SystemError : Exception
    {
        public override string Message => "系统错误 (#10001)";
    }

    public class CanNotConnectException : Exception
    {
        public override string Message => "无法连接至服务器，请检查网络并重启游戏再试 (#10002)";
    }

    public class TimeOutException : Exception
    {
        public override string Message => "连接超时 (#10003)";
    }

    public class UnknownException : Exception
    {
        public override string Message => "未知错误 (#10004)";
    }

    public class ReadConfigException : Exception
    {
        public override string Message => "读取配置文件出错，参数格式不正确 (#10005)";
    }

    public class SingletonAddException : Exception
    {
        public override string Message => "添加单例到单例表时遇到错误 (#10006)";
    }

    public class SingletonGetException : Exception
    {
        public override string Message => "不能从单例表中获取到指定的单例 (#10007)";
    }

    public class SocketWrongInfoException : Exception
    {
        public override string Message => "收到错误的返回信息 (#10008)";
    }

    public class SocketCreateListenException : Exception
    {
        public override string Message => "无法创建监听，请重新启动服务器再试 (#10009)";
    }

    public class SocketGetClientException : Exception
    {
        public override string Message => "无法获取客户端信息 (#10010)";
    }

    public class ListeningSocketCanNotSendException : Exception
    {
        public override string Message => "监听Socket不能用于发送信息 (#10011)";
    }

    public class ConnectFailedException : Exception
    {
        public override string Message => "连接到服务器失败 (#10012)";
    }

    public class LostConnectException : Exception
    {
        public override string Message => "与服务器连接中断 (#10013)";
    }

    public class FindServerFailedException : Exception
    {
        public override string Message => "查找可用的服务器失败，请重启FunGame (#10014)";
    }

    public class FormHasBeenOpenedException : Exception
    {
        public override string Message => "目标窗口可能已处于打开状态 (#10015)";
    }

    public class FormCanNotOpenException : Exception
    {
        public override string Message => "无法打开指定窗口 (#10016)";
    }

    public class ServerErrorException : Exception
    {
        public override string Message => "服务器遇到问题需要关闭，请重新启动服务器！ (#10017)";
    }

    public class CanNotSendToClientException : Exception
    {
        public override string Message => "无法向客户端传输消息 (#10018)";
    }

    public class MySQLConfigException : Exception
    {
        public override string Message => "MySQL服务启动失败：无法找到MySQL配置文件 (#10019)";
    }

    public class CanNotLogOutException : Exception
    {
        public override string Message => "无法登出您的账号，请联系服务器管理员 (#10020)";
    }

    public class NoUserLogonException : Exception
    {
        public override string Message => "用户未登录 (#10021)";
    }

    public class SQLQueryException : Exception
    {
        public override string Message => "执行SQL查询时遇到错误 (#10022)";
    }

    public class CanNotIntoRoomException : Exception
    {
        public override string Message => "无法加入指定房间 (#10023)";
    }

    public class CanNotSendTalkException : Exception
    {
        public override string Message => "无法发送公共信息 (#10024)";
    }

    public class CanNotSendEmailException : Exception
    {
        public override string Message => "无法发送邮件 (#10025)";
    }

    public class SmtpHelperException : Exception
    {
        public override string Message => "无法创建Smtp服务 (#10026)";
    }

    public class IndexOutOfArrayLengthException : Exception
    {
        public override string Message => "索引超过数组长度上限 (#10027)";
    }

    public class SocketCreateReceivingException : Exception
    {
        public override string Message => "无法创建监听套接字 (#10028)";
    }

    public class GetRoomListException : Exception
    {
        public override string Message => "获取房间列表失败 (#10029)";
    }

    public class QuitRoomException : Exception
    {
        public override string Message => "退出房间失败 (#10030)";
    }
}
