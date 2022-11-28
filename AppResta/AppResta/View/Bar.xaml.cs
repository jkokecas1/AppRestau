using AppResta.Model;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections;
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
    public partial class Bar : ContentPage
    {
        // VARIABLES GLOBALES
        public List<Model.Cart> cart = new List<Model.Cart>();
        public static bool band;
        List<Model.Ordenes> ORDEN;
        Model.Empleado empleado;

        // CONSTRUCTOR
        public Bar(List<Model.Ordenes> orden = null, Model.Empleado empleado =null)
        {
            
            InitializeComponent();
            ORDEN = orden;
            this.empleado = empleado;   
            
            timebar.Text = DateTime.Now.ToString("t");
           // bartender.Text += " : "+empleado.pin;

            if (ORDEN != null)
                barListView.ItemsSource = ORDEN;
            else
                barListView.ItemsSource = Services.OrdenesService.OrdeneBar();

            Device.StartTimer(TimeSpan.FromSeconds(0.7), () =>
            {
                cargar.IsEnabled = false;
                cargar.IsRunning = false;
                cargar.IsVisible = false;
                barListView.IsVisible = true;
                return false;
            });

            Device.StartTimer(TimeSpan.FromSeconds(3), updateHistorial);
            Device.StartTimer(TimeSpan.FromSeconds(60), updateTimeLive);
            
        }

        // METODOS

        public void init(List<Model.Ordenes> ordenes)
        {
            timebar.Text = DateTime.Now.ToString("t");
            barListView.ItemsSource = ordenes;
            barHistorialListView.ItemsSource = Services.OrdenesService.OrdeneBarEmpleado(empleado.id + "");
        }
       
        bool updateTimeLive()
        {
            Device.BeginInvokeOnMainThread(() => init(Services.OrdenesService.OrdeneBar()));
            
            return true;
        } 

        bool updateHistorial()
        {
            init(Services.OrdenesService.OrdeneBar());
            cargar2.IsEnabled = false;
            cargar2.IsRunning = false;
            cargar2.IsVisible = false;
            barHistorialListView.IsVisible = true;
            return false;
        }

        private void cocinaListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectionDataAsync(e.PreviousSelection, e.CurrentSelection);
        }

        public void UpdateSelectionDataAsync(IEnumerable<object> previousSelectedContact, IEnumerable<object> currentSelectedContact)

        {
            Model.Ordenes idorden = currentSelectedContact.FirstOrDefault() as Model.Ordenes;

            PopupNavigation.Instance.PushAsync(new VerBar(orden: idorden,ordenes: ORDEN, activas: barListView,emleado: empleado.id+""));

            /*String[] bear = new string[4];

            Model.Ordenes ord = new Model.Ordenes();
            var idorden = currentSelectedContact.FirstOrDefault() as Model.Ordenes;

            
            bool answer = await DisplayAlert("informacion", "CONFIRAMA LA ORDEN", "CONFIRMAR", "CANCELAR");
            //await PopupNavigation.Instance.PushAsync(question);


            if (answer)
            {

                var client = new HttpClient();
                client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + idorden.id + "&mesa=" + idorden.mesa + "&opc=" + 1);//110&mesa=MESA-4
                                                                                                                                                                                              // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + idorden.id + "&mesa=" + idorden.mesa + "&opc=" + 1);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    string json = content.ToString();
                    var jsonArray = JArray.Parse(json.ToString());
                    //Console.WriteLine(jsonArray);
                    foreach (var item in jsonArray)
                    {
                        itemsFinalizar(idorden.id + "", item["idItem"].ToString());
                    }
                }

            }
            else
            {
                barListView.ItemsSource = null;
                barListView.ItemsSource = Ordene();
            }*/

        }

      
        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            Task.Delay(100);
            barListView.ItemsSource = null;
            barListView.ItemsSource = Services.OrdenesService.OrdeneBar();
            //Refresh_Ordenes.IsRefreshing = false;
        }
        
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        private void exit_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync(false);
        }

        private async void Fecha_cajero_DateSelected(object sender, DateChangedEventArgs e)
        {
            var fechamostrar = e.NewDate.ToString("D");
            await DisplayAlert("Alert", fechamostrar, "OK");
        }
    }
}