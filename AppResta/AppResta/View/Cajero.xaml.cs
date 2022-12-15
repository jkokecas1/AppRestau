using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
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
    public partial class Cajero : ContentPage
    {

        // VARIABLES    
        List<Model.Ordenes> listordAux = new List<Model.Ordenes>();
        static List<Model.Ordenes> ordenList;
        Model.Empleado empleado;

        // CONSTRUCTOR
        public Cajero(List<Model.Ordenes> ordenes, Model.Empleado empleado)
        {

            InitializeComponent();
            tiempoCajero.Text = DateTime.Now.ToString("t");
            this.empleado = empleado;
            nombreEmpl.Text = empleado.pin + "";
            BindingContext = new ViewModel.OrdenesViewModel(Navigation);
            //btnNotification.Clicked += BtnNotification_Clicked;
            Device.StartTimer(TimeSpan.FromSeconds(0.4), () =>
            {
                //  cargar.IsEnabled = false;
                // cargar.IsRunning = false;
                // cargar.IsVisible = false;
                RefreshOrdenes.IsVisible = true;
                return false;
            });
            ordenList = ordenes;
            init(ordenList);
            ///cocinaHistorial.ItemsSource = Services.OrdenesService.OrdenecajeroEmpleado(empleado.id.ToString(), DateTime.Now.ToString("yyyy-MM-dd"));
            Device.StartTimer(TimeSpan.FromSeconds(3), updateHistorial);
            Device.StartTimer(TimeSpan.FromSeconds(60), updateTimeLive);
        }


        // METODOS

        public void init(List<Model.Ordenes> ord)
        {
            tiempoCajero.Text = DateTime.Now.ToString("t");
            cajeroHistorial.ItemsSource = Services.OrdenesService.OrdenecajeroEmpleado(empleado.id.ToString(), DateTime.Now.ToString("yyyy-MM-dd"));
            if (ord != null) {
                ordenesListView.ItemsSource = ord;//await App.Database.GetOrdenesAsync(); //
                ordenList = ord;
            }
            
        }

        bool updateTimeLive()
        {
            Device.BeginInvokeOnMainThread(() => init(Services.OrdenesService.OrdenCaja()));
            return true;
        }


        bool updateHistorial()
        {
            // init(Services.OrdenesService.OrdeneBar());
            // cargar2.IsEnabled = false;
            //cargar2.IsRunning = false;
            //cargar2.IsVisible = false;
            cajeroHistorial.IsVisible = true;
            cajeroHistorial.ItemsSource = Services.OrdenesService.OrdenecajeroEmpleado(empleado.id.ToString(), DateTime.Now.ToString("yyyy-MM-dd")); ;
           
            return false;
        }

        private void Button_Pagar(object sender, EventArgs e)
        {
            int id = Int32.Parse(((MenuItem)sender).CommandParameter.ToString());


            Pago pago = new Pago(ordenList, id, ordenesListView, empleado);
            PopError error = new PopError("LA ORDEN AUN NO SE ESTA LISTA");
            foreach (Model.Ordenes orden in ordenList)
            {
                if (orden.id == id)
                {
                    Console.WriteLine(orden.estado);
                    if (orden.estado == "! Terminado !")
                    {

                        // if(!pago.IsVisible && !error.IsVisible)

                        Navigation.PushAsync(pago, false);
                    }
                    else
                    {
                        // if (!pago.IsVisible && !error.IsVisible)
                        PopupNavigation.Instance.PushAsync(error);
                    }
                }


            }

        }

        public void select_Item(object sender, SelectedItemChangedEventArgs e)
        {
            var orden = e.SelectedItem as Model.Ordenes;

            //Navigation.PushAsync(new Main(false, idOrden: orden.id));

        }



        public static double ExtrasItem(string id)
        {
            double totoal = 0.0;
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/platillo/index.php?op=obtenerExtrasAsItem&iditem=" + id);

            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                if (json.Equals("[]"))
                {
                    totoal = 0.0;

                }
                else
                {
                    var jsonArray = JArray.Parse(json.ToString());

                    foreach (var item in jsonArray)
                    {
                        totoal += Double.Parse(item["precio"].ToString().Replace(",", "."));
                    }

                }

            }
            return totoal;
        }




        private void RefreshOrdenes_Refreshing(object sender, EventArgs e)
        {
            Task.Delay(100);
            init(Services.OrdenesService.OrdenCaja());
            //VIVRACION
            /* try
             {
                 // Use default vibration length
                 Vibration.Vibrate();

                 // Or use specified time
                 var duration = TimeSpan.FromSeconds(1);
                 Vibration.Vibrate(duration);
             }
             catch (FeatureNotSupportedException ex)
             {
                 // Feature not supported on device
             }
             catch (Exception ex)
             {
                 // Other error has occurred.
             }*/
            RefreshOrdenes.IsRefreshing = false;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync(false);
        }
        
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            listordAux.Clear();
            SearchBar searchBar = (SearchBar)sender;
           
            if (searchBar.Text != "")
            {
               
                for (int i = 0; i < ordenList.Count; i++)
                {
                    if (ordenList[i].mesero.ToLower().Contains(searchBar.Text.ToLower()) )
                    {
                        listordAux.Add(ordenList[i]);
                    }
                }

                ordenesListView.ItemsSource = listordAux;

            }
            else {
                ordenesListView.ItemsSource = ordenList;
            }
        }

        private  void Fecha_cajero_DateSelected(object sender, DateChangedEventArgs e)
        {
            var fechamostrar = e.NewDate.ToString("yyyy-MM-dd");
            cajeroHistorial.ItemsSource = Services.OrdenesService.OrdenecajeroEmpleado(empleado.id + "", fechamostrar);
        }

    }
}