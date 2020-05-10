using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public class BaseObjectRepository : Repository<BaseObject>
    {
        public BaseObjectRepository(DbContext context) : base(context)
        {

        }

        public static string[] Includes
        {
            get
            {
                return new string[] { "Image", "Localizations" };
            }
        }

        public override void InsertDependencies(BaseObject entity)
        {
            new ImageRepository(Context).InsertDependencies(entity.Image);

            var localizationRepository = new LocalizationRepository(Context);

            foreach (var localization in entity.Localizations)
            {
                localizationRepository.InsertDependencies(localization);
            }
        }

        public override bool Create(BaseObject entity)
        {
            var isInserted = false;

            new ImageRepository(Context).Create(entity.Image);

            foreach (var localization in entity.Localizations)
            {
                new LocalizationRepository(Context).Create(localization);
            }

            return isInserted;
        }

        public override void UpdateOrCreateNavigations(BaseObject existing, BaseObject modified)
        {
            if (existing.Image != null)
            {
                new ImageRepository(Context).Delete(existing.Image);
            }

            existing.Image = modified.Image;

            foreach (var existingObjectLocalization in existing.Localizations)
            {
                foreach (var modifiedObjectLocalization in modified.Localizations)
                {
                    modifiedObjectLocalization.Language = new LanguageRepository(Context).GetUnique(modifiedObjectLocalization.Language) ?? modifiedObjectLocalization.Language;

                    if (existingObjectLocalization.Equals(modifiedObjectLocalization))
                    {
                        //new LocalizationRepository(Context)
                        //    .Update(entity:modifiedObjectLocalization,
                        //        filter:l=>l.Id==modifiedObjectLocalization.Id,
                        //        Locali);
                    }
                    else
                    {
                        new LocalizationRepository(Context).Create(modifiedObjectLocalization);
                    }
                }
            }

            var willBeRemovedLocalizationList = new List<string>(); 

            foreach (var localizationObjectToRemove in existing.Localizations.Select(el => el.UniqueValue).Except(modified.Localizations.Select(ml => ml.UniqueValue)))
            {
                willBeRemovedLocalizationList.Add(localizationObjectToRemove);

                if (existing.Localizations.Any(el => el.UniqueValue == localizationObjectToRemove))
                    new LocalizationRepository(Context).Delete(existing.Localizations.FirstOrDefault(el => el.UniqueValue == localizationObjectToRemove));
            }



        }

        public override void DeleteDependencies(BaseObject entity)
        {
            if (entity.Image != null)
                ChangeEntityState(entity.Image, EntityState.Deleted);

            var localizationRepository = new LocalizationRepository(Context);

            foreach (var localization in entity.Localizations.ToList())
            {
                localizationRepository.DeleteDependencies(localization);

                ChangeEntityState(localization,EntityState.Deleted);
            }
        }

        //public override bool Delete(BaseObject entity)
        //{
        //    var isDeleted = false;

        //    if (entity.Image != null)
        //        isDeleted = new ImageRepository(Context).Delete(entity.Image);

        //    return isDeleted;
        //}
    }
}
