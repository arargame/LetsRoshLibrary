using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.UnitofWork
{
    public interface IUnitOfWork : IDisposable
    {
        bool Commit(params string[] parameters);
    }
}
