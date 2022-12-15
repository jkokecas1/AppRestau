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
    public partial class MainKiosko : ContentPage
    {
        List<Model.Platillos> platillo;
        List<Model.Categorias> categorias;
        Model.Empleado emp;
        public MainKiosko(Model.Empleado emp)
        {
            InitializeComponent();
            platillo= Services.CartService.Platillos("");
            categorias = Services.CartService.Categorias2();
            this.emp = emp;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Kiosco(platillo, categorias, emp));
        }
    }
}