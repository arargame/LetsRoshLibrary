using LetsRoshLibrary.Core.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{
    public class Hero : Character
    {
        public static List<Hero> Load()
        {
            var list = new List<Hero>();

            var htmlParser = new HtmlParser("https://www.dotabuff.com/heroes", DataAccessType.FromWeb).Load();

            var heroesNode = HtmlParser.GetDescendantsByAttribute(htmlParser.Document.DocumentNode, "div", "class", "hero-grid").FirstOrDefault();

            var heroLinkNodes = heroesNode.Descendants("a");

            foreach (var heroNode in heroLinkNodes)
            {
                var hero = new Hero();

                var detailUrl = string.Format("https://www.dotabuff.com{0}", heroNode.GetAttributeValue("href", ""));

                LoadDetail(hero, detailUrl);


                var imgNode = HtmlParser.GetDescendantsByAttribute(heroNode, "div", "class", "hero").FirstOrDefault();
                var imgUrl = string.Format("https://www.dotabuff.com{0}", new string(imgNode.Attributes["style"].DeEntitizeValue.SkipWhile(c => c != '/').TakeWhile(c => c != ')').ToArray()));
                hero.LoadImage(imgUrl);

                list.Add(hero);
            }


            return list;
        }

        public static void LoadDetail(Hero hero,string url)
        {
            var htmlParser = new HtmlParser(url, DataAccessType.FromWeb).Load();

            var mainNode = htmlParser.Document.DocumentNode;

            var n = HtmlParser.GetDescendantsByAttribute(mainNode, "div", "class", "header-content-title").FirstOrDefault();

            hero.Name = HtmlParser.GetDescendantsByAttribute(mainNode,"div","class", "header-content-title").FirstOrDefault().InnerText;
        }
    }
}
