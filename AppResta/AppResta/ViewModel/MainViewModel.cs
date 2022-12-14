using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using AppResta.View;
using AppResta.Model;
using AppResta.Services.Categorias;

namespace AppResta.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        readonly ICategoiaRespository _categoriaRespository = new CategoriasService();

        #region VARIABLES
        public bool _Token;
        int _id;
        string _Nombre;
        string _Estado;
        string _expand;

        public List<Categorias> categoria;
        CollectionView mesasListView;
        #endregion

        #region CONSTRUCTOR
        public MainViewModel(INavigation navigation, bool _Token, CollectionView mesasListView)
        {
            Navigation = navigation;
            this._Token = _Token;
            this.mesasListView = mesasListView;
        }

        public MainViewModel()
        {

        }
        #endregion

        #region OBJETOS

        public int ID
        {
            get { return _id; }
            set { SetValue(ref _id, value); }
        }

        public string Nombre
        {
            get { return _Nombre; }
            set { SetValue(ref _Nombre, value); }
        }

        public string Estado
        {
            get { return _Estado; }
            set { SetValue(ref _Estado, value); }

        }

        public string Expand
        {
            get { return _expand; }
            set { SetValue(ref _expand, value); }

        }
        #endregion

        #region PROCEOS
        public async Task PageLogin()
        {
            await Navigation.PopAsync();
        }

        public async Task Pago()
        {
            //await Navigation.PushAsync(new Pago(), false);
       
        }
        public async Task Mesas()
        {
            View.Mesa.initUpdate(null, mesasListView);
            await Navigation.PopAsync(false);
        }
        public async Task Orden()
        {
            await Navigation.PushAsync(new View.Ordenes(), false);

        }
        public async Task Historial()
        {
            await Navigation.PushAsync(new Historial(), false);

        }
        public void ProcesoSimple()
        {
            if (_Token == true)
            {

                //Navigation.PushAsync(new Login(true));
                //Navigation.PushAsync(new Login(true));
            }
        }

        #endregion
        #region COMAND

        public ICommand NavLoginCommand => new Command(ProcesoSimple);
        public ICommand Pagocommand => new Command(async () => await Pago());
        public ICommand Mesascommand => new Command(async () => await Mesas());
        public ICommand Ordencommand => new Command(async () => await Orden());
        public ICommand Historialcommand => new Command(async () => await Historial());
        public ICommand ValidarToken => new Command(ProcesoSimple);
        #endregion
    }
}
