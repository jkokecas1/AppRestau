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
    public partial class Bar : ContentPage
    {
        public Bar()
        {
            InitializeComponent();
            cocinaListView.ItemsSource = Ordene();
        }
        public List<Model.Cart> cart = new List<Model.Cart>();

        public static List<Model.Ordenes> Ordene()
        {
            Model.Ordenes orden;
            string[] itemaux = new string[3];
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();
            //http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=110&mesa=MESA-4
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
                    if (item["estado"].ToString() != "3")
                    {
                        itemaux = bebidas(item["id"].ToString(), item["mesa"].ToString());
                        if (itemaux[0] != null) {
                           
                            orden.id = Int32.Parse(item["id"].ToString());
                            orden.fecha_orden = itemaux[0]; // Items Bebidaas
                            orden.fecha_start = itemaux[1]; // Item Mesa
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
                return ordenList;
            }
            else
            {
                return null;
            }
        }

        public static string[] bebidas(string id, string mesa) { 
            string [] ordenList = new string[4];
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden="+id+"&mesa="+mesa);//110&mesa=MESA-4
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                // Console.WriteLine(jsonArray);
                foreach (var item in jsonArray)
                {
                    ordenList[0] += item["cantidad"] + " X " + item["nombre"] + "\n       --"+ item["comentario"].ToString().Replace("-"," ")+"\n \n";
                    ordenList[1] = item["mesa"].ToString();
                    ordenList[2] = item["idItem"].ToString();
                    ordenList[2] = item["idItem"].ToString();

                }
                return ordenList;
            }
            return ordenList;
        }

        private void cocinaListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //UpdateSelectionData(e.PreviousSelection, e.CurrentSelection);
        }
        public void UpdateSelectionData(IEnumerable<object> previousSelectedContact, IEnumerable<object> currentSelectedContact)

        {
            
            Model.Ordenes ord = new Model.Ordenes();
            var idorden = currentSelectedContact.FirstOrDefault() as Model.Ordenes;

            List<Model.Ordenes> cart = Ordene();

            foreach (Model.Ordenes item in cart)
            {

                if (item.id == idorden.id)
                {
                    ord.id = Int32.Parse(item.id.ToString());
                    ord.fecha_orden = item.fecha_orden.ToString();

                    Console.WriteLine(item.fecha_start.ToString());
                    if (item.fecha_start.ToString() != "")
                    {
                        if (!item.fecha_start.ToString().Equals("0000-00-00 00:00:00"))
                            ord.fecha_start = item.fecha_start.ToString().Remove(0, 10);
                    }
                    else
                    {
                        ord.fecha_start = item.fecha_start.ToString();
                    }

                    if (item.fecha_estimada.ToString() != "")
                    {
                        if (!item.fecha_estimada.ToString().Equals("0000-00-00 00:00:00"))
                            ord.fecha_estimada = item.fecha_estimada.ToString().Remove(0, 10);
                    }
                    else
                    {

                        ord.fecha_estimada = item.fecha_estimada.ToString();
                    }
                    switch (ord.estado)
                    {
                        case "1": ord.estado = "En espera"; break;
                        case "2": ord.estado = "Preparando..."; break;
                        case "3": ord.estado = "! Terminado !"; break;
                    }
                    ord.fecha_cerada = item.fecha_cerada.ToString();
                    ord.mesero =item.mesero.ToString();
                    ord.mesa = item.mesa.ToString();
                    ord.total = item.total.ToString();
                    ord.pago = Int32.Parse(item.pago.ToString());

                }

            }

            List<Model.Ordenes> auxList = Ordene();
            PopupNavigation.Instance.PushAsync(new VerOrden(ord, auxList, cocinaListView));

        }

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            Task.Delay(100);
            cocinaListView.ItemsSource = null;
            cocinaListView.ItemsSource = Ordene();
            //Refresh_Ordenes.IsRefreshing = false;
        }
    }
}