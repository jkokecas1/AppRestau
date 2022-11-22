﻿
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using AppResta.View;
using AppResta.ViewModel;
using AppResta.Services;
using AppResta.Model;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;

namespace AppResta.ViewModel
{

    public class LoginViewModel : BaseViewModel
    {
        readonly iLoginRespository _loginRespository = new LoginService();
        

        public event PropertyChangedEventHandler PropertyChanged;

        #region VARIABLES
        string _Pin;
        bool _IsInternet;

        List<Model.Mesas> mesas;
        List<Model.Ordenes> ordenes;
        List<Model.Ordenes> ordenesBar;
        List<Model.Ordenes> ordenesCocina;

        #endregion

        #region CONSTRUCTOR
        public LoginViewModel(INavigation navigation, bool internet)
        {
            _IsInternet = internet;
            Navigation = navigation;
            Num0Command = new Command(() => Pin += "0");
            Num1Command = new Command(() => Pin += "1");
            Num2Command = new Command(() => Pin += "2");
            Num3Command = new Command(() => Pin += "3");
            Num4Command = new Command(() => Pin += "4");
            Num5Command = new Command(() => Pin += "5");
            Num6Command = new Command(() => Pin += "6");
            Num7Command = new Command(() => Pin += "7");
            Num8Command = new Command(() => Pin += "8");
            Num9Command = new Command(() => Pin += "9");
            
            BorrarCommand = new Command(() => Pin = validarPIN());
            mesas = Services.LoginService.Mesas();
            ordenes = Services.OrdenesService.Ordene();
            ordenesBar = Services.OrdenesService.OrdeneBar();
            ordenesCocina = Services.OrdenesService.OrdeneCocina();
        }
        #endregion

        #region OBJETOS
        public string Pin
        {
            get { return _Pin; }
            set { SetValue(ref _Pin, value); }
        }

        public bool IsInternet { 
            get { return _IsInternet; }
            set { SetValue(ref _IsInternet, value); }
        }


        #endregion

        #region PROCEOS
        public string validarPIN() {
            if (Pin != "")
            {
                return Pin.Remove(Pin.Length - 1, 1);
            }
            else { 
                return "";
            }
        }


        public string Number
        {
            set
            {
                if (Pin != "" )
                {
                    Pin = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Number"));
                }
            }
            get
            {
                return Pin;
            }
        }

       

        public async Task Validacion()
        {
            bool resulPin = Regex.IsMatch(Pin, @"/^[0-4]$/{4}$");

            if (!resulPin)
            {
                await DisplayAlert("Error", "Email and Password is invalid", "Ok");

            }

        }

        string nombre;
        public async Task IsExisteAcount()
        {

            if (Pin == null)
            {
                await DisplayAlert("Error", "User not exist", "Ok");

            }
            List<Empleado> empleado;
            if (IsInternet)
            {
                empleado = await App.Database.GetEmpleadoAsync();
                //empleado = _loginRespository.Login(Pin);
            }
            else {
                Console.WriteLine("No internet" + IsInternet);
               
                empleado = await App.Database.GetEmpleadoAsync();
            }

            
            Model.Empleado emp = new Empleado(); ;
            if (empleado != null)
            {
                foreach (Empleado item in empleado)
                {
                    if (Pin == item.pin) {
                        emp.id = item.id;
                        emp.nombre = item.nombre;
                        emp.pin = item.pin;
                        emp.puesto = item.puesto;
                        emp.email = item.email;
                        emp.celular = item.celular;
                    }
                    
                }

                if (emp.puesto == "Cajero")
                {

                    //Cajero
                    await Navigation.PushAsync(new Cajero(), false);
                }
                else if (emp.puesto == "Mesero")
                {

                    await Navigation.PushAsync(new Mesa(mesas, empleado: emp, interent: IsInternet), false);
                }
                else if (emp.puesto == "Cocinero")
                {

                    //Cocinero
                    await Navigation.PushAsync(new Cocina(orden: ordenesCocina), false);
                }
                else if (emp.puesto == "Barra")
                {

                    // Barra
                    await Navigation.PushAsync(new Bar(orden: ordenesBar), false);

                }
                else {
                    // Barra
                    await Navigation.PushAsync(new PRUEBAS(), false);
                }

                Pin = "";
            }
            else
            {
                await DisplayAlert("Error", "User not exist", "Ok");
            }
        }


        #endregion
        #region COMAND
        public ICommand ValidacionCommand => new Command(async () => await Validacion());


        public ICommand NavMainCommand => new Command(async () => await IsExisteAcount());

        public ICommand Num0Command { private set; get; }
        public ICommand Num1Command { private set; get; }
        public ICommand Num2Command { private set; get; }
        public ICommand Num3Command { private set; get; }
        public ICommand Num4Command { private set; get; }
        public ICommand Num5Command { private set; get; }
        public ICommand Num6Command { private set; get; }
        public ICommand Num7Command { private set; get; }
        public ICommand Num8Command { private set; get; }
        public ICommand Num9Command { private set; get; }

        public ICommand BorrarCommand { private set; get; }

        //ublic ICommand ProcesoSimpleCommand => new Command(isExisteAcount);
        #endregion
    }
}
