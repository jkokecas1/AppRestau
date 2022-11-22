
using Newtonsoft.Json.Linq;
using PdfSharp.Xamarin.Forms;
using PdfSharpCore;
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
        public Model.Cart cartItem;

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
            //BindingContext = new ViewModel.TiketViewModel(this.Navigation, id.ToString(), ordenes.mesa);
            //productos.IsVisible = true;
            //linea1.IsVisible = true;
            //linea2.IsVisible = true;
            NombTicket.Text = "Ticket  Nº " + id;
            //NombMesero.Text = "Mesero                                            " + ordenes.mesero;
            //FechaAbierto.Text = "Abierto                                            " + ordenes.fecha_orden;
            //FechaCerada.Text = "Cuenta cerrada                             " + ordenes.fecha_estimada;
            //NumMesa.Text = "Mesa n0                                         " + ordenes.mesa;
            PagoTotal.Text = ordenes.total;
            TotalExtras.Text = "" + ordenes.pago;
            Efectivo.Text = ordenes.total;
            Tarjeta.Text = "" + ordenes.pago;
            Pago.Text = "Pago";
            Monto.Text = "Monto";
            // TP.Text = "Tiempo  25 MIN";
             productos.ItemsSource = CartMesa(id + "", ordenes.mesa);

            


        }
        public void init(List<Model.Ordenes> orden) {
            Model.Ordenes ordenes = new Model.Ordenes();

            foreach (Model.Ordenes ordenes1 in orden)
            {
                if (ordenes1.id.ToString() == id)
                {
                    ordenes = ordenes1;
                }
            }

            //productos.IsVisible = true;
            //linea1.IsVisible = true;
            //linea2.IsVisible = true;
            NombTicket.Text = "Ticket  Nº " + id;
            //NombMesero.Text = "Mesero                                            " + ordenes.mesero;
            //FechaAbierto.Text = "Abierto                                            " + ordenes.fecha_orden;
            //FechaCerada.Text = "Cuenta cerrada                             " + ordenes.fecha_estimada;
            //NumMesa.Text = "Mesa n0                                         " + ordenes.mesa;
            PagoTotal.Text = ordenes.total;
            TotalExtras.Text = "" + ordenes.pago;
            Efectivo.Text = ordenes.total;
            Tarjeta.Text = "" + ordenes.pago;
            Pago.Text = "Pago";
            Monto.Text = "Monto";
            // TP.Text = "Tiempo  25 MIN";
            // productos.ItemsSource = CartMesa(id + "", ordenes.mesa);
            
            //this.BindingContext = new ViewModel.TiketViewModel(Navigation, ordenes);
        }

         public List<Model.Cart> CartMesa(string id, string mesa)
        {
            List<Model.Cart> cart = new List<Model.Cart>();
            var sub = new List<Model.Cart>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoFinal&idOrden=" + id + "&mesa=" + mesa);
           // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarrito&idOrden=" + id + "&mesa=" + mesa);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    cartItem = new Model.Cart();
                    cartItem.id = Int32.Parse(item["id"].ToString());
                    cartItem.idItem = Int32.Parse(item["idItem"].ToString());
                    cartItem.platillo = item["nombre"].ToString();
                    cartItem.cantidad = Int32.Parse(item["cantidad"].ToString());
                    cartItem.precio = Convert.ToDouble(item["precio"].ToString().Replace(",", "."));
                    cartItem.total = (double)(cartItem.precio * cartItem.cantidad);
                    cartItem.visible = "false";

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
            var pdfpath = basepath + "/Tiket" + id + ".pdf";
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

