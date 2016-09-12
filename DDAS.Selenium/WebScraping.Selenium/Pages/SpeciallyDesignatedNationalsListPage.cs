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
using  DDAS.Models.Interfaces;
namespace WebScraping.Selenium.Pages
{
    public class SpeciallyDesignatedNationalsListPage: ISearchPage
    { //: BaseSearchPage 

        private string _folderPath;
        [DllImport("urlmon.dll")]
        public static extern long URLDownloadToFile(long pCaller, string szURL, string szFileName, long dwReserved, long lpfnCB);

        //public SpeciallyDesignatedNationalsListPage(IWebDriver driver) : base(driver)
        public SpeciallyDesignatedNationalsListPage(string folderPath) 
        {
            _folderPath = folderPath;
            //Open();
            //SaveScreenShot("SpeciallyDesignatedNationalsList.png");
        }

        public  string Url {
            get {
                return 
                @"http://www.treasury.gov/resource-center/sanctions/SDN-List/Pages/default.aspx";
            }
        }

        public  SiteEnum SiteName {
            get {
                return SiteEnum.SpeciallyDesignedNationalsListPage;
            }
        }

        public void DownloadSDNList()
        {
            string fileName = _folderPath + @"\test.pdf"; // "c:\\development\\temp\\test.pdf";
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

            using (PdfReader reader = new PdfReader(_folderPath + @"\test.pdf"))
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

        public  ResultAtSite Search(string NameToSearch)
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

        //public  void LoadContent(string NameToSearch)
        //{
        //    DownloadSDNList();

        //}

        public string FindSubString(string SearchString, string NameToSearch)
        {
            SearchString = SearchString.ToLower();

            string[] FullName = NameToSearch.Trim().Split(' ');

            int count = 0;
            string WordMatched = null;

            for (int i = 0; i < FullName.Length; i++)
            {
                if (SearchString.Contains(FullName[i].ToLower()))
                {
                    count = count + 1;
                    WordMatched = WordMatched + " " + FullName[i].Trim();
                }
            }
            return WordMatched;
        }

        public void LoadContent()
        {
            DownloadSDNList();
        }

        public ResultAtSite GetResultAtSite(string NameToSearch)
        {
            //refactor: remove Search
            return Search(NameToSearch);
        }

        public void LoadContent(string NameToSearch)
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
