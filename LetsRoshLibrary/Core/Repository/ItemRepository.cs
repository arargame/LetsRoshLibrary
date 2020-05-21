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
        public static string[] AllIncludes
        {
            get
            {
                return Includes.Union(ThenIncludes).ToArray();
            }
        }

        public override string[] GetIncludes()
        {
            return Includes;
        }

        public override string[] GetThenIncludes()
        {
            return ThenIncludes;
        }

        public static string[] Includes
        {
            get
            {
                return BaseObjectRepository.Includes;
            }
        }
        public static string[] ThenIncludes
        {
            get
            {
                return BaseObjectRepository.ThenIncludes;
            }
        }

        public ItemRepository(DbContext context) : base(context)
        {

        }

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
