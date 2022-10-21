using AppResta.ViewModel;
using Newtonsoft.Json.Linq;
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
        int band = 0;

        List<Model.Cart> cart = new List<Model.Cart>();
<<<<<<< HEAD
        Model.Cart cartItem;
        public Main(bool _Token)
        {
=======
        Model.Cart cartItem = new Model.Cart();
        public Main(bool _Token)
        {

            
           
>>>>>>> f08c6908ad7f3725157b187e463ea7e9acb1cfab
            if (_Token == false)
            {
                Navigation.PushAsync(new Login());
                Console.WriteLine("No token");
            }
            else
            {

                InitializeComponent();


                testListView.ItemsSource = Categorias2();
                //test2ListView.ItemsSource = Platillos("");
                BindingContext = new MainViewModel(Navigation, _Token);

            }

        }
<<<<<<< HEAD

=======
        
>>>>>>> f08c6908ad7f3725157b187e463ea7e9acb1cfab
        public void select_Item(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectionData(e.PreviousSelection, e.CurrentSelection);
        }

<<<<<<< HEAD
        async void UpdateSelectionData(IEnumerable<object> previousSelectedContact, IEnumerable<object> currentSelectedContact)
        {

=======
        void UpdateSelectionData(IEnumerable<object> previousSelectedContact, IEnumerable<object> currentSelectedContact)
        {
           
>>>>>>> f08c6908ad7f3725157b187e463ea7e9acb1cfab
            if (band == 0)
            {
                var i = currentSelectedContact.FirstOrDefault(); // Obtenemos el indice de la lista
                var categoria = currentSelectedContact.FirstOrDefault() as Model.Categorias;

                if (SubCategorias(categoria.id) != null)
                {
                    testListView.ItemsSource = SubCategorias(categoria.id);
<<<<<<< HEAD
=======
                    
>>>>>>> f08c6908ad7f3725157b187e463ea7e9acb1cfab

                }
                else
                {
<<<<<<< HEAD
                    var platillo = currentSelectedContact.FirstOrDefault() as Model.Platillos;
                    testListView.ItemsSource = Platillos("&idcate=" + categoria.nombre);
                }
=======

                    //var i = e.SelectedItemIndex; // Obtenemos el indice de la lista
                    var platillo = currentSelectedContact.FirstOrDefault() as Model.Platillos;

                    testListView.ItemsSource = Platillos("&idcate="+ categoria.nombre);

                    //imagens.Source = platillo.url;
                }



>>>>>>> f08c6908ad7f3725157b187e463ea7e9acb1cfab
            }
            else if (band == 1)
            {
                var subcategoria = currentSelectedContact.FirstOrDefault() as Model.SubCategorias;
                var i = currentSelectedContact.FirstOrDefault(); // Obtenemos el indice de la lista
                var platillo = currentSelectedContact.FirstOrDefault() as Model.Platillos;
<<<<<<< HEAD
=======

                testListView.ItemsSource = Platillos("&idsub="+subcategoria.nombre);
>>>>>>> f08c6908ad7f3725157b187e463ea7e9acb1cfab


                testListView.ItemsSource = Platillos("&idsub=" + subcategoria.nombre);
            }
            else if (band == 2)
            {
<<<<<<< HEAD
                var platillo = currentSelectedContact.FirstOrDefault() as Model.Platillos;

                DisplayAlert("Sourcces", "Agregar \n ID:" + platillo.id + "\n Nombre :" + platillo.nombre + "\n Categoria:" + platillo.categoria, "Ok");
                
                cartItem = new Model.Cart();
                
                if (cart.Count == 0) // Caso 1: Carrito vacio
                {
                    
                    cartItem.id = platillo.id;
                    cartItem.platillo = platillo.nombre;
                    cartItem.cantidad = 1;
                    cartItem.precio = Convert.ToDouble(platillo.precio);
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
                        if (cart[i].id == platillo.id) // Caso 2.1: El platillo existe
                        {
                            //Console.WriteLine("Es igual suma 1");
                            cart[i].cantidad += 1;
                            band = 1;
                            break;
                        }
                    }
                    if (band == 0) {
                        Console.WriteLine("Ya exite un platillo y se agrega el otro");
                        cartItem.id = platillo.id;
                        cartItem.platillo = platillo.nombre;
                        cartItem.cantidad = 1;
                        cartItem.precio = Convert.ToDouble(platillo.precio);
                        cartItem.total = (int)(cartItem.precio * cartItem.cantidad);
                        cart.Add(cartItem);
                    }
                }
                test2ListView.ItemsSource = null;
                test2ListView.ItemsSource = cart;
                test2ListView.IsRefreshing = false;
                int totalpay = 0;
                for (int i = 0; i < cart.Count; i++)
                {
                    totalpay += cart[i].total;
                }
                Console.WriteLine(totalpay);
                totalpago.Text = totalpay.ToString();

=======
                
                var platillo = currentSelectedContact.FirstOrDefault() as Model.Platillos;
>>>>>>> f08c6908ad7f3725157b187e463ea7e9acb1cfab

                DisplayAlert("Sourcces", "Agregar \n Nombre:" + platillo.nombre + "\n Precio :" + platillo.precio + "\n Categoria:" + platillo.categoria, "Ok");

                cartItem.platillo = platillo.nombre;
                cartItem.cantidad =1;
                cartItem.precio = Convert.ToDouble(platillo.precio);
                cartItem.total = (int)(cartItem.precio * cartItem.cantidad);
                
                cart.Add(cartItem);
                cart.Add(cartItem);
                cart.Add(cartItem);
                cart.Add(cartItem);
                cart.Add(cartItem);
                test2ListView.ItemsSource = cart;
                                
            }
            
        }

<<<<<<< HEAD
        }

        public void returnCategorias(object sender, EventArgs e)
        {
            band = 0;
            testListView.ItemsSource = Categorias2();


=======
  
        public void returnCategorias(object sender, EventArgs e) { 
            band = 0;
            testListView.ItemsSource = Categorias2();
            
>>>>>>> f08c6908ad7f3725157b187e463ea7e9acb1cfab
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

<<<<<<< HEAD

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/Platillo/index.php?op=obtenerPlatillos" + opc);


=======
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/Platillo/index.php?op=obtenerPlatillos"+opc);
>>>>>>> f08c6908ad7f3725157b187e463ea7e9acb1cfab
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                ;
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
<<<<<<< HEAD

                    string subcategoria = item["subcategoria"].ToString();


=======
                    string subcategoria =item["subcategoria"].ToString();
>>>>>>> f08c6908ad7f3725157b187e463ea7e9acb1cfab
                    //Console.WriteLine(urls);
                    var byteArray = Convert.FromBase64String(urls);
                    Stream stream = new MemoryStream(byteArray);
                    var imageSource = ImageSource.FromStream(() => stream);
                    //MyImage.Source = imageSource;
                    platillo.id = id;
                    platillo.nombre = nombre;
                    platillo.descrip = descrip;
                    platillo.precio = precio;
                    platillo.url = imageSource;
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
    }
}