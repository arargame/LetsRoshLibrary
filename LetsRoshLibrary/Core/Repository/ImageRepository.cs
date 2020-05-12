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
    public class ImageRepository : Repository<Image>
    {
        public ImageRepository(DbContext context) : base(context)
        {

        }

        public override Expression<Func<Image, bool>> UniqueFilter(Image entity)
        {
            return i => i.Name == entity.Name && i.Path == entity.Path && i.Data == entity.Data;
        }

        public override void InsertDependencies(Image entity)
        {
            ChangeEntityState(entity, EntityState.Added);
        }

        public override bool Create(Image entity)
        {
            return base.Create(entity);
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

        public override string[] GetIncludes()
        {
            return BaseObjectRepository.Includes;
        }
    }
}
