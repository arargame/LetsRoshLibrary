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

        public override void ConvertToPersistent(BaseObject entity)
        {
            new ImageRepository(Context).ConvertToPersistent(entity.Image);

            var localizationRepository = new LocalizationRepository(Context);

            foreach (var localization in entity.Localizations)
            {
                localizationRepository.ConvertToPersistent(localization);
            }
        }

        public static string[] Includes
        {
            get
            {
                return new string[] { "Image", "Localizations" };
            }
        }

        public static string[] ThenIncludes
        {
            get
            {
                return new string[] { "Localizations.Language" };
            }
        }

        public static string[] AllIncludes
        {
            get
            {
                return Includes.Union(ThenIncludes).ToArray();
            }
        }

        public override string[] GetIncludes()
        {
            return Includes;
        }

        public override string[] GetThenIncludes()
        {
            return ThenIncludes;
        }

        public override BaseObject GetUniqueLight(BaseObject entity)
        {
            return base.GetUniqueLight(entity);
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

        public override void InsertUpdateOrDeleteGraph(BaseObject entity)
        {
            var existingEntity = GetExistingEntity(entity);

            var imageRepository = new ImageRepository(Context);

            if (existingEntity.Image != null)
            {
                if (entity.Image == null)
                    imageRepository.Delete(existingEntity.Image);
                else
                    imageRepository.Update(entity.Image);
            }
            else
            {
                imageRepository.Create(entity.Image);
            }


            var localizationRepository = new LocalizationRepository(Context);

            foreach (var localization in entity.Localizations)
            {
                if (localizationRepository.IsItNew(localization))
                {
                    localizationRepository.Create(localization);
                }
                else
                {
                    localizationRepository.Update(localization);
                }
            }

            var existingEntityLocalizations = existingEntity.Localizations.ToList();

            for (int i = 0; i < existingEntityLocalizations.Count; i++)
            {
                var localization = existingEntityLocalizations.ToList()[i];

                if (!entity.Localizations.Any(l => l.Id == localization.Id))
                    localizationRepository.Delete(localization);
            }
        }
    }
}
