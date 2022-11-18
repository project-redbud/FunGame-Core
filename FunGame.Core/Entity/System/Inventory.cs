using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public User? User { get; set; } = null;
        public Hashtable? Characters { get; set; } = new Hashtable();
        public Hashtable? Items { get; set; } = new Hashtable();
    }
}
