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
                //@"https://www.minfin.ru",
                @"http://roskazna.ru",
                //@"http://nalog.ru",
                //@"http://www.customs.ru",
                //@"http://www.fsrar.ru"
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

                var rows = table.Descendants("tr").ToList();
                if (rows.Count > 0)
                    rows.RemoveAt(0); // Delete header of table

                var links = new List<HtmlNode>();
                foreach (var row in rows)
                {
                    var a = row.Descendants("a").FirstOrDefault();
                    if (a != null)
                        links.Add(a);
                }
                //.Where(a => a.GetAttributeValue("href", "").Contains("opendata"));

                var dataLinkCount = 0;
                foreach (var n in links)
                {
                    //Console.WriteLine(n.GetAttributeValue("href", "not-found"));
                    var href = n.GetAttributeValue("href", string.Empty);
                    if (href == string.Empty)
                        continue;

                    var dataPage = site + href;
                    var htmlWeb = new HtmlWeb();
                    var htmlDataPage = htmlWeb.Load(dataPage);
                    var dataLink = htmlDataPage.DocumentNode.SelectNodes("//tr").FirstOrDefault(tr => tr.InnerText.ToLower().Contains("гиперссылка"))?.Descendants("a").FirstOrDefault();
                    Console.WriteLine(dataPage);
                    if (dataLink != null)
                    {
                        Console.WriteLine(dataLink.GetAttributeValue("href", string.Empty));
                        dataLinkCount++;
                    }
                        
                    Console.WriteLine();
                    /*
                    var dataRows = from tr in htmlDataPage.DocumentNode.SelectNodes("//tr")
                                   let innerText = tr.InnerText.ToLower()
                                   where innerText.Contains("гиперссылка")
                                   select tr;
                    var dataLinks = new List<HtmlNode>();
                    foreach (var row in dataRows)
                    {
                        var a = row.Descendants("a").FirstOrDefault();
                        if (a != null)
                            dataLinks.Add(a);
                    }*/
                    //foreach (var link in dataLinks)
                    //    Console.WriteLine(link.GetAttributeValue("href", string.Empty));
                    

                    
                }
                Console.WriteLine(links.Count());
                Console.WriteLine(dataLinkCount);
            }
            
            Console.ReadLine();
        }
    }
}
