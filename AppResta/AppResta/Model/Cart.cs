using System;
using System.Collections.Generic;
using System.Text;

namespace AppResta.Model
{
    public class Cart
    {
        public int id { get; set; }
        public string platillo { get; set; }
        public int cantidad { get; set; }
        public double precio { get; set; }
        public int total { get; set; }
        public string estado { get; set; }
      

    }
}
