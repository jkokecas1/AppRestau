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
    public class TiketService
    {
        // Get new data rows
        public static async Task GetAllOrdenAsync(Action<IEnumerable<Model.Cart>> action, string id,string mesa)
        {
          
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoFinal&idOrden=" + id + "&mesa=" +mesa);
            Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoFinal&idOrden=" + id + "&mesa=" + mesa);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<Model.Cart> cart = new List<Model.Cart>();
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                
                
                foreach (var item in jsonArray)
                {

                    Model.Cart cartItem = new Model.Cart();

                    cartItem.id = Int32.Parse(item["id"].ToString());
                    cartItem.idItem = Int32.Parse(item["idItem"].ToString());
                    cartItem.platillo = item["platillo"].ToString();
                    cartItem.cantidad = Int32.Parse(item["cantidad"].ToString());
                    cartItem.precio = Convert.ToDouble(item["precio"].ToString().Replace(",", "."));
                    cartItem.total = (double)(cartItem.precio * cartItem.cantidad);
                    cartItem.comentario = item["comentario"].ToString();


                   // Console.WriteLine(cartItem.id+","+cartItem.idItem + "," + cartItem.platillo + "," + cartItem.cantidad + "," + cartItem.precio);
                    cart.Add(cartItem);
                }
                //var list = JsonConvert.DeserializeObject<IEnumerable<Model.Cart>>(await response.Content.ReadAsStringAsync());
                
                action(cart);
            }

        }

    }

}
