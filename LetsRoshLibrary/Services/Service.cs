using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;


namespace LetsRoshLibrary.Services
{
    public abstract class Service<T> where T : BaseObject
    {
        public Service()
        {
            
        }

        public bool Any(Expression<Func<T, bool>> filter = null)
        {
            using (var uow = new Dota2UnitofWork())
            {
                return GetRepository(uow.Context).Any(filter);
            }
        }

        public T AnonymousTypeToT(object anonymous)
        {
            T t = null;

            try
            {
                t = (T)Activator.CreateInstance(typeof(T));

                var anonymousObjectsProperties = anonymous.GetType().GetProperties();

                var tsProperties = typeof(T).GetProperties();

                foreach (var property in anonymousObjectsProperties)
                {
                    if (tsProperties.Any(tp=>tp.Name == property.Name))
                    {
                        var tProperty = tsProperties.FirstOrDefault(tp => tp.Name == property.Name && tp.CanWrite);

                        if (tProperty == null)
                            continue;

                        if (tProperty.Name == property.Name && tProperty.PropertyType.Name == property.PropertyType.Name)
                            t.GetType().GetProperty(property.Name).SetValue(t, property.GetValue(anonymous, null));

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Save(new Log(ex.Message));
            }

            return t;
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

        public virtual void ConvertToPersistent(T disconnectedEntity, object persistent = null, Func<object> populatePersistent = null)
        {
            persistent = persistent ?? populatePersistent();

            if (persistent == null)
            {
                return;
            }

            var connectedEntity = AnonymousTypeToT(persistent);

            var isExist = new[] { disconnectedEntity }.Any(new Repository<T>().UniqueFilter(connectedEntity, false).Compile());

            if (isExist)
                ReplaceIds(disconnectedEntity, connectedEntity);
            else
                disconnectedEntity.ChangeEntityState(System.Data.Entity.EntityState.Added);
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

        public void ReplaceIds(T localEntity,T existingEntity)
        {
            localEntity.Id = existingEntity.Id;

            localEntity.ChangeEntityState(EntityState.Unchanged);
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
                if (!GetRepository(uow.Context).Update(entity))
                    return false;

                isCommitted = uow.Commit();
            }

            return isCommitted;
        }
    }
}
