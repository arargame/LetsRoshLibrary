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
            return new CharacterRepository().GetIncludes();
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
        }

        public override void CreateUpdateOrDeleteGraph(Hero entity)
        {
            new CharacterRepository(Context).CreateUpdateOrDeleteGraph(entity);
        }

        public override void DeleteDependencies(Hero entity)
        {
            new CharacterRepository(Context).DeleteDependencies(entity);
        }

        public override Expression<Func<Hero, bool>> UniqueFilter(Hero entity, bool forEntityFramework = true)
        {
            return h => h.Name == entity.Name;
        }
    }
}
