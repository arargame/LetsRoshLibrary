using LetsRoshLibrary.Core.Context;
using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Model;
using LetsRoshLibrary.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshTestApp
{

    public static class Extensions
    {
        public static List<PropertyInfo> GetDbSetProperties(this DbContext context)
        {
            var dbSetProperties = new List<PropertyInfo>();
            var properties = context.GetType().GetProperties();

            foreach (var property in properties)
            {
                var setType = property.PropertyType;

                var isDbSet = setType.IsGenericType && (typeof(IDbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()) || setType.GetInterface(typeof(IDbSet<>).FullName) != null);


                if (isDbSet)
                {
                    dbSetProperties.Add(property);
                }
            }

            return dbSetProperties;

        }
    }
    class Program
    { 
        static void Main()
        {
            using (var uow = new Dota2UnitofWork())
            {
                uow.Load<Item>();

                new Repository<Item>(uow.Context);

                var dbSetProperties = uow.Context.GetDbSetProperties();
                List<object> dbSets = dbSetProperties.Select(x => x.GetValue(uow.Context, null)).ToList();
            }

            var itemService = new ItemService();

            var list = itemService.Select(includes: ItemRepository.AllIncludes);

            //var persistentItem = itemService.Select(includes:ItemRepository.AllIncludes).FirstOrDefault();

            var persistentItem = new Item() { LinkParameter = "blink1010" };

            itemService.Update(persistentItem);

            //using (var uow =new Dota2UnitofWork())
            //{
            //    var imageRepository = new ImageRepository(uow.Context);

            //    var xcc = imageRepository.Get(persistentItem.ImageId.Value, imageRepository.GetAllIncludes());

            //    var b = imageRepository.Any(imageRepository.UniqueFilter(persistentItem.Image));
            //}

            itemService.ConvertToPersistent(persistentItem);

            //itemService.Update(list.FirstOrDefault());


            var itemSample = new ItemService().Get(i => i.LinkParameter == "blink", ItemRepository.AllIncludes);

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



            Console.ReadLine();
        }
    }
}
