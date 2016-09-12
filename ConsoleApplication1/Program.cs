

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Tests;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            PageTests sut = new PageTests();
            sut.SetUp();
            sut.TestFDADebarPage();
        }
    }
}
