using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Core.UnitofWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{
    public class Language : BaseObject
    {
        public static List<Language> LanguagesFromDota2 = new List<Language>()
        {
            new Language("brazilian","Português - Brasil","pt"),
            new Language("bulgarian","Български","bg"),
            new Language("czech","Čeština","cs"),
            new Language("danish","Dansk",""),
            new Language("dutch","Nederlands",""),
            new Language("english","English",""),
            new Language("finnish","Suomi",""),
            new Language("french","Français","fr"),
            new Language("german","Deutsch","de"),
            new Language("greek","Ελληνικά",""),
            new Language("hungarian","Magyar",""),
            new Language("italian","Italiano","it"),
            new Language("japanese","日本語",""),
            new Language("korean","한국어","ko"),
            new Language("norwegian","Norsk",""),
            new Language("polish","Polski","pl"),
            new Language("portuquese","Português",""),
            new Language("russian","Русский",""),
            new Language("romanian","Română",""),
            new Language("schinese","简体中文",""),
            new Language("spanish","Español - España","es"),
            new Language("swdedish","Svenska",""),
            new Language("tchinese","繁體中文",""),
            new Language("thai","ไทย","th"),
            new Language("turkish","Türkçe","tr"),
            new Language("ukrainian","Українська","uk"),
        };

        public string NativeName { get; set; }

        public string Code { get; set; }

        public static Language DefaultLanguage
        {
            get
            {
                return LanguagesFromDota2.Where(l => l.Code == "en")
                                            .FirstOrDefault();
            }
        }

        public Language() { }

        public Language(string name, string nativeName, string code)
        {
            Name = name;
            NativeName = nativeName;
            Code = code;
        }

        public static Language GetFromDb(string name)
        {
            Language language = null;

            using (var uow = new Dota2UnitofWork())
            {
                var repository = uow.Load<Language>();

                language = repository.Get(l => l.Name == name);
            }

            return language;
        }

    }
}
