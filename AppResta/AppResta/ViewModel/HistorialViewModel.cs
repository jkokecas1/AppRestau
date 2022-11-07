using AppResta.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppResta.ViewModel
{
    public class HistorialViewModel : BaseViewModel
    {
        #region VARIABLES
        string _Numero;
        #endregion

        #region CONSTRUCTOR
        public HistorialViewModel(INavigation navigation)
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
            await Navigation.PushAsync(new Mesa());

        }
        public async Task Orden()
        {
            await Navigation.PopAsync(false);

        }
        public async Task Historial()
        {
            await Navigation.PushAsync(new Historial());

        }

        public async Task ProcesoAsyncrono()
        {

        }
        public async Task IrComanda()
        {
            Object[] datos = { true, "", "", "" };
            await Navigation.PushAsync(new Main(false));

        }
        public void ProcesoSimple()
        {

        }
        public void Alerta()
        {

        }
        #endregion


        #region COMANDOS
        public ICommand ProcesoAsyncommand => new Command(async () => await ProcesoAsyncrono());
        public ICommand IrComandacommand => new Command(async () => await IrComanda());
        public ICommand Mesascommand => new Command(async () => await Mesas());
        public ICommand Ordencommand => new Command(async () => await Orden());
        public ICommand Historialcommand => new Command(async () => await Historial());
        public ICommand ProcesoSimpCommand => new Command(ProcesoSimple);
        #endregion
    }
}

