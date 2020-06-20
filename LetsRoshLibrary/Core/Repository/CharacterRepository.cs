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
    public class CharacterRepository : Repository<Character>
    {
        public override string[] GetIncludes()
        {
            return new BaseObjectRepository().GetIncludes().Union(new[] { "Skills" }).ToArray();
        }

        public override string[] GetThenIncludes()
        {
            return new BaseObjectRepository().GetThenIncludes().Union(new[] { "Skills.Image", "Skills.Localizations", "Skills.Localizations.Language" }).ToArray();
        }

        public CharacterRepository(DbContext context) : base(context) { }

        public CharacterRepository() { }

        public override void CreateDependencies(Character entity)
        {
            new BaseObjectRepository(Context).CreateDependencies(entity);

            var skillRepository = new SkillRepository(Context);

            foreach (var skill in entity.Skills)
            {
                skillRepository.CreateDependencies(skill);
            }
        }

        public override void CreateUpdateOrDeleteGraph(Character entity)
        {
            var existingEntity = GetEntityFromContext(entity);

            new BaseObjectRepository(Context).CreateUpdateOrDeleteGraph(entity);

            var skillRepository = new SkillRepository(Context);

            foreach (var skill in entity.Skills)
            {
                if (skillRepository.IsItNew(skill))
                {
                    skillRepository.Create(skill);
                }
                else
                {
                    skillRepository.Update(skill);
                }
            }

            var existingEntitySkills = existingEntity.Skills.ToList();

            for (int i = 0; i < existingEntitySkills.Count; i++)
            {
                var skill = existingEntitySkills.ToList()[i];

                if (!entity.Skills.Any(s => s.Id == skill.Id))
                    skillRepository.Delete(skill);
            }
        }

        public override void DeleteDependencies(Character entity)
        {
            new BaseObjectRepository(Context).DeleteDependencies(entity);

            var skillRepository = new SkillRepository(Context);

            foreach (var skill in entity.Skills.ToList())
            {
                skillRepository.Delete(skill);
            }
        }

        public override Expression<Func<Character, bool>> UniqueFilter(Character entity, bool forEntityFramework = true)
        {
            return h => h.Name == entity.Name;
        }
    }
}
