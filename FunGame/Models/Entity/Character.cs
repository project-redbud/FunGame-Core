using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Models.Entity
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string FirstName { get; set; } = "";
        public User? User { get; set; } = null;
        public CharacterStatistics? Statistics { get; set; } = null;
        public int Level { get; set; } = 1;
        public decimal EXP { get; set; }
        public decimal HP { get; set; }
        public decimal MP { get; set; }
        public decimal EP { get; set; }
        public decimal ATK { get; set; }
        public decimal DEF { get; set; }
        public decimal PhysicalReduction { get; set; }
        public decimal MDF { get; set; }
        public decimal HPRecovery { get; set; } = 0;
        public decimal MPRecovery { get; set; } = 0;
        public decimal EPRecovery { get; set; } = 0;
        public decimal SPD { get; set; }
        public decimal ATR { get; set; }
        public decimal CritRate { get; set; } = 0.05M;
        public decimal CritDMG { get; set; } = 1.25M;
        public decimal EvadeRate { get; set; } = 0.05M;
        public Hashtable? Skills { get; set; } = new Hashtable();
        public Hashtable? Items { get; set; } = new Hashtable();

        public Character()
        {
            
        }
    }
}
