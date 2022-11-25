using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerBar : Rg.Plugins.Popup.Pages.PopupPage
    {
        // VARIABLES GLOBALES
        public static List<Model.Cart> ORDEN;
        Model.Ordenes orden;
        CollectionView activas;
        string empl;
       
       
      

        // CONSTRUCTOR
        public VerBar(Model.Ordenes orden, List<Model.Ordenes> ordenes = null, CollectionView activas = null, string emleado="")
        {
            InitializeComponent();
            this.empl = emleado;
            int a = -1;
            if (ordenes.Count < ordenes.Count / 2)
            {
                a = ordenes.IndexOf(orden);
            }
            else
            {
                ordenes.Reverse();
                a = ordenes.IndexOf(orden);
            }
            //numeroOrden.Text = "ORDEN NUMERO: " + orden.id + "";

            if (a != -1)
                orden = ordenes[a];

            ORDEN = Services.CartService.CartMesaBebidas(orden.id.ToString(), orden.mesa);

            barlist.ItemsSource = ORDEN;
            this.orden = orden;
            this.activas = activas;
        }

     

        // METODOS

        /*****************************************************************
         *  METODO btnListo_Clicked
         *      TERMINA LA ORDEN 
         *****************************************************************/
        private void btnListo_Clicked(object sender, EventArgs e)
        {
            foreach (Model.Cart car in ORDEN)
            {
                string cadena2 = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateEstadoItem&idItem=" + car.idItem;
                SET_DATOS(cadena2);
            }

            int bebidas = Int32.Parse(ObtenerNumeorDeItems(Int32.Parse(orden.id + ""), 1));
            int platillos = Int32.Parse(ObtenerNumeorDeItems(Int32.Parse(orden.id + ""), 2));
            int bebidas2 = Int32.Parse(ObtenerNumeorDeItemsPlatillos(Int32.Parse(orden.id + ""), 1));
            int platillos2 = Int32.Parse(ObtenerNumeorDeItemsPlatillos(Int32.Parse(orden.id + ""), 2));
            //  Console.WriteLine("Platillos:" + bebidas +" = "+ bebidas2);
            //Console.WriteLine("BEBIDAS:" + platillos + " = " + platillos2);
            var h = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            string cadena0 = "";
            if (bebidas == bebidas2 && platillos == platillos2)
            {
                cadena0 = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateOrden&estado=3" + "&fecha_inicio=" + h.Replace("/", "-").Replace(" ", "-") + "&fecha_estimada=" + h.Replace("/", "-").Replace(" ", "-") + "&idCart=" + orden.id+ "&empleado=" + empl;
                SET_DATOS(cadena0);
                string cadena = "http://192.168.1.112/resta/admin/mysql/Orden/index.php?op=updateEstadoOrden&idItem=" + orden.id;
                SET_DATOS(cadena);
            }

              activas.ItemsSource = Services.OrdenesService.OrdeneBar();
           // inactivas.ItemsSource = Services.OrdenesService.OrdeneBarEmpleado(empl);

            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }


        /*****************************************************************
         *  METODO btnListo_Clicked
         *      TERMINA LA ORDEN 
         *****************************************************************/
        public void SET_DATOS(string c)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(c);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                // DisplayAlert("Eliminado","", "OK");
            }

        }


        /*****************************************************************
         *  METODO cerrarPop
         *      sACA DEL APILA LA VENTANA EMERGENTE
         *****************************************************************/
        private void cerrarPop(object sender, EventArgs e)
        {
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }



        public static string ObtenerNumeorDeItemsPlatillos(int id, int opc)
        {
            string cantidad = "";

            var client1 = new HttpClient();

            client1.BaseAddress = new Uri(("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=ObtenerNumeorDeItemsPlatillos&idCart=" + id + "&opc=" + opc));
            HttpResponseMessage response1 = client1.GetAsync(client1.BaseAddress).Result;
            if (response1.IsSuccessStatusCode)
            {
                var content1 = response1.Content.ReadAsStringAsync().Result;
                string json1 = content1.ToString();

                var jsonArray1 = JArray.Parse(json1.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray1)
                {
                    cantidad = item["cantidad"].ToString();
                }
            }
            return cantidad;
        }

        public static string ObtenerNumeorDeItems(int id, int opc)
        {
            string cantidad = "";

            var client1 = new HttpClient();

            client1.BaseAddress = new Uri(("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=ObtenerNumeorDeItems&idCart=" + id + "&opc=" + opc));
            HttpResponseMessage response1 = client1.GetAsync(client1.BaseAddress).Result;
            if (response1.IsSuccessStatusCode)
            {
                var content1 = response1.Content.ReadAsStringAsync().Result;
                string json1 = content1.ToString();

                var jsonArray1 = JArray.Parse(json1.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray1)
                {
                    cantidad = item["cantidad"].ToString();
                }
            }
            return cantidad;
        }

    }
}