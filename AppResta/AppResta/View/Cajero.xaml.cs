using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
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
    public partial class Cajero : ContentPage
    {
            static List<Model.Ordenes> ordenList;
            public Cajero()
        {
            
            InitializeComponent();
            BindingContext = new ViewModel.OrdenesViewModel(Navigation);
            //btnNotification.Clicked += BtnNotification_Clicked;
            Device.StartTimer(TimeSpan.FromSeconds(1.2), () => {
                cargar.IsEnabled = false;
                cargar.IsRunning = false;
                cargar.IsVisible = false;
                RefreshOrdenes.IsVisible = true;
                return false;
            });
           
            init();
            // 
        }

        public void init()
        {
            ordenList = Ordene();
            if (ordenList != null)
                ordenesListView.ItemsSource = ordenList;//await App.Database.GetOrdenesAsync(); //
        }

        private void Button_Pagar(object sender, EventArgs e)
        {
            int id = Int32.Parse(((MenuItem)sender).CommandParameter.ToString());


            Pago pago = new Pago(ordenList, id, ordenesListView);
            PopError error = new PopError("LA ORDEN AUN NO SE ESTA LISTA");
            foreach (Model.Ordenes orden in ordenList)
            {
                if (orden.id == id)
                {
                    Console.WriteLine(orden.estado);
                    if (orden.estado == "! Terminado !")
                    {

                        // if(!pago.IsVisible && !error.IsVisible)

                        Navigation.PushAsync(pago, false);
                    }
                    else
                    {
                        // if (!pago.IsVisible && !error.IsVisible)
                        PopupNavigation.Instance.PushAsync(error);
                    }
                }


            }

        }

        public void select_Item(object sender, SelectedItemChangedEventArgs e)
        {
            var orden = e.SelectedItem as Model.Ordenes;

            //Navigation.PushAsync(new Main(false, idOrden: orden.id));

        }

        public static List<Model.Ordenes> Ordene()
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

                foreach (var item in jsonArray)
                {

                    orden = new Model.Ordenes();

                    orden.id = Int32.Parse(item["id"].ToString());
                    orden.fecha_orden = item["fecha_orden"].ToString().Remove(0, 10);
                    if (item["fecha_start"].ToString() != "")
                    {
                        orden.fecha_start = item["fecha_start"].ToString().Remove(0, 10);
                        orden.fecha_estimada = item["fecha_estimada"].ToString().Remove(0, 10);
                    }
                    //PLATILLOS
                    orden.fecha_cerada = item["fecha_cerada"].ToString();
                    switch (item["estado"].ToString())
                    {
                        case "1": orden.estado = "En espera"; orden.fecha_estimada = "#11111"; break;
                        case "2": orden.estado = "Preparando... "; orden.fecha_estimada = "#11111"; break;
                        case "3": orden.estado = "! Terminado !"; orden.fecha_estimada = "#ff0000"; break;
                    }
                    //BEBIDAS
                    orden.mesero = item["mesero"].ToString();
                    orden.mesa = item["mesa"].ToString();
                  //  orden.totoalExtras = item["totoalExtras"].ToString();
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

        

        public static double ExtrasItem(string id)
        {
            double totoal = 0.0;
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/platillo/index.php?op=obtenerExtrasAsItem&iditem=" + id);

            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                if (json.Equals("[]"))
                {
                    totoal = 0.0;

                }
                else
                {
                    var jsonArray = JArray.Parse(json.ToString());

                    foreach (var item in jsonArray)
                    {
                        totoal += Double.Parse(item["precio"].ToString().Replace(",", "."));
                    }

                }

            }
            return totoal;
        }

       

        
        private void RefreshOrdenes_Refreshing(object sender, EventArgs e)
        {
            Task.Delay(100);
            init();
            //VIVRACION
            /* try
             {
                 // Use default vibration length
                 Vibration.Vibrate();

                 // Or use specified time
                 var duration = TimeSpan.FromSeconds(1);
                 Vibration.Vibrate(duration);
             }
             catch (FeatureNotSupportedException ex)
             {
                 // Feature not supported on device
             }
             catch (Exception ex)
             {
                 // Other error has occurred.
             }*/
            RefreshOrdenes.IsRefreshing = false;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }
    }
}