using LetsRoshLibrary.Core.Context;
using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshTestApp
{
    class Program
    {
        static void Main()
        {
            var itemList = Item.SelectFromDb().ToList();

            foreach (var item in itemList)
            {
                var bb = Item.DeleteFromDb(item);
            }


            var itemsTask = Item.Load(linkParameter:"blink");

            var items = itemsTask.Result;

            if (items.FirstOrDefault().Localizations.Any(l => l == null))
                throw new Exception("hellow");

            Item.SaveToDb(items.FirstOrDefault());


            Console.ReadLine();
        }
    }
}
