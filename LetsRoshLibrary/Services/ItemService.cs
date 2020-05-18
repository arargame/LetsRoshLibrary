using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Services
{
    public class ItemService : Service<Item>
    {
        public ItemService() { }

        public override Repository<Item> GetRepository(DbContext context)
        {
            return new ItemRepository(context);
        }
    }
}
