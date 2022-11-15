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

        public void init()
        {



            numeroOrden.Text = "# " + ordenes.id;
            cart = CartMesa(ordenes.id.ToString(), ordenes.mesa);

            ordenlist.ItemsSource = cart;

            foreach (Model.Cart carro in cart)
            {
                if (carro.estado == "2")
                {
                    HoraInicioOrden.Text = ordenes.fecha_start;
                    CronometroOrden.Text = "Preparando ...";
                    HoraEstimadaOrden.Text = ordenes.fecha_estimada;

                    btnIniciar.IsEnabled = false;
                    horas.IsEnabled = false;

                    btnIniciar.IsVisible = false;
                    horas.IsVisible = false;
                }
            }
            /*if (ordenes.fecha_start != null)
            {
                
                if (ordenes.fecha_start.Length > 0) {
                    if (ordenes.estado == "1") {
                        Console.WriteLine("Es igual a uno");
                    }
                    Console.WriteLine("Es igual a uno"+ ordenes.estado);
                    HoraInicioOrden.Text = ordenes.fecha_start;
                    CronometroOrden.Text = "Preparando ...";
                    HoraEstimadaOrden.Text = ordenes.fecha_estimada;
                    
                    btnIniciar.IsEnabled = false;
                    horas.IsEnabled = false;
                }
               

            }*/
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
        public void ExtrasItem(Model.Cart c)
        {

            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/platillo/index.php?op=obtenerExtrasAsItem&iditem=" + c.idItem);
            //Console.WriteLine("http://192.168.1.112/resta/admin/mysql/platillo/index.php?op=obtenerExtrasAsItem&iditem=" + c.idItem);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                if (json.Equals("[]"))
                {
                    c.extras = "0 EXTRAS";
                }
                else
                {
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

            //client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarrito&idOrden=" + id + "&mesa=" + mesa);
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa+"&opc="+2);
           // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 2);
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
                    cartItem.platillo = "X  " + item["nombre"].ToString();
                    cartItem.cantidad = Int32.Parse(item["cantidad"].ToString());

                    if (item["comentario"] != null && item["comentario"].ToString() != "")
                    {
                        cartItem.comentario = item["comentario"].ToString().ToUpper().Replace("-"," ");
                    }
                    else
                    {
                        cartItem.comentario = "SIN COMENTARIOS";
                    }
                    cartItem.estado = item["estado"].ToString();
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
                var h = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                var fecha = DateTime.Now.ToString("yyyy-MM-dd");
                string fechaIn = h; 
                HoraInicioOrden.Text = h.Remove(0, 10).Replace(" ", "");
                h = DateTime.Now.AddMinutes(hora).ToString();
                string fechaFn = h;
                Console.WriteLine(HoraInicioOrden.Text +"+==="+ HoraEstimadaOrden.Text);
                // CronometroOrden.Text = "00:00:00";
                HoraEstimadaOrden.Text = h.Remove(0, 10).Replace(" ", "").Replace("PM", "");
                btnIniciar.IsEnabled = false;
                horas.IsEnabled = false;
                btnIniciar.IsVisible = false;
                horas.IsVisible = false;
                ////Cambiar el estado de la orden // updateOrden
                 string cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateOrden&estado=2" + "&fecha_inicio=" + fecha.Replace("/", "-") + "-"+ HoraInicioOrden.Text.Replace(" ", "-") + "&fecha_estimada=" + fecha.Replace("/", "-") + "-" + HoraEstimadaOrden.Text + "&idCart=" + ordenes.id;
                Console.WriteLine(cadena);

                 popAgregar(cadena);
                collection.ItemsSource = null;
                collection.ItemsSource = Cocina.Ordene();

            }
            else
            {

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
                    // 
                }
                else
                {
                    DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo " + cadena2, "OK");

                }
            }
            int bebidas = Int32.Parse(ObtenerNumeorDeItems(Int32.Parse(ordenes.id+""), 1));
            int platillos = Int32.Parse(ObtenerNumeorDeItems(Int32.Parse(ordenes.id+""), 2));
            int bebidas2 = Int32.Parse(ObtenerNumeorDeItemsPlatillos(Int32.Parse(ordenes.id + ""), 1));
            int platillos2 = Int32.Parse(ObtenerNumeorDeItemsPlatillos(Int32.Parse(ordenes.id + ""), 2));
            Console.WriteLine("Platillos:" + bebidas +" = "+ bebidas2);
            Console.WriteLine("BEBIDAS:" + platillos + " = " + platillos2);
            var h = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            string cadena0 = "";
            if (bebidas == bebidas2 && platillos == platillos2)
            {
                cadena0 = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateOrden&estado=3" + "&fecha_inicio=" + h.Replace("/", "-").Replace(" ", "-") + "&fecha_estimada=" + h.Replace("/", "-").Replace(" ", "-") + "&idCart=" + ordenes.id;
                popAgregar(cadena0);
                string cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateEstadoOrden&idItem=" + ordenes.id;
                popAgregar(cadena);
            }
            collection.ItemsSource = null;
            collection.ItemsSource = Cocina.Ordene();
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
           
        }
        public static string ObtenerNumeorDeItemsPlatillos(int id, int opc)
        {
            string cantidad = "";

            var client1 = new HttpClient();

            client1.BaseAddress = new Uri(("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=ObtenerNumeorDeItemsPlatillos&idCart=" + id + "&opc=" + opc));
            HttpResponseMessage response1 = client1.GetAsync(client1.BaseAddress).Result;
            if (response1.IsSuccessStatusCode)
            {
                var content1 = response1.Content.ReadAsStringAsync().Result;
                string json1 = content1.ToString();

                var jsonArray1 = JArray.Parse(json1.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray1)
                {
                    cantidad = item["cantidad"].ToString();
                }
            }
            return cantidad;
        }

        public static string ObtenerNumeorDeItems(int id, int opc)
        {
            string cantidad = "";

            var client1 = new HttpClient();

            client1.BaseAddress = new Uri(("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=ObtenerNumeorDeItems&idCart=" + id + "&opc=" + opc));
            HttpResponseMessage response1 = client1.GetAsync(client1.BaseAddress).Result;
            if (response1.IsSuccessStatusCode)
            {
                var content1 = response1.Content.ReadAsStringAsync().Result;
                string json1 = content1.ToString();

                var jsonArray1 = JArray.Parse(json1.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray1)
                {
                    cantidad = item["cantidad"].ToString();
                }
            }
            return cantidad;
        }
        void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {

            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
            if (selectedIndex != -1)
            {
                hora = Int32.Parse(picker.SelectedItem.ToString().Replace(" minutos", ""));

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