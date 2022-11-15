
using Newtonsoft.Json.Linq;
using PdfSharp.Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using Path = System.IO.Path;
namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Ticket : Rg.Plugins.Popup.Pages.PopupPage
    {

        List<Model.Cart> cart = new List<Model.Cart>();

        public Model.Cart cartItem = new Model.Cart();

        string id = "";

        public Ticket(List<Model.Ordenes> orden, int id)
        {
            InitializeComponent();
            this.id = id + "";
            Model.Ordenes ordenes = new Model.Ordenes();

            foreach (Model.Ordenes ordenes1 in orden)
            {
                if (ordenes1.id == id)
                {
                    ordenes = ordenes1;
                }
            }

            //productos.IsVisible = true;
            //linea1.IsVisible = true;
            //linea2.IsVisible = true;
            NombTicket.Text = "Ticket  Nº " + id;
            NombMesero.Text = "Mesero                                            " + ordenes.mesero;
            //FechaAbierto.Text = "Abierto                                            " + ordenes.fecha_orden;
            FechaCerada.Text = "Cuenta cerrada                             " + ordenes.fecha_estimada;
            NumMesa.Text = "Mesa n0                                         " + ordenes.mesa;
            PagoTotal.Text = "Total:               " + ordenes.total;
            TotalExtras.Text = "Total extras:                 " + ordenes.pago;
            Efectivo.Text = "Efectivo:                " + ordenes.total;
            Tarjeta.Text = "Tarjeta:                       " + ordenes.pago;
            Pago.Text = "Pago";
            Monto.Text = "Monto";
            // TP.Text = "Tiempo  25 MIN";

            //productos.ItemsSource = CartMesa(orden.id + "", orden.mesa);
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

                    orden = new Model.Ordenes();

                    orden.id = Int32.Parse(item["id"].ToString());
                    orden.fecha_orden = item["fecha_orden"].ToString(); ;
                    orden.fecha_cerada = item["fecha_cerada"].ToString(); ;
                    orden.mesero = item["mesero"] + "";
                    orden.mesa = item["mesa"].ToString();
                    orden.total = item["total"].ToString(); ;
                    orden.pago = Int32.Parse(item["pago"].ToString()); ;
                    ordenList.Add(orden);


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

        private void Button_Clicked(object sender, EventArgs e)
        {
            var pdf = PDFManager.GeneratePDFFromView(this.tabla); // aqui le paso la vista que quiero que vuelva pdf
            var basepath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //var pdfpath = Path.Combine(basepath, $"/mypdf.pdf");
            var pdfpath = basepath + "/Tiket"+id+".pdf";
            pdf.Save(pdfpath);
            try
            {
                Share.RequestAsync(new ShareFileRequest(new ShareFile(pdfpath)));
                
            }
            catch
            {
                
            }

        }
    }
}

