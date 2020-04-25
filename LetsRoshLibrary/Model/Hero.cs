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

                    if (skillImageNode == null)
                        throw new Exception("skillImageNode is null");

                    skill.LoadImage(skillImageNode.GetAttributeValue("src",""));

                    var skillNameNode = HtmlParser.GetDescendantsByAttribute(skillNode,"div","class","abilityHeaderRowDescription")
                                                    .FirstOrDefault()
                                                    .Descendants("h2")
                                                    .FirstOrDefault();
                    if (skillNameNode == null)
                        throw new Exception("skillNameNode is null");

                    skill.Name = skillNameNode.InnerText;

                    var skillDescriptionNode = HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "abilityHeaderRowDescription")
                                                    .FirstOrDefault()
                                                    .Descendants("p")
                                                    .FirstOrDefault();

                    if (skillDescriptionNode == null)
                        throw new Exception("skillDescriptionNode is null");

                    skill.Description = skillDescriptionNode.InnerText;

                    var manaCostNode = HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "mana")
                                                .FirstOrDefault()
                                                .ChildNodes
                                                .Last();

                    if (manaCostNode == null)
                        throw new Exception("manaCostNode is null");

                    skill.ManaCost = manaCostNode.InnerText.Trim();

                    var cooldownNode = HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "cooldown")
                                                .FirstOrDefault()
                                                .ChildNodes
                                                .Last();

                    if (cooldownNode == null)
                        throw new Exception("cooldownNode is null");

                    skill.CoolDown = cooldownNode.InnerText.Trim();

                    var abilitiesDiv = HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "abilityFooterBox").FirstOrDefault();

                    if (abilitiesDiv == null)
                        throw new Exception("abilitiesDiv is null");

                    var leftAbilitiesDiv = HtmlParser.GetDescendantsByAttribute(abilitiesDiv, "div","class", "abilityFooterBoxLeft").FirstOrDefault();

                    var rightAbilitiesDiv = HtmlParser.GetDescendantsByAttribute(abilitiesDiv, "div", "class", "abilityFooterBoxRight").FirstOrDefault();

                    skill.Extra = leftAbilitiesDiv.InnerText;

                    var extrasList = leftAbilitiesDiv.ChildNodes
                                                    .Union(rightAbilitiesDiv.ChildNodes)
                                                    .Select(cn => cn.InnerText.Trim())
                                                    .Where(t => !string.IsNullOrWhiteSpace(t))
                                                    .ToList();

                    var extras = new List<string>();

                    for (int i = 0; i < extrasList.Count/2; i++)
                    {
                        var pair = extrasList.Skip(i * 2).Take(2).ToList();

                        extras.Add(string.Format("{0}{1}", pair[0], pair[1]));
                    }

                    skill.Extra = string.Join(",", extras);

                    var videoNode = HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "abilityVideoContainer").FirstOrDefault().Descendants("iframe").FirstOrDefault();

                    if (videoNode == null)
                        throw new Exception("videoNode is null");

                    skill.Video = videoNode.GetAttributeValue("src","");

                    var loreNode = HtmlParser.GetDescendantsByAttribute(skillNode,"div","class", "abilityLore").FirstOrDefault();

                    if (loreNode == null)
                        throw new Exception("loreNode is null");

                    skill.Lore = loreNode.InnerText;

                    hero.Skills.Add(skill);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
