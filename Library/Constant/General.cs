using System.Text;
using Milimoe.FunGame.Core.Entity;

/**
 * 此文件保存常用的对象常量
 */
namespace Milimoe.FunGame.Core.Library.Constant
{
    public class General
    {
        // Static Variable
        public static Empty EntityInstance => new();
        public static User UnknownUserInstance => new();
        public static Room HallInstance => new();
        public static Encoding DefaultEncoding => Encoding.Unicode;
        public static string GeneralDateTimeFormat => "yyyy-MM-dd HH:mm:ss.fff";
        public static DateTime DefaultTime => new(1970, 1, 1, 8, 0, 0);

        // Const
        public const int MaxRetryTimes = 20;
        public const int MaxTask_1C2G = 10;
        public const int MaxTask_2C2G = 20;
        public const int MaxTask_4C4G = 40;

        public const int SocketByteSize = 512 * 1024;
        public const int StreamByteSize = 2048;
    }
}
