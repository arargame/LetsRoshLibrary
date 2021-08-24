using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Web
{
    public enum DataAccessType
    {
        FromFile,
        FromString,
        FromWeb
    }

    public class HtmlParser
    {
        public DataAccessType AccessType { get; set; }
        public HtmlDocument Document { get; set; }
        public string Url { get; set; }
        public string Data { get; set; }

        public HtmlParser(string url, DataAccessType dataAccessType)
        {
            Url = url;

            AccessType = dataAccessType;
        }


        public virtual HtmlParser Load(string path = null,string fromString = null)
        {
            try
            {
                switch (AccessType)
                {
                    case DataAccessType.FromFile:

                        Document.Load(path);

                        break;

                    case DataAccessType.FromString:

                        Document.LoadHtml(fromString);

                        break;

                    case DataAccessType.FromWeb:

                        var web = new HtmlWeb();

                        Document = web.Load(Url);

                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return this;
        }

        public async Task<HtmlParser> LoadAsync(string path = null, string fromString = null)
        {
            return await Task.Run(() => Load(path, fromString));
        }


        public virtual HtmlParser Parse()
        {
            return this;
        }

        public static List<HtmlNode> GetDescendantsByAttribute(HtmlNode node, string descendantName, string attributeName, string attributeValue)
        {
            return node.Descendants(descendantName)
                .Where(d => d.Attributes.Any(a => a.Name == attributeName)
                    && d.Attributes[attributeName].Value == attributeValue)
                .ToList();
        }
    }
}
