using HtmlAgilityPack;
using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Core.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{

    //<script type="text/javascript" src="http://www.dota2.com/public/javascript/tooltips.js?v=FZiJuvUz3kj3&amp;l=turkish" ></script>
    //<script type = "text/javascript" src="http://www.dota2.com/public/javascript/heropedia.js?v=b5800661&amp;l=turkish" ></script>
    //URL: "http://www.dota2.com/jsfeed/heropediadata?feeds=itemdata,abilitydata,herodata&v=5800661HyAlWAmyrl84&l=turkish"

    //   function LoadHeropediaData(rgFeeds )
    //   {
    //       // see if we've already loaded any of the requested feeds
    //       rgNeededFeeds = [];
    //   outerloop:
    //       for (x = 0; x < rgFeeds.length; x++)
    //       {
    //       innerloop:
    //           for (y = 0; y < g_rgRequestedFeeds.length; y++)
    //           {
    //               if (rgFeeds[x] == g_rgRequestedFeeds[y])
    //                   continue outerloop;
    //           }
    //           rgNeededFeeds.push(rgFeeds[x]);
    //           g_rgRequestedFeeds.push(rgFeeds[x]);
    //       }
    //       if (!rgNeededFeeds.length)
    //           return;

    //       strFeeds = rgNeededFeeds.join(',');
    //       var URL = (location.protocol == 'https:') ? 'https://www.dota2.com/' : 'http://www.dota2.com/';
    //       URL = URL + 'jsfeed/heropediadata?feeds=' + strFeeds + '&v=58006613ZsnXxyeyX00&l=english';
    //$.ajax(

    //       {
    //       type: 'GET',
    //		cache: true,
    //		url: URL,
    //		dataType: 'jsonp',
    //		jsonpCallback: 'HeropediaDFReceive'

    //       }
    //);
    //   }

    public class Item : BaseObject
    {
        [Required]
        [Index(IsUnique = true)]
        public string LinkParameter { get; set; }
        public string Cost { get; set; }
        public string Attribute { get; set; }
        public string ManaCost { get; set; }
        public string CoolDown { get; set; }
        public string Bonus { get; set; }
        public string Recipe { get; set; }
        public string Notes { get; set; }
        public string Lore { get; set; }
        public string Qual { get; set; }
        public string Components { get; set; }

        public Item() { }





        public static void ConvertToPersistent(Item item)
        {
            using (var uow = new Dota2UnitofWork())
            {
                new ItemRepository(uow.Context).ConvertToPersistent(item);

                new BaseObjectRepository(uow.Context).ConvertToPersistent(item);
            }
        }



        public static async Task<List<Item>> Load(string linkParameter = null,Language language = null)
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

                    var action = new Action(()=> 
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
                Log.Save(new Log(ex.Message,LogType.Error));
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

                            foreach (var data in dataGroup.Where(dg=>dg.Name == item.LinkParameter))
                            { 
                                try
                                {
                                    if (!string.IsNullOrEmpty(data.ChildValues["lore"].ToString()))
                                        item.AddLocalization(Localization.Create(item, lang, "Item", "Lore", data.ChildValues["lore"].ToString()));

                                    var attributeHtml = data.ChildValues["attrib"].ToString();

                                    var doc = new HtmlDocument();

                                    doc.LoadHtml(attributeHtml);

                                    var itemAttributes = string.Join(",", doc.DocumentNode.InnerText.Split('\n'));

                                    if (!string.IsNullOrEmpty(itemAttributes))
                                        item.AddLocalization(Localization.Create(item, lang, "Item", "Attribute", itemAttributes));

                                    var doc2 = new HtmlDocument();
                                    doc2.LoadHtml(data.ChildValues["desc"].ToString());

                                    if (doc2 == null || doc2.DocumentNode == null)
                                        throw new Exception("buırda");

                                    if (!string.IsNullOrEmpty(doc2.DocumentNode.InnerText))
                                        item.AddLocalization(Localization.Create(item, lang, "Item", "Description", doc2.DocumentNode.InnerText));

                                    if (!string.IsNullOrEmpty(data.ChildValues["notes"].ToString()))
                                        item.AddLocalization(Localization.Create(item, lang, "Item", "Notes", data.ChildValues["notes"].ToString()));

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


           
            //var htmlParser = new HtmlParser("https://www.dotabuff.com/items/", DataAccessType.FromWeb).Load();

            //var itemLinkNodes = HtmlParser.GetDescendantsByAttribute(htmlParser.Document.DocumentNode, "div", "class", "content-inner")
            //                                .FirstOrDefault()
            //                                .Descendants("tbody")
            //                                .FirstOrDefault()
            //                                .Descendants("tr");

            //foreach (var itemNode in itemLinkNodes)
            //{
            //    var item = new Item();

            //    var linkNode = itemNode.Descendants("a").FirstOrDefault();

            //    //var detailUrl = string.Format("https://www.dotabuff.com{0}", linkNode.GetAttributeValue("href", ""));

            //    var detailUrl = "https://www.dotabuff.com/items/satanic";

            //    var detailNode = new HtmlParser(detailUrl, DataAccessType.FromWeb).Load().Document.DocumentNode;

            //    LoadDetail(item, detailNode);
            //}

            return itemList;
        }

        //public static void LoadDetailFromDotaBuff(Item item, HtmlNode mainNode)
        //{
        //    try
        //    {
        //        if (mainNode == null)
        //            throw new Exception("mainNode is null");

        //        var imgNode = HtmlParser.GetDescendantsByAttribute(mainNode, "div", "class", "header-content-container")
        //                                .FirstOrDefault()
        //                                .Descendants("img")
        //                                .FirstOrDefault();

        //        if (imgNode == null)
        //            throw new Exception("imgNode is null");

        //        item.LoadImage(string.Format("https://www.dotabuff.com{0}", imgNode.GetAttributeValue("src", "")));

        //        var detailNode = HtmlParser.GetDescendantsByAttribute(mainNode, "div", "class", "portable-show-item-details-default")
        //                                    .FirstOrDefault();

        //        if (detailNode == null)
        //            throw new Exception("detailNode is null");

        //        var nameNode = HtmlParser.GetDescendantsByAttribute(detailNode, "div", "class", "name").FirstOrDefault();


        //        if (nameNode == null)
        //            throw new Exception("nameNode is null");

        //        item.Name = nameNode.InnerText;

        //        var priceNode = HtmlParser.GetDescendantsByAttribute(detailNode, "span", "class", "number").FirstOrDefault();

        //        if (priceNode == null)
        //            throw new Exception("priceNode is null");

        //        item.Cost = priceNode.InnerText;

        //        var statsNode = HtmlParser.GetDescendantsByAttribute(detailNode, "div", "class", "stats")
        //                                    .FirstOrDefault()
        //                                    .Descendants("div");

        //        if (statsNode == null)
        //            throw new Exception("statsNode is null");

        //        var statList = new List<string>();

        //        foreach (var statNode in statsNode)
        //        {
        //            var spanNodes = statNode.Descendants("span");

        //            if (spanNodes.Count() != 2)
        //                throw new Exception("spanNodes count must be more than 2");

        //            statList.Add(string.Format("{0}:{1}", spanNodes.Last().InnerText.Trim(), spanNodes.First().InnerText.Trim()));
        //        }

        //        item.Bonus = string.Join(",", statList);

        //        var abilitiesNode = HtmlParser.GetDescendantsByAttribute(detailNode,"div","class", "description").FirstOrDefault();

        //        if (abilitiesNode == null)
        //            throw new Exception("abilitiesNode is null");

        //        foreach (var abilityNode in abilitiesNode.ChildNodes)
        //        {
        //            var ability = new ItemAbility();

        //            var abilityTitleNode = HtmlParser.GetDescendantsByAttribute(abilityNode, "div", "class", "description-block-header").FirstOrDefault();

        //            if (abilityTitleNode == null)
        //                throw new Exception("abilityTitleNode is null");

        //            ability.Name = abilityTitleNode.InnerText.Trim();

        //            abilityNode.ChildNodes.RemoveAt(0);
        //            ability.Description = abilityNode.InnerText.Trim();
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
    }
}
