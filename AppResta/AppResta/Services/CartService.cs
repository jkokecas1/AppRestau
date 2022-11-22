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
    public class CartService
    {

       

        // Get new data rows
        public static List<Model.Cart> CartMesa(string id, string mesa)
        {
            var client = new HttpClient();
            Model.Cart cartItem;
            List<Model.Cart> cart = new List<Model.Cart>();


            //client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarrito&idOrden=" + id + "&mesa=" + mesa);
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 2);
            // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 2);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    cartItem = new Model.Cart();

                    cartItem.id = Int32.Parse(item["id"].ToString());
                    cartItem.idItem = Int32.Parse(item["idItem"].ToString());
                    cartItem.platillo = "X  " + item["nombre"].ToString();
                    cartItem.cantidad = Int32.Parse(item["cantidad"].ToString());

                    if (item["comentario"] != null && item["comentario"].ToString() != "")
                    {
                        cartItem.comentario = item["comentario"].ToString().ToUpper().Replace("-", " ");
                    }
                    else
                    {
                        cartItem.comentario = "SIN COMENTARIOS";
                    }
                    cartItem.estado = item["estado"].ToString();
                    cartItem.precio = Convert.ToDouble(item["precio"].ToString().Replace(",", "."));
                    cartItem.total = (double)(cartItem.precio * cartItem.cantidad);
                    cartItem.visible = "false";

                  //  ExtrasItem(cartItem);


                    cart.Add(cartItem);

                }

                return cart;
            }
            else
            {
                return null;
            }
        }
        public static void ExtrasItem(Model.Cart c)
        {

            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/platillo/index.php?op=obtenerExtrasAsItem&iditem=" + c.idItem);
            //Console.WriteLine("http://192.168.1.112/resta/admin/mysql/platillo/index.php?op=obtenerExtrasAsItem&iditem=" + c.idItem);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                if (json.Equals("[]"))
                {
                    c.extras = "0 EXTRAS";
                }
                else
                {
                    var jsonArray = JArray.Parse(json.ToString());

                    foreach (var item in jsonArray)
                    {
                        if (Int32.Parse(item["item"].ToString()) == c.idItem)
                        {
                            c.extras += item["extra"].ToString() + ", ";
                        }
                    }
                }
            }
        }

        public static List<Model.Categorias> Categorias2()
        {
            Model.Categorias categoria;
            var userInfo = new List<Model.Categorias>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/categoria/index.php?op=obtenerCategorias");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                ;
                var jsonArray = JArray.Parse(json.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray)
                {
                    categoria = new Model.Categorias();
                    int ids = Int32.Parse(item["id"].ToString());
                    string nombre = item["nombre"].ToString();
                    string estado = item["estatus"].ToString();
                    // Console.WriteLine(item["nombre"].ToString());
                    categoria.id = ids;
                    categoria.nombre = nombre;
                    categoria.estatus = estado;
                    userInfo.Add(categoria);
                }
                //userInfo.Add(categoria);
                //testListView.ItemsSource = userInfo
                return userInfo;
            }
            else
            {
                return null;
            }
        }

    }

}
