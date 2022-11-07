using System;
using System.Collections.Generic;
using System.Text;

namespace AppResta.Model
{
    public class Pagos
    {
        public int id { get; set; }
        public int idcart { get; set; }
        public double monto { get; set; }
        public string tipoPago { get; set; }
        public string propina { get; set; }
        public string total { get; set; }
    }
}
