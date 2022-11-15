using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AppResta.Model;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Ordenes : ContentPage
    {

        static List<Model.Ordenes> ordenList;
        public Ordenes(Main mainPage = null)
        {
            InitializeComponent();
            BindingContext = new ViewModel.OrdenesViewModel(Navigation);
            //btnNotification.Clicked += BtnNotification_Clicked;
            if(mainPage != null)
                Navigation.RemovePage(mainPage);
            init();
        }

        public void init()
        {
            ordenList = Ordene();
            if(ordenList != null)
                ordenesListView.ItemsSource = ordenList;//await App.Database.GetOrdenesAsync(); //
        }

        private void Button_Pagar(object sender, EventArgs e)
        {
            int id = Int32.Parse(((MenuItem)sender).CommandParameter.ToString());


            Pago pago = new Pago(ordenList, id);
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
                    orden.fecha_cerada = ObtenerNumeorDeItems(orden.id, 1) + "/" + ObtenerNumeorDeItemsPlatillos(orden.id, 1);
                    switch (item["estado"].ToString())
                    {
                        case "1": orden.estado = "En espera"; break;
                        case "2": orden.estado = "Preparando... "; break;
                        case "3": orden.estado = "! Terminado !"; break;
                    }
                    //BEBIDAS
                    orden.mesero = ObtenerNumeorDeItems(orden.id,2) + "/" + ObtenerNumeorDeItemsPlatillos(orden.id,2);
                    orden.mesa = item["mesa"].ToString();
                    orden.totoalExtras = obtenerPagoFinal(orden.id)[1].ToString();
                    orden.total = obtenerPagoFinal(orden.id)[0].ToString() ;// 
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

        public static double[] obtenerPagoFinal(int id)
        {
            double []total = new double[2];

            var client1 = new HttpClient();

           
            client1.BaseAddress = new Uri(("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=ObtenerPrecioItems&idCart=" + id));
           
            HttpResponseMessage response1 = client1.GetAsync(client1.BaseAddress).Result;
            if (response1.IsSuccessStatusCode)
            {
                var content1 = response1.Content.ReadAsStringAsync().Result;
                string json1 = content1.ToString();

                var jsonArray1 = JArray.Parse(json1.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray1)
                {
                    total[0] += Double.Parse(item["precio"].ToString().Replace(",", ".")) ;
                    total[1] += Double.Parse(ExtrasItem(item["item"].ToString())+"");
                }

                
            }
            return total;
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

        public static string ObtenerNumeorDeItems(int id, int opc)
        {
            string cantidad = "";

            var client1 = new HttpClient();

            client1.BaseAddress = new Uri(("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=ObtenerNumeorDeItems&idCart=" + id + "&opc=" + opc));
            HttpResponseMessage response1 = client1.GetAsync(client1.BaseAddress).Result;
            if (response1.IsSuccessStatusCode)
            {
                var content1 = response1.Content.ReadAsStringAsync().Result;
                string json1 = content1.ToString();

                var jsonArray1 = JArray.Parse(json1.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray1)
                {
                    cantidad = item["cantidad"].ToString();
                }
            }
            return cantidad;
        }

        public static string ObtenerNumeorDeItemsPlatillos(int id,int opc)
        {
            string cantidad = "";
            
            var client1 = new HttpClient();

            client1.BaseAddress = new Uri(("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=ObtenerNumeorDeItemsPlatillos&idCart=" + id+"&opc="+opc));
            HttpResponseMessage response1 = client1.GetAsync(client1.BaseAddress).Result;
            if (response1.IsSuccessStatusCode)
            {
                var content1 = response1.Content.ReadAsStringAsync().Result;
                string json1 = content1.ToString();

                var jsonArray1 = JArray.Parse(json1.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray1)
                {
                    cantidad = item["cantidad"].ToString();
                }
            }
            return cantidad;
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
    }
}