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
