using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public class UniqueFilter<T> where T : BaseObject
    {
        public UniqueFilter() { }

        public Expression<Func<T, bool>> Get(T entity, bool forEntityFramework = true)
        {
            var type = typeof(T);

            if (type.Name == typeof(Item).Name)
            {
                var function = new Func<Item, bool>(i => i.LinkParameter == (entity as Item).LinkParameter);

                Expression<Func<Item, bool>> expression = Expression.Lambda<Func<Item, bool>>(Expression.Call(function.Method));

                return expression as Expression<Func<T, bool>>;
            }

            return o => o.Id == entity.Id;
        }
    }
}
