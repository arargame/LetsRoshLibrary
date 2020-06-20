using HtmlAgilityPack;
using LetsRoshLibrary.Core.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{
    public class Hero : Character
    {
        public string Roles { get; set; }

        public Guid? PortraitId { get; set; }

        [ForeignKey("PortraitId")]
        public Image Portrait { get; set; }

        public Hero() { }

        public override void SetLocalization(Language language)
        {
            Localizations.Add(new Localization(this,language, "Hero", "Roles", Roles));
            Localizations.Add(new Localization(this,language, "Hero", "Bio", Bio));

            Skills.ToList().ForEach(s => s.SetLocalization(language));
        }
    }
}
