using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public class ItemRepository : Repository<Item>
    {
        public ItemRepository(DbContext context) : base(context)
        {

        }

        public override bool Insert(Item entity)
        {
            var isInserted = false;

            new BaseObjectRepository(Context).Insert(entity);

            isInserted =  base.Insert(entity);

            return isInserted;
        }

        public override bool Delete(Item entity)
        {
            var isDeleted = false;

            new BaseObjectRepository(Context).Delete(entity);

            isDeleted = base.Delete(entity);

            return isDeleted;
        }
    }
}
