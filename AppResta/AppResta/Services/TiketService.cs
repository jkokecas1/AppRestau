/*:-----------------------------------------------------------------------------
*:                          PRACTICANTES 
*:                          AW SOFTWARE
*: 
*:                           Ago – Dic 
*: 
*: 
*:                  Clase para consumir web services
*: 
*: 
*: Archivo      :   TiketService.cs
*: Autor        :   Jorge Gerardo Moreno Castillo
*:                  Jaen
*:              
*: Fecha        :   20/SEP/2022
*: Compilador   :   Microsoft Visual Studio Community 2022 (64-bit) - Current Version 17.2.6
*: Descripción  :   Esta clase implementa el consumo de api de la pagina web.
*                   Se forma por un metodo por cada api.
*: Ult.Modif.   :   2022/01/12
*: Fecha Modificó Modificacion
*:=============================================================================
*: 24/SEP/2022 Jorge - Se creo GetAllOrdenAsync() obtner el carrito que se va apagar
*:----------------------------------------------------------------------------
*/
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
        //--------------------------------------------------------------------------
        // Metodo que obtine el carrito a pagar
        // 

        public static async Task GetAllOrdenAsync(Action<IEnumerable<Model.Cart>> action, string id,string mesa)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoFinal&idOrden=" + id + "&mesa=" +mesa);
           // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoFinal&idOrden=" + id + "&mesa=" + mesa);
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
                    cart.Add(cartItem);
                }
               
                action(cart);
            }

        }

    }

}
