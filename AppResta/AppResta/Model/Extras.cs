using System;
using System.Collections.Generic;
using System.Text;

namespace AppResta.Model
{
    public class Extras
    {

        public int platillo_id_platillo { get; set; }
        public string nombre { get; set; }
        public int id_esxtra { get; set; }
        public string extra_nombre { get; set; }
        public double extra_precio { get; set; }
        public int extra_estaus { get; set; }
    }
}
