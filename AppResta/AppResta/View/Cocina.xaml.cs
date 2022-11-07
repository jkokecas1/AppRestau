using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Cocina : ContentPage
    {
        public Cocina()
        {
            InitializeComponent();
            cocinaListView.ItemsSource = Ordene();
        }
        public List<Model.Cart> cart = new List<Model.Cart>();

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
                    if (item["estado"].ToString() != "3") {
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
                        orden.mesero = Int32.Parse(item["mesero"].ToString());
                        orden.mesa = item["mesa"].ToString();
                        orden.total = item["total"].ToString();
                        orden.pago = Int32.Parse(item["pago"].ToString());


                        ordenList.Add(orden);

                    }
                   
                }
                return ordenList;
            }
            else
            {
                return null;
            }
        }

        private void cocinaListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectionData(e.PreviousSelection, e.CurrentSelection);
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
                    if (item.fecha_start.ToString() != "") {
                        if(!item.fecha_start.ToString().Equals("0000-00-00 00:00:00"))
                            ord.fecha_start = item.fecha_start.ToString().Remove(0, 10);
                    }
                    else {
                        ord.fecha_start = item.fecha_start.ToString();
                    }

                    if (item.fecha_estimada.ToString() != "" )
                    {
                        if (!item.fecha_estimada.ToString().Equals("0000-00-00 00:00:00"))
                            ord.fecha_estimada = item.fecha_estimada.ToString().Remove(0, 10);
                    }
                    else
                    {
                        
                        ord.fecha_estimada = item.fecha_estimada.ToString();
                    }
                    switch (ord.estado) {
                        case "1": ord.estado = "En espera"; break;
                        case "2": ord.estado = "Preparando..."; break;
                        case "3": ord.estado = "! Terminado !"; break;
                    }
                    ord.fecha_cerada = item.fecha_cerada.ToString();
                    ord.mesero = Int32.Parse(item.mesero.ToString());
                    ord.mesa = item.mesa.ToString();
                    ord.total = item.total.ToString();
                    ord.pago = Int32.Parse(item.pago.ToString());

                }

            }

                List<Model.Ordenes> auxList = Ordene();
                PopupNavigation.Instance.PushAsync(new VerOrden(ord, auxList, cocinaListView));

        }

    }
}