using System.Text;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Library.Constant
{
    public class General
    {
        // Static Variable
        public static Empty EntityInstance { get; } = new();
        public static Room HallInstance { get; } = Api.Utility.Factory.GetHall();
        public static Encoding DefaultEncoding { get; } = Encoding.Unicode;

        // Const
        public const int MaxRetryTimes = 20;
        public const int MaxTask_1C2G = 10;
        public const int MaxTask_2C2G = 20;
        public const int MaxTask_4C4G = 40;

        public const int SocketByteSize = 512 * 1024;
        public const int StreamByteSize = 2048;
    }
}
