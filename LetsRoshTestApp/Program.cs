using LetsRoshLibrary.Core.Context;
using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Model;
using LetsRoshLibrary.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshTestApp
{
    class Program
    { 
        static void Main()
        {

            //using (var uow = new Dota2UnitofWork())
            //{
            //    var rep = new CharacterRepository(uow.Context);

            //    var chars = rep.Select().ToList();
            //}

            //Logda fazlalık var Create Hero da
            // parametreli ve paralel işlemeli Hero.Load
            //Log ta id ler gelebilir
            // Log ta total etkilenen nesneleri toparlıyor 2000 tane nesne insert edildi diye

            var heroes = HeroService.LoadFromDota2Com("tiny", Language.LanguagesFromDota2.Where(l => l.Name == "turkish").FirstOrDefault()).Result;


            foreach (var hero in heroes)
            {
                var heroService = new HeroService();

                heroService.ConvertToPersistent(hero);
            }



            //using (var uow = new Dota2UnitofWork())
            //{
            //    var rep = new HeroRepository(uow.Context);

            //    foreach (var hero in heroes)
            //    {
            //        rep.Create(hero);

            //        var bb = uow.Commit();
            //    }
            //}


            //var imageService = new ImageService();
            //var img = imageService.Get(i=>i.Id.ToString()== "C5C3910A-ECA1-EA11-BA95-C8D3FF210A02");

            //var itemService2 = new ItemService();

            //var abyssal_blade = itemService2.Select(i=>i.LinkParameter== "abyssal_blade",ItemRepository.AllIncludes).FirstOrDefault();

            ////img.Id = Guid.NewGuid();

            ////abyssal_blade.Image = img;

            //itemService2.ConvertToPersistent(abyssal_blade);

            //var ggg = itemService2.CreateOrUpdate(abyssal_blade);



            //var il = new List<Item>()
            //{
            //    new Item(){LinkParameter="X1"},
            //    new Item(){LinkParameter="X2"},
            //};

            //var x = il.Any(new UniqueFilter<Item>().Get(new Item() { LinkParameter = "X2" }).Compile());

            var itemService = new ItemService();



            //var list = itemService.Select(includes: ItemRepository.AllIncludes);
            //language: Language.LanguagesFromDota2.Where(l => l.Name == "bulgarian").FirstOrDefault()
            
            //var webResults = ItemService.LoadFromDota2Com(language: Language.LanguagesFromDota2.Where(l => l.Name == "bulgarian").FirstOrDefault()).Result;

            var webResults = new ItemService().Select(i => i.Localizations.Any(l => l.Language.Name == "bulgarian"), new ItemRepository().GetAllIncludes());

            //webResults.OrderBy(wr => wr.LinkParameter).FirstOrDefault().Localizations.FirstOrDefault().Language = new Language() { Name = "test", Code = "tst" };

            //var it = webResults.FirstOrDefault();
            //itemService.ConvertToPersistent(it);

            //using (var uow = new Dota2UnitofWork())
            //{
            //    var imageRepository = new ImageRepository(uow.Context);

            //   // var unique = imageRepository.GetUnique(it.Image,true);

            //    var ddgsg = imageRepository.Create(it.Image);

            //    var ddgsg2= uow.Commit();
            //}

            var counter = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var result in webResults.OrderBy(wr=>wr.LinkParameter))
            {
                itemService.ConvertToPersistent(result);

                result.Localizations.FirstOrDefault().Description = "test";

                var bb102012 = itemService.CreateOrUpdate(result);

                if (!bb102012)
                    throw new Exception();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("lp : {0},counter = {1},id= {2}", result.LinkParameter, counter, result.Id);
                Console.ForegroundColor = ConsoleColor.White;
            }

            sw.Stop();

            Console.ReadLine();

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


            var itemSample = new ItemService().Get(i => i.LinkParameter == "blink", new ItemRepository().GetAllIncludes());

            var lclclc = new Localization(itemSample, Language.LanguagesFromDota2.FirstOrDefault(), "className", "propertyName", Guid.NewGuid().ToString());

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
