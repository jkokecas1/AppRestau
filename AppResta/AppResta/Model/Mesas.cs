using System;
using System.Collections.Generic;
using System.Text;

namespace AppResta.Model
{
    public class Mesas
    {
        public int id { get; set; }
        public string mesa { get; set; }
        public string id_orden { get; set; }
        public string orden { get; set; }
        public string mesero { get; set; }
        public string ubicacion { get; set; }   
    }
}
