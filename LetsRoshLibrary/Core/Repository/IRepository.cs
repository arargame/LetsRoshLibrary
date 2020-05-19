using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public interface IRepository<T> where T : class
    {
        T Get(Guid entityId, params string[] includes);
        IQueryable<T> All(params string[] includes);
        //bool Any(Expression<Func<T, bool>> filter = null);
        //IQueryable<T> Select(Expression<Func<T, bool>> filter = null, params string[] includes);
        bool Create(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        bool Contains(Expression<Func<T, bool>> predicate);
        int Count(Expression<Func<T, bool>> predicate = null);
    }
}
