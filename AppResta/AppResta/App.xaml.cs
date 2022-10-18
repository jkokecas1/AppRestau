using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AppResta.View;

namespace AppResta
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Main(false));
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
