using WebScraping.Tests;

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

            var test = new TestSites();

            test.TestDDASSites();


        }
    }
}
