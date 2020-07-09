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
        public ImageService(bool enableProxyCreationForContext = true) : base(enableProxyCreationForContext)
        {

        }

        public override void ConvertToPersistent(Image disconnectedEntity, Image persistent = null, Func<Image> populatePersistent = null)
        {
            populatePersistent = () =>
            {
                using (var uow = new Dota2UnitofWork())
                {
                    var repository = new ImageRepository(uow.Context);

                    return repository.Select(repository.UniqueFilter(disconnectedEntity), repository.GetAllIncludes())
                    .Select(q => new
                    {
                        q.Id,
                        q.Name,
                        q.Path,
                        q.Data
                    })
                    .ToList()
                    .Select(qi => new Image()
                    {
                        Id = qi.Id,
                        Name = qi.Name,
                        Path = qi.Path,
                        Data = qi.Data
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

        public override void SetRepository(Repository<Image> repository = null)
        {
            base.SetRepository(new ImageRepository());
        }
    }
}
