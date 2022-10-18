using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Mesa : ContentPage
    {
        public Mesa()
        {
            InitializeComponent();
            BindingContext = new ViewModel.MesaViewModel(Navigation);
            Navigation.RemovePage(new Login());
        }
    }
}