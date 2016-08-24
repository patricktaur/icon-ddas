using System;
using System.Linq;
using NUnit.Framework;
using WebScraping.Selenium.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using Utilities;
using Utilities.WordTemplate;
using WebScraping.Selenium.SearchEngine;
using DDAS.Models.Enums;

namespace WebScraping.Tests
{
    [TestFixture]
    public class FirstTest
    {
        private IWebDriver _Driver;
        //private FDADebarPage _DebarPage;
        [TestFixtureSetUp]
        public void SetUp()
        {
            _Driver = new EdgeDriver();
            //_DebarPage = new FDADebarPage(_Driver);
        }

        [Test]
        public void TestFDADebarPage()
        {
            var date = DateTime.Now;
            using (FDADebarPage  page = new FDADebarPage(_Driver))
            {
                page.LoadDebarredPersonList();
                page.DebarredPersons.SaveCSV("test.csv");

                var db = page.DebarredPersons.Where(
                    x => x.NameOfPerson.Contains("James")).FirstOrDefault();
                if (db == null)
                {
                    Console.Write("Not found");
                }
                else
                {
                    Console.Write("Found");
                }
            }
            Console.WriteLine("Start Time: {0}", date);
            Console.WriteLine("End Time: {0}", DateTime.Now);
        }

        [Test]
        public void TestProposalToDebarPage()
        {
            var date = DateTime.Now;
            using (ERRProposalToDebarPage ProposalToDebarPage = new ERRProposalToDebarPage(_Driver))
            {
                ProposalToDebarPage.LoadProposalToDebarList();
                ProposalToDebarPage.propToDebar.SaveCSV("ProposalToDebarList.csv");

                var result = ProposalToDebarPage.propToDebar.Where(
                    x => x.name.Contains("Van")).FirstOrDefault();

                if (result == null)
                {
                    Console.WriteLine("Not Found!");
                }
                else
                {
                    Console.WriteLine("Bingo!");
                }
            }
            Console.WriteLine();
            Console.WriteLine("Start Time: {0}", date);
            Console.WriteLine("End Time: {0}", DateTime.Now);
        }

        [Test]
        public void TestClinicalInvestigatorInspectionPage()
        {
            var date = DateTime.Now;
            using (ClinicalInvestigatorInspectionPage ClinicalInvestigatorPage = 
                new ClinicalInvestigatorInspectionPage(_Driver))
            {
                ClinicalInvestigatorPage.SearchTerms("John");

                ClinicalInvestigatorPage.LoadClinicalInvestigatorList();

                while (ClinicalInvestigatorPage.GetNextList())
                {
                    ClinicalInvestigatorPage.LoadClinicalInvestigatorList();
                }
                    //ClinicalInvestigatorPage.GetNextList();
                ClinicalInvestigatorPage.clinicalInvestigatorList.SaveCSV
                    ("ClinicalInvestigatorInspection.csv");

                var result = ClinicalInvestigatorPage.clinicalInvestigatorList.Where(
                    x => x.Name.Contains("simelaro")).FirstOrDefault();

                if (result == null)
                {
                    Console.WriteLine("Not Found!");
                }
                else
                {
                    Console.WriteLine("Bingo!");
                }
            }
            Console.WriteLine();
            Console.WriteLine("Start Time: {0}", date);
            Console.WriteLine("End Time: {0}", DateTime.Now);
        }

        [Test]
        public void TestAdequateAssuranceListPage()
        {
            var date = DateTime.Now;
            using (AdequateAssuranceListPage AdequateAssuranceList =
                new AdequateAssuranceListPage(_Driver))
            {
                AdequateAssuranceList.LoadAdequateAssuranceInvestigators();
                AdequateAssuranceList.AdequateAssuranceIvestigatorList.SaveCSV
                    ("ClinicalInvestigatorInspection.csv");

                var result = AdequateAssuranceList.AdequateAssuranceIvestigatorList.Where(
                    x => x.NameAndAddress.Contains("WASHINGTON")).FirstOrDefault();

                if (result == null)
                {
                    Console.WriteLine("Not Found!");
                }
                else
                {
                    Console.WriteLine("Bingo!");
                }
            }
            Console.WriteLine();
            Console.WriteLine("Start Time: {0}", date);
            Console.WriteLine("End Time: {0}", DateTime.Now);
        }

