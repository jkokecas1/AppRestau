using Newtonsoft.Json.Linq;
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
    public partial class ItemPlatillo : Rg.Plugins.Popup.Pages.PopupPage
    {
        private readonly ListView carrito;


        public List<Model.Cart> cart = new List<Model.Cart>();

         Model.Cart cartItem = new Model.Cart();

        Model.Extras extra = new Model.Extras();
        Model.Platillos platillos;
        string mesasGlb;
        int band = 0, cant; 
       
        List<Model.Extras> listExtra;


        

        

        public ItemPlatillo(Model.Platillos platillo, string mesa, int bandera, List<Model.Cart> cart, ListView carrito = null)
        {
            listExtra = new List<Model.Extras>();
            mesasGlb = mesa;
            //idItems = cart[index].idItem;
            //cant = cantidad;
            this.cart = cart;
            this.carrito = carrito;
            band = bandera;
            InitializeComponent();
            platillos = platillo;
            nombPlatillo.Text = platillos.nombre;
            descPlatillo.Text = platillos.descrip;

            

            extrasListView.ItemsSource = Extras(platillos.id);

        }


        private void cerrarPop(object sender, EventArgs e)
        {
            this.IsVisible = false;
            //Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        public void cantidadPlatillo(object sender, ValueChangedEventArgs e) {
            var value = e.NewValue;
            valCantidad.Text = value.ToString();
        }

        void selectmultiple(object sender, SelectionChangedEventArgs e)
        {
            listExtra.Clear();
            foreach (Model.Extras values in e.CurrentSelection) {
                listExtra.Add(values);
            }

        }

        public void agregarItemCart(object sender, EventArgs e)
        {

            int index= 0;
            int idplatillo = platillos.id;
            int cantidad = Int32.Parse(valCantidad.Text);
            var hora = DateTime.Now.ToString("yyyy-dd-yy HH:MM:ss");
            var fecha = DateTime.Now.ToString("MM-dd-yy");

            
            int extra = 0;
            string comentario = comentTxt.Text;
            
            string cadena = "";
            double total = cantidad + Convert.ToDouble(platillos.precio.Replace(",", "."));

            
            cartItem = new Model.Cart();
            if (cart.Count == 0) // Caso 1: Carrito vacio
            {

                cartItem.id = platillos.id;
                cartItem.platillo = platillos.nombre;
                cartItem.cantidad = cantidad;
                cartItem.precio = Convert.ToDouble(platillos.precio.Replace(",", "."));
                cartItem.total = (int)(cartItem.precio * cartItem.cantidad);

                cart.Add(cartItem);
                //Console.WriteLine("Entra a primer elemento del carrito");
               
            }
            else // Caso 2: Cariito con al menuos un platillo
            {
                int band = 0;
                for (int i = 0; i < cart.Count; i++)
                {
                    // Console.WriteLine("IdCart " + cart[i].id + " platillo id: " + platillo.id);
                    if (cart[i].id == platillos.id) // Caso 2.1: El platillo existe
                    {
                        //Console.WriteLine("Es igual suma 1");
                        cart[i].cantidad += cantidad;
                        cart[i].total = (double)(cart[i].precio * cart[i].cantidad);
                        index = i;
                        band = 1;
                        break;
                    }
                }
                if (band == 0)
                {
                    //Console.WriteLine("Ya exite un platillo y se agrega el otro");
                    cartItem.id = platillos.id;
                    cartItem.platillo = platillos.nombre;
                    cartItem.cantidad = cantidad;
                    cartItem.precio = Convert.ToDouble(platillos.precio.Replace(",", "."));
                    cartItem.total = (double)(cartItem.precio * cartItem.cantidad);
                    cart.Add(cartItem);
                    
                }
                

            }


            carrito.ItemsSource = null;
            carrito.ItemsSource = cart;
            cant = cart[index].cantidad;

            if (band == 0) // CASO 0: SI EL CARITO PARA ESA MESA NO EXITE 
            {
                cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=insertItemPlatillo&idplatillo=" + idplatillo.ToString() + "&cantidad=" + cantidad.ToString() + "&fecha=" + hora+ "&mesa=" + mesasGlb.ToString() + "&total=" + total.ToString() + "&comen=" + comentario;
                popAgregar(cadena);
                Console.WriteLine("CASO 0: " + cadena);
            }
            else if (band == 1) // CASO 1: SI LE CARRITO EXISTE, Y ES EL MISMO PRODUCTO
            {
                cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateItemPlatillo&cantidad=" + (cant+cantidad) + "&idPlatillo=" + platillos.id + "&idItem=" + cart[index].idItem;
                popAgregar(cadena);
                Console.WriteLine("CASO 1: " + cadena);
            }
            else if (band == 2) // CASP 2: SI EL CARRITO EXISTE, Y NO ES EL MISMO PLATILLO
            {
                cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=insertarItems&cantidad=" + cantidad + "&idPlatillo=" + platillos.id +"&mesa="+ mesasGlb;
                popAgregar(cadena);
                Console.WriteLine("CASO 2: "+ cadena);

            }

            if (listExtra.ToArray().Length != 0) // CASO Extras: EN CASO DE QUE AGREGE EXTRAS 
            {
                //Console.WriteLine("YESS: " +listExtra.ToArray().Length);
                for (int i = 0; i < listExtra.ToArray().Length; i++) {
                    cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=agregarExtras&extra=" + listExtra.ToArray()[i].id_esxtra;
                   
                    var client = new HttpClient();
                    client.BaseAddress = new Uri(cadena);
                    HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

                }
            }

            cant = 0;
            
        }

        public void popAgregar(string c){
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
                DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo "+c, "OK");

            }
        }

        public List<Model.Extras> Extras(int id)
        {
            List<Model.Extras> extras = new List<Model.Extras>();
            var sub = new List<Model.Extras>();
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/Platillo/index.php?op=obtenerExtras&id="+id.ToString());
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    extra = new Model.Extras();

                    int idplat = Int32.Parse(item["platillo_id_platillo"].ToString());
                    string nombrePlatillo = item["nombre"].ToString();
                    int idextra = Int32.Parse(item["id_esxtra"].ToString());
                    string nombreExtra = item["extra_nombre"].ToString();
                    double precioExtra = Convert.ToDouble(item["extra_precio"].ToString().Replace(",", "."));
                   
                    extra.platillo_id_platillo = idplat;
                    extra.nombre = nombrePlatillo;
                    extra.id_esxtra = idextra;
                    extra.extra_nombre = nombreExtra;
                    extra.extra_precio = precioExtra;


                    extras.Add(extra);

                }
                return extras;
            }
            else
            {
                return null;
            }
        }
    }
}