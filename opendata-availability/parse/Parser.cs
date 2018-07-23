using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace parse
{
    public class Parser
    {
        public static async Task Start(string site)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = await web.LoadFromWebAsync(site + @"/opendata");

            var tables = from tbl in htmlDoc.DocumentNode.SelectNodes("//table")
                         let innerText = tbl.InnerText.ToLower()
                         where innerText.Contains("набор") && innerText.Contains("данных")
                         select tbl;
            var table = tables.FirstOrDefault();

            if (table == null)
            {
                Console.WriteLine("talbe not found");
                return;
            }

            var rows = table.Descendants("tr").ToList();
            var links = new List<HtmlNode>();

            if (rows.Count < 2)
            {
                Console.WriteLine("Empty table");
                return;
            }
            for (var i = 1; i < rows.Count; i++) // the first row (rows[0]) is a header. Skip it.
            {
                var a = rows[i].Descendants("a").FirstOrDefault();
                if (a != null)
                    links.Add(a);
            }
            //.Where(a => a.GetAttributeValue("href", "").Contains("opendata"));

            var dataLinkCount = 0;
            var structrueDescrCount = 0;
            foreach (var n in links)
            {
                //Console.WriteLine(n.GetAttributeValue("href", "not-found"));
                var href = n.GetAttributeValue("href", string.Empty);
                if (href == string.Empty)
                    continue;

                var dataPage = site + href;
                var htmlWeb = new HtmlWeb();
                var htmlDataPage = await htmlWeb.LoadFromWebAsync(dataPage);
                var dataLink = htmlDataPage.DocumentNode.SelectNodes("//tr").FirstOrDefault(tr => tr.InnerText.ToLower().Contains("гиперссылка"))?.Descendants("a").FirstOrDefault();
                Console.WriteLine(dataPage);
                if (dataLink != null)
                {
                    Console.WriteLine(dataLink.GetAttributeValue("href", string.Empty));
                    dataLinkCount++;
                }

                var structureDescrLink = htmlDataPage.DocumentNode.SelectNodes("//tr").FirstOrDefault(tr =>
                {
                    return tr.Descendants("td").FirstOrDefault(td =>
                    {
                        var tdInnerText = td.InnerText.ToLower();
                        return tdInnerText.Contains("описан") && tdInnerText.Contains("структур") && tdInnerText.Contains("данн");
                    }) != null;
                })?.Descendants("a").FirstOrDefault();

                if (structureDescrLink != null)
                {
                    Console.WriteLine(structureDescrLink.GetAttributeValue("href", string.Empty));
                    structrueDescrCount++;
                }
                else
                {
                    var descr = htmlDataPage.DocumentNode.SelectNodes("//tr").FirstOrDefault(tr =>
                    {
                        var innerText = tr.InnerText.ToLower();
                        return innerText.Contains("описан") && innerText.Contains("структур") &&
                               innerText.Contains("данн");
                    });
                    var a_s = descr.Descendants("a");
                    var temp = 1;
                }
                Console.WriteLine();
            }
            Console.WriteLine(links.Count);
            Console.WriteLine(dataLinkCount);
            Console.WriteLine(structrueDescrCount);
        }

        public static async Task<(bool dataValid, bool structureValid)> DataPageProcessing(HtmlNode n, string site)
        {
            var href = n.GetAttributeValue("href", string.Empty);
            if (href == string.Empty)
                return (false, false);

            var dataValid = false;
            var structureValid = false;

            var dataPage = site + href;
            var htmlWeb = new HtmlWeb();
            var htmlDataPage = await htmlWeb.LoadFromWebAsync(dataPage);
            var dataLink = htmlDataPage.DocumentNode.SelectNodes("//tr").FirstOrDefault(tr => tr.InnerText.ToLower().Contains("гиперссылка"))?.Descendants("a").FirstOrDefault();
            Console.WriteLine(dataPage);
            if (dataLink != null)
            {
                Console.WriteLine(dataLink.GetAttributeValue("href", string.Empty));
                dataValid = true;
            }

            var structureDescrLink = htmlDataPage.DocumentNode.SelectNodes("//tr").FirstOrDefault(tr =>
            {
                return tr.Descendants("td").FirstOrDefault(td =>
                {
                    var tdInnerText = td.InnerText.ToLower();
                    return tdInnerText.Contains("описан") && tdInnerText.Contains("структур") && tdInnerText.Contains("данн");
                }) != null;
            })?.Descendants("a").FirstOrDefault();

            if (structureDescrLink != null)
            {
                Console.WriteLine(structureDescrLink.GetAttributeValue("href", string.Empty));
                structureValid = true;
            }
            else
            {
                var descr = htmlDataPage.DocumentNode.SelectNodes("//tr").FirstOrDefault(tr =>
                {
                    var innerText = tr.InnerText.ToLower();
                    return innerText.Contains("описан") && innerText.Contains("структур") &&
                           innerText.Contains("данн");
                });
                var a_s = descr.Descendants("a");
                var temp = 1;
            }
            Console.WriteLine();
            return (dataValid, structureValid);
        }
    }
}
