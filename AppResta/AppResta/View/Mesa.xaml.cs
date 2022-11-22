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
    public partial class Mesa : ContentPage
    {
        string nomb = "";
        bool internet;
        List<Model.Ordenes> list;
        List<Model.Cart> listMain;
        List<Model.Mesas> mesa;
        List<Model.Categorias> categorias;
        public Mesa(List<Model.Mesas> mesas=null,Model.Empleado empleado = null, bool interent= false)
        {
            InitializeComponent();
          
            this.internet = interent;
           
            Device.StartTimer(TimeSpan.FromSeconds(1.2), () => {
                cargar.IsEnabled = false;
                cargar.IsRunning = false;
                cargar.IsVisible = false;
                mesasListView.IsVisible = true;
                return false;
            });

            BindingContext = new ViewModel.MesaViewModel(Navigation);
            nomb = empleado.nombre;
            nombreEmpl.Text = "Bienvenido: "+empleado.nombre;
            puestoEmpl.Text = empleado.puesto;

            if(mesas != null)
                 mesa = mesas; //Mesas();
            else
                mesa = Mesas();

            init(mesa);
            list = Services.OrdenesService.Ordene();
            categorias = Services.CartService.Categorias2();
            

            //Navigation.RemovePage(new Login());
        }

        public void init(List<Model.Mesas> mesa) {
            //List<Model.Mesas> mesas = await App.Database.GetMesaAsync();

            //if (internet)
            //{
            mesasListView.ItemsSource = mesa;
            //List<Model.Mesas> m = Mesas();
            
          //  }

        }

        private void RefreshMesas_Refreshing(object sender, EventArgs e)
        {
            List<Model.Mesas> mesa = Mesas();

            RefreshMesas.IsRefreshing = true;
            Task.Delay(800);
            if (mesa != null)
                mesasListView.ItemsSource = mesa;

            RefreshMesas.IsRefreshing = false;
        }

        public void select_Item(object sender, SelectionChangedEventArgs e)
        {
            var mesas = e.CurrentSelection.FirstOrDefault() as Model.Mesas;
            // mesasListView.ItemsSource = null;
            listMain = Services.CartService.CartMesa(mesas.id_orden, mesas.mesa);
            //mesasListView.ItemsSource = mesa;
            Navigation.PushAsync(new Main(true, idOrden: Int32.Parse(mesas.id_orden), nomb, mesas.mesa, listMain, categorias),false);

            //mesasListView.IsRefreshing = false;
        }
 
        public List<Model.Mesas> Mesas()
        {
            Model.Mesas mesa;

            var mesas = new List<Model.Mesas>();
            var client = new HttpClient();
            
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/empleados/index.php?op=obtenerMesas");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                string[] array = new string[2];
                foreach (var item in jsonArray)
                {
                    mesa = new Model.Mesas();
                    int id = Int32.Parse(item["id"].ToString());
                    string nombre = item["mesa"].ToString();
                    string mesero = item["mesa"].ToString();
                    array = OrdenInMesas(nombre);
                    mesa.id = id;
                    mesa.mesa = nombre;
                    if (array[0] != null)
                    {
                        mesa.orden = array[0];
                        mesa.id_orden = array[1];
                    }
                    else {
                        mesa.orden ="0";
                        mesa.id_orden = "0";
                    }
                    /* if (Int32.Parse(item["estado"].ToString()) == 0)
                    {
                       mesa.orden = "#2C67E6";
                        mesa.id_orden = item["id_orden"].ToString();
                    }
                    else if (Int32.Parse(item["estado"].ToString()) == 1)
                    {
                        mesa.orden = "#3AE62C ";
                        mesa.id_orden = item["id_orden"].ToString();
                    }
                    else if (Int32.Parse(item["estado"].ToString()) == 2)
                    {
                        mesa.orden = "#F7DB2F";
                        mesa.id_orden = item["id_orden"].ToString();
                    }
                    else
                    {
                        mesa.orden = "#E62C2C";
                        mesa.id_orden = item["id_orden"].ToString();
                    }*/
                    mesa.ubicacion = item["ubicacion"].ToString();
                    mesas.Add(mesa);
                }

                return mesas;
            }
            else
            {
                return null;
            }
        }

        //VALIDA SI LA MESA TINE ORDEN ACTIVA
        public string[] OrdenInMesas(string mesa)
        {
            var client = new HttpClient();
            string[] array = new string[2];
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenMesas&mesa=" + mesa);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                foreach (var item in jsonArray)
                {
                    //Console.WriteLine(item["estado"]);
                    if (Int32.Parse(item["cont"].ToString()) == 1)
                    {
                        if (Int32.Parse(item["estado"].ToString()) == 0)
                        {
                            array[0] = "#2C67E6";
                            array[1] = item["id_orden"].ToString();
                        }
                        else if (Int32.Parse(item["estado"].ToString()) == 1)
                        {
                            array[0] = "#3AE62C ";
                            array[1] = item["id_orden"].ToString();
                        } else if (Int32.Parse(item["estado"].ToString()) == 2) {
                            array[0] = "#F7DB2F";
                            array[1] = item["id_orden"].ToString();
                        }
                        else {
                            array[0] = "#E62C2C";
                            array[1] = item["id_orden"].ToString();
                        }
                        
                    }
                    /*
                    if(Int32.Parse(item["cont"].ToString()) == 0)
                    {
                        array[0] = "#02B942";
                    }*/

                }
                
            }
            else
            {
                return null;
            }

            return array;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Ordenes(ord: list), false);
        }
    }
}