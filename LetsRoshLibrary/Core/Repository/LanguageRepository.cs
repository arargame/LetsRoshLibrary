using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public class LanguageRepository : Repository<Language>
    {
        public LanguageRepository(DbContext context) : base(context)
        {

        }

        public override Language GetUnique(Language entity, bool withIncludes = false)
        {
            return Get(l => l.Name == entity.Name);
        }


        //public override void InsertDependencies(Language entity)
        //{

        //}
        //public override bool Insert(Language entity)
        //{
        //    var isInserted = false;

        //    if (Any(l => l.Name == entity.Name))
        //        entity = new Repository<Language>(Context).Get(l => l.Name == entity.Name);
        //    else
        //        ChangeEntityState(entity, EntityState.Added);

        //    return isInserted;
        //}
    }
}
