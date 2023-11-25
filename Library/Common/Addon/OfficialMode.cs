using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    internal class OfficialModeMix8Players : GameMode
    {
        public override string Name => "OfficialModeMix8Players";

        public override string Description => "8人混战模式";

        public override string Version => FunGameInfo.FunGame_Version;

        public override string Author => FunGameInfo.FunGame_CopyRight;

        public override GameMap Map => General.DefaultOfficialMap;
    }

    internal class OfficialModeTeam4Players : GameMode
    {
        public override string Name => "OfficialModeTeam4Players2Characters";

        public override string Description => "4人团队2V2模式（双角色）";

        public override string Version => FunGameInfo.FunGame_Version;

        public override string Author => FunGameInfo.FunGame_CopyRight;

        public override GameMap Map => General.DefaultOfficialMap;
    }

    internal class OfficialModeTeam6Players : GameMode
    {
        public override string Name => "OfficialModeTeam6Players1Character";

        public override string Description => "6人团队3V3模式（单角色）";

        public override string Version => FunGameInfo.FunGame_Version;

        public override string Author => FunGameInfo.FunGame_CopyRight;

        public override GameMap Map => General.DefaultOfficialMap;
    }
    
    internal class OfficialModeSolo3Characters : GameMode
    {
        public override string Name => "OfficialModeSolo3Characters";

        public override string Description => "单人对弈模式（三角色）";

        public override string Version => FunGameInfo.FunGame_Version;

        public override string Author => FunGameInfo.FunGame_CopyRight;

        public override GameMap Map => General.DefaultOfficialMap;
    }
    
    internal class OfficialModeFastAuto8Players : GameMode
    {
        public override string Name => "OfficialModeFastAuto8Players";

        public override string Description => "8人快速自走模式";

        public override string Version => FunGameInfo.FunGame_Version;

        public override string Author => FunGameInfo.FunGame_CopyRight;

        public override GameMap Map => General.DefaultOfficialMap;
    }
}
