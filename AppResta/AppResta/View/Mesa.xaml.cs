using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Mesa : ContentPage
    {
        string nomb = "";
        public Mesa(string nombre = "NA")
        {
            InitializeComponent();
            BindingContext = new ViewModel.MesaViewModel(Navigation);
            nomb = nombre;
            nombreEmpl.Text = "Bienvenido : "+nombre;
            mesasListView.ItemsSource = Mesas();
            Navigation.RemovePage(new Login());
        }
        public void select_Item(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectionData(e.PreviousSelection, e.CurrentSelection);
        }

        public void UpdateSelectionData(IEnumerable<object> previousSelectedContact, IEnumerable<object> currentSelectedContact)
        {
            var mesas = currentSelectedContact.FirstOrDefault() as Model.Mesas;


            //Object[] datos = { false, nomb, mesas.mesa };
            Navigation.PushAsync(new Main(true,idOrden:0, nomb, mesas.mesa));
        }
        public List<Model.Mesas> Mesas()
        {
            Model.Mesas mesa;

            var mesas = new List<Model.Mesas>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/empleados/index.php?op=obtenerMesas");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    mesa = new Model.Mesas();
                    int id = Int32.Parse(item["id"].ToString());
                    string nombre = item["mesa"].ToString();

                    mesa.id = id;
                    mesa.mesa = nombre;
                    mesas.Add(mesa);
                }

                return mesas;
            }
            else
            {
                return null;
            }
        }
    }
}