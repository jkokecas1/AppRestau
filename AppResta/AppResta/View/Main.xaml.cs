using AppResta.ViewModel;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Main : ContentPage
    {
        int band = 0, ordenID;
        string mesaglb;
        public static List<Model.Cart> cart = new List<Model.Cart>();

        public Model.Cart cartItem = new Model.Cart();
        public static double totalpay = 0.0;
        public Main(bool _Token, int idOrden = 0, string nombre = "", string mesa = "MES-0")
        //public Main(Object[] datos)
        {
            ordenID = idOrden;
            mesaglb = mesa;
            if (_Token == false)
            {
                Navigation.PushAsync(new Login());
                //Console.WriteLine("No token");
            }
            else
            {

                InitializeComponent();
                // Refreshplatillos.IsRefreshing = true;
                testListView.ItemsSource = Categorias2();


                if (idOrden != 0)
                {
                    cart.Clear();
                    totalpago.Text = "0";
                    totalpay = 0;
                    cart = CartMesa(idOrden.ToString(), mesaglb);
                    test2ListView.ItemsSource = null;
                    test2ListView.ItemsSource = cart;
                    test2ListView.IsRefreshing = false;
                    //Console.WriteLine(cart.Count);
                    for (int i = 0; i < cart.Count; i++)
                    {
                        totalpay += cart[i].total;
                    }
                    totalpago.Text = totalpay.ToString();

                }
                else
                {
                    cart.Clear();
                }


                BindingContext = new MainViewModel(Navigation, _Token);
                MesaTexto.Text = "# Mesa:" + mesa;
                NombTexto.Text = "Mesero: " + nombre;
                OrdenTexto.Text = "# Orden: " + idOrden.ToString();

            }

        }



        public void SelectItem(object sender, SelectedItemChangedEventArgs e)
        {
            var itme = e.SelectedItem as Model.Cart;
            Console.WriteLine(itme.id);



        }
        //----- REFRESCA EL CONTENIDO DE LA LISTA DE CARRITO ------//
        private void RefreshCart_Refreshing(object sender, EventArgs e)
        {


            if (test2ListView.ItemsSource != null)
            {
                Task.Delay(700);
                ordenID = Int32.Parse(IdOrden(mesaglb)[0]);

                cart.Clear();

                cart = CartMesa(ordenID + "", mesaglb);

                if (cart != null)
                {
                    test2ListView.ItemsSource = null;
                    test2ListView.ItemsSource = cart;
                    RefreshCart.IsRefreshing = false;
                }
                actualizar(test2ListView, totalpago);
            }
            else
            {
                Task.Delay(100);
                RefreshCart.IsRefreshing = false;
            }



        }
        //----- REFRESCA EL CONTENIDO DE LA LISTA DE CATEGORIA ------//
        private void RefreshMesas_Refreshing(object sender, EventArgs e)
        {
            Task.Delay(700);
            band = 0;
            testListView.ItemsSource = Categorias2();

            // Refreshplatillos.IsRefreshing = false;

        }

        //----- REFRESCA EL CONTENIDO DE LA LISTA DE CARRITO ------//
        private void SwipeItem_Editar(object sender, EventArgs e)
        {
            if (IdOrden(mesaglb)[1] == "3" || IdOrden(mesaglb)[1] == "2")
            {
                PopupNavigation.Instance.PushAsync(new PopError("El platillo se esat preparando"));
            }
            else
            {
                string carito = ((MenuItem)sender).CommandParameter.ToString();
                // Console.WriteLine(carito);
                //int idItem = Int32.Parse(((MenuItem)sender).CommandParameter.ToString());
                Model.Platillos platillo = new Model.Platillos(); ;
                int index = 0;
                for (int i = 0; i < cart.Count; i++)
                {
                    if (cart[i].platillo == carito) // Caso 2.1: El platillo existe
                    {
                        var client = new HttpClient();
                        index = i;
                        client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/Platillo/index.php?op=obtenerPlatillos&idPlatillo=" + cart[i].platillo.Replace(" ", "%20"));
                        HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
                        if (response.IsSuccessStatusCode)
                        {

                            var content = response.Content.ReadAsStringAsync().Result;
                            string json = content.ToString();

                            var jsonArray = JArray.Parse(json.ToString());

                            foreach (var item in jsonArray)
                            {

                                if (item["nombre"].ToString() == carito)
                                {
                                    //Console.WriteLine("eNTRE "+carito);
                                    platillo.id = Int32.Parse(item["id"].ToString());
                                    platillo.nombre = item["nombre"].ToString();
                                    platillo.descrip = item["descrip"].ToString();
                                    platillo.precio = item["precio"].ToString();
                                    platillo.url = item["url"].ToString().Remove(0, 23);
                                    platillo.estatus = Int32.Parse(item["estatus"].ToString());
                                    platillo.categoria = item["categoria"].ToString();
                                    platillo.clasificacion = item["clasificacion"].ToString();
                                    platillo.subcategoria = item["subcategoria"].ToString();

                                }
                            }
                            break;
                        }

                    }
                }


                PopupNavigation.Instance.PushAsync(new ItemPlatillo(platillo, mesaglb, bandera: 1, cart, test2ListView, cart[index].idItem, totalpago:totalpago));

            }

        }

        private void SwipeItem_Eliminar(object sender, EventArgs e)
        {
            if (IdOrden(mesaglb)[1] == "2" || IdOrden(mesaglb)[1] == "1")
            {
                PopupNavigation.Instance.PushAsync(new PopError("El platillo se esat preparando"));
            }
            else
            {
                var carito = ((MenuItem)sender).CommandParameter.ToString();


                DisplayAlert("Estas seguro de Eliminarlo" + carito, "OK", "CANCELAR");
                for (int i = 0; i < cart.Count; i++)
                {
                    if (cart[i].idItem == Int32.Parse(carito)) // Caso 2.1: El platillo existe
                    {
                        cart.RemoveAt(i);
                        //Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=updateItems&estado=" + carito);
                        GET_DATOS("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=updateItems&estado=" + carito);

                    }
                }

                test2ListView.ItemsSource = null;
                test2ListView.ItemsSource = cart;

            }


            // Actualizar el estado del item seleccionado

        }


        private void Oredenar_orden(object sender, EventArgs e)
        {
            string total = totalpago.Text;
            /*
            double total = 0.0;

            var client1 = new HttpClient();
            client1.BaseAddress = new Uri(("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=ObtenerPrecioItems&idCart=" + ordenID));
            HttpResponseMessage response1 = client1.GetAsync(client1.BaseAddress).Result;
            if (response1.IsSuccessStatusCode)
            {
                var content1 = response1.Content.ReadAsStringAsync().Result;
                string json1 = content1.ToString();

                var jsonArray1 = JArray.Parse(json1.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray1)
                {
                    total += Double.Parse(item["precio"].ToString().Replace(",", "."));
                }
            }*/

            if (total != "0")
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerEstadoCart&idOrden=" + ordenID));
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    string json = content.ToString();

                    var jsonArray = JArray.Parse(json.ToString());

                    //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                    foreach (var item in jsonArray)
                    {
                        if (item["estado"].Equals("3"))
                        {
                            GET_DATOS("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=updateEstadoOrdenMesas&estado=0&id=" + ordenID + "&total=" + total);
                            Navigation.PushAsync(new View.Ordenes(this), false);
                            break;
                        }
                        else
                        {
                            GET_DATOS("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=updateEstadoOrdenMesas&estado=1&id=" + ordenID + "&total=" + total);
                            Navigation.PushAsync(new View.Ordenes(this), false);
                            break;
                        }
                    }
                }


            }
            else
            {
                PopupNavigation.Instance.PushAsync(new PopError("AGREGA PRODUCTOS AL CARRITO ANTES DE ORDENAR"));
            }

        }

        public void select_Item(object sender, SelectionChangedEventArgs e)
        {

            UpdateSelectionData(e.PreviousSelection, e.CurrentSelection);
        }

        public void UpdateSelectionData(IEnumerable<object> previousSelectedContact, IEnumerable<object> currentSelectedContact)
        {
            double totalpay = 0.0;


            if (band == 0)
            {
                var i = currentSelectedContact.FirstOrDefault(); // Obtenemos el indice de la lista
                var categoria = currentSelectedContact.FirstOrDefault() as Model.Categorias;

                if (SubCategorias(categoria.id) != null)
                {
                    testListView.ItemsSource = SubCategorias(categoria.id);

                }
                else
                {

                    //var platillo = currentSelectedContact.FirstOrDefault() as Model.Platillos;
                    testListView.ItemsSource = Platillos("&idcate=" + categoria.nombre);
                }
            }
            else if (band == 1)
            {
                var subcategoria = currentSelectedContact.FirstOrDefault() as Model.SubCategorias;
                var i = currentSelectedContact.FirstOrDefault(); // Obtenemos el indice de la lista
                var platillo = currentSelectedContact.FirstOrDefault() as Model.Platillos;

                testListView.ItemsSource = Platillos("&idsub=" + subcategoria.nombre);

            }
            else if (band == 2)
            {

                var platillo = currentSelectedContact.FirstOrDefault() as Model.Platillos;
                cartItem = new Model.Cart();
                //DisplayAlert(platillo.nombre, " Nombre :" + platillo.precio + "\n Categoria:" + platillo.descrip,"Cancelar", "Ok");

                //ItemPlatillo itemPlatillo = new ItemPlatillo(platillo, mesaglb, 0);

                //PopupNavigation.Instance.PushAsync(itemPlatillo);

                /*
                * CASO 1: CREO EL CARRITO Y AGREGO EL ITEM SELECCIONADO
                * CASO 2: EL CARRITO EXITE, Y EL ITEM EXISTE, EL ITEME AUMENTA SU CANTIDAD
                * CASO 3: EL CARRRITO EXISTE, EL ITEM NO ESTA EN EL CARRITO, SE AGREGA
                * 
                *  1 - INSERT CARTS, INSERT EL ITEM, SELECT CARTS, INSERTAR LA RELACCION(ITEM,CART)
                *  2 - UPDATE CANTIDAD ITEM
                *  3 - INSERTAMOS ITEM,SELECT CARTS, INSERT RELACION(ITEM, CART)
                */
                if (IdOrden(mesaglb)[1] == "3")
                {

                }

                if (cart.Count == 0 && IdOrden(mesaglb)[1] != "3") // Caso 0: Carrito vacio
                {

                    PopupNavigation.Instance.PushAsync(new ItemPlatillo(platillo, mesaglb, bandera: 0, cart, test2ListView, totalpago: totalpago));


                }
                else
                { // Caso 1

                    int band = 0;
                    int index = 0;
                    for (int i = 0; i < cart.Count; i++)
                    {
                        if (cart[i].id == platillo.id && platillo != null) // Caso 2.1: El platillo existe
                        {
                            index = i;
                            band = 1;
                            break;
                        }
                    }

                    if (band == 1)
                    { // CASO 1.1
                      // itemPlatillo = new ItemPlatillo(platillo, mesaglb, bandera: 1, cart, test2ListView);
                        PopupNavigation.Instance.PushAsync(new ItemPlatillo(platillo, mesaglb, bandera: 1, cart, test2ListView,totalpago: totalpago));
                    }
                    else if (band == 0) // CASO 1.2
                    {

                        //Console.WriteLine("Ya exite un platillo y se agrega el otro");
                        // itemPlatillo = new ItemPlatillo(platillo, mesaglb, bandera: 2, cart, test2ListView);
                        PopupNavigation.Instance.PushAsync(new ItemPlatillo(platillo, mesaglb, bandera: 2, cart, test2ListView,totalpago: totalpago));

                    }

                }

            }
          


        }

        public static void actualizar(ListView test2ListView, Label totalpago)
        {
           // totalpago.Text = "0.0";
            totalpay = 0;
            test2ListView.ItemsSource = null;
            test2ListView.ItemsSource = cart;
            test2ListView.IsRefreshing = false;

            for (int i = 0; i < cart.Count; i++)
            {
                totalpay += cart[i].total;
            }
            //Console.WriteLine(totalpay);
            totalpago.Text = totalpay.ToString();
        }

        public void returnCategorias(object sender, EventArgs e)
        {
            band = 0;
            testListView.ItemsSource = Categorias2();

        }


        public void GET_DATOS(string c)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(c);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                // DisplayAlert("Eliminado","", "OK");
            }

        }
        public List<Model.Categorias> Categorias2()
        {
            Model.Categorias categoria;
            var userInfo = new List<Model.Categorias>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/categoria/index.php?op=obtenerCategorias");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                ;
                var jsonArray = JArray.Parse(json.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray)
                {
                    categoria = new Model.Categorias();
                    int ids = Int32.Parse(item["id"].ToString());
                    string nombre = item["nombre"].ToString();
                    string estado = item["estatus"].ToString();
                    // Console.WriteLine(item["nombre"].ToString());
                    categoria.id = ids;
                    categoria.nombre = nombre;
                    categoria.estatus = estado;
                    userInfo.Add(categoria);
                }
                //userInfo.Add(categoria);
                //testListView.ItemsSource = userInfo
                return userInfo;
            }
            else
            {
                return null;
            }
        }
        public List<Model.SubCategorias> SubCategorias(int id)
        {
            band = 1;
            Model.SubCategorias subcategoria;
            var sub = new List<Model.SubCategorias>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/categoria/index.php?op=obtenerSubCategorias&id=" + id);

            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                ;
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    subcategoria = new Model.SubCategorias();
                    int ids = Int32.Parse(item["id"].ToString());
                    string nombre = item["nombre"].ToString();
                    int estado = Int32.Parse(item["estatus"].ToString());
                    int idcategoria = Int32.Parse(item["idCategoria"].ToString());
                    //Console.WriteLine(item["nombre"].ToString());
                    subcategoria.id = ids;
                    subcategoria.nombre = nombre;
                    subcategoria.estatus = estado;
                    subcategoria.idCategoria = idcategoria;
                    sub.Add(subcategoria);
                }
                if (sub.Count() == 0)
                {
                    return null;
                }
                else
                {
                    return sub;
                }

            }
            else
            {

                return null;
            }
        }


        //public List<Model.Platillos> Platillos(int idCategoria, int idSbCategoria)
        public List<Model.Platillos> Platillos(string opc)
        {
            band = 2;
            Model.Platillos platillo;
            var sub = new List<Model.Platillos>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/Platillo/index.php?op=obtenerPlatillos" + opc);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();

                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    platillo = new Model.Platillos();
                    int id = Int32.Parse(item["id"].ToString());
                    string nombre = item["nombre"].ToString();
                    string descrip = item["descrip"].ToString();
                    string precio = item["precio"].ToString();
                    string urls = item["url"].ToString().Remove(0, 23);
                    int estatus = Int32.Parse(item["estatus"].ToString());
                    string categoria = item["categoria"].ToString();
                    string clasificacion = item["clasificacion"].ToString();
                    string subcategoria = item["subcategoria"].ToString();

                    //Console.WriteLine(urls);
                    var byteArray = Convert.FromBase64String(urls);

                    Stream stream = new MemoryStream(byteArray);
                    var imageSource = ImageSource.FromStream(() => stream);
                    //MyImage.Source = imageSource;
                    platillo.id = id;
                    platillo.nombre = nombre;
                    platillo.descrip = descrip;
                    platillo.precio = precio;
                    platillo.url = urls;
                    platillo.estatus = estatus;
                    platillo.categoria = categoria;
                    platillo.clasificacion = clasificacion;
                    platillo.subcategoria = subcategoria;


                    sub.Add(platillo);

                }
                return sub;
            }
            else
            {
                return null;
            }
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



        public string[] IdOrden(string mesa)
        {

            var client = new HttpClient();
            string[] orden = new string[2];
            string ordenID = "";
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoOrden&mesa=" + mesa);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    orden[0] = item["ordern"].ToString();
                    orden[1] = item["estado"].ToString();
                }

            }
            //Console.WriteLine(ordenID);
            return orden;
        }

    }


}
