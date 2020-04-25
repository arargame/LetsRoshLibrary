using HtmlAgilityPack;
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
        public Image Portrait { get; set; }

        public string Roles { get; set; }

        public List<Skill> Skills = new List<Skill>();

        public static List<Hero> Load()
        {
            var list = new List<Hero>();

            var htmlParser = new HtmlParser("http://www.dota2.com/heroes/", DataAccessType.FromWeb).Load();

            var heroLinkNodes = HtmlParser.GetDescendantsByAttribute(htmlParser.Document.DocumentNode, "a", "class", "heroPickerIconLink");

            foreach (var heroNode in heroLinkNodes)
            {
                var hero = new Hero();

                var detailUrl = heroNode.GetAttributeValue("href", "");

                var detailNode = new HtmlParser(detailUrl, DataAccessType.FromWeb).Load().Document.DocumentNode;

                LoadDetail(hero, detailNode);

                LoadSkills(hero, detailNode);

                list.Add(hero);
            }


            return list;
        }

        public static void LoadDetail(Hero hero,HtmlNode mainNode)
        {
            try
            {
                if (mainNode == null)
                    throw new Exception("mainNode is null");

                var heroNameNode = HtmlParser.GetDescendantsByAttribute(mainNode, "div", "id", "centerColContent")
                                                .FirstOrDefault()
                                                .Descendants("h1")
                                                .FirstOrDefault();
                if (heroNameNode == null)
                    throw new Exception("heroNameNode is null");

                var imageNode = HtmlParser.GetDescendantsByAttribute(mainNode, "div", "id", "heroTopPortraitContainer")
                                            .FirstOrDefault()
                                            .Descendants("img")
                                            .FirstOrDefault();

                if (imageNode == null)
                    throw new Exception("imageNode is null");

                var rolesNode = HtmlParser.GetDescendantsByAttribute(mainNode, "p", "id", "heroBioRoles")
                                            .FirstOrDefault();

                if (rolesNode == null)
                    throw new Exception("rolesNode is null");

                var primaryStatsNodes = HtmlParser.GetDescendantsByAttribute(mainNode, "div", "class", "overview_StatVal");

                if (primaryStatsNodes == null)
                    throw new Exception("primaryStatsNode is null");


                hero.Name = heroNameNode.InnerText;

                hero.LoadImage(imageNode.GetAttributeValue("src",""));

                hero.Roles = string.Join(",", rolesNode.InnerText.Split('-').Select(t => t.Trim()));


                foreach (var node in primaryStatsNodes)
                {
                    foreach (var attribute in node.Attributes)
                    {
                        if (attribute.Name == "id")
                            switch (attribute.Value)
                            {
                                case "overview_IntVal":
                                    hero.Intelligence = node.InnerHtml;
                                    break;

                                case "overview_AgiVal":
                                    hero.Agility = node.InnerHtml;
                                    break;

                                case "overview_StrVal":
                                    hero.Strength = node.InnerHtml;
                                    break;

                                case "overview_AttackVal":
                                    hero.Damage = node.InnerHtml;
                                    break;

                                case "overview_SpeedVal":
                                    hero.MovementSpeed = node.InnerHtml;
                                    break;

                                case "overview_DefenseVal":
                                    hero.Armor = node.InnerHtml;
                                    break;
                            }
                    }
                }

                var portraitNode = HtmlParser.GetDescendantsByAttribute(mainNode, "img", "id", "heroPrimaryPortraitImg").FirstOrDefault();

                if (portraitNode == null)
                    throw new Exception("portraitNode is null");

                hero.Portrait = Image.Load(imageNode.GetAttributeValue("src", ""));

                var bioNode = HtmlParser.GetDescendantsByAttribute(mainNode, "div", "id", "bioInner").FirstOrDefault();

                if (bioNode == null)
                    throw new Exception("bioNode is null");

                hero.Bio = bioNode.InnerText.Trim();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LoadSkills(Hero hero,HtmlNode node)
        {
            try
            {
                var skillNodes = HtmlParser.GetDescendantsByAttribute(node, "div", "class", "abilitiesInsetBoxInner");

                foreach (var skillNode in skillNodes)
                {
                    var skill = new Skill();

                    var skillImageNode = HtmlParser.GetDescendantsByAttribute(skillNode, "img", "class", "overviewAbilityImg").FirstOrDefault();

                    skill.LoadImage(skillImageNode.GetAttributeValue("src",""));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
