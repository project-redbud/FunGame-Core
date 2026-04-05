using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class AddonDataRequestEventArgs(IServerModel client, string from, string target) : GeneralEventArgs
    {
        public IServerModel Client { get; set; } = client;
        public string From { get; set; } = from;
        public string Target { get; set; } = target;
    }
}
