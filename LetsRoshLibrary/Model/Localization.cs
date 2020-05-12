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

        public override bool Equals(object obj)
        {
            var localization = obj as Localization;

            return ClassName == localization.ClassName && PropertyName == localization.PropertyName && Language.Id == localization.Language.Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

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

        public static bool DeleteFromDb(Expression<Func<Localization, bool>> filter)
        {
            var isCommitted = false;

            Guid? entityId = null;

            try
            {
                using (var uow = new Dota2UnitofWork())
                {
                    var localizationRepository = new LocalizationRepository(uow.Context);

                    var entity = localizationRepository.Get(filter, LocalizationRepository.Includes);

                    entityId = entity?.Id;

                    if (!localizationRepository.Delete(entity))
                        throw new Exception("LocalizationRepository Delete Exception");

                    isCommitted = uow.Commit();
                }
            }
            catch (Exception ex)
            {
                Log.Save(new Log(ex.Message, entityId: entityId?.ToString()));
            }

            return isCommitted;
        }

        public static Localization GetFromDb(Expression<Func<Localization, bool>> filter, params string[] includes)
        {
            Localization entity = null;

            using (var uow = new Dota2UnitofWork())
            {
                entity = uow.Load<Localization>().Get(filter, includes);
            }

            return entity;
        }


    }
}
