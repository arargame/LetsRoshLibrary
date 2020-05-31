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
    public class LanguageService : Service<Language>
    {
        public LanguageService() { }

        public override void ConvertToPersistent(Language disconnectedEntity, object persistent = null, Func<object> populatePersistent = null)
        {
            populatePersistent = () =>
            {
                using (var uow = new Dota2UnitofWork())
                {
                    var repository = new LanguageRepository(uow.Context);

                    return repository.Select(repository.UniqueFilter(disconnectedEntity), repository.GetAllIncludes())
                    .Select(q => new
                    {
                        q.Id,
                        q.Name
                    })
                    .SingleOrDefault();
                }
            };

            base.ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);
        }

        public override void SetRepository(Repository<Language> repository = null)
        {
            base.SetRepository(new LanguageRepository());
        }
    }
}
