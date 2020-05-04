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
                Log.Save(new Log(string.Format("Class : {0}, Error : {1}", typeof(T).BaseType.FullName, ex.ToString())));
            }

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
                Log.Save(new Log(string.Format("Class : {0}, Error : {1}", typeof(T).BaseType.FullName, ex.ToString())));
            }

            return false;
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
                Log.Save(new Log(description: string.Format("Class : {0}, Error : {1}", typeof(T).BaseType.FullName, ex.ToString()), entityId: entityId.ToString()));
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
                Log.Save(new Log(string.Format("Class : {0}, Error : {1}", typeof(T).BaseType.FullName, ex.ToString())));
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


        public virtual bool Insert(T entity)
        {
            bool isInserted = false;

            entity.Id = Guid.NewGuid();
            entity.AddedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            entity.IsActive = true;

            try
            {
                DbSet.Add(entity);
                var state = Context.Entry(entity).State;

                isInserted = true;

                foreach (var ent in Context.ChangeTracker.Entries())
                {
                    Console.WriteLine(ent.GetType().Name + " : " + ent.State);
                }
            }
            catch (Exception ex)
            {
                Log.Save(new Log(description: string.Format("Class : {0}, Error : {1}", entity.GetType().BaseType.FullName, ex.ToString()), entityId: entity.Id.ToString()));
            }

            return isInserted;
        }

        public bool Update(T entity)
        {
            bool isUpdated = false;

            entity.ModifiedDate = DateTime.Now;

            try
            {
                DbSet.Attach(entity);

                Context.Entry(entity).State = EntityState.Modified;

                isUpdated = true;
            }
            catch (Exception ex)
            {
                Log.Save(new Log("Repository",
                                    "Update",
                                    string.Format("Class : {0}, Error : {1}", entity.GetType().BaseType.FullName, ex.ToString()),
                                    LogType.Error,
                                    entity.Id.ToString()));
            }

            return isUpdated;
        }

        public virtual bool Delete(T entity)
        {
            bool isDeleted = false;

            try
            {
                if (Context.Entry(entity).State == EntityState.Detached) //Concurrency için
                {
                    DbSet.Attach(entity);
                }

                DbSet.Remove(entity);

                isDeleted = true;
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
