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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshTestApp
{
    class Program
    {
        static void Main()
        {
            var itemService = new ItemService();

            var list = itemService.Select(includes: ItemRepository.AllIncludes);

            var persistentItem = itemService.Select(includes:ItemRepository.AllIncludes).FirstOrDefault();

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
