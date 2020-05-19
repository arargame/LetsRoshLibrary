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

        public ImageRepository() { }

        //LINQ to Entities does not recognize the method 'Boolean SequenceEqual[Byte](System.Collections.Generic.IEnumerable`1[System.Byte], System.Collections.Generic.IEnumerable`1[System.Byte])' method, and this method cannot be translated into a store expression.
        //public override bool Any(Expression<Func<Image, bool>> filter = null,Image image = null)
        //{
        //    try
        //    {
        //        IQueryable<Image> query = DbSet;

        //        return query.Where(filter)
        //                    .ToList()
        //                    .Any(i => i.Data.SequenceEqual(image.Data));

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Save(new Log(string.Format("Class : {0}, Error : {1}", typeof(Image).FullName, ex.ToString())));
        //    }

        //    return false;
        //}

        public override Expression<Func<Image, bool>> UniqueFilter(Image entity,bool forEntityFramework = true)
        {
            if(forEntityFramework)
                return i => i.Name == entity.Name && i.Path == entity.Path && i.Data == entity.Data;
            else
                return i => i.Name == entity.Name && i.Path == entity.Path && i.Data.SequenceEqual(entity.Data);
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
