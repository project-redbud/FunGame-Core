using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class UserProfile
    {
        /// <summary>
        /// 头像链接
        /// </summary>
        public string AvatarUrl { get; set; } = "";

        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; } = "";

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; } = "";

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDay { get; set; } = General.DefaultTime;

        /// <summary>
        /// 关注者
        /// </summary>
        public int Followers { get; set; } = 0;

        /// <summary>
        /// 正在关注
        /// </summary>
        public int Following { get; set; } = 0;

        /// <summary>
        /// 头衔
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// 用户组
        /// </summary>
        public string UserGroup { get; set; } = "";
    }
}
