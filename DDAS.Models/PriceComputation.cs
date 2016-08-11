using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;

namespace DDAS.Models
{
    public class PriceComputation
    {
        public int MyProperty { get; set; }
        private ICollection<Painting> _paintings;
        private Painting _newPainting;
        public PriceComputation(ICollection<Painting> paintings, Painting newPainting)
        {
            _paintings = paintings;
            _newPainting = newPainting;
        }

        public double getSIR()
        {
            double totalSIR = 0;

            foreach (Painting pt in _paintings)
            {
                totalSIR = totalSIR + pt.squareInchRate;
            }
            return totalSIR;
        }
        
        public long getPrice(double totalSIR)
        {             
            long weightedAverage = 0;

            weightedAverage = Convert.ToInt64(totalSIR / _paintings.Count);

            return Convert.ToInt64(_newPainting.Area * weightedAverage);
        }
    }
}
