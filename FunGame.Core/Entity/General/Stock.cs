using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.General
{
    public class Stock
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public User? User { get; set; } = null;
        public Hashtable? Characters { get; set; } = new Hashtable();
        public Hashtable? Items { get; set; } = new Hashtable();
    }
}
