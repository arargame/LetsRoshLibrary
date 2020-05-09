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

        public override bool Insert(BaseObject entity)
        {
            var isInserted = false;

            new ImageRepository(Context).Insert(entity.Image);

            foreach (var localization in entity.Localizations)
            {
                new LocalizationRepository(Context).Insert(localization);
            }

            return isInserted;
        }

        public override bool UpdateNavigations(BaseObject existing, BaseObject local)
        {
            var isUpdated = false;

            if (existing.Image != null)
            {
                new ImageRepository(Context).Delete(existing.Image);
            }

            existing.Image = local.Image;

            foreach (var existingLocalization in existing.Localizations)
            {
                foreach (var localLocalization in local.Localizations)
                {
                    if (existingLocalization.UniqueValue == localLocalization.UniqueValue)
                    {
                        //existingLocalization.
                    }
                    else
                    {
                        new LocalizationRepository(Context).Insert(localLocalization);
                    }
                }
            }


            return isUpdated;
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
