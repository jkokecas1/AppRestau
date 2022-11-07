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
    public partial class PopError : Rg.Plugins.Popup.Pages.PopupPage
    {
        public PopError(string messaje)
        {
            InitializeComponent();
            /// titulo.Text = titulo;
            mensaje.Text = messaje;
        }
        private void cerrarPop(object sender, EventArgs e)
        {
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }
    }
}