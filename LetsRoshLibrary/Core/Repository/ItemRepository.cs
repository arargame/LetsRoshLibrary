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
        public ItemRepository(DbContext context) : base(context)
        {

        }

        public static string[] Includes
        {
            get
            {
                return BaseObjectRepository.Includes;
            }
        }

        public override void InsertDependencies(Item entity)
        {
            new BaseObjectRepository(Context).InsertDependencies(entity);
        }

        public override bool Insert(Item entity)
        {
            var isInserted = false;

            //InsertDependencies(entity);

            isInserted = base.Insert(entity);

            foreach (var ent in Context.ChangeTracker.Entries())
            {
                Console.WriteLine(ent.Entity.GetType().Name + " : " + ent.State);
            }

            return isInserted;
        }

        public override void DeleteDependencies(Item entity)
        {
            new BaseObjectRepository(Context).DeleteDependencies(entity);
        }

        //public override bool Delete(Item entity)
        //{
        //    var isDeleted = false;

        //    isDeleted = base.Delete(entity);

        //    return isDeleted;
        //}
    }
}
