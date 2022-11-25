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

        public List<Model.Categorias> CATEGORIAS;
        public List<Model.SubCategorias> SUBCATEGORIAS;

        public static string[] mesa_orden;
        Model.Empleado empleado;


        public Main(bool _Token, bool band, int idOrden = 0, Model.Empleado empleado= null, string mesa = "MES-0", List<Model.Cart> lsit=null, List<Model.Categorias> categoria = null)
        //public Main(Object[] datos)
        {
            ordenID = idOrden;
            mesaglb = mesa;
            CATEGORIAS = categoria;
            if (_Token == false)
            {
                Navigation.PushAsync(new Login(false));
                //Console.WriteLine("No token");
            }
            else
            {

                InitializeComponent();
               
                // Refreshplatillos.IsRefreshing = true;
                testListView.ItemsSource = CATEGORIAS;


                if (ordenID != 0 && band == true )
                {
                    cart.Clear();
                    totalpago.Text = "0";
                    totalpay = 0;

                    if (lsit != null)
                        cart = lsit;
                    else
                        cart = CartMesa(ordenID.ToString(), mesaglb);

                    test2ListView.ItemsSource = null;
                    test2ListView.ItemsSource = cart;
                    test2ListView.IsRefreshing = false;
                    //Console.WriteLine(cart.Count);
                    for (int i = 0; i < cart.Count; i++)
                    {
                        totalpay += cart[i].total;
                    }
                    totalpago.Text = totalpay.ToString();
                   // mesa_orden = IdOrden(mesaglb);

                }
                else
                {
                    btnOrdenar.IsEnabled = true;
                    cart.Clear();
                }
                this.empleado = empleado;

                BindingContext = new MainViewModel(Navigation, _Token);
                MesaTexto.Text = "# Mesa:" + mesa;
                NombTexto.Text = "Mesero: " + empleado.pin ;
                OrdenTexto.Text = "# Orden: " + ordenID.ToString();

            }

            
           
           // SUBCATEGORIAS = SubCategoria();

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
                ordenID = Int32.Parse(mesa_orden[0]);

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
            test2ListView.ItemTemplate = null;
            testListView.ItemsSource = CATEGORIAS;
            // Refreshplatillos.IsRefreshing = false;

        }

        //----- REFRESCA EL CONTENIDO DE LA LISTA DE CARRITO ------//
        private void SwipeItem_Editar(object sender, EventArgs e)
        {
            if (mesa_orden[1] == "3" || mesa_orden[1] == "2")
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


                PopupNavigation.Instance.PushAsync(new ItemPlatillo(platillo, mesaglb, bandera: 1, cart, test2ListView, cart[index].idItem, totalpago:totalpago , empleado));

            }

        }

        private void SwipeItem_Eliminar(object sender, EventArgs e)
        {
            if (mesa_orden[1] == "2" || mesa_orden[1] == "1")
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

        /*
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
        /*
                    mesa_orden = IdOrden(mesaglb);
                    int ord = Int32.Parse(mesa_orden[1]);
                    string idO = mesa_orden[0];
                    //Console.WriteLine("Ordenar butno"+IdOrden(mesaglb)[1]);
                    if (ord == 1 || ord == 2)
                    {
                        PopupNavigation.Instance.PushAsync(new PopError("LE ORDEN SE ESTA PREPARANDO"));
                    }else if (ord == 0 || ord == 3)
                    {

                        var client = new HttpClient();
                        client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerEstadoCart&idOrden=" + mesaglb);
                       // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerEstadoCart&idOrden=" + mesaglb);
                        HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var content = response.Content.ReadAsStringAsync().Result;
                            string json = content.ToString();

                            var jsonArray = JArray.Parse(json.ToString());
                            var h = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss").Replace(" ", "-");
                           // Console.WriteLine(h);
                            //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                            foreach (var item in jsonArray)
                            {

                                if (item["estado"].Equals("3"))
                                {
                                    GET_DATOS("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=updateEstadoOrdenMesas&estado=0&id=" + idO + "&total=" + total+ "&fecha=" + h);
                                    // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=updateEstadoOrdenMesas&estado=0&id=" + idO + "&total=" + total + "&fecha=" + h);
                                    Navigation.PopAsync(false);
                                    //Navigation.PushAsync(new View.Ordenes(this), false);
                                    break;
                                }
                                else
                                {
                                    GET_DATOS("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=updateEstadoOrdenMesas&estado=1&id=" + idO + "&total=" + total + "&fecha=" + h.Replace(" ", "-"));
                                    //8 Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=updateEstadoOrdenMesas&estado=1&id=" + idO + "&total=" + total + "&fecha=" + h);
                                    Navigation.PopAsync(false);
                                    //Navigation.PushAsync(new View.Ordenes(this), false);
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
                    */

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
            int ord = Int32.Parse(IdOrden(mesaglb)[1]);
            string idO = IdOrden(mesaglb)[0];
            //Console.WriteLine("Ordenar butno" + IdOrden(mesaglb)[1]);
            if (ord == 1 || ord == 2)
            {
                PopupNavigation.Instance.PushAsync(new PopError("LE ORDEN SE ESTA PREPARANDO"));
            }
            else if (ord == 0 || ord == 3)
            {

                var client = new HttpClient();
                client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerEstadoCart&idOrden=" + mesaglb);
                // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerEstadoCart&idOrden=" + mesaglb);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    string json = content.ToString();

                    var jsonArray = JArray.Parse(json.ToString());
                    var h = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss").Replace(" ", "-");
                    //Console.WriteLine(h);
                    //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                    foreach (var item in jsonArray)
                    {

                        if (item["estado"].Equals("3"))
                        {
                            Console.WriteLine("Main = Estado 3");
                                /////CAMBIAR A MESERO
                            GET_DATOS("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=updateEstadoOrdenMesas&estado=0&id=" + idO + "&total=" + total + "&fecha=" + h+ "&mesero="+empleado.id);
                            
                            Navigation.PopAsync(false);
                         
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Main = Estado"+ item["estado"]);
                            GET_DATOS("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=updateEstadoOrdenMesas&estado=1&id=" + idO + "&total=" + total + "&fecha=" + h.Replace(" ", "-") + "&mesero=" + empleado.id);
                            Navigation.PopAsync(false);
                            //Navigation.PushAsync(new View.Ordenes(this), false);
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
        Model.Categorias categoria;
        public void UpdateSelectionData(IEnumerable<object> previousSelectedContact, IEnumerable<object> currentSelectedContact)
        {

            if (band == 0)
            {
                var i = currentSelectedContact.FirstOrDefault(); // Obtenemos el indice de la lista
               categoria = currentSelectedContact.FirstOrDefault() as Model.Categorias;

                //if (SubCategorias(categoria.id) != null)
                if (SubCategorias(categoria.id) != null)
                {
                  

                    testListView.ItemsSource = SubCategorias(categoria.id);
                    tituloR.Text += "/" + categoria.nombre;
                }
                else
                {
                    tituloR.Text += "/" + categoria.nombre;
                    //var platillo = currentSelectedContact.FirstOrDefault() as Model.Platillos;
                    testListView.ItemsSource = Platillos("&idcate=" + categoria.nombre);
                }
            }
            else if (band == 1)
            {
                var subcategoria = currentSelectedContact.FirstOrDefault() as Model.SubCategorias;
               // var i = currentSelectedContact.FirstOrDefault(); // Obtenemos el indice de la lista
                //var platillo = currentSelectedContact.FirstOrDefault() as Model.Platillos;

                testListView.ItemsSource = Platillos("&idsub=" + subcategoria.nombre);
                tituloR.Text +=  "/" + subcategoria.nombre;

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
                /*if (IdOrden(mesaglb)[1] == "3")
                {

                }*/
                mesa_orden = IdOrden(mesaglb);
                if (cart.Count == 0 && mesa_orden[1] != "3") // Caso 0: Carrito vacio
                {
                    PopupNavigation.Instance.PushAsync(new ItemPlatillo(platillo, mesaglb, bandera: 0, cart, test2ListView, totalpago: totalpago, empleado: empleado));
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
                        PopupNavigation.Instance.PushAsync(new ItemPlatillo(platillo, mesaglb, bandera: 1, cart, test2ListView,totalpago: totalpago, empleado:  empleado));
                    }
                    else if (band == 0) // CASO 1.2
                    {

                        //Console.WriteLine("Ya exite un platillo y se agrega el otro");
                        // itemPlatillo = new ItemPlatillo(platillo, mesaglb, bandera: 2, cart, test2ListView);
                        PopupNavigation.Instance.PushAsync(new ItemPlatillo(platillo, mesaglb, bandera: 2, cart, test2ListView,totalpago: totalpago, empleado: empleado ));

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
            //Console.WriteLine(band);
            if (band == 1) {
                tituloR.Text = "CATEGORIAS";
                band = 0;
                testListView.ItemsSource = CATEGORIAS;
            }
            else if (band == 2)
            {
              
                tituloR.Text = "Categorias".ToUpper() +"/" + categoria.nombre;
                testListView.ItemsSource = SubCategorias(categoria.id);
                if (SubCategorias(categoria.id) != null) { 
                    tituloR.Text = "CATEGORIAS";
                     band = 0;
                testListView.ItemsSource = CATEGORIAS;
                }
                else {
                    tituloR.Text = "CATEGORIAS";
                    band = 0;
                    testListView.ItemsSource = CATEGORIAS;


                }
                   
            }
            
            
            //band = 0;


            //testListView.ItemsSource = Categorias2();

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
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    categoria = new Model.Categorias();
                    categoria.id = Int32.Parse(item["id"].ToString());
                    categoria.nombre = item["nombre"].ToString();
                    categoria.estatus = item["estatus"].ToString();
                    userInfo.Add(categoria);
                }

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
            //Console.WriteLine("http://192.168.1.112/resta/admin/mysql/categoria/index.php?op=obtenerSubCategorias&id=" + id);
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
                    //Console.WriteLine(item["nombre"].ToString());
                    subcategoria.id = Int32.Parse(item["id"].ToString());
                    subcategoria.nombre = item["nombre"].ToString();
                    subcategoria.estatus = Int32.Parse(item["estatus"].ToString());
                    subcategoria.idCategoria = Int32.Parse(item["idCategoria"].ToString());
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

         public List<Model.SubCategorias> SubCategoria()
        {
            band = 1;
            Model.SubCategorias subcategoria;
            var sub = new List<Model.SubCategorias>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/categoria/index.php?op=allSubCategorias");
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
                    //Console.WriteLine(item["nombre"].ToString());
                    subcategoria.id = Int32.Parse(item["id"].ToString());
                    subcategoria.nombre = item["nombre"].ToString();
                    subcategoria.estatus = Int32.Parse(item["estatus"].ToString());
                    subcategoria.idCategoria = Int32.Parse(item["idCategoria"].ToString());
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
                    //Console.WriteLine(urls);
                    /*
                        var byteArray = Convert.FromBase64String(urls);
                        Stream stream = new MemoryStream(byteArray);
                        var imageSource = ImageSource.FromStream(() => stream);
                    */
                    //MyImage.Source = imageSource;
                    platillo.id = Int32.Parse(item["id"].ToString());
                    platillo.nombre = item["nombre"].ToString();
                    platillo.descrip = item["descrip"].ToString();
                    platillo.precio = item["precio"].ToString();
                    platillo.url = item["url"].ToString().Remove(0, 23);
                    platillo.estatus = Int32.Parse(item["estatus"].ToString());
                    platillo.categoria = item["categoria"].ToString();
                    platillo.clasificacion = item["clasificacion"].ToString();
                    platillo.subcategoria = item["subcategoria"].ToString();


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

        private void BtnSalir_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync(false);
            Navigation.PopAsync(false);
        }

    }


}
