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
    public partial class ConfirmarPago : Rg.Plugins.Popup.Pages.PopupPage
    {
        Model.Pagos pago;
        public ConfirmarPago(Model.Pagos pago)
        {
            this.pago = pago;
            InitializeComponent();
           
            switch (pago.tipoPago)
            {
                case "1": pago.tipoPago = "EFECTIVO"; break;
                case "2": pago.tipoPago = "TARJETA"; break;
                case "3": pago.tipoPago = "EFECTIVO Y TARJETA"; break;
            }
            tipo.Text = pago.tipoPago;
            monto.Text = pago.monto+"";
            cambio.Text = (pago.monto  -  Double.Parse(pago.total) )+"";
            total.Text = pago.total+"";
            

        }
        private void cerrarPop(object sender, EventArgs e)
        {
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        public void confirmarPago(object sender, EventArgs e) {
            var h = DateTime.Now.ToString("yyyy-MM-dd-HH:MM:ss");

            switch (pago.tipoPago)
            {
                case "EFECTIVO": pago.tipoPago = "1"; break;
                case "TARJETA": pago.tipoPago = "2"; break;
                case "EFECTIVO Y TARJETA": pago.tipoPago = "3"; break;
            }


            string cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=insertarPago&IDpago=1"  +
                                "&tipo=" + pago.tipoPago.ToString() + "&monto=" + pago.monto + "&fecha=" + h.ToString() + "&IDcart=" + pago.idcart;
            Console.WriteLine(cadena);
             var client = new HttpClient();
            client.BaseAddress = new Uri(cadena);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            }
            else
            {
                DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo " + cadena, "OK");

            }
        }

    }
}