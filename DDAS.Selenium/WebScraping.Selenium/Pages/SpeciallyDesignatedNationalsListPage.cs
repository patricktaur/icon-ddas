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
using System.Collections.Specialized;

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

        public override string Url {
            get {
                return 
                @"http://www.treasury.gov/resource-center/sanctions/SDN-List/Pages/default.aspx";
            }
        }

        public override SiteEnum SiteName {
            get {
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

        public List<NamesClass> GetTextFromPDF(string NameToSearch)
        {
            List<NamesClass> Names = new List<NamesClass>();

            StringBuilder text = new StringBuilder();
            //string[] PDFDataArray;

            using (PdfReader reader = new PdfReader("c:\\development\\temp\\test.pdf"))
            {
                string PageContent;

                //PDFDataArray = new string[reader.NumberOfPages];

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string WordsMatched = null;

                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy(); 
                    PageContent = PdfTextExtractor.GetTextFromPage(reader, i, strategy);
                    Console.WriteLine(PageContent.Length);
                    //PDFDataArray[i - 1] = temp;

                    //text.Append(temp);

                    string[] SplitNames = PageContent.Split(']');

                    for (int records = 0; records < SplitNames.Length; records++)
                    {
                        WordsMatched = null;

                        string TempRecord = SplitNames[records].Replace(" ", "");

                        WordsMatched = FindSubString(TempRecord, NameToSearch);

                        if (WordsMatched != null)
                        {
                            NamesClass NamesFromPDF = new NamesClass();

                            NamesFromPDF.Names = SplitNames[records];
                            NamesFromPDF.PageNumbers = i;
                            NamesFromPDF.WordsMatched = WordsMatched;
                            Names.Add(NamesFromPDF);
                        }
                    }
                }
            }
            return Names;
        }

        public void SearchNames(string[] PDFDataArray, string SearchString)
        {
            int[] SearchHits = new int[PDFDataArray.Length];

            for (int i=0; i < PDFDataArray.Length; i++)
            {
                var textToSearch = PDFDataArray[i];
                var textToSearchLength = PDFDataArray[i].Length;
                var SearchStringLength = SearchString.Length;

                SearchHits[i] = (textToSearchLength - textToSearch.ToLower().Replace(SearchString.ToLower(), "").Length)
                    / SearchStringLength;

                PDFDataArray[i].Replace(" ", "");

                var SearchResults = PDFDataArray.Any();

            }

        }

        public override ResultAtSite Search(string NameToSearch)
        {
            ResultAtSite result = new ResultAtSite();

            string[] SearchName = NameToSearch.Split(' ');

            result.SiteName = SiteName.ToString();

            List<NamesClass> NamesFromPDF = GetTextFromPDF(NameToSearch);

            //List<NamesClass> TempNamesFromPDF = new List<NamesClass>();

            //TempNamesFromPDF = NamesFromPDF;
            
            //for (int Names = 0; Names < SearchName.Length; Names++)
            //{
            //    var results = NamesFromPDF.Where(x => x.Names.ToLower().Contains
            //    (SearchName[Names].ToLower()));

            //    if (results != null)
            //    {
            //        var SecondWordMatch = results.Where(x => x.Names.ToLower().Contains
            //        (SearchName[Names]));
            //    }
            //}

            foreach (NamesClass Names in NamesFromPDF)
            {
                result.Results.Add(new MatchResult
                {
                    MatchLocation = Names.PageNumbers.ToString() + ", " + Names.WordsMatched,
                    MatchName = Names.Names
                });
            }
            return result;
        }

        public override void LoadContent(string NameToSearch)
        {
            DownloadSDNList();

        }

        public class NamesClass
        {
            public string Names { get; set; }
            public int PageNumbers { get; set; }
            public string WordsMatched { get; set; }
        }
    }
}
