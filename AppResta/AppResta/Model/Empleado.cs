using System;
using System.Collections.Generic;
using System.Text;


namespace AppResta.Model
{
   
    public class Empleado
    {
  
        public int id { get; set; }
        public string nombre { get; set; }
        public string pin { get; set; }
        public string puesto { get; set; }
        public string email { get; set; }
        public string celular { get; set; }
        public int estatus { get; set; }
    }
}
