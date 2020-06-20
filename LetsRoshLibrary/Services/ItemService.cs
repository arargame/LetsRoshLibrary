using HtmlAgilityPack;
using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Services
{
    public class ItemService : Service<Item>
    {
        public ItemService()  
        {
        
        }

        public override void ConvertToPersistent(Item disconnectedEntity, object persistent = null, Func<object> populatePersistent = null)
        {
            populatePersistent = () => 
            {
                using (var uow = new Dota2UnitofWork())
                {
                    var repository = new ItemRepository(uow.Context);

                    return repository.Select(repository.UniqueFilter(disconnectedEntity), repository.GetAllIncludes())
                    .Select(q => new
                    {
                        q.Id,
                        q.LinkParameter,
                        Image = new
                        {
                            Id = (Guid?)q.Image.Id,
                            q.Image.Name,
                            q.Image.Path,
                            q.Image.Data
                        },
                        Localizations = q.Localizations.Select(l => new
                        {
                            l.Id,
                            l.BaseObjectId,
                            l.LanguageId,
                            l.PropertyName
                        })
                    })
                    .SingleOrDefault();
                }
            };

            persistent = persistent ?? populatePersistent();

            base.ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);

            new BaseObjectService().ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);
        }


        public static async Task<List<Item>> LoadFromDota2Com(string linkParameter = null, Language language = null)
        {
            var itemList = new List<Item>();

            var jsonResult = "";

            try
            {
                using (var webClient = new WebClient())
                {
                    jsonResult = await Task.Run(() => webClient.DownloadString("http://www.dota2.com/jsfeed/heropediadata?feeds=itemdata&v=5800661HyAlWAmyrl84&l=english")).ConfigureAwait(false);
                }

                var resultToJObject = JObject.Parse(jsonResult);

                var taskList = new List<Task>();

                Stopwatch sw = new Stopwatch();
                sw.Start();

                foreach (JProperty child in resultToJObject["itemdata"].Children())
                {
                    if (!string.IsNullOrWhiteSpace(linkParameter) && child.Name != linkParameter)
                    {
                        continue;
                    }

                    var action = new Action(() =>
                    {
                        var item = new Item();

                        item.Name = child.Value["dname"].ToString();

                        item.LinkParameter = child.Name;

                        item.Cost = child.Value["cost"].ToString();

                        item.ManaCost = child.Value["mc"].ToString();

                        item.CoolDown = child.Value["cd"].ToString();

                        item.Lore = child.Value["lore"].ToString();

                        item.LoadImage(string.Format("http://cdn.dota2.com/apps/dota2/images/items/{0}", child.Value["img"].ToString()), item.LinkParameter);

                        var attributeHtml = child.Value["attrib"].ToString();

                        var doc = new HtmlDocument();
                        doc.LoadHtml(attributeHtml);

                        item.Attribute = string.Join(",", doc.DocumentNode.InnerText.Split('\n'));

                        var doc2 = new HtmlDocument();
                        doc2.LoadHtml(child.Value["desc"].ToString());

                        item.Description = doc2.DocumentNode.InnerText;

                        item.Notes = child.Value["notes"].ToString();

                        item.Qual = child.Value["qual"].ToString();

                        item.Components = string.Join(",", child.Value["components"].Children().Select(c => c.Value<string>()));

                        Console.WriteLine("The item with '{0}' name has been fetched", item.Name);

                        itemList.Add(item);
                    });


                    //await Task.Run(() => action());


                    taskList.Add(Task.Run(() =>
                    {
                        action();
                    }));
                }

                await Task.WhenAll(taskList);

                sw.Stop();
                Console.WriteLine(sw.Elapsed);

                //00:00:12.3362018
                //00:00:11.6084804
            }
            catch (Exception ex)
            {
                Log.Save(new Log(ex.Message, LogType.Error));
            }




            try
            {

                var languages = language != null ? Language.LanguagesFromDota2.Where(l => language.Name == l.Name) : Language.LanguagesFromDota2;

                var taskList = new List<Task>();

                foreach (var lang in languages)
                {
                    using (var webClient = new WebClient())
                    {
                        jsonResult = await Task.Run(() => webClient.DownloadString(string.Format("http://www.dota2.com/jsfeed/heropediadata?feeds=itemdata&v=5800661HyAlWAmyrl84&l={0}", lang.Name)));
                    }

                    var resultToJObject = JObject.Parse(jsonResult);

                    var dataGroup = resultToJObject["itemdata"].Children()
                        .Select(c => new
                        {
                            Name = (c as JProperty).Name,
                            ChildValues = (c as JProperty).Value
                        });


                    foreach (var item in itemList)
                    {
                        taskList.Add(Task.Run(() =>
                        {
                            foreach (var data in dataGroup.Where(dg => dg.Name == item.LinkParameter))
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(data.ChildValues["lore"].ToString()))
                                        item.AddLocalization(new Localization(item, lang, "Item", "Lore", data.ChildValues["lore"].ToString()));

                                    var attributeHtml = data.ChildValues["attrib"].ToString();

                                    var doc = new HtmlDocument();

                                    doc.LoadHtml(attributeHtml);

                                    var itemAttributes = string.Join(",", doc.DocumentNode.InnerText.Split('\n'));

                                    if (!string.IsNullOrEmpty(itemAttributes))
                                        item.AddLocalization(new Localization(item, lang, "Item", "Attribute", itemAttributes));

                                    var doc2 = new HtmlDocument();

                                    doc2.LoadHtml(data.ChildValues["desc"].ToString());

                                    if (doc2 == null || doc2.DocumentNode == null)
                                        throw new Exception("burda");

                                    if (!string.IsNullOrEmpty(doc2.DocumentNode.InnerText))
                                        item.AddLocalization(new Localization(item, lang, "Item", "Description", doc2.DocumentNode.InnerText));

                                    if (!string.IsNullOrEmpty(data.ChildValues["notes"].ToString()))
                                        item.AddLocalization(new Localization(item, lang, "Item", "Notes", data.ChildValues["notes"].ToString()));

                                    Console.WriteLine("Localizations  : of '{0}' language : {1}", item.Name, lang.Name);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                        }));
                    }
                }

                await Task.WhenAll(taskList);
            }
            catch (Exception ex)
            {
                Log.Save(new Log(ex.Message, LogType.Error));
            }

            return itemList;
        }

        public override void SetRepository(Repository<Item> repository = null)
        {
            base.SetRepository(new ItemRepository());
        }
    }
}
