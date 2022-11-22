using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Cocina : ContentPage
    {
        List<Model.Ordenes> ORDEN;
        public Cocina(List<Model.Ordenes> orden= null)
        {

            InitializeComponent();
            if (orden != null) {
                ORDEN = orden;
                cocinaListView.ItemsSource = ORDEN;
            }
            else {
                ORDEN = Ordene();
                cocinaListView.ItemsSource = ORDEN;
            }
               

            
            Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
            {
                cargar.IsEnabled = false;
                cargar.IsRunning = false;
                cargar.IsVisible = false;
                cocinaListView.IsVisible = true;
                return false;
            });

           // cocinaListView.ItemsSource = ord;
        }
        public List<Model.Cart> cart = new List<Model.Cart>();

        public static List<Model.Ordenes> Ordene()
        {
            Model.Ordenes orden;
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();
            string[] aux = new string[3];

            var h = DateTime.Now.ToString("yyyy-MM-dd");
            orden = new Model.Ordenes();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrden");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                //Console.WriteLine(jsonArray);
                /*foreach (var item in jsonArray)
                {
                    //if (item["fecha_orden"].ToString().Remove(10, 9) == h)
                   // {
                        aux = bebidas(item["id"].ToString(), item["mesa"].ToString());

                        if (aux[0] != null)
                        {
                            if (true) {
                                orden.id = Int32.Parse(item["id"].ToString());
                                orden.fecha_orden = item["fecha_orden"].ToString();
                                orden.fecha_start = item["fecha_start"].ToString();
                                orden.fecha_estimada = item["fecha_estimada"].ToString();
                                orden.fecha_cerada = item["fecha_cerada"].ToString();
                                switch (item["estado"].ToString())
                                {
                                    case "1": orden.estado = "En espera"; break;
                                    case "2": orden.estado = "Preparando..."; break;
                                    case "3": orden.estado = "! Terminado !"; break;
                                }
                                orden.mesero = item["mesero"].ToString();
                                orden.mesa = item["mesa"].ToString();
                                orden.total = item["total"].ToString();
                                orden.pago = Int32.Parse(item["pago"].ToString());
                                ordenList.Add(orden);

                            }


                           


                        }
                   // }

                }*/
                foreach (var item in jsonArray)
                {
                    if (item["fecha_orden"].ToString().Remove(10, 9) == h){
                        aux = bebidas(item["id"].ToString(), item["mesa"].ToString());

                        if (aux[0] != null)
                        {
                            //Console.WriteLine(item["estado"].ToString());
                            orden = new Model.Ordenes();
                            // Console.WriteLine("Estado ORDEN = " + item["estado"] + " ---" + aux[0]);
                            if (Int32.Parse(item["estado"] + "") == 2 || Int32.Parse(item["estado"] + "") == 1 || Int32.Parse(item["estado"] + "") == 0)
                            {

                                orden.id = Int32.Parse(item["id"].ToString());
                                orden.fecha_orden = item["fecha_orden"].ToString();
                                orden.fecha_start = item["fecha_start"].ToString();
                                orden.fecha_estimada = item["fecha_estimada"].ToString();
                                orden.fecha_cerada = item["fecha_cerada"].ToString();
                                switch (item["estado"].ToString())
                                {
                                    case "1": orden.estado = "En espera"; break;
                                    case "2": orden.estado = "Preparando..."; break;
                                    case "3": orden.estado = "! Terminado !"; break;
                                }
                                orden.mesero = item["mesero"].ToString();
                                orden.mesa = item["mesa"].ToString();
                                orden.total = item["total"].ToString();
                                orden.pago = Int32.Parse(item["pago"].ToString());


                                ordenList.Add(orden);

                            }
                        }
                    }
                    
                }
                
                return ordenList;
            }
            else
            {
                return null;
            }
        }

        public static string[] bebidas(string id, string mesa)
        {
            string[] ordenList = new string[3];
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 2);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                foreach (var item in jsonArray)
                {
                    ordenList[0] += item["cantidad"] + "X" + item["nombre"] + "\n";
                    ordenList[1] = item["mesa"].ToString();
                    ordenList[2] = item["idItem"].ToString();

                }
                return ordenList;
            }
            return ordenList;
        }

        private void cocinaListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectionData(e.PreviousSelection, e.CurrentSelection);
        }
        public void UpdateSelectionData(IEnumerable<object> previousSelectedContact, IEnumerable<object> currentSelectedContact)

        {

            Model.Ordenes ord = new Model.Ordenes();
            Model.Ordenes idorden = currentSelectedContact.FirstOrDefault() as Model.Ordenes;

            //List<Model.Ordenes> cart = Ordene();
           /* int a = ORDEN.IndexOf(idorden);
           
            if(a != -1)
                ord = ORDEN[a];*/
            
           // List<Model.Ordenes> auxList = Ordene();
            PopupNavigation.Instance.PushAsync(new VerOrden(idorden, ORDEN, cocinaListView));

        }

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            Task.Delay(100);
            cocinaListView.ItemsSource = null;
            ORDEN = Ordene();
            cocinaListView.ItemsSource = ORDEN;

            Refresh_Ordenes.IsRefreshing = false;
        }


        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        private void exit_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine("Salir");
            Navigation.PopAsync();
        }

        private void exit_Clicked_1(object sender, EventArgs e)
        {
            Console.WriteLine("Salir");
           // Navigation.PopAsync();
            this.Navigation.PopAsync(true);
        }
    }
}