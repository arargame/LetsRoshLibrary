using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public class BaseObjectRepository : Repository<BaseObject>
    {
        public BaseObjectRepository(DbContext context) : base(context)
        {

        }

        public override bool Insert(BaseObject entity)
        {
            var isInserted = false;

            foreach (var localization in entity.Localizations)
            {
                localization.Language = new Repository<Language>(Context).Get(l => l.Name == localization.Language.Name);
            }

            return isInserted;
        }

        public override bool Delete(BaseObject entity)
        {
            var isDeleted = false;

            if (entity.Image != null)
                isDeleted = new Repository<Image>(Context).Delete(entity.Image);

            return isDeleted;
        }
    }
}
