﻿using System;
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
        public List<Categorias> categoria;
        #endregion

        #region CONSTRUCTOR
        public MainViewModel(INavigation navigation, bool _Token)
        {
            Navigation = navigation;
            this._Token = _Token;
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
        #endregion

        #region PROCEOS
        public async Task PageLogin()
        {
            await Navigation.PopAsync();
        }

        public async Task Pago()
        {
            await Navigation.PushAsync(new Pago());
       
        }
        public async Task Mesas()
        {
            await Navigation.PushAsync(new Mesa());

        }
        public void ProcesoSimple()
        {
            if (_Token == true)
            {

                Navigation.PushAsync(new Login());
            }
        }


       
        #endregion
        #region COMAND

        public ICommand NavLoginCommand => new Command(ProcesoSimple);
        public ICommand Pagocommand => new Command(async () => await Pago());
        public ICommand Mesascommand => new Command(async () => await Mesas());
        public ICommand ValidarToken => new Command(ProcesoSimple);
        #endregion
    }
}