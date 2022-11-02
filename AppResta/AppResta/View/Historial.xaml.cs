using AppResta.Model;
using Newtonsoft.Json.Linq;
using Plugin.InputKit.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Historial : ContentPage
    {

        List<Model.Cart> cart = new List<Model.Cart>();

        public Model.Cart cartItem = new Model.Cart();


        public Historial()
        {
            InitializeComponent();
            BindingContext = new ViewModel.HistorialViewModel(Navigation);
            ordenesListView.ItemsSource = Ordene();

        }


        public void select_Item(object sender, SelectedItemChangedEventArgs e)
        {
            var orden = e.SelectedItem as Model.Ordenes;
            productos.IsVisible = true;
            linea1.IsVisible = true;
            linea2.IsVisible = true;
            NombTicket.Text = "Ticket  Nº " + orden.id;
            NombMesero.Text = "Mesero                                            " + orden.mesero;
            FechaAbierto.Text = "Abierto                                            " + orden.fecha_orden;
            FechaCerada.Text = "Cuenta cerrada                                " + orden.fecha_cerada;
            NumMesa.Text = "Mesa n0                                         " + orden.mesa;
            PagoTotal.Text = "Total ***************  " + orden.total;
            Efectivo.Text = "Efectivo **************  " + orden.pago;
            Tarjeta.Text = "Tarjeta ***************  " + orden.pago;
            Pago.Text = "Pago";
            Monto.Text = "Monto";
            TP.Text = "Tiempo  25 MIN";

            productos.ItemsSource = CartMesa(orden.id + "", orden.mesa);
        }

        public List<Model.Ordenes> Ordene()
        {
            Model.Ordenes orden;
            var ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenHistorial");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    if (Int32.Parse(item["pago"].ToString()) != 0)
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

                }
                return ordenList;
            }
            else
            {
                return null;
            }


        }


        public List<Model.Cart> CartMesa(string id, string mesas)
        {

            var sub = new List<Model.Cart>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarrito&idOrden=" + id + "&mesa=" + mesas);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    cartItem = new Model.Cart();


                    int ids = Int32.Parse(item["id"].ToString());
                    int iditem = Int32.Parse(item["idItem"].ToString());
                    string nombre = item["nombre"].ToString();
                    int cantidad = Int32.Parse(item["cantidad"].ToString());
                    double precio = Convert.ToDouble(item["precio"].ToString().Replace(",", "."));
                    double total = (double)(precio * cantidad);


                    cartItem.id = ids;
                    cartItem.idItem = iditem;
                    cartItem.platillo = nombre;
                    cartItem.cantidad = cantidad;
                    cartItem.precio = precio;
                    cartItem.total = total;


                    cart.Add(cartItem);

                }
                return cart;
            }
            else
            {
                return null;
            }

        }
    }


}