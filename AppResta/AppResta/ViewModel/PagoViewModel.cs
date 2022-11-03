using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppResta.ViewModel
{
    public class PagoViewModel : BaseViewModel
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        #region VARIABLES

        string _Pin;
        int _Next;

        #endregion

        #region CONSTRUCTOR
        public PagoViewModel(INavigation navigation)
        {
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

        }
        #endregion

        #region OBJETOS
        public string Pin
        {
            get { return _Pin; }
            set { SetValue(ref _Pin, value); }
        }

        public int Next
        {
            get { return _Next; }
            set { SetValue(ref _Next, value); }
        }
        #endregion

        #region PROCEOS
        public string validarPIN()
        {
            if (Pin != "")
            {
                return Pin.Remove(Pin.Length - 1, 1);
            }
            else
            {
                return "";
            }
        }


        public string Number
        {
            set
            {
                if (Pin != "")
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
           
        }
        public async Task Siguiente()
        {
            if (Next == 0) {
                Console.WriteLine("Next 0");
                Next = 1;
            }else if (Next == 1)
            {
                Console.WriteLine("Next 1");
                Next++;
            }else if (Next == 2)
            {
                Console.WriteLine("Next 2");
                Next = 0;
            }
        }

        #endregion
        #region COMAND
        public ICommand ValidacionCommand => new Command(async () => await Validacion());


        public ICommand NavMainCommand => new Command(async () => await IsExisteAcount());
        public ICommand NextCampo => new Command(async () => await Siguiente());
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
