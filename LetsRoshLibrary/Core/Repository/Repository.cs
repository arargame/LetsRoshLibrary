using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseObject
    {
        protected readonly DbContext Context;
        protected readonly DbSet<T> DbSet;

        public Repository(DbContext context)
        {
            try
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                Context = context;
                DbSet = context.Set<T>();
            }
            catch (Exception ex)
            {
                Log.Save(new Log(string.Format("Class : {0}, Error : {1}", typeof(T).FullName, ex.ToString())));
            }

        }

        public void ShowChangeTrackerEntriesStates()
        {
            foreach (var e in Context.ChangeTracker.Entries())
            {
                Console.WriteLine("{0}:{1}", e.Entity.GetType().Name, e.State);
            }
        }

        public virtual string[] GetIncludes()
        {
            return null;
        }

        public T GetEntityFromContext(T entity)
        {
            return Context.ChangeTracker
                        .Entries()
                        .Select(entityEntry => (entityEntry.Entity as BaseObject))
                        .FirstOrDefault(e => e.Id == entity.Id) as T;
        }

        public void ChangeEntityState(T entity, EntityState entityState)
        {
            //if (GetEntityFromContext(entity) != null)
            //    throw new Exception(string.Format("The state of the entity with {0} type,{1} Id has already changed", entity.GetType().Name, entity.Id));

            Context.Entry(entity).State = entityState;
        }

        public EntityState GetEntityState(T entity)
        {
            return Context.Entry(entity).State;
        }

        public bool Any(Expression<Func<T,bool>> filter = null)
        {
            try
            {
                IQueryable<T> query = DbSet;

                return query.Any(filter);
            }
            catch (Exception ex)
            {
                Log.Save(new Log(string.Format("Class : {0}, Error : {1}", typeof(T).FullName, ex.ToString())));
            }

            return false;
        }

        public virtual T GetUnique(T entity,bool withIncludes = false)
        {
            throw new Exception("");
        }

        public T Get(Expression<Func<T, bool>> filter, params string[] includes)
        {
            try
            {
                IQueryable<T> query = DbSet;

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                return query.SingleOrDefault(filter);

            }
            catch (Exception ex)
            {
                Log.Save(new Log(string.Format("Class : {0}, Error : {1}", typeof(T).FullName, ex.ToString())));
            }

            return null;
        }

        public T Get(Guid entityId, params string[] includes)
        {
            try
            {
                IQueryable<T> query = DbSet;

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                return query.SingleOrDefault(t => t.Id == entityId);
            }
            catch (Exception ex)
            {
                Log.Save(new Log(description: string.Format("Class : {0}, Error : {1}", typeof(T).FullName, ex.ToString()), entityId: entityId.ToString()));
            }

            return DbSet.Find(entityId);
        }

        public IQueryable<T> All(params string[] includes)
        {
            IQueryable<T> query = DbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        //https://docs.microsoft.com/en-us/ef/core/querying/tracking
        public IQueryable<T> Select(Expression<Func<T, bool>> filter = null, params string[] includes)
        {
            try
            {
                IQueryable<T> query = DbSet;

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                return filter != null ? query.Where(filter).AsQueryable() : query.AsQueryable();
            }
            catch (Exception ex)
            {
                Log.Save(new Log(string.Format("Class : {0}, Error : {1}", typeof(T).FullName, ex.ToString())));
            }

            return DbSet;
        }

        public async Task<List<T>> SelectAsync(Expression<Func<T, bool>> filter = null, params string[] includes)
        {
            return await Task.Run(() =>
            {
                return Select(filter, includes).ToListAsync();
            });
        }

        public virtual void DeleteDependencies(T entity) { }

        public virtual void InsertDependencies(T entity) { }

        public virtual void UpdateDependencies(T entity) { }

        public virtual bool Create(T entity)
        {
            bool isInserted = false;

            entity.Id = Guid.NewGuid();

            entity.AddedDate = DateTime.Now;

            entity.ModifiedDate = DateTime.Now;

            entity.IsActive = true;

            try
            {
                InsertDependencies(entity);

                //DbSet.Add(entity);
                ChangeEntityState(entity,EntityState.Added);

                var state = Context.Entry(entity).State;

                isInserted = true;

                if (isInserted)
                {
                    var message = new
                    {
                        Count = Context.ChangeTracker.Entries().Count(),
                        Description = string.Join(",", Context.ChangeTracker.Entries().Select(e => string.Format("{0}:{1}", e.Entity.GetType().Name, e.State)))
                    };

                    if (entity.GetType().Name != "Log")
                        Log.Save(new Log(string.Format("In insertion process {0} entities was affected.Message : {1}", message.Count, message.Description), LogType.Info, entity.Id.ToString()));

                    Console.WriteLine(string.Format("In insertion process {0} entities was affected.Message : {1}", message.Count, message.Description));

                    ShowChangeTrackerEntriesStates();
                }
            }
            catch (Exception ex)
            {
                Log.Save(new Log(description: string.Format("Class : {0}, Error : {1}", entity.GetType().FullName, ex.ToString()), entityId: entity.Id.ToString()));
            }

            return isInserted;
        }


        public virtual void UpdateOrCreateNavigations(T existing, T local)
        {

        }

        public virtual bool Update(T entity,
            Expression<Func<T,bool>> filter,
            string[] includes,
            Action<T> actionToUpdateNavigations = null,
            bool checkAllProperties = false,
            params string[] modifiedProperties)
        {
            try
            {
                var existingEntity = Get(filter, includes.ToArray());

                if (existingEntity == null)
                    throw new Exception(string.Format("There is no such an entity with following informations ({0},{1})", existingEntity.GetType().Name, existingEntity.Id));

                var ee = Context.Entry(existingEntity);

                modifiedProperties = checkAllProperties ? modifiedProperties.Union(ee.OriginalValues.PropertyNames)
                                                        .Distinct()
                                                        .ToArray() : modifiedProperties;

                foreach (var property in modifiedProperties)
                {
                    if (new[] { "Id" }.Union(includes).Any(p => p == property))
                        continue;

                    typeof(T).GetProperty(property)
                        .SetValue(existingEntity, typeof(T).GetProperty(property).GetValue(entity, null));
                }

                if (actionToUpdateNavigations != null)
                    actionToUpdateNavigations.Invoke(existingEntity);

                if (modifiedProperties.Any() || actionToUpdateNavigations!=null)
                {
                    return Update(existingEntity);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Save(new Log(ex.Message));

                return false;
            }
        }

        public virtual bool Update(T entity)
        {
            bool isUpdated = false;

            entity.ModifiedDate = DateTime.Now;

            try
            {
                DbSet.Attach(entity);

                //Context.Entry(entity).State = EntityState.Modified;
                ChangeEntityState(entity,EntityState.Modified);

                isUpdated = true;

                ShowChangeTrackerEntriesStates();
            }
            catch (Exception ex)
            {
                Log.Save(new Log(string.Format("Class : {0}, Error : {1}", entity.GetType().FullName, ex.ToString()),
                                    LogType.Error,
                                    entity.Id.ToString()));
            }

            return isUpdated;
        }

        //Bütün neslerin bağımsız silinebilmesi için fonksiyonlar yap ve servise ekle
        public virtual bool Delete(T entity)
        {
            bool isDeleted = false;

            try
            {
                if (Context.Entry(entity).State == EntityState.Detached) //Concurrency için
                {
                    DbSet.Attach(entity);
                }
                
                DeleteDependencies(entity);
                //DbSet.Remove(entity);
                ChangeEntityState(entity,EntityState.Deleted);

                isDeleted = true;

                if (isDeleted)
                {
                    var message = new
                    {
                        Count = Context.ChangeTracker.Entries().Count(),
                        Description = string.Join(",", Context.ChangeTracker.Entries().Select(e => string.Format("{0}:{1}", e.Entity.GetType().Name, e.State)))
                    };

                    if (entity.GetType().Name != "Log")
                        Log.Save(new Log(string.Format("In deleting process {0} entities was affected.Message : {1}", message.Count, message.Description), LogType.Info, entity.Id.ToString()));

                    Console.WriteLine(string.Format("In deleting process {0} entities was affected.Message : {1}", message.Count, message.Description));

                    ShowChangeTrackerEntriesStates();
                }
            }
            catch (Exception ex)
            {
                Log.Save(new Log(description: string.Format("Class : {0}, Error : {1}", entity.GetType().BaseType.FullName, ex.ToString()), entityId: entity.Id.ToString()));
            }

            return isDeleted;
        }

        public bool Contains(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Count(predicate) > 0;
        }

        public int Count(Expression<Func<T, bool>> predicate = null)
        {
            return predicate != null ? DbSet.Count(predicate) : DbSet.Count();
        }

        public List<T> SqlQuery(string commandText, SqlParameterCollection commandParameters)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            for (int i = 0; i < commandParameters.Count; i++)
            {
                parameters.Add(new SqlParameter(commandParameters[i].ParameterName, commandParameters[i].Value));
            }

            return DbSet.SqlQuery(commandText, parameters.ToArray())
                        .ToList();
        }
    }
}
