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
        public static string[] AllIncludes
        {
            get
            {
                return Includes.Union(ThenIncludes).ToArray();
            }
        }
        public static string[] Includes
        {
            get
            {
                return new string[] { "Image", "Localizations" };
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

        public static string[] ThenIncludes
        {
            get
            {
                return new string[] { "Localizations.Language" };
            }
        }


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

        public override void CreateUpdateOrDeleteGraph(BaseObject entity)
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

        public override void DeleteDependencies(BaseObject entity)
        {
            if (entity.Image != null)
                ChangeEntityState(entity.Image, EntityState.Deleted);

            var localizationRepository = new LocalizationRepository(Context);

            foreach (var localization in entity.Localizations.ToList())
            {
                localizationRepository.DeleteDependencies(localization);

                ChangeEntityState(localization, EntityState.Deleted);
            }
        }


        public override void CreateDependencies(BaseObject entity)
        {
            new ImageRepository(Context).CreateDependencies(entity.Image);

            var localizationRepository = new LocalizationRepository(Context);

            foreach (var localization in entity.Localizations)
            {
                localizationRepository.CreateDependencies(localization);
            }
        }
    }
}
