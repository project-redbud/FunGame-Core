using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Server
{
    public abstract class BaseModel
    {
        public abstract bool Running { get; }
        public abstract ClientSocket? Socket { get; }
        public abstract Task? Task { get; }
        public abstract string ClientName { get; }
        public abstract User? User { get; }
        public List<BaseModel> GetClientsList => ClientThreads.GetList();
        public int ClientsCount => ClientThreads.Count;

        public abstract bool Read(ClientSocket socket);
        public abstract bool Send(ClientSocket socket, SocketMessageType type, params object[] objs);
        public abstract void Start();

        private readonly ThreadManager ClientThreads = new();

        public bool AddClient(string ClientName, BaseModel t)
        {
            return ClientThreads.Add(ClientName, t);
        }

        public bool RemoveClient(string ClientName)
        {
            return ClientThreads.Remove(ClientName);
        }

        public bool ContainsClient(string ClientName)
        {
            return ClientThreads.ContainsKey(ClientName);
        }

        public BaseModel GetClient(string ClientName)
        {
            return ClientThreads[ClientName];
        }

    }
}
