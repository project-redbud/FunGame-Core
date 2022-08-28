using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Models.Entity
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

        public PassiveSkill()
        {
            Active = false;
        }
    }
}
