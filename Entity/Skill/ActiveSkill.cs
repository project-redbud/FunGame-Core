using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class ActiveSkill : Skill
    {
        public decimal MP { get; set; } = 0;
        public decimal EP { get; set; } = 0;
        public decimal Reference1 { get; set; } = 0;
        public decimal Reference2 { get; set; } = 0;
        public decimal Reference3 { get; set; } = 0;
        public decimal Reference4 { get; set; } = 0;
        public decimal Reference5 { get; set; } = 0;
        public decimal Reference6 { get; set; } = 0;
        public decimal Reference7 { get; set; } = 0;
        public decimal Reference8 { get; set; } = 0;
        public decimal Reference9 { get; set; } = 0;
        public decimal Reference10 { get; set; } = 0;

        protected ActiveSkill()
        {
            Active = true;
        }

        internal static ActiveSkill GetInstance()
        {
            return new();
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is ActiveSkill s && s.Name == Name;
        }
    }
}
