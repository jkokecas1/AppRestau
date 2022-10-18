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
        public Main(bool _Token)
        {


            if (_Token == false)
            {
                Navigation.PushAsync(new Login());
                Console.WriteLine("No token");
            }
            else
            {

                InitializeComponent();

                
                testListView.ItemsSource = Categorias2();
                BindingContext = new MainViewModel(Navigation, _Token);

            }

        }
        public void select_Item(object sender, SelectedItemChangedEventArgs e)
        {
            if (band == 0)
            {
                var i = e.SelectedItemIndex; // Obtenemos el indice de la lista
                var categoria = e.SelectedItem as Model.Categorias;

                if (SubCategorias(categoria.id) != null)
                {

                    testListView.ItemsSource = SubCategorias(categoria.id);
                    
                }
                else {
                    
                    //var i = e.SelectedItemIndex; // Obtenemos el indice de la lista
                    var platillo = e.SelectedItem as Model.Platillos;

                    testListView.ItemsSource = Platillos();
                    
                }

            }
            else if (band == 1)
            {
                var i = e.SelectedItemIndex; // Obtenemos el indice de la lista
                var platillo = e.SelectedItem as Model.Platillos;

                testListView.ItemsSource = Platillos();
                

            }
            else if (band == 2)
            {
                DisplayAlert("Sourcces", "Agregar", "Ok");

            }

        }
        public void mesas(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Mesa());
        }
        public void returnCategorias(object sender, EventArgs e) { 
            band = 0;
            testListView.ItemsSource = Categorias2();
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
                else {
                    return sub;
                }
        
            }
            else
            {
               
                return null;
            }
        }


        //public List<Model.Platillos> Platillos(int idCategoria, int idSbCategoria)
        public List<Model.Platillos> Platillos()
        {
            band = 2;
            Model.Platillos platillo;
            var sub = new List<Model.Platillos>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/Platillo/index.php?op=obtenerPlatillos");
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
                    string urls = item["url"].ToString().Remove(0,23);
                    int estatus = Int32.Parse(item["estatus"].ToString());
                    string categoria = item["categoria"].ToString();
                    string clasificacion = item["clasificacion"].ToString();
                    string subcategoria =item["subcategoria"].ToString();
                    Console.WriteLine(urls);
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
                
                //userInfo.Add(categoria);
                //testListView.ItemsSource = userInfo
                return sub;
            }
            else
            {
                return null;
            }
        }
    }
}