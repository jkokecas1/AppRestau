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
    public partial class ItemPlatillo : Rg.Plugins.Popup.Pages.PopupPage
    {

        Model.Extras extra = new Model.Extras();
        Model.Platillos platillos;
        string mesasGlb;
        int band = 0, idItems, cant;
        List<Model.Extras> listExtra;
        
        public ItemPlatillo(Model.Platillos platillo, string mesa, int bandera, int idItem= 0, int cantidad=0)
        {
            listExtra = new List<Model.Extras>();
            mesasGlb = mesa;
            idItems = idItem;
            cant = cantidad;
            band = bandera;
            InitializeComponent();
            platillos = platillo;
            nombPlatillo.Text = platillos.nombre;
            descPlatillo.Text = platillos.descrip;
            

            extrasListView.ItemsSource = Extras(platillos.id); 
            
        }

       
        private void cerrarPop(object sender, EventArgs e)
        {
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        public void cantidadPlatillo(object sender, ValueChangedEventArgs e) {
            var value = e.NewValue;
            valCantidad.Text = value.ToString();
        }

        void selectmultiple(object sender, SelectionChangedEventArgs e)
        {
            foreach (Model.Extras values in e.CurrentSelection  ) {
                listExtra.Add(values);
                
            }
            
        }

        public void agregarItemCart(object sender, EventArgs e)
        {
            int idplatillo= platillos.id;
            int cantidad = Int32.Parse(valCantidad.Text);
            var hora = DateTime.Now.ToString("hh:mm:ss");
            var fecha = DateTime.Now.ToString("MM-dd-yy");
            int extra = 0;
            string comentario = comentTxt.Text;
            var client = new HttpClient();
            string cadena = "";
            double total = cantidad + Convert.ToDouble(platillos.precio.Replace(",", "."));


            
            for (int i = 0; i < listExtra.ToArray().Length; i++){
                Console.WriteLine(listExtra.ToArray()[i].extra_nombre);

            }

            /*if (band == 0) // CASO 0: SI EL CARITO PARA ESA MESA NO EXITE 
            {
                cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=insertItemPlatillo&idplatillo=" + idplatillo.ToString() + "&idextra=" + extra + "&cantidad=" + cantidad.ToString() + "&fecha=" + fecha + hora + "&mesa=" + mesasGlb.ToString() + "&total=" + total.ToString() + "&comen=" + comentario;
                client.BaseAddress = new Uri(cadena);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
                if (response.IsSuccessStatusCode)
                {
                    Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
                }
                else
                {
                    DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo", "OK");

                }
            }
            else if (band == 1) // CASO 1: SI LE CARRITO EXISTE, Y ES EL MISMO PRODUCTO
            {
                cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateItemPlatillo&cantidad=" + (cant+cantidad) + "&idPlatillo=" + platillos.id + "&idItem=" + idItems;
                Console.WriteLine(cadena);
                client.BaseAddress = new Uri(cadena);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
                if (response.IsSuccessStatusCode)
                {
                    Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
                }
                else
                {
                    DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo", "OK");

                }
            }
            else if (band == 2) // CASP 2: SI EL CARRITO EXISTE, Y NO ES EL MISMO PLATILLO
            {
                cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=insertarItems&cantidad=" + cantidad + "&idPlatillo=" + platillos.id + "&idItem=" + idItems;
                client.BaseAddress = new Uri(cadena);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
                if (response.IsSuccessStatusCode)
                {
                    Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
                }
                else
                {
                    DisplayAlert("Error", "Fallo el registro \n Intentalo de nuevo", "OK");
                }

            }*/


            cant = 0;
           
        }

        public List<Model.Extras> Extras(int id)
        {
            List<Model.Extras> extras = new List<Model.Extras>();
            var sub = new List<Model.Extras>();
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/Platillo/index.php?op=obtenerExtras&id="+id.ToString());
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    extra = new Model.Extras();

                    int idplat = Int32.Parse(item["platillo_id_platillo"].ToString());
                    string nombrePlatillo = item["nombre"].ToString();
                    int idextra = Int32.Parse(item["id_esxtra"].ToString());
                    string nombreExtra = item["extra_nombre"].ToString();
                    double precioExtra = Convert.ToDouble(item["extra_precio"].ToString().Replace(",", "."));
                   
                    extra.platillo_id_platillo = idplat;
                    extra.nombre = nombrePlatillo;
                    extra.id_esxtra = idextra;
                    extra.extra_nombre = nombreExtra;
                    extra.extra_precio = precioExtra;


                    extras.Add(extra);

                }
                return extras;
            }
            else
            {
                return null;
            }
        }
    }
}