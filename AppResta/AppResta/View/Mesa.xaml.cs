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
        // VARIABLES GLOBALES
        string nomb = "";
        bool internet;
        List<Model.Ordenes> list;
        List<Model.Cart> listMain, listMainAux;
        static List<Model.Mesas> mesa;
        List<Model.Categorias> categorias;
        List<Model.SubCategorias> subcategoria;
        Model.Empleado empleado;
        int iDorden;
        List<Model.Platillos> platillos;

        // CONSTRUCTOR
        public Mesa(List<Model.Ordenes> orden =null, List<Model.Categorias> categorias2 = null, List<Model.SubCategorias> subcategoria = null, List<Model.Mesas> mesas = null, Model.Empleado empleado = null, bool interent = false, List<Model.Platillos> platillos = null)
        {
            InitializeComponent();
            tiempoCajero.Text = DateTime.Now.ToString("t");
            this.internet = interent;

            Device.StartTimer(TimeSpan.FromSeconds(1.2), () =>
            {
                cargar.IsEnabled = false;
                cargar.IsRunning = false;
                cargar.IsVisible = false;
                mesasListView.IsVisible = true;
                return false;
            });

            BindingContext = new ViewModel.MesaViewModel(Navigation);
            nomb = empleado.nombre;
            //nombreEmpl.Text = "Bienvenido: " + empleado.nombre;
            //puestoEmpl.Text = empleado.puesto;

            if (mesas != null)
                mesa = mesas; //Mesas();
            else
                mesa = Services.MesasService.ObtenerMesas();

            if (orden != null)
                list = orden; 
            else
                list = Services.OrdenesService.Ordene();

            if (categorias2 != null)
                categorias = categorias2;
            else
                categorias = Services.CartService.Categorias2();

            if (subcategoria != null)
                this.subcategoria = subcategoria;
            else
                this.subcategoria = Services.CartService.SubCategorias();
            
            if (platillos != null)
                this.platillos = platillos;
            else
                this.platillos = Services.CartService.Platillos("");

            init(mesa);


            listMainAux = Services.CartService.Carts(DateTime.Now.ToString("yyyy-MM-dd"));
            this.empleado = empleado;
            Device.StartTimer(TimeSpan.FromSeconds(60), updateTimeLive);
        }

        /*********************************************************************
         *  METODO INIT:
         *      INICIALIZA TODAS LAS ACCIONES
         *********************************************************************/

        public void init(List<Model.Mesas> mesa)
        {
            //List<Model.Mesas> mesas = await App.Database.GetMesaAsync();
            listMainAux = Services.CartService.Carts(DateTime.Now.ToString("yyyy-MM-dd"));
            //if (internet)
            //{
            tiempoCajero.Text = DateTime.Now.ToString("t");
            mesasListView.ItemsSource = mesa;
            iDorden = Services.OrdenesService.IDordene() +1;

            //List<Model.Mesas> m = Mesas();

            //  }

        }
        /*********************************************************************
         *  METODO INIT:
         *      INICIALIZA TODAS LAS ACCIONES
         *********************************************************************/

        public static void initUpdate(List<Model.Mesas> m, CollectionView mesasListView)
        {
            //List<Model.Mesas> mesas = await App.Database.GetMesaAsync();

            //if (internet)
            //{
           
            if (m != null && mesasListView != null)
                mesasListView.ItemsSource = m;
            else {
                mesasListView.ItemsSource = null;
                mesasListView.ItemsSource = mesa;
            }
           
            //iDorden = Services.OrdenesService.IDordene() + 1;

            //List<Model.Mesas> m = Mesas();

            //  }

        }

        /*********************************************************************
         *  METODO updateTimeLive:
         *      ACTUALIZA LOS DATOS CON EL HILO PRINCIPAL
         *********************************************************************/
        bool updateTimeLive()
        {
            tiempoCajero.Text = DateTime.Now.ToString("t");
            Device.BeginInvokeOnMainThread(() => init(Services.MesasService.ObtenerMesas()));
            return true;
        }

        /*********************************************************************
         *  METODO RefreshMesas_Refreshing:
         *      REFRESCA LA LISTA DE LAS MESAS
         *********************************************************************/
        private void RefreshMesas_Refreshing(object sender, EventArgs e)
        {
            List<Model.Mesas> mesa = Services.MesasService.ObtenerMesas();//Mesas();

            RefreshMesas.IsRefreshing = true;
            Task.Delay(800);
            if (mesa != null)
                mesasListView.ItemsSource = mesa;

            RefreshMesas.IsRefreshing = false;
        }

        /*********************************************************************
         *  METODO select_Item:
         *      SELECCIONA EL ELEMENTO DE LA LISTA Y ABRE LA PANTALLA DE MAIN
         *********************************************************************/
        public void select_Item(object sender, SelectionChangedEventArgs e)
        {
            var mesas = e.CurrentSelection.FirstOrDefault() as Model.Mesas;
            // listMain.Clear();
            listMain = new List<Model.Cart>();
            //// CHACAR
          if (listMainAux != null)
                foreach(Model.Cart c in listMainAux)
                {
                    if(c.id+"" == mesas.id_orden )
                    {
                        listMain.Add(c);
                      // Console.WriteLine(c.id + " == "+mesas.id_orden +" && "+ c.mesa + " == " + mesas.mesa);
                    }
                    //Console.WriteLine(c.id + " == " + mesas.id_orden + " && " + c.mesa + " == " + mesas.mesa);
                }
         
            if (Int32.Parse(mesas.id_orden) == 0)
                Navigation.PushAsync(new Main(true, band: false, idOrden: iDorden, empleado, mesas.mesa, listMain, categorias, subcategoria, mesasListView,platillos: platillos), false);
            else {
                //Console.WriteLine("Diferente de 0");
               // listMain = Services.CartService.CartMesa(mesas.id_orden, mesas.mesa);
                Navigation.PushAsync(new Main(true, band: true, idOrden: Int32.Parse(mesas.id_orden), empleado, mesas.mesa, listMain, categorias, subcategoria, mesasListView, platillos: platillos), false);
            }

           
        }

        /*********************************************************************
         *  METODO OnBackButtonPressed:
         *      DESACTIVA EL BOTON DE REGRESAR
         *********************************************************************/
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        /*********************************************************************
         *  METODO Button_Clicked:
         *      ABRE LA PAGINA DE ORDENES
         *********************************************************************/
        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Ordenes(ord: list), false);
        }
    }
}