        [Test]
        public void TestDisqualifiedInvestigators()
        {
            var date = DateTime.Now;
            using (ClinicalInvestigatorDisqualificationPage DisqualifiedInvestigatorList =
                new ClinicalInvestigatorDisqualificationPage(_Driver))
            {
                DisqualifiedInvestigatorList.LoadDisqualificationProceedingsList();
                DisqualifiedInvestigatorList.DisqualifiedInvestigatorList.SaveCSV
                    ("ClinicalInvestigatorInspection.csv");

                var result = DisqualifiedInvestigatorList.DisqualifiedInvestigatorList.Where(
                    x => x.Name.Contains("Bender")).FirstOrDefault();

                if (result == null)
                {
                    Console.WriteLine("Not Found!");
                }
                else
                {
                    Console.WriteLine("Bingo!");
                }
            }
            Console.WriteLine();
            Console.WriteLine("Start Time: {0}", date);
            Console.WriteLine("End Time: {0}", DateTime.Now);
        }

        [Test]
        public void TestCBERClinicalInvestigators()
        {
            var date = DateTime.Now;
            using (CBERClinicalInvestigatorInspectionPage CBERClinicalInvestigator =
                new CBERClinicalInvestigatorInspectionPage(_Driver))
            {
                CBERClinicalInvestigator.LoadNextInspectionList();
                //CBERClinicalInvestigator.LoadCBERClinicalInvestigators();
                CBERClinicalInvestigator.ClinicalInvestigator.SaveCSV
                    ("ClinicalInvestigatorInspection.csv");

                var result = CBERClinicalInvestigator.ClinicalInvestigator.Where(
                    x => x.Name.Contains("DIANA")).FirstOrDefault();

                if (result == null)
                {
                    Console.WriteLine("Not Found!");
                }
                else
                {
                    Console.WriteLine("Bingo!");
                }
            }
            Console.WriteLine();
            Console.WriteLine("Start Time: {0}", date);
            Console.WriteLine("End Time: {0}", DateTime.Now);
        }

        [Test]
        public void TestSDNListPage()
        {
            var date = DateTime.Now;
            using (SpeciallyDesignatedNationalsListPage SDNList = 
                new SpeciallyDesignatedNationalsListPage(_Driver))
            {
                SDNList.DownloadSDNList();

                string[] output = SDNList.GetTextFromPDF();
                SDNList.SearchNames(output, "A Rahman");
            }
            Console.WriteLine();
            Console.WriteLine("Start Time: {0}", date);
            Console.WriteLine("End Time: {0}", DateTime.Now);
        }

        [Test]
        public void TestExclusionDatabaseSearchPage()
        {
            var date = DateTime.Now;
            using (ExclusionDatabaseSearchPage ExclusionPage =
                new ExclusionDatabaseSearchPage(_Driver))
            {
                ExclusionPage.SearchTerms("", "baird");

                ExclusionPage.LoadExclusionsDatabaseList();
                ExclusionPage.ExclusionDatabaseList.SaveCSV
                    ("ExclusionsDatabaseSearchList.csv");

                var result = ExclusionPage.ExclusionDatabaseList.Where(
                    x => x.FirstName.Contains("JOHN")).FirstOrDefault();

                if (result == null)
                {
                    Console.WriteLine("Not Found!");
                }
                else
                {
                    Console.WriteLine("Bingo!");
                }
            }
            Console.WriteLine();
            Console.WriteLine("Start Time: {0}", date);
            Console.WriteLine("End Time: {0}", DateTime.Now);
        }

        public void TestOpenXML()
        {
            ReplaceTextFromWordTemplate template = new ReplaceTextFromWordTemplate();

            template.ReplaceTextFromWord("Pradeep","Clarity");
        }

        public void TestSearchEngine()
        {
            SearchEngine searchEngine = new SearchEngine();
            var result = searchEngine.SearchName("pradeep");
        }
    }
}
