﻿using System.Collections;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Addons
{
    public interface IGameModeServer : IAddon, IAddonController
    {
        public bool StartServer(string GameMode, Room Room, List<User> Users, IServerModel RoomMasterServerModel, Dictionary<string, IServerModel> OthersServerModel, params object[] args);

        public Hashtable GamingMessageHandler(string username, GamingType type, Hashtable data);
    }
}
