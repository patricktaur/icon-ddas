using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models
{
    [Serializable]
    public class MyCar
    {
        public Make make { get; set; }
        public string name { get; set; }
    }

    public partial class Make
    {
        public string Model { get; set; }
        public string Manufacturer { get; set; }
    }
}
