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

        public static List<Model.Cart> AllCartMesa()
        {
            var client = new HttpClient();
            Model.Cart cartItem;
            List<Model.Cart> cart = new List<Model.Cart>();

            var h = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=0&mesa=MESA-0&opc=" + 3 + "&fecha="+h+"-00:00:00");
            Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=0&mesa=MESA-0&opc=" + 3 + "&fecha=" + h + "-00:00:00");

            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
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
                    cartItem.mesa = item["mesa"].ToString();
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


        // Get new data rows
        public static List<Model.Cart> CartMesa(string id, string mesa)
        {
            var client = new HttpClient();
            Model.Cart cartItem;
            List<Model.Cart> cart = new List<Model.Cart>();


            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 2+"&fecha=2022-11-22");
            Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 2 + "&fecha=2022-11-22");

            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
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
                    cartItem.mesa = item["mesa"].ToString();
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

        public static List<Model.Cart> CartMesaBebidas(string id, string mesa)
        {
            var client = new HttpClient();
            Model.Cart cartItem;
            List<Model.Cart> cart = new List<Model.Cart>();


            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 1 + "&fecha=2022-11-22");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
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



        public static List<Model.Cart> Carts(string fecha)
        {
            var client = new HttpClient();
            Model.Cart cartItem;
            List<Model.Cart> cart = new List<Model.Cart>();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=1&mesa=MESA-3&opc=" + 3+"&fecha="+ fecha+"-07:00:00");
          // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=1&mesa=MESA-3&opc=" + 3 + "&fecha=" + fecha + "-07:00:00");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
               // Console.WriteLine(json);
                foreach (var item in jsonArray)
                {
                    cartItem = new Model.Cart();

                    cartItem.id = Int32.Parse(item["id"].ToString());
                    cartItem.idItem = Int32.Parse(item["idItem"].ToString());
                    cartItem.platillo = "X  " + item["nombre"].ToString();
                    cartItem.cantidad = Int32.Parse(item["cantidad"].ToString());
                    cartItem.mesa = item["mesa"].ToString();
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

                    //Console.WriteLine(cartItem.id);
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
          
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
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
                var jsonArray = JArray.Parse(json.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray)
                {
                    categoria = new Model.Categorias();
             
                    categoria.id = Int32.Parse(item["id"].ToString());
                    categoria.nombre = item["nombre"].ToString();
                    categoria.estatus = item["estatus"].ToString();
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
