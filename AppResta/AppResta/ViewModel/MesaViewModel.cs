using AppResta.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppResta.ViewModel
{
    public class MesaViewModel : BaseViewModel
    {
        #region VARIABLES
       
        private ObservableCollection<Model.Mesas> items;
        //private Model.Mesas _selectMesa { get; set; };
        #endregion

        #region CONSTRUCTOR
        public MesaViewModel(INavigation navigation)
        {
            Navigation = navigation;
            /*
            Items = new ObservableCollection<Model.Mesas>();
            _ = Mesa.GetAllNewsAsync(list =>
            {

                foreach (Model.Mesas item in list)
                {
                    Items.Add(item);
                }
            });*/

        }
        #endregion

        #region OBJETOS
      
        public ObservableCollection<Model.Mesas> Items
        {
            get { return items; }
            set
            {

                items = value;
            }
        }
        /*
        public Model.Mesas SelectMesa { 
            get { return _selectMesa; }
            set {
                if (_selectMesa != value) { 
                    _selectMesa = value;

                    HandleSelectItem();
                } 
            }
        }

        private void HandleSelectItem()
        {
           // Navigation.PushAsync(new Main(true, idOrden: Int32.Parse(mesas.id_orden), nomb, mesas.mesa), false);
        }*/
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

