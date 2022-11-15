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
    public partial class PopQuestion : Rg.Plugins.Popup.Pages.PopupPage
    {
        public PopQuestion(string mensaje)
        {
            InitializeComponent();
            this.mensaje.Text = mensaje;
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