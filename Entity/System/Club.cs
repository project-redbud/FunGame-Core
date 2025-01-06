using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class Club : BaseEntity
    {
        public string Prefix { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsNeedApproval { get; set; } = false;
        public bool IsPublic { get; set; } = false;
        public double ClubPoins { get; set; } = 0;
        public User? Master { get; set; }
        public Dictionary<long, User> Admins { get; set; } = [];
        public Dictionary<long, User> Members { get; set; } = [];
        public Dictionary<long, User> Applicants { get; set; } = [];
        public Dictionary<long, DateTime> MemberJoinTime { get; set; } = [];
        public Dictionary<long, DateTime> ApplicationTime { get; set; } = [];

        public override bool Equals(IBaseEntity? other)
        {
            return other is Club && other?.Id == Id;
        }
    }
}
