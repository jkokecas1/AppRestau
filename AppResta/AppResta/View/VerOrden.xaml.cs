using System;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerOrden : Rg.Plugins.Popup.Pages.PopupPage
    {

        public VerOrden(Model.Ordenes ordenes)
        {
            InitializeComponent();
            numeroOrden.Text = "Orden # " + ordenes.id;

        }
        private void cerrarPop(object sender, EventArgs e)
        {
            this.IsVisible = false;
            //Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

    }
}