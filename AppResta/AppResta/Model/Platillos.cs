using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SQLite;
namespace AppResta.Model
{
    public class Platillos
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string nombre { get; set; }
        public string descrip { get; set; }
        public string precio { get; set; }
        public string url { get; set; }
        public ImageSource imgurl { get; set; }
        public int estatus { get; set; }
        public string categoria { get; set; }
        public string clasificacion { get; set; }
        public string subcategoria { get; set; }
    }
}
