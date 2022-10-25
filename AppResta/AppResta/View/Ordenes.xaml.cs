using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Ordenes : ContentPage
    {
        public Ordenes()
        {
            InitializeComponent();
            BindingContext = new ViewModel.OrdenesViewModel(Navigation);

            ordenesListView.ItemsSource = Ordene();
        }

        public void select_Item(object sender, SelectedItemChangedEventArgs e)
        {
            var orden = e.SelectedItem as Model.Ordenes;
            
            Navigation.PushAsync(new Main(false, idOrden: orden.id_carts_cart));
        }

        public List<Model.Ordenes> Ordene()
        {
            Model.Ordenes orden;
            var ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrden");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    orden = new Model.Ordenes();
                    int id = Int32.Parse(item["id_carts_cart"].ToString());
                    string fecha = item["carts_cartco_date_addedl"].ToString();
                    string estado = Int32.Parse(item["carts_cart_estado"].ToString()) == 1 ? "En proceso" : "Terminado";
                    string mesa = item["carts_cart_mesa"].ToString();
                    orden.id_carts_cart = id;
                    orden.carts_cartco_date_addedl = fecha;
                    orden.carts_cart_estado = estado;
                    orden.mesa = mesa;
                    ordenList.Add(orden);
                }
                return ordenList;
            }
            else
            {
                return null;
            }
        }
    }
}