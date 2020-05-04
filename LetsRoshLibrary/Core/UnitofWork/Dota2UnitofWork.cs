using LetsRoshLibrary.Core.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.UnitofWork
{
    public class Dota2UnitofWork : UnitOfWork
    {
        public override void Initialize()
        {
            Context = new MainContext();
        }
    }
}
