using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public class ImageRepository : Repository<Image>
    {
        public ImageRepository(DbContext context) : base(context)
        {

        }

        public override void InsertDependencies(Image entity)
        {
            ChangeEntityState(entity, EntityState.Added);
        }

        public override bool Insert(Image entity)
        {
            return base.Insert(entity);
        }

        public override void DeleteDependencies(Image entity)
        {
            ChangeEntityState(entity,EntityState.Deleted);
        }

        //public override bool Delete(Image entity)
        //{
        //    var isDeleted = false;

        //    isDeleted = new Repository<Image>(Context).Delete(entity);

        //    return isDeleted;
        //}
    }
}
