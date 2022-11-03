using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerOrden : Rg.Plugins.Popup.Pages.PopupPage
    {
        public List<Model.Cart> cart = new List<Model.Cart>();

        public Model.Cart cartItem = new Model.Cart();
        Model.Ordenes ordenes;
        int hora = 0;

        public VerOrden(Model.Ordenes ordenes)
        {
            this.ordenes = ordenes;
            InitializeComponent();
            
            init();
        }

        public void init() {
            
            numeroOrden.Text = "Orden # " + ordenes.id;
            ordenlist.ItemsSource = CartMesa(ordenes.id.ToString(), ordenes.mesa);
            var hora = new List<string>();
           
            hora.Add("10 minutos");
            hora.Add("15 minutos");
            hora.Add("20 minutos");
            hora.Add("25 minutos");
            hora.Add("30 minutos");
            hora.Add("35 minutos");
            hora.Add("40 minutos");
            hora.Add("45 minutos");

            horas.ItemsSource = hora;
        }

        private void cerrarPop(object sender, EventArgs e)
        {
            this.IsVisible = false;
            //Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }
        public List<Model.Cart> CartMesa(string id, string mesa)
        {

            var sub = new List<Model.Cart>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarrito&idOrden=" + id + "&mesa=" + mesa);

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
            //var _count = DateTime.Now.ToString("HH:MM:ss");
            if (hora != 0)
            {
                var h = DateTime.Now.ToString("HH:MM:ss");

                HoraInicioOrden.Text = "H.I" + h;
                h = DateTime.Now.AddMinutes(hora).ToString();

               // CronometroOrden.Text = "00:00:00";
                HoraEstimadaOrden.Text = "H.E" + h.Remove(0, 9);
                btnIniciar.IsEnabled = false;
                horas.IsEnabled = false;

              /*  Device.StartTimer(
         TimeSpan.FromSeconds(1),
         () =>
         {
            
             _count = DateTime.Now.ToString("HH:MM:ss");


            
             this.CronometroOrden.Text = $"{_count}";
             return true;
         });*/
            }
            else {
                DisplayAlert("INCORRECTP", "SELECCIONA EL TIMO ESTIMADAO", "OK");
            }

          
        }

        void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {

            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
            if (selectedIndex != -1)
            {
                hora= Int32.Parse(picker.SelectedItem.ToString().Replace(" minutos",""));
           
            }
        }
    }
}