namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// aka 2FA
    /// </summary>
    public class TFA
    {
        private readonly Dictionary<string, string> TFACodes = new();

        public virtual bool IsAvailable(string username)
        {
            return true;
        }

        public string GetTFACode(string username)
        {
            return TFACodes.ContainsKey(username) ? TFACodes[username] : Verification.CreateVerifyCode(Library.Constant.VerifyCodeType.MixVerifyCode, 5);
        }

        public bool Authenticate(string username, string code)
        {
            if (!IsAvailable(username)) return false;
            if (TFACodes.ContainsKey(username) && TFACodes[username] == code)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
