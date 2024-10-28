using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 角色的一些个人信息
    /// </summary>
    /// <param name="name"></param>
    /// <param name="firstname"></param>
    /// <param name="nickname"></param>
    public class CharacterProfile(string name, string firstname, string nickname)
    {
        /// <summary>
        /// 角色的姓
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// 角色的名字
        /// </summary>
        public string FirstName { get; set; } = firstname;

        /// <summary>
        /// 角色的昵称
        /// </summary>
        public string NickName { get; set; } = nickname;

        /// <summary>
        /// 角色的出生地
        /// </summary>
        public string Birthplace { get; set; } = "";

        /// <summary>
        /// 角色的出生日期
        /// </summary>
        public DateTime Birthday { get; set; } = General.DefaultTime;

        /// <summary>
        /// 角色的身份
        /// </summary>
        public string Status { get; set; } = "";

        /// <summary>
        /// 角色的隶属
        /// </summary>
        public string Affiliation { get; set; } = "";

        /// <summary>
        /// 角色的性别
        /// </summary>
        public string Sex { get; set; } = "";

        /// <summary>
        /// 角色的身高
        /// </summary>
        public string Height { get; set; } = "";

        /// <summary>
        /// 角色的体重
        /// </summary>
        public string Weight { get; set; } = "";

        /// <summary>
        /// 角色的故事
        /// </summary>
        public Dictionary<string, string> Stories { get; set; } = [];

        /// <summary>
        /// 复制一个角色资料
        /// </summary>
        /// <returns></returns>
        public CharacterProfile Copy()
        {
            return new(Name, FirstName, NickName)
            {
                Birthplace = Birthplace,
                Birthday = Birthday,
                Status = Status,
                Affiliation = Affiliation,
                Sex = Sex,
                Height = Height,
                Weight = Weight,
                Stories = new(Stories)
            };
        }
    }
}