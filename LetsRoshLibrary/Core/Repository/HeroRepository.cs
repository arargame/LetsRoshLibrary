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
    public class HeroRepository : Repository<Hero>
    {
        public override string[] GetIncludes()
        {
            return new CharacterRepository().GetIncludes().Union(new[] { "Portrait"}).ToArray();
        }

        public override string[] GetThenIncludes()
        {
            return new CharacterRepository().GetThenIncludes();
        }

        public HeroRepository(DbContext context) : base(context) { }

        public HeroRepository() { }

        public override void CreateDependencies(Hero entity)
        {
            new CharacterRepository(Context).CreateDependencies(entity);

            new ImageRepository(Context).CreateDependencies(entity.Portrait);
        }

        public override void CreateUpdateOrDeleteGraph(Hero entity)
        {
            new CharacterRepository(Context).CreateUpdateOrDeleteGraph(entity);

            var existingEntity = GetEntityFromContext(entity);

            var imageRepository = new ImageRepository(Context);

            if (existingEntity.Portrait != null)
            {
                if (entity.Portrait == null)
                    imageRepository.Delete(existingEntity.Portrait);
                else if (new[] { existingEntity.Portrait }.Any(imageRepository.UniqueFilter(entity.Portrait, false).Compile()))
                    imageRepository.Update(entity.Portrait);
                else
                    imageRepository.Create(entity.Portrait);
            }
            else if (entity.Portrait != null)
            {
                imageRepository.Create(entity.Portrait);
            }

            if (entity.Portrait != null)
            {
                entity.PortraitId = entity.Portrait.Id;

                if (imageRepository.GetEntityState(entity.Portrait) == EntityState.Added)
                    existingEntity.Portrait = entity.Portrait;
            }
        }

        public override void DeleteDependencies(Hero entity)
        {
            new CharacterRepository(Context).DeleteDependencies(entity);

            if (entity.Portrait != null)
                new ImageRepository(Context).Delete(entity.Portrait);
        }

        public override Expression<Func<Hero, bool>> UniqueFilter(Hero entity, bool forEntityFramework = true)
        {
            return h => h.Name == entity.Name;
        }
    }
}
