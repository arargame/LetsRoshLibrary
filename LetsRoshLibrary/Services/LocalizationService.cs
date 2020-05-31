using HtmlAgilityPack;
using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Services
{
    public class LocalizationService : Service<Localization>
    {
        public LocalizationService()
        {

        }

        public override void ConvertToPersistent(Localization disconnectedEntity, object persistent = null, Func<object> populatePersistent = null)
        {
            populatePersistent = () =>
            {
                using (var uow = new Dota2UnitofWork())
                {
                    var repository = new LocalizationRepository(uow.Context);

                    return repository.Select(repository.UniqueFilter(disconnectedEntity), repository.GetAllIncludes())
                    .Select(q => new
                    {
                        q.Id,
                        q.BaseObjectId,
                        q.LanguageId,
                        q.PropertyName
                    })
                    .SingleOrDefault();
                }
            };

            new LanguageService().ConvertToPersistent(disconnectedEntity.Language);

            disconnectedEntity.LanguageId = disconnectedEntity.Language.Id;
            disconnectedEntity.BaseObjectId = disconnectedEntity.BaseObject.Id;

            base.ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);
        }

        public override void SetRepository(Repository<Localization> repository = null)
        {
            base.SetRepository(new LocalizationRepository());
        }
    }
}
