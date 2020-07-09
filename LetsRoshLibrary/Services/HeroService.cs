using HtmlAgilityPack;
using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using LetsRoshLibrary.Core.Web;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Services
{
    public class HeroService : Service<Hero>
    {
        public HeroService(bool enableProxyCreationForContext = true) : base(enableProxyCreationForContext)
        {

        }

        //public override Hero AnonymousTypeToT(object anonymous)
        //{
        //    var hero = new BaseObjectService().AnonymousTypeToT(anonymous) as Hero;

        //    hero = AnonymousTypeToT(hero);

        //    return hero;
        //}

        public override void ConvertToPersistent(Hero disconnectedEntity, Hero persistent = null, Func<Hero> populatePersistent = null)
        {
            populatePersistent = () =>
            {
                using (var uow = new Dota2UnitofWork())
                {
                    var repository = new HeroRepository(uow.Context);

                    new HeroService().Select();

                    //return repository.Select(repository.UniqueFilter(disconnectedEntity)).ToList()
                    //.Select(s=> s as Hero)
                    //.SingleOrDefault();

                    return repository.Select(repository.UniqueFilter(disconnectedEntity), repository.GetAllIncludes())
                    .Select(q => new
                    {
                        q.Id,
                        q.Name,
                        q.ImageId
                    })
                    .ToList()
                    .Select(h => new Hero()
                    {
                        Id = h.Id,
                        Name = h.Name,
                        ImageId = h.ImageId
                    })
                    .SingleOrDefault();
                }
            };

            persistent = persistent ?? populatePersistent();

            base.ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);

            new CharacterService().ConvertToPersistent(disconnectedEntity, persistent, populatePersistent);
        }

        public static async Task<List<Hero>> LoadFromDota2Com(string linkParameter = null,Language language = null)
        {
            if (language == null)
                language = Language.DefaultLanguage;

            var list = new List<Hero>();

            var htmlParser = new HtmlParser(string.Format("http://www.dota2.com/heroes/?l={0}", language.Name.ToLower()), DataAccessType.FromWeb).Load();

            var heroLinkNodes = HtmlParser.GetDescendantsByAttribute(htmlParser.Document.DocumentNode, "a", "class", "heroPickerIconLink");

            var taskList = new List<Task>();

            foreach (var heroNode in heroLinkNodes)
            {
                var heroLinkParameter = heroNode.Id.Replace("link_", "");

                if (!string.IsNullOrWhiteSpace(linkParameter) && heroLinkParameter != linkParameter)
                {
                    continue;
                }

                var action = new Action(() =>
                {
                    var detailUrl = string.Format("{0}?l={1}", heroNode.GetAttributeValue("href", ""), language.Name.ToLower());

                    var detailNode = new HtmlParser(detailUrl, DataAccessType.FromWeb).Load().Document.DocumentNode;

                    var hero = new Hero();

                    hero.LinkParameter = heroLinkParameter;

                    LoadDetail(hero, detailNode);
                    LoadSkills(hero, detailNode);
                    hero.SetLocalization(language);

                    list.Add(hero);
                });

                taskList.Add(Task.Run(() =>
                {
                    action();
                }));
            }

            await Task.WhenAll(taskList);

            return list;
        }

        private static Hero LoadDetail(Hero hero,HtmlNode mainNode)
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

                hero.Name = heroNameNode.InnerText;

                var imageNode = HtmlParser.GetDescendantsByAttribute(mainNode, "div", "id", "heroTopPortraitContainer")
                                            .FirstOrDefault()
                                            .Descendants("img")
                                            .FirstOrDefault();

                if (imageNode == null)
                    throw new Exception("imageNode is null");

                hero.LoadImage(imageNode.GetAttributeValue("src", ""), hero.Name);

                var rolesNode = HtmlParser.GetDescendantsByAttribute(mainNode, "p", "id", "heroBioRoles")
                                            .FirstOrDefault();

                if (rolesNode == null)
                    throw new Exception("rolesNode is null");

                hero.Roles = string.Join(",", rolesNode.InnerText.Split('-').Select(t => t.Trim()));

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

                //var portraitNode = HtmlParser.GetDescendantsByAttribute(mainNode, "img", "id", "heroPrimaryPortraitImg").FirstOrDefault();

                //if (portraitNode == null)
                //    throw new Exception("portraitNode is null");

                //hero.Portrait = Image.Load(imageNode.GetAttributeValue("src", ""), hero.Name);

                var bioNode = HtmlParser.GetDescendantsByAttribute(mainNode, "div", "id", "bioInner").FirstOrDefault();

                if (bioNode == null)
                    throw new Exception("bioNode is null");

                hero.Bio = bioNode.InnerText.Trim();

                Console.WriteLine("Hero : {0} is added", hero.Name);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return hero;
        }

        private static Hero LoadSkills(Hero hero,HtmlNode node = null)
        {
            try
            {
                var skillNodes = HtmlParser.GetDescendantsByAttribute(node, "div", "class", "abilitiesInsetBoxInner");

                foreach (var skillNode in skillNodes)
                {
                    var skill = new Skill();

                    var skillNameNode = HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "abilityHeaderRowDescription")
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

                    var manaCostNode = HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "mana").Any() ?
                        HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "mana")
                                                .FirstOrDefault()
                                                .ChildNodes
                                                .Last() : null;

                    skill.ManaCost = manaCostNode?.InnerText.Trim();

                    var cooldownNode = HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "cooldown").Any() ?
                        HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "cooldown")
                                                .FirstOrDefault()
                                                .ChildNodes
                                                .Last() : null;

                    skill.CoolDown = cooldownNode?.InnerText.Trim();

                    var abilitiesDiv = HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "abilityFooterBox").FirstOrDefault();

                    if (abilitiesDiv == null)
                        throw new Exception("abilitiesDiv is null");

                    var leftAbilitiesDiv = HtmlParser.GetDescendantsByAttribute(abilitiesDiv, "div", "class", "abilityFooterBoxLeft").FirstOrDefault();

                    var rightAbilitiesDiv = HtmlParser.GetDescendantsByAttribute(abilitiesDiv, "div", "class", "abilityFooterBoxRight").FirstOrDefault();

                    skill.Extra = leftAbilitiesDiv.InnerText;

                    var extrasList = leftAbilitiesDiv.ChildNodes
                                                    .Union(rightAbilitiesDiv.ChildNodes)
                                                    .Select(cn => cn.InnerText.Trim())
                                                    .Where(t => !string.IsNullOrWhiteSpace(t))
                                                    .ToList();

                    var extras = new List<string>();

                    for (int i = 0; i < extrasList.Count / 2; i++)
                    {
                        var pair = extrasList.Skip(i * 2).Take(2).ToList();

                        extras.Add(string.Format("{0}{1}", pair[0], pair[1]));
                    }

                    skill.Extra = string.Join(",", extras);

                    var videoNode = HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "abilityVideoContainer").Any() ? HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "abilityVideoContainer").FirstOrDefault().Descendants("iframe").FirstOrDefault() : null;

                    skill.Video = videoNode?.GetAttributeValue("src", "");

                    var loreNode = HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "abilityLore").Any() ? HtmlParser.GetDescendantsByAttribute(skillNode, "div", "class", "abilityLore").FirstOrDefault() : null;

                    skill.Lore = loreNode?.InnerText;

                    hero.AddSkill(skill);

                    Console.WriteLine("Skill : {0} of {1} is added", skill.Name, hero.Name);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return hero;
        }

        public override void SetRepository(Repository<Hero> repository = null)
        {
            base.SetRepository(new HeroRepository());
        }
    }
}
