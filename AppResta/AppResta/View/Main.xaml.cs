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

        // VARIABLEs
        int band = 0, ordenID;
        string mesaglb;
        public static List<Model.Cart> cart = new List<Model.Cart>();
        List<Model.SubCategorias> subAux;
        List<Model.Platillos> platAux;
        public Model.Cart cartItem = new Model.Cart();
        public static double totalpay = 0.0;

        public List<Model.Categorias> CATEGORIAS;
        public List<Model.SubCategorias> SUBCATEGORIAS;
        public List<Model.Platillos> PLATILLOS;

        public static string[] mesa_orden;
        Model.Empleado empleado;
        CollectionView mesasListView;

        Model.Categorias categoria;


        public Main(bool _Token, bool band, int idOrden = 0, Model.Empleado empleado = null, string mesa = "MES-0", List<Model.Cart> lsit = null, List<Model.Categorias> categoria = null, List<Model.SubCategorias> subcategorias = null, CollectionView mesasListView = null, List<Model.Platillos> platillos = null)
        {
            ordenID = idOrden;
            mesaglb = mesa;
            CATEGORIAS = categoria;
            this.mesasListView = mesasListView;
            if (_Token == false)
            {
                Navigation.PushAsync(new Login(false));
                //Console.WriteLine("No token");
            }
            else
            {
                InitializeComponent();
                tiempoCajero.Text = DateTime.Now.ToString("t");
                // Refreshplatillos.IsRefreshing = true;
                // 

                if (ordenID != 0 && band == true)
                {
                    cart.Clear();
                    totalpago.Text = "0";
                    totalpay = 0;

                    if (lsit != null)
                    {
                        cart = lsit;
                        test2ListView.ItemsSource = null;
                        test2ListView.ItemsSource = cart;
                        test2ListView.IsRefreshing = false;
                    }

                    for (int i = 0; i < cart.Count; i++)
                    {
                        totalpay += cart[i].total;
                    }
                    totalpago.Text = totalpay.ToString();
                }
                else
                {
                    btnOrdenar.IsEnabled = true;
                    cart.Clear();
                }
                this.empleado = empleado;

                BindingContext = new MainViewModel(Navigation, _Token, mesasListView);
                MesaTexto.Text = "# Mesa:" + mesa;
                //NombTexto.Text = "Mesero: " + empleado.pin ;
                OrdenTexto.Text = "# Orden: " + ordenID.ToString();

            }
            Device.StartTimer(TimeSpan.FromSeconds(0.2), () =>
            {
                testListView.ItemsSource = CATEGORIAS;
                return false;
            });
            if (subcategorias != null)
            {
                SUBCATEGORIAS = subcategorias;
            }
            if (platillos != null)
            {
                PLATILLOS = platillos;
            }

        }

        //----- REFRESCA EL CONTENIDO DE LA LISTA DE CARRITO ------//
        private void RefreshCart_Refreshing(object sender, EventArgs e)
        {
            if (test2ListView.ItemsSource != null)
            {
                Task.Delay(700);
                ordenID = Int32.Parse(mesa_orden[0]);
                cart.Clear();
                cart = Services.CartService.CartitoMesa(ordenID + "", mesaglb);
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
        private void SwipeItem_Invoked(object sender, EventArgs e)
        {
            mesa_orden = Services.OrdenesService.IdOrden(mesaglb);
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
                                    platillo.subcategoria = cart[i].extras;

                                }
                            }
                            break;
                        }

                    }
                }


                PopupNavigation.Instance.PushAsync(new ItemPlatillo(platillo, mesaglb, bandera: 1, cart, test2ListView, cart[index].idItem, totalpago: totalpago, empleado, cantsteper: cart[index].cantidad));

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

            string[] arrayOrden = Services.OrdenesService.IdOrden(mesaglb);
            int ord;
            string idO;
            if (arrayOrden[0] != null && arrayOrden[1] != null)
            {
                ord = Int32.Parse(arrayOrden[1]);//IdOrden(mesaglb)[1]);
                idO = arrayOrden[0];//IdOrden(mesaglb)[0];
            }
            else
            {
                ord = 0;//IdOrden(mesaglb)[1]);
                idO = ordenID.ToString();//IdOrden(mesaglb)[0];
            }



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
                            //Console.WriteLine("Main = Estado 3");
                            /////CAMBIAR A MESERO
                            GET_DATOS("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=updateEstadoOrdenMesas&estado=0&id=" + idO + "&total=" + total + "&fecha=" + h + "&mesero=" + empleado.id);

                            Navigation.PopAsync(false);

                            break;
                        }
                        else
                        {
                            //Console.WriteLine("Main = Estado" + item["estado"]);
                            GET_DATOS("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=updateEstadoOrdenMesas&estado=1&id=" + idO + "&total=" + total + "&fecha=" + h.Replace(" ", "-") + "&mesero=" + empleado.id);
                            Navigation.PopAsync(false);
                            //Navigation.PushAsync(new View.Ordenes(this), false);
                            break;
                        }
                    }
                }
                View.Mesa.initUpdate(Services.MesasService.ObtenerMesas(), mesasListView);

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
            subAux = new List<Model.SubCategorias>();
            platAux = new List<Model.Platillos>();
            //Caso 0: if is exits a subcategoira

            if (band == 0)
            {
                categoria = currentSelectedContact.FirstOrDefault() as Model.Categorias;

                foreach (Model.SubCategorias s in SUBCATEGORIAS)
                {
                   
                    if (s.idCategoria == categoria.id)
                    {
                        Console.WriteLine(s.nombre);
                        subAux.Add(s);
                    }
                }


                if (subAux.Count > 0)
                {
                   
                    testListView.ItemsSource = subAux;
                    tituloR.Text += "/" + categoria.nombre;
                    band = 1;
                }
                else
                {
                    foreach (Model.Platillos p in PLATILLOS)
                    {
                        
                        if (p.categoria == categoria.nombre)
                        {
                            platAux.Add(p);
                        }
                    }
                    tituloR.Text += "/" + categoria.nombre;
                    testListView.ItemsSource = platAux;//Platillos("&idcate=" + categoria.nombre);
                    band = 2;
                }
            }
            else if (band == 1)
            {
               // platAux.Clear();
                var sub = currentSelectedContact.FirstOrDefault() as Model.SubCategorias;
                foreach (Model.Platillos p in PLATILLOS)
                {
                    if (p.subcategoria == sub.nombre)
                    {
                        platAux.Add(p);
                    }
                }
                tituloR.Text += "/" + sub.nombre;
                testListView.ItemsSource = platAux;//Platillos("&idsub=" + sub.nombre);
                band = 2;
            }
            else if (band == 2)
            {

                var platillo = currentSelectedContact.FirstOrDefault() as Model.Platillos;

                cartItem = new Model.Cart();
                
                mesa_orden = Services.OrdenesService.IdOrden(mesaglb); //IdOrden(mesaglb);
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
                        //Console.WriteLine(cart[i].platillo+" == "+platillo.nombre);
                        if (cart[i].platillo == platillo.nombre && platillo != null ) // Caso 2.1: El platillo existe
                        {
                            index = i;
                            band = 1;
                            break;
                        }
                    }
                    if (band == 1)
                    { // CASO 1.1
                        PopupNavigation.Instance.PushAsync(new ItemPlatillo(platillo,mesaglb, bandera: 1, cart, test2ListView, totalpago: totalpago, empleado: empleado));
                    }
                    else if (band == 0) // CASO 1.2
                    {
                        PopupNavigation.Instance.PushAsync(new ItemPlatillo(platillo, mesaglb, bandera: 2, cart, test2ListView, totalpago: totalpago, empleado: empleado));
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
            if (band == 1)
            {
                tituloR.Text = "CATEGORIAS";
                band = 0;
                testListView.ItemsSource = CATEGORIAS;
            }
            else if (band == 2)
            {
                foreach (Model.SubCategorias s in SUBCATEGORIAS)
                {
                    if (s.idCategoria == categoria.id)
                    {
                        subAux.Add(s);
                    }
                }
                if (subAux.Count > 0)
                {
                    tituloR.Text = "Categorias".ToUpper() + "/" + categoria.nombre.ToUpper();
                    testListView.ItemsSource = subAux;//SubCategorias(categoria.id);
                    band = 1;
                }
                else
                {
                    tituloR.Text = "CATEGORIAS";
                    band = 0;
                    testListView.ItemsSource = CATEGORIAS;
                }
               // subAux.Clear();

            }
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

        private void test2ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }

        private void BtnSalir_Clicked(object sender, EventArgs e)
        {

            Navigation.PopAsync(false);
        }
    }
}
