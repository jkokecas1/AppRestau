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
    internal class TiketViewModel :BaseViewModel
    {
        #region VARIABLES
        string _id;
        private ObservableCollection<Model.Cart> items;
        #endregion

        public ObservableCollection<Model.Cart> Platillo { get { return items; } set { items = value; } }

        #region CONSTRUCTOR
        public TiketViewModel(INavigation navigation, string id, string mesa)
        {
            Navigation = navigation;
            Console.WriteLine(id+" + " +mesa);
            //Navigation = navigation;
            //Console.WriteLine(id);
            Platillo = new ObservableCollection<Model.Cart>(){
                new Model.Cart()
                {
                   platillo  = "PLATILLO DE PRUEBA"
                },
                  new Model.Cart()
                {
                    platillo  = "PLATILLO DE PRUEBA2"
                },
             };

            
            

        }
        #endregion

        #region OBJETOS

       
        public string Id { get { return _id; } set { _id = value; } }
        #endregion

        #region PROCESOS
        /*
        public async Task GetCartAsync(string id,string mesa)
        {
            await Services.TiketService.GetAllOrdenAsync(list =>
            {
                foreach (Model.Cart item in list)
                {
                    Items.Add(item);
                }

            }, id,mesa);
        }
        */
        #endregion


        #region COMANDOS
      
        #endregion

    }
}
