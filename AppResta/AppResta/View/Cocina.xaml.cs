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
                    int id = Int32.Parse(item["id"].ToString());
                    string fecha_orden = item["fecha_orden"].ToString();
                    string fecha_cerrado = item["fecha_cerada"].ToString();
                    string mesa = item["mesa"].ToString();
                    string total = item["total"].ToString();
                    int pago = Int32.Parse(item["pago"].ToString());
                    int mesero = Int32.Parse(item["mesero"].ToString());
                    orden.id = id;
                    orden.fecha_orden = fecha_orden;
                    orden.fecha_cerada = fecha_cerrado;
                    orden.mesero = mesero;
                    orden.mesa = mesa;
                    orden.total = total;
                    orden.pago = pago;
                    ordenList.Add(orden);
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

                    int id = Int32.Parse(item.id.ToString());
                    string fecha_orden = item.fecha_orden.ToString();
                    string fecha_cerrado = item.fecha_cerada.ToString();
                    string mesa = item.mesa.ToString();
                    string total = item.total.ToString();
                    int pago = Int32.Parse(item.pago.ToString());
                    int mesero = Int32.Parse(item.mesero.ToString());
                    ord.id = id;
                    ord.fecha_orden = fecha_orden;
                    ord.fecha_cerada = fecha_cerrado;
                    ord.mesero = mesero;
                    ord.mesa = mesa;
                    ord.total = total;
                    ord.pago = pago;
                }
            }


            PopupNavigation.Instance.PushAsync(new VerOrden(ord));

        }




    }
}