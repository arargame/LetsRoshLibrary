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
        public Guid CharacterId { get; set; }

        [ForeignKey("CharacterId")]
        public Character Character { get; set; }
        public string ManaCost { get; set; }
        public string CoolDown { get; set; }
        public string Extra { get; set; }
        public string Lore { get; set; }
        public string Video { get; set; }

        public override void SetLocalization(Language language)
        {
            Localizations.Add(new Localization(this,language, "Skill", "Extra", Extra));
            Localizations.Add(new Localization(this,language, "Skill", "Description", Description));
            Localizations.Add(new Localization(this,language, "Skill", "Lore", Lore));
        }
    }
}
