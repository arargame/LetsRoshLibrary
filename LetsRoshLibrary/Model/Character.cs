using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{
    public abstract class Character : BaseObject
    {
        public string Intelligence { get; set; }
        public string Agility { get; set; }
        public string Strength { get; set; }
        public string Damage { get; set; }
        public string MovementSpeed { get; set; }
        public string Armor { get; set; }
        public string Bio { get; set; }
    }
}
