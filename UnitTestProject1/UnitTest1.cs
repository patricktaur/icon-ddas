using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //int givenPeriod = 4;

            //double[] totalSIRofAllPeriods = new double[givenPeriod];

            //long[] totalWeightedAverageofAllPeriods = new long[givenPeriod];


            Painting newPainting = new Painting();

            //collection of paintings for 2015

            newPainting.Height = 19;
            newPainting.Width = 19;
            newPainting.salesPrice = 1310425;

            Assert.AreEqual(3630, newPainting.Area);

            Painting newPainting2 = new Painting();
            newPainting2.Height = 39.5;
            newPainting2.Width = 19.25;
            newPainting2.salesPrice = 17100000;

            Painting newPainting3 = new Painting();
            newPainting3.Height = 40;
            newPainting3.Width = 40;
            newPainting3.salesPrice = 8571640;

            Painting newPainting4 = new Painting();
            newPainting4.Height = 16;
            newPainting4.Width = 16;
            newPainting4.salesPrice = 1629836;

            Painting newPainting5 = new Painting();
            newPainting5.Height = 16;
            newPainting5.Width = 16;
            newPainting5.salesPrice = 1908995;

            Painting newPainting6 = new Painting();
            newPainting6.Height = 40;
            newPainting6.Width = 40;
            newPainting6.salesPrice = 14816692;

            Painting newPainting7 = new Painting();
            newPainting7.Height = 38.75;
            newPainting7.Width = 38.75;
            newPainting7.salesPrice = 14309750;

            Painting newPainting8 = new Painting();
            newPainting8.Height = 59.125;
            newPainting8.Width = 59.125;
            newPainting8.salesPrice = 16921500;

            Painting newPainting9 = new Painting();
            newPainting9.Height = 47.25;
            newPainting9.Width = 47.125;
            newPainting9.salesPrice = 20938000;

            Painting newPainting10 = new Painting();
            newPainting10.Height = 15.75;
            newPainting10.Width = 15.75;
            newPainting10.salesPrice = 1908995;
            
            
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
            pts.Add(newPainting4);
            pts.Add(newPainting5);
            pts.Add(newPainting6);
            pts.Add(newPainting7);
            pts.Add(newPainting8);
            pts.Add(newPainting9);
            pts.Add(newPainting10);

            PriceComputation computation = new PriceComputation(pts, freshPainting);
            double totalSIR = 0;
            totalSIR = computation.getSIR();
            long price = computation.getPrice(totalSIR);

            Assert.AreEqual(86042.62699, totalSIR);
        }
    }
}
