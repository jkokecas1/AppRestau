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
    public class HistorialViewModel : BaseViewModel
    {
        #region VARIABLES
        string _Numero;
        private ObservableCollection<Model.Cart> items;
        private ObservableCollection<Model.Ordenes> history;
        #endregion

        #region CONSTRUCTOR
        public  HistorialViewModel(INavigation navigation, int id, string mesa)
        {
            Navigation = navigation;

           
            Items = new ObservableCollection<Model.Cart>() {
                
            };

            GetHistoryDetaillAsync(id,mesa);

            History = new ObservableCollection<Model.Ordenes>() { };

            GetHistoryAsync();


        }
        #endregion

        #region OBJETOS
        public string Numero { get { return _Numero; } set { SetValue(ref _Numero, value); } }       
        public ObservableCollection<Model.Cart> Items{ get { return items; } set { items = value;}}
        public ObservableCollection<Model.Ordenes> History { get { return history; } set { history = value; } }
        

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

        public async Task GetHistoryDetaillAsync(int id ,string mesa)
        {
           await Services.HostorialService.GetAllNewsAsync(list =>
            {
                foreach (Model.Cart item in list)
                {

                    Items.Add(item);
                }

            }, id, mesa);
        }

        public async Task GetHistoryAsync()
        {
            await Services.HostorialService.GetAllHistoryAsync(list =>
            {
                foreach (Model.Ordenes item in list)
                {

                    History.Add(item);
                }

            });
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
      
        public ICommand IrComandacommand => new Command(async () => await IrComanda());
        public ICommand Mesascommand => new Command(async () => await Mesas());
        public ICommand Ordencommand => new Command(async () => await Orden());
        public ICommand Historialcommand => new Command(async () => await Historial());
        public ICommand ProcesoSimpCommand => new Command(ProcesoSimple);
        #endregion
    }
}

