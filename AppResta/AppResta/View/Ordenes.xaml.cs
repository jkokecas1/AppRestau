using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AppResta.Model;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Ordenes : ContentPage
    {

        List<Model.Ordenes> ordenList;
        public Ordenes()
        {
            InitializeComponent();
            BindingContext = new ViewModel.OrdenesViewModel(Navigation);
            //btnNotification.Clicked += BtnNotification_Clicked;
            init();
        }

        async void init() {
             ordenList = Ordene();
            ordenesListView.ItemsSource = ordenList;//await App.Database.GetOrdenesAsync(); //
        }

        private void Button_Pagar(object sender, EventArgs e)
        {
            //Navigation.PushAsync(new Pago(ordenList),false);
        }

        public void select_Item(object sender, SelectedItemChangedEventArgs e)
        {
            var orden = e.SelectedItem as Model.Ordenes;

            //Navigation.PushAsync(new Main(false, idOrden: orden.id));
            
        }
      
        public List<Model.Ordenes> Ordene()
        {
            Model.Ordenes orden;
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();
          
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrden");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
               // Console.WriteLine(jsonArray);
                foreach (var item in jsonArray)
                {
                    orden = new Model.Ordenes();
                  
                    orden.id = Int32.Parse(item["id"].ToString());
                    orden.fecha_orden = item["fecha_orden"].ToString().Substring(11);
                    orden.fecha_cerada = item["fecha_cerada"].ToString().Substring(11);
                    orden.mesero = Int32.Parse(item["mesero"].ToString());
                    orden.mesa = item["mesa"].ToString();
                    orden.total = item["total"].ToString();
                    orden.pago = Int32.Parse(item["pago"].ToString());
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