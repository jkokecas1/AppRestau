using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AppResta.Services
{
    public class HostorialService
    {
        // Get new data rows
        public static async Task GetAllNewsAsync(Action<IEnumerable<Model.Cart>> action,int id, string mesas)
        {

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoFinal&idOrden=" + id + "&mesa=" + mesas);
            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
               List<Model.Cart> cart = new List<Model.Cart>();
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    var cartItem = new Model.Cart();

                    cartItem.id = Int32.Parse(item["id"].ToString());
                    cartItem.idItem = Int32.Parse(item["idItem"].ToString());
                    cartItem.platillo = item["platillo"].ToString();
                    cartItem.cantidad = Int32.Parse(item["cantidad"].ToString());
                    cartItem.precio = Convert.ToDouble(item["precio"].ToString().Replace(",", "."));
                    cartItem.total = (double)(cartItem.precio * cartItem.cantidad);
                    cartItem.comentario = item["comentario"].ToString();
                    //var list = JsonConvert.DeserializeObject<IEnumerable<Model.Cart>>(await response.Content.ReadAsStringAsync());

                    cart.Add(cartItem);
                }

               // var list = JsonConvert.DeserializeObject<IEnumerable<Model.Cart>>(await response.Content.ReadAsStringAsync());
                
                action(cart);
            }

        }

        public static async Task GetAllHistoryAsync(Action<IEnumerable<Model.Ordenes>> action)
        {

            Model.Ordenes orden;
            var ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenHistorial");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {

                    orden = new Model.Ordenes();
                    
                     orden.id = Int32.Parse(item["id"].ToString());
                     orden.fecha_orden = item["fecha_orden"].ToString(); ;
                     orden.fecha_cerada = item["fecha_cerada"].ToString(); ;
                     orden.mesero = item["mesero"]+"";
                     orden.mesa = item["mesa"].ToString();
                     orden.total = item["total"].ToString(); ;
                     orden.pago = Int32.Parse(item["pago"].ToString()); 
                    ordenList.Add(orden);

                    
                }

                // var list = JsonConvert.DeserializeObject<IEnumerable<Model.Cart>>(await response.Content.ReadAsStringAsync());

                action(ordenList);
            }

        }
    }
       
}
