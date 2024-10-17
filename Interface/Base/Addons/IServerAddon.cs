using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Interface.Addons
{
    public interface IServerAddon
    {
        public SQLHelper? SQLHelper { get; }
        public MailSender? MailSender { get; }
    }
}
