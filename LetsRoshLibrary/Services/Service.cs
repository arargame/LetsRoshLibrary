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
        public Repository<T> Repository { get; set; }

        public Service(bool enableProxyCreationForContext = true) 
        {
            SetRepository();

            Repository.EnableProxyCreationForContext(enableProxyCreationForContext);
        }

        public bool Any(Expression<Func<T, bool>> filter = null)
        {
            using (var uow = new Dota2UnitofWork())
            {
                return Repository.SetContext(uow.Context).Any(filter);
            }
        }

        //public virtual BaseObject AnonymousTypeToT(BaseObject baseObject)
        //{
        //    return null;
        //}

        public virtual T AnonymousTypeToT(object anonymous)
        {
            T t = null;

            try
            {
                t = (T)Activator.CreateInstance(typeof(T));

                var anonymousObjectsProperties = anonymous.GetType().GetProperties();

                var tsProperties = typeof(T).GetProperties();

                foreach (var property in anonymousObjectsProperties)
                {
                    if (tsProperties.Any(tp => tp.Name == property.Name))
                    {
                        var tProperty = tsProperties.FirstOrDefault(tp => tp.Name == property.Name && tp.CanWrite);

                        if (tProperty == null)
                            continue;

                        if (tProperty.Name == property.Name && tProperty.PropertyType.Name == property.PropertyType.Name)
                            tProperty.SetValue(t, property.GetValue(anonymous, null));
                        else
                        {
                            Console.WriteLine(property.Name);
                        }

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
                Repository.SetContext(uow.Context).Create(entity);

                isCommitted = uow.Commit();
            }

            return isCommitted;
        }

        public bool CreateOrUpdate(T entity)
        {
            var isCommitted = false;

            using (var uow = new Dota2UnitofWork())
            {
                var repository = Repository.SetContext(uow.Context);

                if (!(repository.IsItNew(entity) ? repository.Create(entity) : repository.Update(entity)))
                    return false;

                isCommitted = uow.Commit();
            }

            return isCommitted;
        }

        //public Expression<Func<T,bool>> Predicate(T entity)
        //{
        //    if (typeof(T).Name == typeof(Item).Name)
        //    {
        //        //return i => (i as Item).LinkParameter == (entity as Item).LinkParameter;
                
        //        var func = new ItemRepository().UniqueFilter(entity as Item).Compile();

        //        var func2 = new Func<T, bool>((T t)=> 
        //        {
        //            return func(t as Item);
        //        });

        //        Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(Expression.Call(func2.Method));

        //        return null;
        //    }

        //    return null;
        //}

        public virtual void ConvertToPersistent(T disconnectedEntity, T persistent = null, Func<T> populatePersistent = null)
        {
            persistent = persistent ?? populatePersistent();

            if (persistent == null)
            {
                return;
            }

            //var connectedEntity = AnonymousTypeToT(persistent);

            var connectedEntity = persistent;

            var isExist = new[] { disconnectedEntity }.Any(Repository.UniqueFilter(connectedEntity, false).Compile());

            if (isExist)
                ReplaceIds(disconnectedEntity, connectedEntity);
            else
                Console.WriteLine("It is not exist : Type : {0}, DisconnectedEntity : {1}, Persistent : {2}", typeof(T).Name, disconnectedEntity.Id, persistent.Id);
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
                    var repository = Repository.SetContext(uow.Context);

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

        public List<T> Find(Guid id, params string[] includes)
        {
            return Select(t => t.Id == id, includes);
        }

        public T Get(Expression<Func<T, bool>> filter, params string[] includes)
        {
            T entity = null;

            using (var uow = new Dota2UnitofWork())
            {
                entity = uow.Load<T>().Get(filter, includes);

                uow.Load<T>().ShowChangeTrackerEntriesStates();

                uow.Commit();
            }

            return entity;
        }

        //public virtual Repository<T> GetRepository()
        //{
        //    return new Repository<T>();
        //}

        //public virtual Repository<T> GetRepository(DbContext context)
        //{
        //    return new Repository<T>(context);
        //}

        public void ReplaceIds(T localEntity,T existingEntity)
        {
            localEntity.Id = existingEntity.Id;
        }

        public List<T> Select(Expression<Func<T, bool>> filter = null, params string[] includes)
        {
            List<T> results = new List<T>();

            using (var uow = new Dota2UnitofWork())
            {
                uow.Context.Configuration.ProxyCreationEnabled = Repository.ProxyCreationEnabled;

                results = Repository.SetContext(uow.Context)
                                    .Select(filter, includes)
                                    .ToList();
            }

            return results;
        }

        public virtual void SetRepository(Repository<T> repository = null)
        {
            Repository = repository ?? new Repository<T>();
        }

        public bool Update(T entity)
        {
            var isCommitted = false;

            using (var uow = new Dota2UnitofWork())
            {
                if (!Repository.SetContext(uow.Context).Update(entity))
                    return false;

                isCommitted = uow.Commit();
            }

            return isCommitted;
        }
    }
}
