using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppResta.Model
{
    public class Platillos
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descrip { get; set; }
        public string precio { get; set; }
        public ImageSource url { get; set; }
        public int estatus { get; set; }
        public string categoria { get; set; }
        public string clasificacion { get; set; }
        public string subcategoria { get; set; }
    }
}
