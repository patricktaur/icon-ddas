using DDAS.Models;
using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestProject1;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            UnitTest1 test = new UnitTest1();

            test.TestMethod1();

            return;
            //int givenPeriod = 4;

            //double[] totalSIRofAllPeriods = new double[givenPeriod];

            //long[] totalWeightedAverageofAllPeriods = new long[givenPeriod];

            /*
            Painting newPainting = new Painting();

            //collection of paintings for 2015
            
            newPainting.Height = 22.99;
            newPainting.Width = 15.5;
            newPainting.salesPrice = 15000;

            Painting newPainting2 = new Painting();
            newPainting2.Height = 33.25;
            newPainting2.Width = 21.12;
            newPainting2.salesPrice = 28500;

            Painting newPainting3 = new Painting();
            newPainting3.Height = 15.67;
            newPainting3.Width = 8.49;
            newPainting3.salesPrice = 19000;


            //collection of paintings for 2014
                //......


            //NewPainting
            Painting freshPainting = new Painting();
            freshPainting.Height = 12.25;
            freshPainting.Width = 9.57;

            ICollection<Painting> pts = new Collection<Painting>();
            pts.Add(newPainting);
            pts.Add(newPainting2);
            pts.Add(newPainting3);

            PriceComputation computation = new PriceComputation(pts, freshPainting);
            double totalSIR = computation.getSIR();
            long price = computation.getPrice(totalSIR);
            */
        }
    }
}
