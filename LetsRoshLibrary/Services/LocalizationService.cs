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
        public LocalizationService(bool enableProxyCreationForContext = true) : base(enableProxyCreationForContext)
        {

        }

        public override void ConvertToPersistent(Localization disconnectedEntity, Localization persistent = null, Func<Localization> populatePersistent = null)
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
                    .ToList()
                    .Select(l => new Localization()
                    {
                        Id = l.Id,
                        BaseObjectId = l.BaseObjectId,
                        LanguageId = l.LanguageId,
                        PropertyName = l.PropertyName
                    })
                    .SingleOrDefault();
                }
            };

            persistent = persistent ?? populatePersistent();

            if (persistent == null)
            {
                return;
            }

            persistent.Language = new LanguageService(false).Get(l => l.Id == persistent.LanguageId);

            if (disconnectedEntity.Language != null && persistent.Language != null)
            {
                new LanguageService().ConvertToPersistent(disconnectedEntity.Language, persistent.Language);

                disconnectedEntity.LanguageId = disconnectedEntity.Language.Id;
            }

            base.ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);
        }

        public override void SetRepository(Repository<Localization> repository = null)
        {
            base.SetRepository(new LocalizationRepository());
        }
    }
}
