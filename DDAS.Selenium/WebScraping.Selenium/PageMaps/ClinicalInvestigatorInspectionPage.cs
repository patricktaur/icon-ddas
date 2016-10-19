using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class ClinicalInvestigatorInspectionPage : BaseSearchPage
    {
        public IWebElement ClinicalInvestigatorTable
        {
            get
            {
                IList<IWebElement> Tables = driver.FindElements(By.XPath("//table"));
                return Tables[0];
            }
        }

        public IWebElement ClinicalInvestigatorInputTag
        {
            get
            {
                IList<IWebElement> InputTags = driver.FindElements(
                    By.Name("Keywords"));
                return InputTags[1];
            }
        }

        public IWebElement ClinicalInvestigatorSubmit
        {
            get
            {
                IWebElement SubmitElement = driver.FindElement(By.Name("Submit"));
                return SubmitElement;
            }
        }

        public IWebElement ClinicalInvestigatorNext
        {
            get
            {
                try
                {
                    if (driver.FindElement(By.Id("example_next")).Displayed)
                    {
                        IWebElement Element = driver.FindElement(
                            By.Id("example_next"));
                        return Element;
                    }
                    else
                        return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            }
        }

        public IWebElement ClinicalInvestigatorNextList
        {
            get
            {
                IWebElement Element = driver.FindElement(By.Id("example_paginate"));
                return Element;
            }
        }

        public IWebElement ClinicalInvestigatorAdvancedSearch
        {
            get
            {
                IList<IWebElement> Anchors =
                    driver.FindElements(By.XPath("//a"));

                foreach(IWebElement Anchor in Anchors)
                {
                    if (Anchor.Text.ToLower() == "advanced search")
                        return Anchor;
                }
                throw new NoSuchElementException(
                    "Advanced Search Anchor tag not found!");
            }
        }

        public IWebElement ClinicalInvestigatorFirstNameDropDown
        {
            get
            {
                IWebElement FirstNameDropDown =
                    driver.FindElement(By.Name("SearchFieldCriterion_2"));
                return FirstNameDropDown;
            }
        }

        public IWebElement ClinicalInvestigatorFirstNameTextField
        {
            get
            {
                IWebElement FirstName = 
                    driver.FindElement(By.Name("SearchFieldValue_2"));
                return FirstName;
            }
        }

        public IWebElement ClinicalInvestigatorZipAnchor
        {
            get
            {
                IList<IWebElement> Anchors =
                    driver.FindElements(By.XPath("//a"));

                foreach(IWebElement Anchor in Anchors)
                {
                    if (Anchor.Text.ToLower().Contains(
                        "clinical investigator inspection list zip file"))
                        return Anchor;
                }
                throw new Exception("Could not download ciil list!");
            }
        }
    }
}
