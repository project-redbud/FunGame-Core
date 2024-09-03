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
        public Dictionary<string, User> Admins { get; set; } = [];
        public Dictionary<string, User> Members { get; set; } = [];
        public Dictionary<string, User> Applicants { get; set; } = [];

        public bool Equals(Club other)
        {
            return Equals(other);
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other?.Id == Id;
        }
    }
}
