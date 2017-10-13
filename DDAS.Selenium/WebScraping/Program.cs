using OpenXmlEmbedObjectNew;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebScraping.Tests;

namespace WebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            TestDateFormats();
            
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

        static void TestDateFormats()
        {
            Console.WriteLine("M/d/yy - 10/4/17");
            Console.WriteLine(DateTime.ParseExact("1/4/17","M/d/yy",null, System.Globalization.DateTimeStyles.None));
            Console.WriteLine("MM/dd/yyyy - 1/4/17");
            Console.WriteLine(DateTime.ParseExact("10/14/2017", "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None));
        }
    }
}
