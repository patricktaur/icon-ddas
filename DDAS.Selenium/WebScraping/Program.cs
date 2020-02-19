using OpenXmlEmbedObjectNew;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using DDAS.Models.Entities.Domain.SiteData;

namespace WebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            //foreach (string arg in args)
            //{
            //    Console.WriteLine(arg);
            //}
            //var EmbeddingObjectPath = args[0];
            //var WordDocumentPath = args[1];

            //Console.WriteLine("Calling DLL to embed file");
            //var start = new Start();
            //start.EmbedObjectIntoDocument(EmbeddingObjectPath, WordDocumentPath);
            //Console.WriteLine("file is embedded successfully");

            Console.ReadKey();
        }

        static void ReadJSONFile()
        {
            var filePath = @"C:\Development\p926-ddas\DDAS.API\DataFiles\Downloads\FDAWarningLetters\FDAWarningLettersPage_17_Feb_2020_12_05.json";
            using (var file = File.OpenText(filePath))
            using (var reader = new JsonTextReader(file))
            {
                var jsonData = (JArray)JToken.ReadFrom(reader);
                var myObject = JsonConvert.DeserializeObject<List<FDAWarningLetterFile>>(File.ReadAllText(filePath));
            }
        }

        static void TestDateFormats()
        {
            Console.WriteLine("M/d/yy - 10/4/17");
            Console.WriteLine(DateTime.ParseExact("1/4/17","M/d/yy",null, System.Globalization.DateTimeStyles.None));
            Console.WriteLine("MM/dd/yyyy - 1/4/17");
            Console.WriteLine(DateTime.ParseExact("10/14/2017", "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None));
        }
    }
}
