using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Services
{
    public class SkillService : Service<Skill>
    {
        public SkillService() { }

        public override void ConvertToPersistent(Skill disconnectedEntity, object persistent = null, Func<object> populatePersistent = null)
        {
            base.ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);
        }

        public override void SetRepository(Repository<Skill> repository = null)
        {
            base.SetRepository(new SkillRepository());
        }
    }
}
