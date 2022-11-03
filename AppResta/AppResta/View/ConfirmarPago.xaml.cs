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
    public partial class ConfirmarPago : Rg.Plugins.Popup.Pages.PopupPage
    {
        public ConfirmarPago()
        {
            InitializeComponent();
        }
        private void cerrarPop(object sender, EventArgs e)
        {
            this.IsVisible = false;
            //Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

    }
}