using System;
using System.Collections.Generic;
using System.Text;

namespace AppResta.Model
{
    public class Ordenes
    {
        public int id { get; set; }
        public string fecha_orden { get; set; }
        public string fecha_start { get; set; }
        public string fecha_estimada { get; set; }
        public string fecha_cerada { get; set; }
        public string estado { get; set; }
        public string mesa { get; set; }
        public string total { get; set; }
        public string totoalExtras { get; set; }
        public string mesero { get; set; }
        public int pago { get; set; }
    }
}