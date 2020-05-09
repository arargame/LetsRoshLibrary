using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public class LocalizationRepository : Repository<Localization>
    {
        public LocalizationRepository(DbContext dbContext) : base(dbContext)
        {

        }

        public override void InsertDependencies(Localization entity)
        {
            var languageRepository = new LanguageRepository(Context);

            if (languageRepository.Any(l => l.Name == entity.Language.Name))
            {
                var languageEntityFromContext = languageRepository.GetEntityFromContext(entity.Language);

                if (languageEntityFromContext == null)
                    languageRepository.ChangeEntityState(entity.Language, EntityState.Unchanged);
                else
                    entity.Language = languageEntityFromContext;
            }
            else
                languageRepository.ChangeEntityState(entity.Language, EntityState.Added);
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
    }
}
