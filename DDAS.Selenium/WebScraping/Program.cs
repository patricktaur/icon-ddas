using System.Collections.Generic;
using System.Collections.Specialized;

namespace WebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            //string mystring = "clarityit";
            //string searchterm = "it";

            //int n = mystring.ToLower().Replace(searchterm.ToLower(), "").Length;

            //int count = (mystring.Length - n) / searchterm.Length;

            //int needleCount = (mystring.Length - mystring.ToLower().Replace(searchterm.ToLower(), "").Length)
            //    / searchterm.Length;

            //var test = new FirstTest();
            //test.SetUp();
            //test.TestFDADebarPage(); //tested
            //test.TestProposalToDebarPage(); //Tested
            //test.TestClinicalInvestigatorInspectionPage();//SearchBox
            //test.TestAdequateAssuranceListPage(); //tested
            //test.TestDisqualifiedInvestigators(); //tested
            //test.TestCBERClinicalInvestigators(); //tested
            //test.TestSDNListPage();
            //test.TestExclusionDatabaseSearchPage();//SearchBox
            //test.TestOpenXML();
            //test.TestSearchEngine();

            StringCollection myCollection = new StringCollection();

            myCollection.Add("The cats are crazy!");
            myCollection.Add("That is one huge tower");
            myCollection.Add("Moives everywhere");
            myCollection.Add("got to make it work");
            myCollection.Add("at any cost");


        }
    }
}
