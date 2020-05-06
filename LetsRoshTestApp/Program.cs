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

            var item2 = new Item()
            {
                LinkParameter = "blink",
                Name = "firstItem",
                Image = new Image()
                {
                    Data = Encoding.UTF8.GetBytes("test")
                }
            };


            using (var context = new MainContext())
            {

                context.Set<Item>().Add(item2);


                context.SaveChanges();
            }



            var xx = new Repository<Item>(new MainContext()).Get(i=>i.LinkParameter=="blink");

            var itemList = Item.SelectFromDb().ToList();

            foreach (var item in itemList)
            {
                var bb = Item.DeleteFromDb(item);
            }


            var itemsTask = Item.Load(linkParameter:"blink");

            var items = itemsTask.Result;

            //if (items.FirstOrDefault().Localizations.Any(l => l == null))
            //    throw new Exception("hellow");

            Item.SaveToDb(items.FirstOrDefault());

            using (var uow = new Dota2UnitofWork())
            {
                var repo = new ItemRepository(uow.Context);

                var entity = repo.Get(i => i.LinkParameter == "blink");

                var lang = new Repository<Language>(uow.Context).Get(l => l.Name == Language.DefaultLanguage.Name);

                entity.AddLocalization(Localization.Create(entity, lang, "classnam", "proeprty", "value"));

                var bbb = repo.Update(entity);

                var bb1001 = uow.Commit();
            }


            Console.ReadLine();
        }
    }
}
