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
        public string Roles { get; set; }
        public Image Portrait { get; set; }

        public List<Skill> Skills = new List<Skill>();

        public static List<Hero> Load(Language language = null)
        {
            if (language == null)
                language = Language.DefaultLanguage;

            var list = new List<Hero>();

            var htmlParser = new HtmlParser(string.Format("http://www.dota2.com/heroes/?l={0}", language.Name.ToLower()), DataAccessType.FromWeb).Load();

            var heroLinkNodes = HtmlParser.GetDescendantsByAttribute(htmlParser.Document.DocumentNode, "a", "class", "heroPickerIconLink");

            foreach (var heroNode in heroLinkNodes)
            {
                var hero = new Hero();

                var detailUrl = string.Format("{0}?l={1}", heroNode.GetAttributeValue("href", ""), language.Name.ToLower());

                var detailNode = new HtmlParser(detailUrl, DataAccessType.FromWeb).Load().Document.DocumentNode;

                hero.LoadDetail(detailNode)
                    .LoadSkills(detailNode)
                    .SetLocalization(language);

                list.Add(hero);
            }


            return list;
        }

        private Hero LoadDetail(HtmlNode mainNode)
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

                Name = heroNameNode.InnerText;

                var imageNode = HtmlParser.GetDescendantsByAttribute(mainNode, "div", "id", "heroTopPortraitContainer")
                                            .FirstOrDefault()
                                            .Descendants("img")
                                            .FirstOrDefault();

                if (imageNode == null)
                    throw new Exception("imageNode is null");

                LoadImage(imageNode.GetAttributeValue("src", ""), Name);

                var rolesNode = HtmlParser.GetDescendantsByAttribute(mainNode, "p", "id", "heroBioRoles")
                                            .FirstOrDefault();

                if (rolesNode == null)
                    throw new Exception("rolesNode is null");

                Roles = string.Join(",", rolesNode.InnerText.Split('-').Select(t => t.Trim()));

                var primaryStatsNodes = HtmlParser.GetDescendantsByAttribute(mainNode, "div", "class", "overview_StatVal");

                if (primaryStatsNodes == null)
                    throw new Exception("primaryStatsNode is null");


                foreach (var node in primaryStatsNodes)
                {
                    foreach (var attribute in node.Attributes)
                    {
                        if (attribute.Name == "id")
                            switch (attribute.Value)
                            {
                                case "overview_IntVal":
                                    Intelligence = node.InnerHtml;
                                    break;

                                case "overview_AgiVal":
                                    Agility = node.InnerHtml;
                                    break;

                                case "overview_StrVal":
                                    Strength = node.InnerHtml;
                                    break;

                                case "overview_AttackVal":
                                    Damage = node.InnerHtml;
                                    break;

                                case "overview_SpeedVal":
                                    MovementSpeed = node.InnerHtml;
                                    break;

                                case "overview_DefenseVal":
                                    Armor = node.InnerHtml;
                                    break;
                            }
                    }
                }

                var portraitNode = HtmlParser.GetDescendantsByAttribute(mainNode, "img", "id", "heroPrimaryPortraitImg").FirstOrDefault();

                if (portraitNode == null)
                    throw new Exception("portraitNode is null");

                Portrait = Image.Load(imageNode.GetAttributeValue("src", ""), Name);

                var bioNode = HtmlParser.GetDescendantsByAttribute(mainNode, "div", "id", "bioInner").FirstOrDefault();

                if (bioNode == null)
                    throw new Exception("bioNode is null");

                Bio = bioNode.InnerText.Trim();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return this;
        }

        private Hero LoadSkills(HtmlNode node = null)
        {
            try
            {
                var skillNodes = HtmlParser.GetDescendantsByAttribute(node, "div", "class", "abilitiesInsetBoxInner");

                foreach (var skillNode in skillNodes)
                {
                    var skill = new Skill();

                    var skillNameNode = HtmlParser.GetDescendantsByAttribute(skillNode,"div","class","abilityHeaderRowDescription")
                                                    .FirstOrDefault()
                                                    .Descendants("h2")
                                                    .FirstOrDefault();
                    if (skillNameNode == null)
                        throw new Exception("skillNameNode is null");

                    skill.Name = skillNameNode.InnerText;

                    var skillImageNode = HtmlParser.GetDescendantsByAttribute(skillNode, "img", "class", "overviewAbilityImg").FirstOrDefault();

                    if (skillImageNode == null)
                        throw new Exception("skillImageNode is null");

                    skill.LoadImage(skillImageNode.GetAttributeValue("src", ""), skill.Name);

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

                    Skills.Add(skill);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return this;
        }

        public override void SetLocalization(Language language)
        {
            Localizations.Add(Localization.Create(language, "Hero", "Roles", Roles));
            Localizations.Add(Localization.Create(language, "Hero", "Bio", Bio));

            Skills.ForEach(s => s.SetLocalization(language));
        }
    }
}
