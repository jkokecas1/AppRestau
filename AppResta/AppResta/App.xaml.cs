using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AppResta.View;
using System.IO;
using Xamarin.Essentials;

namespace AppResta
{
    public partial class App : Application
    {

        private static Data.Database database;


        public static Data.Database Database{
            get {
                if (database == null) {
                    database = new Data.Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "restauran.db3"));
                    
                }
               
                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            bool internet;
           

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                internet = true;
                Console.WriteLine("------------------------------------------------------------------------------ Si hay conectividad");
                //Services.OrdenesService.Ordene(true);
                Services.LoginService.Empleados(true);
                //Services.MesasService.Mesas(true);
            }
            else {
                internet = false;
                Console.WriteLine("------------------------------------- SIN CONECTIVIDAD");
                //Services.LoginService.Empleados(false);
                //Services.OrdenesService.Ordene(false);
                //Services.MesasService.Mesas(false);
            }
           
            MainPage = new NavigationPage(new Login(true));

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
