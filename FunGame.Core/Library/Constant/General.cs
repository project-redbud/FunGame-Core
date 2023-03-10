using System.Text;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Library.Constant
{
    public class General
    {
        // Static Variable
        public static Empty EntityInstance { get; } = new();
        public static Encoding DefaultEncoding { get; } = Encoding.Unicode;

        // Const
        public const int MaxRetryTimes = 20;
        public const int MaxTask_1C2G = 10;
        public const int MaxThread_General = 20;
        public const int MaxTask_4C4G = 40;

        public const int SocketByteSize = 2048;
    }
}
