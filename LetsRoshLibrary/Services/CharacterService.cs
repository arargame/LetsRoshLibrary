using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Services
{
    public class CharacterService : Service<Character>
    {
        public CharacterService() { }

        public override void ConvertToPersistent(Character disconnectedEntity, object persistent = null, Func<object> populatePersistent = null)
        {
            base.ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);
        }

        public override void SetRepository(Repository<Character> repository = null)
        {
            base.SetRepository(new CharacterRepository());
        }
    }
}
