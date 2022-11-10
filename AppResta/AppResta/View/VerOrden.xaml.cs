using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
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
        List<Model.Ordenes> aux;
        CollectionView collection;
        public VerOrden(Model.Ordenes ordenes, List<Model.Ordenes> aux, CollectionView collection)
        {
            this.ordenes = ordenes;
            this.aux = aux;
            this.collection = collection;
            
            InitializeComponent();
            
            init();
        }

        public void init() {
            
            

            numeroOrden.Text = "Orden # " + ordenes.id;
            cart = CartMesa(ordenes.id.ToString(), ordenes.mesa);

            ordenlist.ItemsSource = cart;
           
           
            if (ordenes.fecha_start != null)
            {
                if (ordenes.fecha_start.Length > 0) {
                    HoraInicioOrden.Text = ordenes.fecha_start;
                    CronometroOrden.Text = "Preparando ...";
                    HoraEstimadaOrden.Text = ordenes.fecha_estimada;
                    
                    btnIniciar.IsEnabled = false;
                    horas.IsEnabled = false;
                }
               

            }
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
            //collection.SelectedItems = null;
            
            collection.ItemsSource = null;
            collection.ItemsSource = Cocina.Ordene();
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            //
        }
        public void ExtrasItem(Model.Cart c) {

            var client = new HttpClient();
           
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/platillo/index.php?op=obtenerExtrasAsItem&iditem=" + c.idItem);

            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                if (json.Equals("[]"))
                {
                    c.extras = "0 EXTRAS";
                }
                else {
                    var jsonArray = JArray.Parse(json.ToString());

                    foreach (var item in jsonArray)
                    {
                        if (Int32.Parse(item["item"].ToString()) == c.idItem)
                        {
                            c.extras += item["extra"].ToString() + ", ";
                        }
                    }
                }
            }
        }

        public List<Model.Cart> CartMesa(string id, string mesa)
        {
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

                    cartItem.id = Int32.Parse(item["id"].ToString());
                    cartItem.idItem = Int32.Parse(item["idItem"].ToString());
                    cartItem.platillo = item["nombre"].ToString();
                    cartItem.cantidad = Int32.Parse(item["cantidad"].ToString());
                    if (item["comentario"] != null && item["comentario"].ToString() != "")
                    {
                        cartItem.comentario = item["comentario"].ToString().ToUpper().Replace("-"," ");
                    }
                    else {
                        cartItem.comentario = "SIN COMENTARIOS";
                    }
                   
                    cartItem.precio = Convert.ToDouble(item["precio"].ToString().Replace(",", ".")); 
                    cartItem.total = (double)(cartItem.precio * cartItem.cantidad);
                    cartItem.visible = "false";

                    ExtrasItem(cartItem);


                    cart.Add(cartItem);

                }

                return cart;
            }
            else
            {
                return null;
            }
        }
        
        private void Button_Iniciar(object sender, EventArgs e)
        {
            //var _count = DateTime.Now.ToString("HH:MM:ss");
            if (hora != 0)
            {
                var h = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
                string fechaIn = h;
                HoraInicioOrden.Text = h.Remove(0, 9).Replace(" ", "");
                h = DateTime.Now.AddMinutes(hora).ToString();
                string fechaFn = h;
               // CronometroOrden.Text = "00:00:00";
                HoraEstimadaOrden.Text = h.Remove(0, 9).Replace(" ","").Replace("PM","");
                btnIniciar.IsEnabled = false;
                horas.IsEnabled = false;

                //Cambiar el estado de la orden // updateOrden
                string cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateOrden&estado=2"+ "&fecha_inicio="+ fechaIn.Replace("/","-").Replace(" ","-")+"&fecha_estimada=" + fechaIn.Replace("/", "-").Replace("PM","").Replace(" ", "-") + "&idCart=" + ordenes.id;
                //Console.WriteLine(cadena);
                
                popAgregar(cadena);
                collection.ItemsSource = null;
                collection.ItemsSource = Cocina.Ordene();

            }
            else {
                
                PopupNavigation.Instance.PushAsync(new PopError("SELECCIONA EL TIEMPO ESTIMADAO"));
            }

          
        }

        private void Button_Terminar(object sender, EventArgs e)
        {

            foreach (Model.Cart car in cart)
            {
                string cadena2 = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateEstadoItem&idItem=" + car.idItem;
                Console.WriteLine(cadena2);
                var clien2t = new HttpClient();
                clien2t.BaseAddress = new Uri(cadena2);
                HttpResponseMessage respons2e = clien2t.GetAsync(clien2t.BaseAddress).Result;
                if (respons2e.IsSuccessStatusCode)
                {


                    // Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
                }
                else
                {
                    DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo " + cadena2, "OK");

                }
            }

            string cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateEstadoOrden&idItem=" + ordenes.id;
            // Console.WriteLine(cadena);  
            var client = new HttpClient();
            client.BaseAddress = new Uri(cadena);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                collection.ItemsSource = null;
                collection.ItemsSource = Cocina.Ordene();
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            }
            else
            {
                DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo " + cadena, "OK");

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

        public void popAgregar(string c)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(c);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                //this.IsVisible = false;
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            }
            else
            {
                DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo " + c, "OK");

            }
        }
    }
}