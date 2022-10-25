using AppResta.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppResta.ViewModel
{
    public class MesaViewModel : BaseViewModel
    {
        #region VARIABLES
        string _Numero;
        string _NumMesa;
        #endregion

        #region CONSTRUCTOR
        public MesaViewModel(INavigation navigation)
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

        public async Task Orden()
        {
            await Navigation.PushAsync(new Ordenes(), false);

        }
        public async Task Historial()
        {
            await Navigation.PushAsync(new Historial(), false);

        }
        public async Task IrComanda()
        {
            //Object[] datos = { false, "", "","" };
            //Navigation.RemovePage(Login());
            await Navigation.PushAsync(new Main(false), false);

        }
        public void Logout()
        {
          
           Navigation.PopAsync();
            
        }
        public void Alerta()
        {

        }
        #endregion


        #region COMANDOS
        public ICommand IrComandacommand => new Command(async () => await IrComanda());
        public ICommand Ordencommand => new Command(async () => await Orden());
        public ICommand Historialcommand => new Command(async () => await Historial());
        public ICommand NavLoginCommand => new Command(Logout);
        #endregion
    }
}

