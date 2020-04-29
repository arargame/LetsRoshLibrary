using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{
    public class Localization : BaseObject
    {
        public string ClassName { get; set; }

        public string PropertyName { get; set; }

        public Language Language { get; set; }

        public string Value { get; set; }

        public static Localization Create(Language language,string className,string propertyName,string value)
        {
            var localization = new Localization();

            localization.Language = language;
            localization.ClassName = className;
            localization.PropertyName = propertyName;
            localization.Value = value;

            return localization;
        }
    }
}
