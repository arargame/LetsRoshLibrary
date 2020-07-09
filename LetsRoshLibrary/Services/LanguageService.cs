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
        public LanguageService(bool enableProxyCreationForContext = true) : base(enableProxyCreationForContext)
        {

        }

        public override void ConvertToPersistent(Language disconnectedEntity, Language persistent = null, Func<Language> populatePersistent = null)
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
                    .ToList()
                    .Select(ql => new Language()
                    {
                        Id = ql.Id,
                        Name = ql.Name
                    })
                    .SingleOrDefault();
                }
            };

            persistent = persistent ?? populatePersistent();

            if (persistent == null)
            {
                return;
            }

            base.ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);
        }

        public override void SetRepository(Repository<Language> repository = null)
        {
            base.SetRepository(new LanguageRepository());
        }
    }
}
