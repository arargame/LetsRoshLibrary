using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{
    public abstract class Character : BaseObject
    {
        public string LinkParameter { get; set; }
        public string Intelligence { get; set; }
        public string Agility { get; set; }
        public string Strength { get; set; }
        public string Damage { get; set; }
        public string MovementSpeed { get; set; }
        public string Armor { get; set; }
        public string Bio { get; set; }

        public virtual ICollection<Skill> Skills { get; set; }

        public Character()
        {
            Skills = new List<Skill>();
        }

        public Character AddSkill(Skill skill)
        {
            skill.Character = this;

            skill.CharacterId = this.Id;

            Skills.Add(skill);

            return this;
        }
    }
}
