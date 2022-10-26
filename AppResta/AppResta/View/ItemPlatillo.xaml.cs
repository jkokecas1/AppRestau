using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemPlatillo : Rg.Plugins.Popup.Pages.PopupPage
    {

        Model.Extras extra = new Model.Extras();
        public ItemPlatillo(Model.Platillos platillo)
        {
            InitializeComponent();

            nombPlatillo.Text = platillo.nombre;
            descPlatillo.Text = platillo.descrip;
            extrasListView.ItemsSource = Extras(platillo.id);
        }



        private void addPlatillo(object sender, EventArgs e)
        {
             
        }
        

        
        public List<Model.Extras> Extras(int id)
        {
            List<Model.Extras> extras = new List<Model.Extras>();
            var sub = new List<Model.Extras>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/Platillo/index.php?op=obtenerExtras&id="+id.ToString());
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    extra = new Model.Extras();


                    int idplatillo = Int32.Parse(item["platillo_id_platillo"].ToString());
                    string nombrePlatillo = item["nombre"].ToString();
                    int idextra = Int32.Parse(item["id_esxtra"].ToString());
                    string nombreExtra = item["extra_nombre"].ToString();
                    double precioExtra = Convert.ToDouble(item["extra_precio"].ToString().Replace(",", "."));
                   


                    extra.platillo_id_platillo = idplatillo;
                    extra.nombre = nombrePlatillo;
                    extra.id_esxtra = idextra;
                    extra.extra_nombre = nombreExtra;
                    extra.extra_precio = precioExtra;


                    extras.Add(extra);

                }
                return extras;
            }
            else
            {
                return null;
            }
        }
    }
}