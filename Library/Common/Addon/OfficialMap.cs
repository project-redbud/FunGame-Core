using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    internal class OfficialMap16x16 : GameMap
    {
        public override string Name => "OfficialMap16x16";

        public override string Description => "16x16格的地图";

        public override string Version => FunGameInfo.FunGame_Version;

        public override string Author => FunGameInfo.FunGame_CopyRight;

        public override float Width => 16;

        public override float Height => 16;

        public override float Size => 32;
    }
}
