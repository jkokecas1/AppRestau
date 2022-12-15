using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        Model.Extras extraItem = new Model.Extras();
        Model.Platillos platillos;
        string mesasGlb;
        int band = 0, cant, item = 0, cantsteper;

        List<Model.Extras> listExtra, listaExtraItems;

        Label totalpago;
        Model.Empleado empleado;


        public ItemPlatillo(Model.Platillos platillo, string mesa, int bandera, List<Model.Cart> cart, ListView carrito = null, int item = 0, Label totalpago = null, Model.Empleado empleado = null, int cantsteper = 0, int[] size = null)
        {

            listExtra = new List<Model.Extras>();
            mesasGlb = mesa;
            this.item = item;
            this.totalpago = totalpago;
            this.empleado = empleado;
            this.cantsteper = cantsteper;
            //idItems = cart[index].idItem;
            //cant = cantidad;
            this.cart = cart;
            this.carrito = carrito;
            band = bandera;
            InitializeComponent();

            if (size != null)
            {
                frameitem.Margin = new Thickness(size[0], size[1], size[0], size[1]);
            }
            else
            {
                frameitem.Margin = new Thickness(200, 10, 200, 10);
            }

            platillos = platillo;
            nombPlatillo.Text = platillos.nombre;
            descPlatillo.Text = platillos.descrip;
            if (cantsteper != 0)
            {
                stepper.Value = cantsteper;
                btn_agregar.Text = "ACTUALIZAR";
                band = 2;
            }
            else
            {
                stepper.Value = 1;
                band = 1;
                btn_agregar.Text = "AGREGAR";
            }

            listaExtraItems = Extras(platillos.id);

            extrasListView.ItemsSource = listaExtraItems;

            this.totalpago = totalpago;

            //extrasListView.SelectionChanged += selectmultiple;

        }



        private void cerrarPop(object sender, EventArgs e)
        {
            //this.IsVisible = false;
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        public void cantidadPlatillo(object sender, ValueChangedEventArgs e)
        {
            var value = e.NewValue;
            valCantidad.Text = value.ToString();
        }

        void selectmultiple(object sender, SelectionChangedEventArgs e)
        {
            string previous = (e.PreviousSelection.FirstOrDefault() as Model.Extras)?.extra_nombre;
            string current = (e.CurrentSelection.FirstOrDefault() as Model.Extras)?.extra_nombre;


            listExtra.Clear();
            foreach (Model.Extras values in e.CurrentSelection)
            {
                listExtra.Add(values);
            }

        }

        public void agregarItemCart(object sender, EventArgs e)
        {
            //Console.WriteLine(stepper.Value.ToString());
            int valSteper = Int32.Parse(stepper.Value + "");
            int index = 0;
            int idplatillo = platillos.id;
            int cantidad = Int32.Parse(valCantidad.Text);
            var hora = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss").Replace(" ", "-");
            var fecha = DateTime.Now.ToString("MM-dd-yy");
            string comentario = "SIN COMENTARIOS";

            int extra = 0;
            if (comentTxt.Text != null)
            {
                comentario = comentTxt.Text;
            }
            string cadena = "", extras = "";

            double total = cantidad + Convert.ToDouble(platillos.precio.Replace(",", "."));

            for (int i = 0; i < listExtra.ToArray().Length; i++)
            {
                for (int j = 0; j < cart.ToArray().Length; j++)
                {
                    if (listExtra.ToArray()[i].platillo_id_platillo == cart.ToArray()[j].id)
                        extras += listExtra.ToArray()[i].extra_nombre + ", ";
                }
            }

            cartItem = new Model.Cart();

            // Si el carrito esta vacio
            if (cart.Count == 0 && band == 1)
            {
                cartItem.id = platillos.id;
                cartItem.platillo = platillos.nombre;
                cartItem.cantidad = cantidad;
                cartItem.precio = Convert.ToDouble(platillos.precio.Replace(",", "."));
                cartItem.total = (int)(cartItem.precio * cartItem.cantidad);
                cartItem.extras = extras;
                cart.Add(cartItem);
                band = 0;
            }
            // SI SE QUIERE ACTUALIZAR LOS DATOS DEL ITEM
            else
            {
                if (band == 1)
                {
                    cartItem.id = platillos.id;
                    cartItem.platillo = platillos.nombre;
                    cartItem.cantidad = cantidad;
                    cartItem.precio = Convert.ToDouble(platillos.precio.Replace(",", "."));
                    cartItem.total = (int)(cartItem.precio * cartItem.cantidad);
                    cartItem.extras = extras;
                    cart.Add(cartItem);
                    band = 2;
                }
                else if (band == 2)
                {
                    extras = "";
                    for (int i = 0; i < listExtra.ToArray().Length; i++)
                    {
                        for (int j = 0; j < cart.ToArray().Length; j++)
                        {
                            if (listExtra.ToArray()[i].platillo_id_platillo == cart.ToArray()[j].id)
                                extras += listExtra.ToArray()[i].extra_nombre + ", ";
                        }
                    }
                    int contador=0 ;
                    for (int i = 0; i < platillos.subcategoria.Length; i++) {
                        if (platillos.subcategoria[i] == ',') {
                            contador++;
                        }
                    }
                  
                    for (int i = 0; i < cart.Count; i++)
                    {
                                                                                      // SubCategorias == Extras    
                        if (cart[i].platillo == platillos.nombre && cart[i].extras == platillos.subcategoria) // Caso 2.1: El platillo existe
                        {
                            if (cantsteper != 0)
                            {
                                if (valSteper > cart[i].cantidad || valSteper == cart[i].cantidad)
                                {
                                    cart[i].cantidad = valSteper;
                                }
                                else
                                {
                                    int aux = cart[i].cantidad - valSteper;
                                    cart[i].cantidad -= aux;
                                }
                            }
                            else
                            {
                                cart[i].cantidad += valSteper;
                            }

                            cart[i].extras = extras;

                            cart[i].total = (double)(cart[i].precio * cart[i].cantidad);
                            index = i;
                            band = 1;
                            break;
                        }
                    }


                }
            }

           /* if (cart.Count == 0)
            {

                cartItem.id = platillos.id;
                cartItem.platillo = platillos.nombre;
                cartItem.cantidad = cantidad;
                cartItem.precio = Convert.ToDouble(platillos.precio.Replace(",", "."));
                cartItem.total = (int)(cartItem.precio * cartItem.cantidad);

                cart.Add(cartItem);
            }
            else // Caso 2: Cariito con al menuos un platillo
            {
                ///int band = 0;
                for (int i = 0; i < cart.Count; i++)
                {
                    if (extras.Length > 0 && cart[i].platillo == platillos.nombre)
                    {
                        extras = cart[i].extras;
                    }
                    if (cart[i].platillo == platillos.nombre && cart[i].extras == extras) // Caso 2.1: El platillo existe
                    {
                        // Add to cart
                        // cart[i].cantidad < steper ? or > al steper?
                        // But stemper == 1 
                        if (cantsteper != 0)
                        {
                            if (valSteper > cart[i].cantidad || valSteper == cart[i].cantidad)
                            {
                                cart[i].cantidad = valSteper;
                            }
                            else
                            {
                                int aux = cart[i].cantidad - valSteper;
                                cart[i].cantidad -= aux;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("00000");
                            cart[i].cantidad += valSteper;
                        }

                        Console.WriteLine("-----" + cart[i].cantidad);
                        cart[i].total = (double)(cart[i].precio * cart[i].cantidad);
                        index = i;

                        band = 1;
                        break;
                    }
                    else
                    {
                        band = 0;
                    }
                }
                if (band == 0)
                {
                    cartItem.id = platillos.id;
                    cartItem.platillo = platillos.nombre;
                    cartItem.cantidad = cantidad;
                    cartItem.precio = Convert.ToDouble(platillos.precio.Replace(",", "."));
                    cartItem.total = (double)(cartItem.precio * cartItem.cantidad);
                    cart.Add(cartItem);
                }
            }
           */

            carrito.ItemsSource = null;
            carrito.ItemsSource = cart;

            //Main.test2ListView = cart;
            cant = cart[index].cantidad;



            if (band == 0) // CASO 0: SI EL CARITO NO EXISTE PARA LA MESA
            {
                cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=insertItemPlatillo&idplatillo=" + idplatillo.ToString() + "&cantidad=" + cantidad.ToString() + "&fecha=" + hora.Replace(" ", "-") + "&mesa=" + mesasGlb.ToString() + "&total=" + total.ToString() + "&comen=" + comentario.Replace(" ", "-") + "&mesero=" + empleado.id;

                popAgregar1(cadena);
                Console.WriteLine("CASO : " + cadena);
            }
            else if (band == 1) // CASO 1: SI LE CARRITO EXISTE, Y ES EL MISMO PRODUCTO
            {

                cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateItemPlatillo&cantidad=" + (cant) + "&idPlatillo=" + platillos.id + "&idItem=" + cart[index].idItem + "&comen=" + comentario.Replace(" ", "-") + "";
                popAgregar1(cadena);
                Console.WriteLine("CASO 1: " + cadena);
            }
            else if (band == 2) // CASP 2: SI EL CARRITO EXISTE, Y NO ES EL MISMO PLATILLO
            {
                cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=insertarItems&cantidad=" + cantidad + "&idPlatillo=" + platillos.id + "&mesa=" + mesasGlb + "&total=" + total.ToString() + "&comen=" + comentario.Replace(" ", "-");
                /* foreach (var item in cart) {
                     if (item.platillo == platillos.nombre) {
                         //cadena = cadena + item.id;
                        // Console.WriteLine(item.idItem);
                         break;
                     }
                 }*/

                popAgregar1(cadena);
                Console.WriteLine("CASO 2: " + cadena);


            }


            if (listExtra.ToArray().Length != 0) // CASO Extras: EN CASO DE QUE AGREGE EXTRAS 
            {
                //Console.WriteLine("YESS: " +listExtra.ToArray().Length);
                for (int i = 0; i < listExtra.ToArray().Length; i++)
                {
                    cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=agregarExtras&extra=" + listExtra.ToArray()[i].id_esxtra;

                    var client = new HttpClient();
                    client.BaseAddress = new Uri(cadena);
                    HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

                }
            }

            cant = 0;

            Main.actualizar(carrito, totalpago);
        }

        public void popAgregar(string c, Dictionary<string, string> data)
        {
            Uri RequestUri = new Uri(c);
            var client = new HttpClient();
            var json = JsonConvert.SerializeObject(data);
            var contentJSON = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(RequestUri, contentJSON).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            }
            else
            {
                DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo " + c, "OK");
            }

            /* var client = new HttpClient();
             client.BaseAddress = new Uri(c);
             HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
             if (response.IsSuccessStatusCode)
             {
                 // Main.actualizar();
                 Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
             }
             else
             {
                 DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo " + c, "OK");

             }*/
        }
        public void popAgregar1(string c)
        {



            var client = new HttpClient();
            client.BaseAddress = new Uri(c);
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
        public List<Model.Extras> Extras(int id)
        {
            List<Model.Extras> extras = new List<Model.Extras>();
            var sub = new List<Model.Extras>();
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/Platillo/index.php?op=obtenerExtras&id=" + id.ToString());
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    extra = new Model.Extras();

                    extra.platillo_id_platillo = Int32.Parse(item["platillo_id_platillo"].ToString());
                    extra.nombre = item["nombre"].ToString();
                    extra.id_esxtra = Int32.Parse(item["id_esxtra"].ToString());
                    extra.extra_nombre = item["extra_nombre"].ToString();
                    extra.extra_precio = Convert.ToDouble(item["extra_precio"].ToString().Replace(",", "."));

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