using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public class LocalizationRepository : Repository<Localization>
    {
        public LocalizationRepository(DbContext dbContext) : base(dbContext)
        {

        }

        public override void ConvertToPersistent(Localization entity)
        {
            base.ConvertToPersistent(entity);

            new LanguageRepository(Context).ConvertToPersistent(entity.Language);
        }

        public static string[] Includes
        {
            get
            {
                return BaseObjectRepository.Includes.Append("Language").ToArray();
            }
        }

        public override string[] GetIncludes()
        {
            return Includes;
        }

        public override void InsertDependencies(Localization entity)
        {
            var languageRepository = new LanguageRepository(Context);

            if (!languageRepository.IsItNew(entity.Language))
            {
                var languageEntityFromContext = languageRepository.GetEntityFromContext(entity.Language);

                if (languageEntityFromContext == null)
                    languageRepository.ChangeEntityState(entity.Language, EntityState.Unchanged);
                else
                    entity.Language = languageEntityFromContext;
            }
            else
                languageRepository.Create(entity.Language);
                //languageRepository.ChangeEntityState(entity.Language, EntityState.Added);
        }

        //public override bool Insert(Localization entity)
        //{
        //    new LanguageRepository(Context).Insert(entity.Language);

        //     base.Insert(entity);

        //    return true;
        //}

        public override void DeleteDependencies(Localization entity)
        {
            new LanguageRepository(Context).ChangeEntityState(entity.Language,EntityState.Unchanged);
        }

        public override Expression<Func<Localization, bool>> UniqueFilter(Localization entity)
        {
            return l => l.BaseObject.Id == entity.BaseObject.Id && l.Language.Id == entity.Language.Id && l.PropertyName == entity.PropertyName;
        }

        public override void InsertUpdateOrDeleteGraph(Localization entity)
        {
            var languageRepository = new LanguageRepository(Context);

            var existingLanguage = languageRepository.GetUnique(entity.Language);

            if (existingLanguage == null)
            {
                languageRepository.Create(entity.Language);
            }
            else
            {
                languageRepository.Update(entity.Language);
            }
        }

    }
}
