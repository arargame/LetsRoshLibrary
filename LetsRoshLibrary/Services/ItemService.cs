using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Services
{
    public class ItemService : Service<Item>
    {
        public ItemService() { }

        public override void ConvertToPersistent(Item disconnectedEntity, object persistent = null, Func<object> populatePersistent = null)
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
                        q.LinkParameter,
                        Image = new
                        {
                            q.Image.Id,
                            q.Image.Name,
                            q.Image.Path,
                            q.Image.Data
                        },
                        Localizations = q.Localizations.Select(l => new
                        {
                            l.Id,
                            l.BaseObjectId,
                            l.LanguageId,
                            l.PropertyName
                        })
                    })
                    .SingleOrDefault();
                }
            };

            new BaseObjectService().ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);
        }
        public override Repository<Item> GetRepository(DbContext context)
        {
            return new ItemRepository(context);
        }
    }
}
