using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LetsRoshLibrary.Core.Web;

namespace UnitTestProject1
{
    [TestClass]
    public class HtmlParserTest
    {
        [TestMethod]
        public void DataAccessTypeFromWeb()
        {
            var parser = new HtmlParser("http://www.dota2.com", DataAccessType.FromWeb);

            parser.Load();
        }
    }
}
