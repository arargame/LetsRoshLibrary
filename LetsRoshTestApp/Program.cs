using LetsRoshLibrary.Core.Context;
using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
            using (var uow = new Dota2UnitofWork())
            {
                foreach (var language in Language.LanguagesFromDota2)
                {
                    if (!uow.Load<Language>().Any(l => l.Name == language.Name))
                        uow.Load<Language>().Insert(language);
                }

                uow.Commit();
            }


            var item2 = new Item()
            {
                Id = Guid.Parse("7F0AE62C-6990-EA11-BA87-C8D3FF210A02"),
                LinkParameter = "blink",
                Name = "firstItem",
                Image = new Image()
                {
                    Data = Encoding.UTF8.GetBytes("test")
                }
            };

            item2.AddLocalization(Localization.Create(item2,Language.DefaultLanguage,"classname","properttyname","value"));

            using (var context = new MainContext())
            {

                foreach (var localization in item2.Localizations)
                {
                    localization.Language = context.Set<Language>().SingleOrDefault(l => l.Name == localization.Language.Name);
                }

                context.Set<Item>().Add(item2);


                context.SaveChanges();
            }

            using (var uow = new Dota2UnitofWork())
            {
                var repo = uow.Load<Item>();

                item2.Lore = "Lorem ipsum" + DateTime.Now;

                repo.Update(item2, i => i.LinkParameter == item2.LinkParameter, new List<string>() { "Localizations", "Image" },
                    new Action<Item>((existingItem)=> 
                    {
                        if (existingItem.Image == null)
                            existingItem.Image = item2.Image;
                        else
                        {
                            //ImageRepository.Delete will be invoked
                        }
                    })
                    , false, "Lore");

                uow.Commit();
            }

            using (var context = new MainContext())
            {
                var itemToUpdate = item2;

                itemToUpdate.Lore = "Lorem ipsum" + DateTime.Now;

                var isExisted = false;

                var existingEntity = context.Set<Item>().Include("Localizations").Include("Image").SingleOrDefault(i => i.LinkParameter == item2.LinkParameter);

                var ee = context.Entry(existingEntity);

                foreach (var property in ee.OriginalValues.PropertyNames)
                {
                    if (new[] { "Id" }.Any(p => p == property))
                        continue;

                    var typeInfo = typeof(Item);

                    typeInfo.GetProperty(property).SetValue(existingEntity, itemToUpdate.GetType().GetProperty(property).GetValue(itemToUpdate, null));
                }


                var state = context.Entry(existingEntity).State;

                //context.Entry(itemToUpdate).State = isExisted ?
                //           EntityState.Modified :
                //           EntityState.Added;

                context.Entry(existingEntity).State = EntityState.Modified;

                bool saveFailed;
                do
                {
                    saveFailed = false;
                    try
                    {
                        var x = context.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;

                        // Update original values from the database
                        var entry = ex.Entries.Single();
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }

                } while (saveFailed);
            }



            using (var context = new MainContext())
            {

                var itemToRemove = context.Set<Item>().Include("Image").Include("Localizations").SingleOrDefault(i=>i.LinkParameter== "blink");

                

                context.Set<Item>().Remove(itemToRemove);

                var bbb = context.SaveChanges();
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
