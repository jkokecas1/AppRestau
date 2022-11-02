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

            MainPage = new NavigationPage(new Main(false));

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                Console.WriteLine("------------------------------------------------------------------------------      Si hay conectividad");
                //Services.OrdenesService.Ordene(true);
               ////Services.MesasService.Mesas(true);
            }
            else {
                Services.OrdenesService.Ordene(false);
                Services.MesasService.Mesas(false);
            }
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
