using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LetsRoshLibrary.Core.UnitofWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool disposed = false;

        public DbContext Context { get; set; }

        public UnitOfWork(DbContext context)
        {
            Context = context;
        }

        public UnitOfWork()
        {
            Initialize();
        }

        public virtual void Initialize()
        {

        }

        public Repository<T> Load<T>() where T : BaseObject
        {
            return new Repository<T>(Context);
        }

        public bool Commit(params string[] parameters)
        {
            try
            {
                using (TransactionScope tScope = new TransactionScope())
                {
                    Context.SaveChanges();
                    tScope.Complete();
                    return true;
                }
            }
            catch (DbEntityValidationException e)
            {
                var message = "";

                foreach (var eve in e.EntityValidationErrors.Take(1))
                {
                    message += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        message += string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }

                Log.Save(new Log(message + string.Format(",Parameters:{0}", string.Join(",", parameters))));

                return false;
            }
            catch (Exception ex)
            {
                var message = string.Format("ex.Message : {0},ex.InnerException.Message : {1},ex.InnerException.InnerException.Message : {2},parameters : {3}",
                    ex.Message,
                    ex.InnerException?.Message,
                    ex.InnerException.InnerException.Message,
                    string.Join(",", parameters));

                Log.Save(new Log(message));

                return false;
            }
        }


        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
