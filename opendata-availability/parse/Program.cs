using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using HtmlAgilityPack;

namespace parse
{
    class Program
    {
        static void Main(string[] args)
        {
            var sites = new List<string>
            {
                @"https://www.minfin.ru",
                @"http://roskazna.ru",
                @"http://nalog.ru",
                @"http://www.customs.ru",
                @"http://www.fsrar.ru"
            };

            foreach (var site in sites)
            {
                HtmlWeb web = new HtmlWeb();
                var htmlDoc = web.Load(site + @"/opendata");

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
                    
                var rows = table.Descendants("tr");

                var links = new List<HtmlNode>();
                foreach (var row in rows)
                {
                    var a = row.Descendants("a").FirstOrDefault();
                    if (a != null)
                        links.Add(a);
                }
                    //.Where(a => a.GetAttributeValue("href", "").Contains("opendata"));

                foreach (var n in links)
                {
                    //Console.WriteLine(n.GetAttributeValue("href", "not-found"));
                    var href = n.GetAttributeValue("href", string.Empty);
                    if (href == string.Empty)
                        continue;

                    var dataPage = site + href;
                    var htmlWeb = new HtmlWeb();
                    //var htmlDoc = htmlWeb.Load(dataPage);
                    //
                    //var dataFile = htmlDoc.DocumentNode.SelectSingleNode()

                    Console.WriteLine(dataPage);
                }
                Console.WriteLine(links.Count());
            }
            
            Console.ReadLine();
        }
    }
}
