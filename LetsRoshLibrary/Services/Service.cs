using LetsRoshLibrary.Core.Context;
using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Services
{
    public abstract class Service<T> where T : BaseObject
    {
        public Service()
        {
            
        }

        public bool Create(T entity)
        {
            var isCommitted = false;

            using (var uow = new Dota2UnitofWork())
            {
                GetRepository(uow.Context).Create(entity);

                isCommitted = uow.Commit();
            }

            return isCommitted;
        }

        public bool Delete(T entity)
        {
            return Delete(i => i.Id == entity.Id);
        }

        public virtual bool Delete(Expression<Func<T, bool>> filter)
        {
            var isCommitted = false;

            Guid? entityId = null;

            try
            {
                using (var uow = new Dota2UnitofWork())
                {
                    var repository = GetRepository(uow.Context);

                    var existingEntity = repository.Get(filter, repository.GetAllIncludes());

                    entityId = existingEntity?.Id;

                    if (!repository.Delete(existingEntity))
                        throw new Exception(string.Format("{0} Delete Exception", repository.GetType().Name));

                    isCommitted = uow.Commit();
                }
            }
            catch (Exception ex)
            {
                Log.Save(new Log(ex.Message, entityId: entityId?.ToString()));
            }

            return isCommitted;
        }


        public T Get(Expression<Func<T, bool>> filter, params string[] includes)
        {
            T entity = null;

            using (var uow = new Dota2UnitofWork())
            {
                entity = uow.Load<T>().Get(filter, includes);
            }

            return entity;
        }


        public virtual Repository<T> GetRepository(DbContext context)
        {
            return new Repository<T>(context);
        }

        public List<T> Select(Expression<Func<T, bool>> filter = null, params string[] includes)
        {
            List<T> results = new List<T>();

            using (var uow = new Dota2UnitofWork())
            {
                results = GetRepository(uow.Context).Select(filter, includes).ToList();
            }

            return results;
        }


        public bool Update(T entity)
        {
            var isCommitted = false;

            using (var uow = new Dota2UnitofWork())
            {
                GetRepository(uow.Context).Update(entity);

                isCommitted = uow.Commit();
            }

            return isCommitted;
        }
    }
}
