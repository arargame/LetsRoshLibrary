using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public class ItemRepository : Repository<Item>
    {
        public override string[] GetIncludes()
        {
            return new BaseObjectRepository().GetIncludes();
        }

        public override string[] GetThenIncludes()
        {
            return new BaseObjectRepository().GetThenIncludes();
        }

        public ItemRepository(DbContext context) : base(context) { }

        public ItemRepository() { }

        public override void CreateDependencies(Item entity)
        {
            new BaseObjectRepository(Context).CreateDependencies(entity);
        }

        public override void CreateUpdateOrDeleteGraph(Item entity)
        {
            new BaseObjectRepository(Context).CreateUpdateOrDeleteGraph(entity);
        }

        public override void DeleteDependencies(Item entity)
        {
            new BaseObjectRepository(Context).DeleteDependencies(entity);
        }

        public override Expression<Func<Item, bool>> UniqueFilter(Item entity, bool forEntityFramework = true)
        {
            return i => i.LinkParameter == entity.LinkParameter;
        }
    }
}
