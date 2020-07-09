using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Services
{
    public class SkillService : Service<Skill>
    {
        public SkillService(bool enableProxyCreationForContext = true) : base(enableProxyCreationForContext) 
        {
        
        }

        public override void ConvertToPersistent(Skill disconnectedEntity, Skill persistent = null, Func<Skill> populatePersistent = null)
        {
            populatePersistent = () =>
            {
                using (var uow = new Dota2UnitofWork())
                {
                    var repository = new SkillRepository(uow.Context);

                    return repository.Select(repository.UniqueFilter(disconnectedEntity), repository.GetAllIncludes())
                    .Select(q => new
                    {
                        q.Id,
                        q.Name,
                        q.CharacterId,
                        ImageId = q.Image.Id
                    })
                    .ToList()
                    .Select(qs => new Skill()
                    {
                        Id = qs.Id,
                        Name = qs.Name,
                        CharacterId = qs.CharacterId,
                        ImageId = qs.ImageId
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

            new BaseObjectService().ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);
        }

        public override void SetRepository(Repository<Skill> repository = null)
        {
            base.SetRepository(new SkillRepository());
        }
    }
}
