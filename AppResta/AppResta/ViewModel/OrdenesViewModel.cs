using AppResta.View;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppResta.ViewModel
{
    public class OrdenesViewModel : BaseViewModel
    {
        #region VARIABLES
        string _Numero;
        #endregion

        #region CONSTRUCTOR
        public OrdenesViewModel(INavigation navigation)
        {
            Navigation = navigation;
        }
        #endregion

        #region OBJETOS
        public string Numero
        {
            get { return _Numero; }
            set { SetValue(ref _Numero, value); }
        }
        #endregion

        #region PROCESOS

        public async Task Mesas()
        {
            //await Navigation.PushAsync(new Mesa());
            await Navigation.PopAsync(false);
        }
        public async Task Pagar()
        {
            //await Navigation.PushAsync(new Pago());

        }
        public async Task Historial()
        {
            await Navigation.PushAsync(new Historial(),false);

        }

        public async Task IrComanda()
        {
            //Object[] datos = { false, "", "", "" };
            await Navigation.PushAsync(new Main(false));

        }

        #endregion


        #region COMANDOS
        public ICommand IrComandacommand => new Command(async () => await IrComanda());
        public ICommand Mesascommand => new Command(async () => await Mesas());
        public ICommand Pagarcommand => new Command(async () => await Pagar());
        public ICommand Historialcommand => new Command(async () => await Historial());
        
        #endregion
    }
}

