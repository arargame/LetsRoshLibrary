using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{
    public class Localization : BaseObject
    {
        public Guid BaseObjectId { get; set; }

        [ForeignKey("BaseObjectId")]
        public virtual BaseObject BaseObject { get; set; }

        public string ClassName { get; set; }

        public string PropertyName { get; set; }

        public Guid LanguageId { get; set; }

        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; }

        public string Value { get; set; }

        public Localization() { }

        public static Localization Create(BaseObject baseObject,Language language, string className, string propertyName, string value)
        {
            var localization = new Localization
            {
                BaseObject = baseObject,
                Language = Language.GetFromDb(language.Name),
                ClassName = className,
                PropertyName = propertyName,
                Value = value
            };

            return localization;
        }
    }
}
