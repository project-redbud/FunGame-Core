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
            string code = TFACodes.ContainsKey(username) ? TFACodes[username] : Verification.CreateVerifyCode(Library.Constant.VerifyCodeType.MixVerifyCode, 5);
            TaskUtility.RunTimer(() =>
            {
                // 十分钟后删除此码
                TFACodes.Remove(username, out _);
            }, 1000 * 10 * 60);
            return code;
        }

        public bool Authenticate(string username, string code, out string msg)
        {
            msg = "";
            if (!IsAvailable(username))
            {
                msg = "此账号不需要双重认证。";
                return false;
            }
            if (TFACodes.ContainsKey(username) && TFACodes.TryGetValue(username, out string? checkcode) && checkcode != null && checkcode == code)
            {
                TFACodes.Remove(username);
                return true;
            }
            else
            {
                msg = "验证码错误或已过期。";
                return false;
            }
        }
    }
}
