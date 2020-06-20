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
    public class LanguageRepository : Repository<Language>
    {
        public LanguageRepository(DbContext context) : base(context) { }

        public LanguageRepository() { }

        public override Expression<Func<Language, bool>> UniqueFilter(Language entity,bool forEntityFramework = true)
        {
            return l => l.Name == entity.Name;
        }
    }
}
