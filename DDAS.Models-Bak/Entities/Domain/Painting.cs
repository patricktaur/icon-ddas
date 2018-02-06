using System;

namespace DDAS.Models.Entities.Domain
{
    public class Painting
    {
        public long Area { get { return Convert.ToInt64(Height * Width); } }
        public double Height { get; set; }
        public double Width { get; set; }
        public long salesPrice { get; set; }
        public double squareInchRate { get { return (salesPrice / Area); } }


    }
}
