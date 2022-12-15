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
    public partial class PopCarga : Rg.Plugins.Popup.Pages.PopupPage
    {
        public PopCarga(string mensaje)
        {
            InitializeComponent();
            
        }

        private void cerrarPop(object sender, EventArgs e)
        {
            Bar.band = false;
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        public void confirmar() {
            Bar.band = true;
        }
    }
}