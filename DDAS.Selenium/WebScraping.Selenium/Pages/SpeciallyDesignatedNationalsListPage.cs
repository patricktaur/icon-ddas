using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using System.Runtime.InteropServices;
using System.Net;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;

namespace WebScraping.Selenium.Pages
{
    public class SpeciallyDesignatedNationalsListPage : BaseSearchPage //BaseClasses.BasePage
    {
        [DllImport("urlmon.dll")]
        public static extern long URLDownloadToFile(long pCaller, string szURL, string szFileName, long dwReserved, long lpfnCB);

        public SpeciallyDesignatedNationalsListPage(IWebDriver driver) : base(driver)
        {
            Open();
            SaveScreenShot("SpeciallyDesignatedNationalsList.png");
        }

        public override string Url
        {
            get
            {
                return 
                @"http://www.treasury.gov/resource-center/sanctions/SDN-List/Pages/default.aspx";
            }
        }

        public override SiteEnum SiteName
        {
            get
            {
                return SiteEnum.SpeciallyDesignedNationalsListPage;
            }
        }

        public void DownloadSDNList()
        {
            string fileName = "c:\\development\\temp\\test.pdf";
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            string myStringWebResource = "https://www.treasury.gov/ofac/downloads/sdnlist.pdf";
            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", fileName, myStringWebResource);
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.DownloadFile(myStringWebResource, fileName);
        }

        public string[] GetTextFromPDF()
        {
            StringBuilder text = new StringBuilder();
            string[] PDFDataArray;

            DateTime date;

            using (PdfReader reader = new PdfReader("c:\\development\\temp\\test.pdf"))
            {
                string temp;
                PDFDataArray = new string[reader.NumberOfPages];

                Console.WriteLine("Starting to extract PDF Data - {0}", DateTime.Now);

                date = DateTime.Now;

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy(); 
                    temp = PdfTextExtractor.GetTextFromPage(reader, i, strategy);
                    Console.WriteLine(temp.Length);
                    PDFDataArray[i - 1] = temp;

                    text.Append(temp);
                }
            }
            Console.WriteLine("Total Length of the PDF text is {0}", text.Length);
            Console.WriteLine("Time taken to extract data from PDF file is {0}", DateTime.Now);
            return PDFDataArray;
        }

        public void SearchNames(string[] PDFDataArray, string SearchString)
        {
            int[] SearchHits = new int[PDFDataArray.Length];
            //int[] PageNumber = new int[PDFDataArray.Length];

            for (int i=0; i < PDFDataArray.Length; i++)
            {
                var textToSearch = PDFDataArray[i];
                var textToSearchLength = PDFDataArray[i].Length;
                var SearchStringLength = SearchString.Length;

                SearchHits[i] = (textToSearchLength - textToSearch.ToLower().Replace(SearchString.ToLower(), "").Length)
                    / SearchStringLength;
            }
            //Console.WriteLine("Search String {0} was found {1} time(s) in page number(s) {2}", SearchString, string.Join(", ",SearchHits));
        }

        public override ResultAtSite Search(string NameToSearch)
        {
            throw new NotImplementedException();
        }

        public override void LoadContent()
        {
            DownloadSDNList();

        }
    }
}
