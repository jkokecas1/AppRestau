using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AppResta.ViewModel;
using System.Reflection;
using System.IO;
using AppResta.Model;
using System.Collections.ObjectModel;
using System.Net.Http;
using Xamarin.Essentials;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        bool internet;
  
        public Login(bool interent)
        {
            Navigation.PushAsync(new SplashCarga());
            this.internet = interent;
            InitializeComponent();
            
            BindingContext = new ViewModel.LoginViewModel(Navigation, internet);
           
           
        }

        
      

    }
}
