using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
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

        public override string UniqueValue
        {
            get
            {
                return string.Format("{0}{1}{2}", ClassName, PropertyName, LanguageId != Guid.Empty ? LanguageId : Language?.Id);
            }
        }

        public Localization() { }

        public Localization(BaseObject baseObject, Language language, string className, string propertyName, string value)
        {
            BaseObject = baseObject;

            Language = language;

            ClassName = className;

            PropertyName = propertyName;

            Value = value;
        }      
    }
}
