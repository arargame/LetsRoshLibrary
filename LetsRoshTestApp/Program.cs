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
            //var b12421 = Localization.DeleteFromDb(l => l.Id.ToString() == "7F882038-F993-EA11-BA8A-C8D3FF210A02");

            var itemSample = Item.GetFromDb(i => i.LinkParameter == "blink", ItemRepository.Includes);

            var lclclc = Localization.Create(itemSample, Language.LanguagesFromDota2.FirstOrDefault(), "className", "propertyName", Guid.NewGuid().ToString());

            lclclc.BaseObjectId = itemSample.Id;
            lclclc.LanguageId = lclclc.Language.Id;
            lclclc.Name = DateTime.Now.ToString();

            itemSample.AddLocalization(lclclc);

            itemSample.Image = new Image() { Id = itemSample.Image.Id ,Data = Encoding.UTF8.GetBytes(DateTime.Now.ToString()) };
            itemSample.Image.Name = DateTime.Now.ToString();

            itemSample.Lore = "Lorem ımpsum : " + Guid.NewGuid();

            //itemSample.Localizations.Clear();

            using (var uow = new Dota2UnitofWork())
            {
                var itemRepository = new ItemRepository(uow.Context);

                itemRepository.Update(itemSample);

                var b1012010 = uow.Commit();
            }


            using (var uow = new Dota2UnitofWork())
            {
                uow.Context.Set<Item>().Attach(itemSample);
                var itemEE = uow.Context.Entry(itemSample);

                var itemRepository = new ItemRepository(uow.Context);

                //var existingItem = itemRepository.Get(i => i.LinkParameter == "blink", ItemRepository.Includes);

                //existingItem.Lore = itemSample.Lore;

                //existingItem.Localizations = itemSample.Localizations;

                itemRepository.ChangeEntityState(itemSample, EntityState.Modified);
                //new ImageRepository(uow.Context).ChangeEntityState(itemSample.Image,EntityState.Modified);
                //new LocalizationRepository(uow.Context).ChangeEntityState(itemSample.Localizations.FirstOrDefault(),EntityState.Modified);
                var eeImage = uow.Context.Entry(itemSample.Image);
                eeImage.OriginalValues.SetValues(eeImage.GetDatabaseValues());


                var localizationRepository = new LocalizationRepository(uow.Context);
                var existingLocalizations = localizationRepository.Select(l => l.BaseObjectId == itemSample.Id, includes: LocalizationRepository.Includes);

                foreach (var itemSampleLocalization in itemSample.Localizations)
                {
                    if (localizationRepository.IsItNew(itemSampleLocalization))
                    {
                        //localizationRepository.ChangeEntityState(itemSampleLocalization, EntityState.Added);
                        localizationRepository.Create(itemSampleLocalization);
                    }
                    else
                    {
                        var eeLocalization = localizationRepository.GetAsDbEntityEntry(itemSampleLocalization);

                        eeLocalization.OriginalValues.SetValues(eeLocalization.GetDatabaseValues());


                    }
                }


                //foreach (var localization in itemSample.Localizations)
                //{
                //    var eeLocalization = uow.Context.Entry(localization);

                //    if (eeLocalization.GetDatabaseValues() != null)
                //        eeLocalization.OriginalValues.SetValues(eeLocalization.GetDatabaseValues());
                //    else
                //        new LocalizationRepository(uow.Context).ChangeEntityState(localization, EntityState.Added);
                //}

                //var dbLocalizations = new LocalizationRepository(uow.Context).Select(l => l.BaseObjectId == itemSample.Id);

                //foreach (var localization in dbLocalizations)
                //{
                //    foreach (var currentValue in itemSample.Localizations)
                //    {
                //        if (localization.Equals(currentValue))
                //        {
                //            currentValue.Id = localization.Id;
                //        }
                //    }
                //}



             //new LocalizationRepository(uow.Context).Select(l => l.BaseObjectId == itemSample.Id);
                //var dbLocalizations = itemEe.


                //foreach (var dbLocalization in dbLocalizations)
                //{
                //    if(dbLocalization)
                //}


                itemRepository.ShowChangeTrackerEntriesStates();

                var baby = uow.Commit();
            }



            Item.ConvertToPersistent(itemSample);

            Item.UpdateDb(itemSample, modifiedProperties: "Lore");

            //foreach (var localization1 in itemSample.Localizations)
            //{
            //    var persistentLocalization = Localization.GetFromDb(l => l.LanguageId == localization1.LanguageId && l.ClassName==localization1.ClassName && l.PropertyName==localization1.PropertyName);

            //    if (persistentLocalization != null)
            //    {
            //        localization1.Id = persistentLocalization.Id;

            //        Localization.Update();
            //    }
            //    else
            //    {
            //        Localization.Create(localization1,);
            //    }
            //}


            //foreach (var lcl in itemSample.Localizations)
            //{
            //    var bva = Localization.DeleteFromDb(l => l.Id == lcl.Id);
            //}
            


           // var yy = Item.DeleteFromDb(i => i.LinkParameter == "blink");

            using (var uow = new Dota2UnitofWork())
            {
                var langRepo = new LanguageRepository(uow.Context);

                foreach (var language in Language.LanguagesFromDota2)
                {
                    if (!langRepo.Any(l => l.Name == language.Name))
                        langRepo.Create(language);
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
            item2.AddLocalization(Localization.Create(item2, Language.DefaultLanguage, "classname2", "properttyname2", "value2"));

            //item2.Lore = "Lorem ipsum" + DateTime.Now;

            //var b23 = Item.UpdateDb(item2, false, "Lore");

            var xxx = Item.SaveToDb(item2);


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

                repo.Update(item2, i => i.LinkParameter == item2.LinkParameter, ItemRepository.Includes,
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
