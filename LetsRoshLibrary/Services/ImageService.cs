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
    public class ImageService : Service<Image>
    {
        public override void ConvertToPersistent(Image disconnectedEntity, object persistent = null, Func<object> populatePersistent = null)
        {
            populatePersistent = () =>
            {
                using (var uow = new Dota2UnitofWork())
                {
                    var repository = GetRepository(uow.Context);

                    return repository.Select(repository.UniqueFilter(disconnectedEntity), repository.GetAllIncludes())
                    .Select(q => new
                    {
                        q.Id,
                        q.Name,
                        q.Path,
                        q.Data
                    })
                    .SingleOrDefault();
                }
            };

            base.ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);
        }
    }
}
