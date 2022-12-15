/*:-----------------------------------------------------------------------------
*:                          PRACTICANTES 
*:                          AW SOFTWARE
*: 
*:                           Ago – Dic 
*: 
*: 
*:                  CLASE DE LA VISARA BAR
*: 
*: 
*: Archivo      :   Bar.cs
*: Autor        :   Jorge Gerardo Moreno Castillo
*:                  Jaen
*:              
*: Fecha        :   20/SEP/2022
*: Compilador   :   Microsoft Visual Studio Community 2022 (64-bit) - Current Version 17.2.6
*: Descripción  :   Esta clase implementa los metodos para manipular las acciones
*: Ult.Modif.   :   2022/01/12
*: Fecha Modificó Modificacion
*:=============================================================================
*: 24/SEP/2022 Jorge - Se creo GetAllOrdenAsync() obtner el carrito que se va apagar
*:----------------------------------------------------------------------------
*/
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
        //--------------------------------------------------------------------------
        // VARIABLES GLOBALES
        //

        public List<Model.Cart> cart = new List<Model.Cart>();
        public static bool band;
        List<Model.Ordenes> ORDEN;
        Model.Empleado empleado;

        //--------------------------------------------------------------------------
        // CONSTRUCTOR DE LA CALSE, RECIBE LAS REFERECNICAS DEL LOGIN PRINCIPLA
        //

        public Bar(List<Model.Ordenes> orden = null, Model.Empleado empleado =null)
        {
            
            InitializeComponent();
            ORDEN = orden;
            this.empleado = empleado;   
            
            timebar.Text = DateTime.Now.ToString("t");

            if (ORDEN != null)
                barListView.ItemsSource = ORDEN;
            else
                barListView.ItemsSource = Services.OrdenesService.OrdeneBar();

            Device.StartTimer(TimeSpan.FromSeconds(0.3), () =>
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

        //--------------------------------------------------------------------------
        // METODO INIT, INICIALIZA LAS LISTAS 
        //

        public void init(List<Model.Ordenes> ordenes)
        {
            timebar.Text = DateTime.Now.ToString("t");
            barListView.ItemsSource = ordenes;
            barHistorialListView.ItemsSource = Services.OrdenesService.OrdeneBarEmpleado(empleado.id + "", DateTime.Now.ToString("yyyy-MM-dd"));
        }
       
        //--------------------------------------------------------------------------
        // METODO QUE REALIZA UNA ACTUALIZACION DEL LAS LISTAS
        //

        bool updateTimeLive()
        {
            Device.BeginInvokeOnMainThread(() => init(Services.OrdenesService.OrdeneBar()));
            
            return true;
        }
        //--------------------------------------------------------------------------
        // METODO QUE ACTUALIZA LA LISTA DEL HISTORIAL 
        //

        bool updateHistorial()
        {
            init(Services.OrdenesService.OrdeneBar());
            cargar2.IsEnabled = false;
            cargar2.IsRunning = false;
            cargar2.IsVisible = false;
            barHistorialListView.IsVisible = true;
            return false;
        }
        //--------------------------------------------------------------------------
        // METODO QUE REALIZA LA SELCCION DE CADA ITEM
        //

        private void cocinaListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectionDataAsync(e.PreviousSelection, e.CurrentSelection);
        }
        //--------------------------------------------------------------------------
        // METODO QUE ENVIA LA INFROMACION A LA SIGUINETE PAGINA 
        //

        public void UpdateSelectionDataAsync(IEnumerable<object> previousSelectedContact, IEnumerable<object> currentSelectedContact)

        {
            Model.Ordenes idorden = currentSelectedContact.FirstOrDefault() as Model.Ordenes;

            PopupNavigation.Instance.PushAsync(new VerBar(orden: idorden,ordenes: ORDEN, activas: barListView,emleado: empleado.id+""));
        }

        //--------------------------------------------------------------------------
        // METODO REFRESACA LA LISTAS CON LE EVENTO
        //

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            Task.Delay(100);
            barListView.ItemsSource = null;
            barListView.ItemsSource = Services.OrdenesService.OrdeneBar();
            //Refresh_Ordenes.IsRefreshing = false;
        }
        //--------------------------------------------------------------------------
        // METODO LIMITA QUE SE APRECIONADO EL BOTON
        //

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }
        //--------------------------------------------------------------------------
        // METODO CIERRA LA PESTA;A ACTUAL
        //

        private void exit_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync(false);
        }
        //--------------------------------------------------------------------------
        // METODO MUETSA LAS ORDENES POR DIA
        //

        private void Fecha_cajero_DateSelected(object sender, DateChangedEventArgs e)
        {
            var fechamostrar = e.NewDate.ToString("yyyy-MM-dd");
            //Console.WriteLine(fechamostrar);
            barHistorialListView.ItemsSource = Services.OrdenesService.OrdeneBarEmpleado(empleado.id+"", fechamostrar);
        }
    }
}