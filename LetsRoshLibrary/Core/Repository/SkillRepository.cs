using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public class SkillRepository : Repository<Skill>
    {
        public SkillRepository(DbContext context) : base(context) { }

        public SkillRepository() { }

        public override void CreateDependencies(Skill entity)
        {
            new BaseObjectRepository(Context).CreateDependencies(entity);

            var characterRepository = new CharacterRepository(Context);

            if (!characterRepository.IsItNew(entity.Character))
            {
                var existingCharacter = characterRepository.GetExistingEntity(entity.Character);

                if (existingCharacter != null)
                    entity.Character = existingCharacter;
            }

        }

        public override void CreateUpdateOrDeleteGraph(Skill entity)
        {
            new BaseObjectRepository(Context).CreateUpdateOrDeleteGraph(entity);
        }

        public override void DeleteDependencies(Skill entity)
        {
            new BaseObjectRepository(Context).DeleteDependencies(entity);
        }

        public override Expression<Func<Skill, bool>> UniqueFilter(Skill entity, bool forEntityFramework = true)
        {
            return s => s.CharacterId == entity.CharacterId && s.Name == entity.Name;
        }
    }
}
