using DDAS.API;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Net.Http;


namespace DDAS.Run
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:56846/";
            WebApp.Start<Startup>(url: baseAddress);
            Console.WriteLine("Web API started");
            Console.ReadLine();
            



        }
    }
}
