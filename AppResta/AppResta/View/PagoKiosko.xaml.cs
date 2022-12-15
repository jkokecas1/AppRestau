using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PagoKiosko : Rg.Plugins.Popup.Pages.PopupPage
    {
        List<Model.Cart> cart; // 2022/12/05 Creado


        public PagoKiosko(List<Model.Cart> cart)
        {
            InitializeComponent();
            this.cart = Kiosco.cart;
            double total = 0.0;
            foreach (Model.Cart c in cart)
            {
                total += c.precio * c.cantidad;
            }
            totoal.Text = total.ToString();
        }
        private void Caja_Clicked(object sender, EventArgs e) // 2022/12/05 Creado
        {
            // 2022/12/06 Modificacion
            tarjeta.IsVisible = false;
            caja.IsVisible = false;
            load.IsVisible = true;


            string cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=crearCart" +
                    "&fecha=" + DateTime.Now.ToString("yyyy-MM-dd HH-MM-ss").Replace(" ", "-") +
                    "&total=" + "0.0" +
                    "&mesa=" + "Kiosko-1" +
                      "&mesero=" + "11";
            popAgregar(cadena);


            foreach (Model.Cart carrito in cart)
            {
                cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=insertarItems" +
                    "&cantidad=" + carrito.cantidad.ToString() +
                    "&idPlatillo=" + carrito.id.ToString() +
                    "&mesa=" + "Kiosko-1" +
                    "&total=" + "0.0" +
                    "&comen=" + carrito.comentario.Replace(" ", "-");

                popAgregar(cadena);
                ///  Console.WriteLine("CASO : " + cadena);

            }
            Device.StartTimer(TimeSpan.FromSeconds(4), () => {
                titulo.Text = "DATOS CORRECTOS";
                descripcion.Text = "IMPRIME TU ORDEN";
                load.Source = "listo.png";
                imprimir.IsVisible = true;
                return false;
            });
       

        }


        public void popAgregar(string c)// 2022/12/05 Creado
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(c);
            Console.WriteLine(c);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                // Main.actualizar();
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            }
            else
            {
                DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo " + c, "OK");

            }
        }
    }
}