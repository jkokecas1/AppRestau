using System;
using System.Collections.Generic;
using System.Text;

namespace AppResta.Model
{
    public class Cart
    {
        public int idItem { get; set; }
        public int id { get; set; }
        public string platillo { get; set; }
        public string mesa { get; set; }
        public int cantidad { get; set; }
        public double precio { get; set; }
        public double total { get; set; }
        public string extras { get; set; }
        public string estado { get; set; }
        public string visible { get; set; }
        public string comentario { get; set; }

    }
}
