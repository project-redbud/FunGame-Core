﻿using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class PassiveSkill : Skill
    {
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

        protected PassiveSkill()
        {
            Active = false;
        }

        internal static PassiveSkill GetInstance()
        {
            return new();
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is PassiveSkill s && s.Name == Name;
        }
    }
}
