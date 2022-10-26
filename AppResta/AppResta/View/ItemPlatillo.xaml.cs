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
        Model.Platillos platillos;
        public ItemPlatillo(Model.Platillos platillo)
        {
            InitializeComponent();
            platillos = platillo;
            nombPlatillo.Text = platillos.nombre;
            descPlatillo.Text = platillos.descrip;
            extrasListView.ItemsSource = Extras(platillos.id);
        }



        private void cerrarPop(object sender, EventArgs e)
        {
            
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        public void cantidadPlatillo(object sender, ValueChangedEventArgs e) {
            var value = e.NewValue;
            valCantidad.Text = value.ToString();
        }

        public void agregarItemCart()
        {

            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/Platillo/index.php?op=obtenerExtras&id=");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            }
            else {
                DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo", "OK");
            
            }
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