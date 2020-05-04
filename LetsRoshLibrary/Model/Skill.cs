using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{
    public class Skill : BaseObject
    {
        public Guid HeroId { get; set; }

        [ForeignKey("HeroId")]
        public Hero Hero { get; set; }
        public string ManaCost { get; set; }
        public string CoolDown { get; set; }
        public string Extra { get; set; }
        public string Lore { get; set; }
        public string Video { get; set; }

        public override void SetLocalization(Language language)
        {
            Localizations.Add(Localization.Create(this,language, "Skill", "Extra", Extra));
            Localizations.Add(Localization.Create(this,language, "Skill", "Description", Description));
            Localizations.Add(Localization.Create(this,language, "Skill", "Lore", Lore));
        }
    }
}
