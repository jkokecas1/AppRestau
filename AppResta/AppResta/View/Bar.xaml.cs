using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections;
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
    public partial class Bar : ContentPage
    {

        public List<Model.Cart> cart = new List<Model.Cart>();
        public static ArrayList arlist = new ArrayList();
        public static ArrayList arlist2 = new ArrayList();
        public static bool band;

        public Bar(List<Model.Ordenes> orden= null)
        {
            InitializeComponent();
            
            if (orden != null)
                barListView.ItemsSource = orden;
            else
                barListView.ItemsSource = Ordene();

            //List<Model.Ordenes> ord = Ordene();

            Device.StartTimer(TimeSpan.FromSeconds(1.2), () =>
            {
                cargar.IsEnabled = false;
                cargar.IsRunning = false;
                cargar.IsVisible = false;
                barListView.IsVisible = true;
                return false;
            });
            //init(ord);

        }
        public void init(List<Model.Ordenes> ordenes)
        {
            barListView.ItemsSource = ordenes;
        }

        public static List<Model.Ordenes> Ordene()
        {
            Model.Ordenes orden;
            var h = DateTime.Now.ToString("yyyy-MM-dd");
            string[] itemaux = new string[3];
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
                    //Console.WriteLine(item["fecha_orden"].ToString().Remove(10, 9)+"==" +h);
                    orden = new Model.Ordenes();
                    if (item["fecha_orden"].ToString().Remove(10,9) == h && (item["estado"].ToString() == "1" || item["estado"].ToString() == "2"))
                    {
                        itemaux = bebidas(item["id"].ToString(), item["mesa"].ToString());
                        // Console.WriteLine(itemaux);
                        if (itemaux[0] != null)
                        {
                            //arlist.Add(itemaux[0].ToString());
                            // arlist2.Add(itemaux[3].ToString());
                            orden.id = Int32.Parse(item["id"].ToString());
                            orden.fecha_orden = itemaux[0]; // Items Bebidaas
                            orden.fecha_start = itemaux[1]; // Item Mesa
                            orden.mesa = item["mesa"].ToString();
                            //orden.fecha_estimada = item["fecha_estimada"].ToString();
                            // orden.fecha_cerada = item["fecha_cerada"].ToString();
                            /*switch (item["estado"].ToString())
                            {
                                case "1": orden.estado = "En espera"; break;
                                case "2": orden.estado = "Preparando..."; break;
                                case "3": orden.estado = "! Terminado !"; break;
                            }*/
                            // orden.mesero = item["mesero"].ToString();

                            //orden.total = item["total"].ToString();
                            //orden.pago = Int32.Parse(item["pago"].ToString());


                            ordenList.Add(orden);
                        }
                        //   
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
            string[] ordenList = new string[4];
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 1);//110&mesa=MESA-4
           
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                //Console.WriteLine(jsonArray);
                foreach (var item in jsonArray)
                {
                    ordenList[0] += item["cantidad"] + " X " + item["nombre"] + "\n       --" + item["comentario"].ToString().Replace("-", " ") + "\n \n";
                    ordenList[1] = item["mesa"].ToString();
                    ordenList[2] += item["idItem"].ToString();
                    ordenList[3] = item["idItem"].ToString();
                }
                return ordenList;
            }
            return ordenList;
        }

        private void cocinaListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectionDataAsync(e.PreviousSelection, e.CurrentSelection);
        }
        public async Task UpdateSelectionDataAsync(IEnumerable<object> previousSelectedContact, IEnumerable<object> currentSelectedContact)

        {
            String[] bear = new string[4];

            Model.Ordenes ord = new Model.Ordenes();
            var idorden = currentSelectedContact.FirstOrDefault() as Model.Ordenes;

            List<Model.Ordenes> cart = Ordene();
            // PopQuestion question = new View.PopQuestion("  !  CONFRIMA LA ORDEN   !");
            bool answer = await DisplayAlert("informacion", "CONFIRAMA LA ORDEN", "CONFIRMAR", "CANCELAR");
            //await PopupNavigation.Instance.PushAsync(question);


            if (answer)
            {

                var client = new HttpClient();
                client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + idorden.id + "&mesa=" + idorden.mesa + "&opc=" + 1);//110&mesa=MESA-4
                                                                                                                                                                                              // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + idorden.id + "&mesa=" + idorden.mesa + "&opc=" + 1);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    string json = content.ToString();
                    var jsonArray = JArray.Parse(json.ToString());
                    //Console.WriteLine(jsonArray);
                    foreach (var item in jsonArray)
                    {

                        itemsFinalizar(idorden.id + "", item["idItem"].ToString());

                    }
                }
                // barListView.ItemsSource = null;
                //barListView.ItemsSource = Ordene();

            }
            else
            {
                barListView.ItemsSource = null;
                barListView.ItemsSource = Ordene();
            }
        }

        public void itemsFinalizar(string ordnID, string itemID)
        {
            string cadena2 = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateEstadoItem&idItem=" + itemID;

            popAgregar(cadena2);



            int bebidas = Int32.Parse(ObtenerNumeorDeItems(Int32.Parse(ordnID), 1));
            int platillos = Int32.Parse(ObtenerNumeorDeItems(Int32.Parse(ordnID), 2));
            int bebidas2 = Int32.Parse(ObtenerNumeorDeItemsPlatillos(Int32.Parse(ordnID + ""), 1));
            int platillos2 = Int32.Parse(ObtenerNumeorDeItemsPlatillos(Int32.Parse(ordnID + ""), 2));
            Console.WriteLine("Platillos:" + bebidas + " = " + bebidas2);
            Console.WriteLine("BEBIDAS:" + platillos + " = " + platillos2);
            var h = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            string cadena0 = "";
            if (bebidas == bebidas2 && platillos == platillos2)
            {
                cadena0 = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateOrden&estado=3" + "&fecha_inicio=" + h.Replace("/", "-").Replace(" ", "-") + "&fecha_estimada=" + h.Replace("/", "-").Replace(" ", "-") + "&idCart=" + ordnID;
                popAgregar(cadena0);
                string cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateEstadoOrden&idItem=" + ordnID;

                popAgregar(cadena);
            }
            /*else {
                //cadena0 = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateOrden&estado=2" + "&fecha_inicio=" + h.Replace("/", "-").Replace(" ", "-") + "&fecha_estimada=" + h.Replace("/", "-").Replace(" ", "-") + "&idCart=" + ordnID;
            }*/

            /*
            
             switch (estado1)
             {
                 case 2:  break;
             }

             //Console.WriteLine(cadena0);

             popAgregar(cadena0);


             */
            // Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();

        }
        public static string ObtenerNumeorDeItemsPlatillos(int id, int opc)
        {
            string cantidad = "";

            var client1 = new HttpClient();

            client1.BaseAddress = new Uri(("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=ObtenerNumeorDeItemsPlatillos&idCart=" + id + "&opc=" + opc));
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
        public void popAgregar(string c)
        {

            var client = new HttpClient();
            client.BaseAddress = new Uri(c);
            // Console.WriteLine(c);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                barListView.ItemsSource = null;
                barListView.ItemsSource = Ordene();
                // 
            }
            else
            {
                DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo " + c, "OK");

            }
        }
        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            Task.Delay(100);
            barListView.ItemsSource = null;
            barListView.ItemsSource = Ordene();
            //Refresh_Ordenes.IsRefreshing = false;
        }
        /*
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }*/

        private void exit_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync(false);
        }
    }
}