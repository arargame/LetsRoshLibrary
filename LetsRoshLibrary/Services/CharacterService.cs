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
    public class CharacterService : Service<Character>
    {
        public CharacterService(bool enableProxyCreationForContext = true) : base(enableProxyCreationForContext)
        {

        }

        public override void ConvertToPersistent(Character disconnectedEntity, Character persistent = null, Func<Character> populatePersistent = null)
        {
            persistent = persistent ?? populatePersistent();

            if (persistent == null)
            {
                return;
            }

            new BaseObjectService().ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);

            persistent.Skills = new SkillService(false).Select(s => s.CharacterId == persistent.Id);

            if (disconnectedEntity.Skills.Any() && persistent.Skills.Any())
            {
                var skillService = new SkillService();

                foreach (var disconnectedEntitySkill in disconnectedEntity.Skills)
                {
                    disconnectedEntitySkill.Character = disconnectedEntity;

                    disconnectedEntitySkill.CharacterId = disconnectedEntity.Id;

                    skillService.ConvertToPersistent(disconnectedEntitySkill,
                        persistent.Skills.FirstOrDefault(skillService.Repository.UniqueFilter(disconnectedEntitySkill).Compile()));
                }
            }
        }

        public override void SetRepository(Repository<Character> repository = null)
        {
            base.SetRepository(new CharacterRepository());
        }
    }
}
