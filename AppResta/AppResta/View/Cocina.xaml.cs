using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Cocina : ContentPage
    {
        // VARIABLES GLOBALES
        List<Model.Ordenes> ORDEN;
        public List<Model.Cart> cart = new List<Model.Cart>();
        public List<Model.Cart> listCat;
        Model.Empleado empleado;


        // CONSTRUCTOR
        public Cocina(List<Model.Ordenes> orden = null, Model.Empleado empleado =null, List<Model.Cart> listCat = null)
        {

            InitializeComponent();
            fecha.Text = DateTime.Now.ToString("t");
            this.empleado = empleado;
            //chef.Text = empleado.pin;
            if (orden != null)
            {
                ORDEN = orden;
                cocinaListView.ItemsSource = ORDEN;
            }
            else
            {
                ORDEN = Services.OrdenesService.OrdeneCocina();
                cocinaListView.ItemsSource = ORDEN;
            }

           
            fecha.Text = DateTime.Now.ToString("t");
            Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
            {
                cargar.IsEnabled = false;
                cargar.IsRunning = false;
                cargar.IsVisible = false;
                cocinaListView.IsVisible = true;
                return false;
            });
            this.listCat = listCat;//Services.CartService.Carts(DateTime.Now.ToString("yyyy-MM-dd") + "-00:00:00");
            // StartCountDownTimer();
            //updateHistorial();
           Device.StartTimer(TimeSpan.FromSeconds(5), updateHistorial);
            Device.StartTimer(TimeSpan.FromSeconds(60), updateTimeLive);
            // cocinaListView.ItemsSource = ord;
           
        }

        // METODOS



        public void init() {

            // Console.WriteLine(DateTime.Now.ToString("t"));
            ORDEN = Services.OrdenesService.OrdeneCocina();
            fecha.Text = DateTime.Now.ToString("t");
            cocinaListView.ItemsSource = ORDEN;
             cocinaHistorialList.ItemsSource = Services.OrdenesService.OrdeneBarEmpleado(empleado.id + "");
        }

        bool updateTimeLive()
        {
            Device.BeginInvokeOnMainThread(() => init());
            return true;
        }
        bool updateHistorial()
        {
            Console.WriteLine(empleado.id);
             cocinaHistorialList.ItemsSource = Services.OrdenesService.OrdeneBarEmpleado(empleado.id + "");
            cargar2.IsEnabled = false;
            cargar2.IsRunning = false;
            cargar2.IsVisible = false;
            cocinaHistorialList.IsVisible = true;

            return false;
        }
        private void cocinaListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectionData(e.PreviousSelection, e.CurrentSelection);
        }
        public void UpdateSelectionData(IEnumerable<object> previousSelectedContact, IEnumerable<object> currentSelectedContact)

        {

           // Model.Ordenes ord = new Model.Ordenes();
            Model.Ordenes idorden = currentSelectedContact.FirstOrDefault() as Model.Ordenes;

            //List<Model.Ordenes> cart = Ordene();
            /* int a = ORDEN.IndexOf(idorden);

             if(a != -1)
                 ord = ORDEN[a];*/

            // List<Model.Ordenes> auxList = Ordene();
            
            if (idorden != null) {
                PopupNavigation.Instance.PushAsync(new VerOrden(idorden, aux: ORDEN, cocinaListView, listCat, empleado.id));

            }

        }

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            Task.Delay(100);
            cocinaListView.ItemsSource = null;
            ORDEN = Services.OrdenesService.OrdeneCocina();
            cocinaListView.ItemsSource = ORDEN;

            Refresh_Ordenes.IsRefreshing = false;
        }

        
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        private void exit_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine("Salir");
            Navigation.PopAsync();
        }

        private void exit_Clicked_1(object sender, EventArgs e)
        {
            Console.WriteLine("Salir");
            // Navigation.PopAsync();
            this.Navigation.PopAsync(true);
        }
    }
}