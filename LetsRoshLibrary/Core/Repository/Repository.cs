using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseObject
    {
        protected DbContext Context;

        protected DbSet<T> DbSet;
        public bool ProxyCreationEnabled { get; set; }

        public Repository(DbContext context)
        {
            SetContext(context);
        }

        public Repository() { }

        public IQueryable<T> All(params string[] includes)
        {
            IQueryable<T> query = DbSet;

            if (includes != null && includes.Any())
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

            return query;
        }

        public virtual bool Any(Expression<Func<T, bool>> filter = null)
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

        public void ChangeEntityState(T entity, EntityState entityState)
        {
            //if (GetEntityFromContext(entity) != null)
            //    throw new Exception(string.Format("The state of the entity with {0} type,{1} Id has already changed", entity.GetType().Name, entity.Id));

            GetAsDbEntityEntry(entity).State = entityState;
        }

        public virtual void ConvertToPersistent(T entity)
        {
            var persistentObject = Get(UniqueFilter(entity));

            if (persistentObject != null)
                entity.Id = persistentObject.Id;
        }

        public virtual bool Create(T entity)
        {
            bool isInserted = false;

            entity.Id = Guid.NewGuid();

            entity.AddedDate = DateTime.Now;

            entity.ModifiedDate = DateTime.Now;

            entity.IsActive = true;

            try
            {
                if (GetUnique(entity, true) != null)
                    return false;

                CreateDependencies(entity);

                //DbSet.Add(entity);
                ChangeEntityState(entity, EntityState.Added);

                var state = Context.Entry(entity).State;

                isInserted = true;

                if (isInserted)
                {
                    var message = new
                    {
                        Count = GetContextChangeTrackerEntries().Count(),
                        Description = string.Join(",", GetContextChangeTrackerEntries().Select(e => string.Format("{0}:{1}", e.Entity.GetType().Name, e.State)))
                    };

                    //if (entity.GetType().Name != "Log")
                    //    Log.Save(new Log(string.Format("In {0} insertion process {1} entities was affected.Message : {2}", typeof(T).Name,message.Count, message.Description), LogType.Info, entity.Id.ToString()));

                    Console.WriteLine(string.Format("In {0} insertion process {1} entities was affected.Message : {2}", typeof(T).Name, message.Count, message.Description));

                    ShowChangeTrackerEntriesStates();
                }
            }
            catch (Exception ex)
            {
                Log.Save(new Log(description: string.Format("Class : {0}, Error : {1}", entity.GetType().FullName, ex.ToString()), entityId: entity.Id.ToString()));
            }

            return isInserted;
        }

        public virtual void CreateDependencies(T entity) 
        {
            ChangeEntityState(entity, EntityState.Added);
        }

        public virtual void CreateUpdateOrDeleteGraph(T entity) { }

        public bool Contains(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Count(predicate) > 0;
        }

        public int Count(Expression<Func<T, bool>> predicate = null)
        {
            return predicate != null ? DbSet.Count(predicate) : DbSet.Count();
        }

        public virtual bool Delete(T entity)
        {
            bool isDeleted = false;

            //IsNull(entity);

            try
            {
                if (Context.Entry(entity).State == EntityState.Detached) //Concurrency için
                {
                    DbSet.Attach(entity);
                }

                DeleteDependencies(entity);
                //DbSet.Remove(entity);
                ChangeEntityState(entity, EntityState.Deleted);

                isDeleted = true;

                if (isDeleted)
                {
                    var message = new
                    {
                        Count = GetContextChangeTrackerEntries().Count(),
                        Description = string.Join(",\n", GetContextChangeTrackerEntries().Select(e => string.Format("{0} : {1}", e.Entity.GetType().Name, e.State)))
                    };

                    if (entity.GetType().Name != "Log")
                        Log.Save(new Log(string.Format("\nIn deleting process {0} entities was affected.Message : {1} \n", message.Count, message.Description), LogType.Info, entity.Id.ToString()));

                    Console.WriteLine(string.Format("\nIn deleting process {0} entities was affected.Message : {1} \n", message.Count, message.Description));

                    ShowChangeTrackerEntriesStates();
                }
            }
            catch (Exception ex)
            {
                Log.Save(new Log(description: string.Format("Class : {0}, Error : {1}", entity.GetType().BaseType.FullName, ex.ToString()), entityId: entity.Id.ToString()));
            }

            return isDeleted;
        }


        public virtual void DeleteDependencies(T entity) 
        {
            ChangeEntityState(entity, EntityState.Deleted);
        }

        public void EnableProxyCreationForContext(bool enable = true)
        {
            ProxyCreationEnabled = enable;
        }

        public T Get(Expression<Func<T, bool>> filter, params string[] includes)
        {
            try
            {
                IQueryable<T> query = DbSet;

                if (includes != null && includes.Any())
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

                if (includes != null && includes.Any())
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

        public virtual string[] GetIncludes()
        {
            return new[] { "" };
        }

        public virtual string[] GetThenIncludes()
        {
            return new[] { "" };
        }

        public string[] GetAllIncludes()
        {
            var includes = GetIncludes();

            var thenIncludes = GetThenIncludes();

            return includes.Union(thenIncludes)
                            .Where(i => !string.IsNullOrWhiteSpace(i))
                            .ToArray();
        }

        public DbEntityEntry GetAsDbEntityEntry(T entity)
        {
            return Context.Entry(entity);
        }

        public T GetEntityFromContext(T entity)
        {
            return GetContextChangeTrackerEntries()
                        .Select(entityEntry => entityEntry.Entity as BaseObject)
                        .FirstOrDefault(e => e.Id == entity.Id) as T;
        }

        public EntityState GetEntityState(T entity)
        {
            return GetAsDbEntityEntry(entity).State;
        }

        public T GetExistingEntity(T entity,bool throwException = true,bool withAllIncludes = true)
        {
            T t = GetEntityFromContext(entity) ?? GetUnique(entity, withAllIncludes);

            if (t == null && throwException)
                throw new Exception("There is no such an entity in either Db or Context");

            return t;
        }

        public IEnumerable<DbEntityEntry> GetContextChangeTrackerEntries()
        {
            return Context.ChangeTracker.Entries();
        }

        public IEnumerable<string> GetModifiedProperties(T entity)
        {
            var ee = GetAsDbEntityEntry(entity);

            var ByteArrayEquality = new Func<byte[], byte[], bool>(
                (first, second) =>
                {
                    if (first == null || second == null)
                        return false;

                    int i;

                    if (first.Length == second.Length)
                    {
                        i = 0;

                        while (i < first.Length && (first[i] == second[i]))
                        {
                            i++;
                        }

                        if (i == first.Length)
                        {
                            return true;
                        }
                    }

                    return false;
                });

            var byteArrayProperties = entity.GetType().GetProperties().Where(p => p.PropertyType.Name == "Byte[]");

            foreach (var propertyName in ee.CurrentValues.PropertyNames)
            {
                if (new[] { "ModifiedDate","AddedDate", "Timestamp" }.Any(p => p == propertyName))
                    continue;

                if (byteArrayProperties.Any(p => p.Name == propertyName))
                {
                    if (!ByteArrayEquality((byte[])ee.GetDatabaseValues()[propertyName], (byte[])ee.CurrentValues[propertyName]))
                        yield return propertyName;
                }
                else if (ee.GetDatabaseValues()[propertyName]?.ToString() != ee.CurrentValues[propertyName]?.ToString())
                {
                    yield return propertyName;
                }
            }
        }

        public virtual T GetUnique(T entity, bool withAllIncludes = false)
        {
            return Get(UniqueFilter(entity), withAllIncludes ? GetAllIncludes() : null);
        }

        public bool HasAnyModifiedProperty(T entity)
        {
            return GetModifiedProperties(entity).Any();
        }

        public bool IsExistsOnContext(T entity)
        {
            return GetContextChangeTrackerEntries().Any(e => (e.Entity as BaseObject).Id == entity.Id);
        }

        public virtual bool IsItNew(T entity)
        {
            return GetUnique(entity) == null;
        }

        public bool IsNull(T entity)
        {
            return entity == null;
        }

        //https://docs.microsoft.com/en-us/ef/core/querying/tracking
        public IQueryable<T> Select(Expression<Func<T, bool>> filter = null, params string[] includes)
        {
            try
            {
                IQueryable<T> query = DbSet;

                if (includes != null && includes.Any())
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }

                return filter != null ? query.Where(filter) : query;
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

        public Repository<T> SetContext(DbContext context)
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

            return this;
        }

        public void ShowChangeTrackerEntriesStates()
        {
            Console.Write("\nShowChangeTrackerEntriesStates({0}) \n", typeof(T).Name);

            foreach (var e in GetContextChangeTrackerEntries())
            {
                //Hero_5279B33EBBDC032E0417D7CF4CD023D8DF037610FE554626AE177604A8238711
                var entityTypeName = e.Entity.GetType().Name.Contains('_') ? e.Entity.GetType().Name.Split('_')[0] : e.Entity.GetType().Name;

                Console.WriteLine("{0} : {1} ({2})", entityTypeName, e.State, (e.Entity as BaseObject).Id);
            }
        }

        public IQueryable<T> SqlQuery(string commandText, SqlParameterCollection commandParameters)
        {
            IQueryable<T> result = null;

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();

                for (int i = 0; i < commandParameters.Count; i++)
                {
                    parameters.Add(new SqlParameter(commandParameters[i].ParameterName, commandParameters[i].Value));
                }

                result = DbSet.SqlQuery(commandText, parameters.ToArray()).AsQueryable();
            }
            catch (Exception ex)
            {
                Log.Save(new Log(ex.Message));
            }

            return result;
        }

        public virtual bool Update(T entity)
        {
            bool isUpdated = false;

            try
            {
                var existingEntity = GetExistingEntity(entity);

                if (existingEntity != null)
                {
                    entity.Id = existingEntity.Id;

                    entity.AddedDate = existingEntity.AddedDate;

                    entity.AddedInfo = existingEntity.AddedInfo;

                    CreateUpdateOrDeleteGraph(entity);

                    existingEntity = GetExistingEntity(entity);

                    var ee = GetAsDbEntityEntry(existingEntity);

                    ee.CurrentValues.SetValues(entity);

                    //var properties = typeof(T).GetProperties().Where(p => p.CanWrite);

                    //foreach (var property in properties)
                    //{
                    //    var value = property.GetValue(entity, null);

                    //    //property.SetValue(existingEntity, value, null);
                    //}

                    entity = existingEntity;
                }

                if (HasAnyModifiedProperty(entity))
                {
                    entity.ModifiedDate = DateTime.Now;

                    Console.Write("\nModified Type : {0} \n", typeof(T).Name);

                    foreach (var modifiedProperties in GetModifiedProperties(entity).ToList())
                    {
                        Console.WriteLine(modifiedProperties);
                    }

                    ChangeEntityState(entity, EntityState.Modified);

                    isUpdated = true;
                }


                ShowChangeTrackerEntriesStates();
            }
            catch (Exception ex)
            {
                Log.Save(new Log(string.Format("Class : {0}, Error : {1}", entity.GetType().FullName, ex.ToString()),
                                    LogType.Error,
                                    entity.Id.ToString()));
            }

            return isUpdated || GetContextChangeTrackerEntries().Any(e => e.State == EntityState.Modified || e.State == EntityState.Added || e.State == EntityState.Deleted);
        }

        public virtual Expression<Func<T, bool>> UniqueFilter(T entity, bool forEntityFramework = true)
        {
            return o => o.Id == entity.Id;
        }



        //public virtual bool Update(T entity,
        //    Expression<Func<T,bool>> filter,
        //    string[] includes,
        //    bool checkAllProperties = false,
        //    params string[] modifiedProperties)
        //{
        //    try
        //    {
        //        var existingEntity = Get(filter, includes.ToArray());

        //        if (existingEntity == null)
        //            throw new Exception(string.Format("There is no such an entity with following informations ({0},{1})", existingEntity.GetType().Name, existingEntity.Id));

        //        var ee = Context.Entry(existingEntity);

        //        modifiedProperties = checkAllProperties ? modifiedProperties.Union(ee.OriginalValues.PropertyNames)
        //                                                .Distinct()
        //                                                .ToArray() : modifiedProperties;

        //        foreach (var property in modifiedProperties)
        //        {
        //            if (new[] { "Id" }.Union(includes).Any(p => p == property))
        //                continue;

        //            typeof(T).GetProperty(property)
        //                .SetValue(existingEntity, typeof(T).GetProperty(property).GetValue(entity, null));
        //        }

        //        if (modifiedProperties.Any())
        //        {
        //            return Update(existingEntity);
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Save(new Log(ex.Message));

        //        return false;
        //    }
        //}

     
    }
}
