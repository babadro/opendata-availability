using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace parse
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var sites = new List<string>
            {
                @"https://www.minfin.ru",
                //@"http://roskazna.ru",
                //@"http://nalog.ru",
                //@"http://www.customs.ru",
                //@"http://www.fsrar.ru"
            };

            foreach (var site in sites)
            {
                await Parser.Start(site);
            }
            
            Console.ReadLine();
        }
    }
}
