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
*: Archivo      :   CartService.cs
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
*: 20/SEP/2022 Jorge - Se creo AllCartMesa() para obtner los datos de los carritos de ese dia
*: 21/SEP/2022 Jorge - Se mejoro AllCartMesa() para cambiar el dato de los comentarios
*: 21/SEP/2022 Jorge - Se creo CartMesa() para obtner los datos que tine cada mesa
*: 22/SEP/2022 Jorge - Se creo CartMesaBebidas() para obtner los datos que pertenecen al bar
*: 22/SEP/2022 Jorge - Se creo Carts() para obtner los datos por fecha de los caritos
*: 22/SEP/2022 Jorge - Se creo ExtrasItem() para obtner los datos de los platillos que tengan extras
*: 23/SEP/2022 Jorge - Se creo CartitoMesa() para obtner los datos que tine cada mesa
*: 23/SEP/2022 Jorge - Se creo ExtrasItem() para obtner los datos de los platillos que tengan extras
*: 23/SEP/2022 Jorge - Se creo SubCategorias() para obtner los datos que tine cada categoria
*: 23/SEP/2022 Jorge - Se creo Platillo() para obtner los datos de los platillos
*:----------------------------------------------------------------------------
*/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AppResta.Services
{
    public class CartService
    {
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        // Metodo que obtine todos los platillos que tiene cada mesa
        // 

        public static List<Model.Cart> AllCartMesa()
        {
            var client = new HttpClient();
            Model.Cart cartItem;
            List<Model.Cart> cart = new List<Model.Cart>();
            var h = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=0&mesa=MESA-0&opc=" + 3 + "&fecha=" + h + "-00:00:00");
            
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                Console.WriteLine(jsonArray);
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
        //--------------------------------------------------------------------------


        //--------------------------------------------------------------------------
        // Metodo que obtine el carrito segun la mesa y la orden
        //

        public static List<Model.Cart> CartMesa(string id, string mesa)
        {
            var client = new HttpClient();
            Model.Cart cartItem;
            List<Model.Cart> cart = new List<Model.Cart>();


            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 2 + "&fecha=2022-11-22");
            //Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 2 + "&fecha=2022-11-22");

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
        //--------------------------------------------------------------------------


        //--------------------------------------------------------------------------
        // Metodo que obtine del carrito solo las bebidas segun la orden y la mesa
        //

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
        //--------------------------------------------------------------------------


        //--------------------------------------------------------------------------
        // Metodo que obtine todos los caritos segun la fecha
        //

        public static List<Model.Cart> Carts(string fecha)
        {
            var client = new HttpClient();
            Model.Cart cartItem;
            List<Model.Cart> cart = new List<Model.Cart>();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerAllCart&fecha=" + fecha.Replace("-00:00:00",""));
           
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
                    cartItem.platillo = item["nombre"].ToString();
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
                    ExtrasItem(cartItem);
                    if (cartItem.extras == null) {
                        cartItem.extras = "N/A EXTRAS";
                    }
                    cartItem.extras.ToUpper();
                    //Console.WriteLine("............................"+cartItem.extras);
                    cart.Add(cartItem);

                }
                return cart;
            }
            else
            {
                return null;
            }
        }
        //--------------------------------------------------------------------------


        //--------------------------------------------------------------------------
        // Metodo que obtine los extras segun el platillo
        //

        public static void ExtrasItem(Model.Cart c)
        {

            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/platillo/index.php?op=obtenerExtrasAsItem&iditem=" + c.idItem);
           // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/platillo/index.php?op=obtenerExtrasAsItem&iditem=" + c.idItem);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                if (json.Equals("[]"))
                {
                    c.extras = "N/A EXTRAS";
                }
                else
                {
                    var jsonArray = JArray.Parse(json.ToString());
                    //Console.WriteLine(jsonArray);
                    foreach (var item in jsonArray)
                    {
                        if (Int32.Parse(item["item"].ToString()) == c.idItem)
                        {
                            c.extras += item["extra"].ToString() + ", ";
                        }
                        else {
                            c.extras = "N/A EXTRAS";
                        }
                    }
                }
            }
        }
        //--------------------------------------------------------------------------


        //--------------------------------------------------------------------------
        // Metodo que obtine los el carito segun la mesa y la orden
        //

        public static List<Model.Cart> CartitoMesa(string id, string mesa)
        {

            var sub = new List<Model.Cart>();
            var client = new HttpClient();
            Model.Cart cartItem;
            List<Model.Cart> cart = new List<Model.Cart>();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarrito&idOrden=" + id + "&mesa=" + mesa);

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
                    cartItem.platillo = item["nombre"].ToString();
                    cartItem.cantidad = Int32.Parse(item["cantidad"].ToString());
                    cartItem.precio = Convert.ToDouble(item["precio"].ToString().Replace(",", "."));
                    cartItem.total = (double)(cartItem.precio * cartItem.cantidad);
                    cartItem.visible = "false";

                    cart.Add(cartItem);

                }
                return cart;
            }
            else
            {
                return null;
            }
        }
        //--------------------------------------------------------------------------


        //--------------------------------------------------------------------------
        // Metodo que obtine las categorias de cada platillo
        //

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

                foreach (var item in jsonArray)
                {
                    categoria = new Model.Categorias();
                    categoria.id = Int32.Parse(item["id"].ToString());
                    categoria.nombre = item["nombre"].ToString();
                    categoria.estatus = item["estatus"].ToString();
                    userInfo.Add(categoria);
                }

                return userInfo;
            }
            else
            {
                return null;
            }
        }

        //--------------------------------------------------------------------------


        //--------------------------------------------------------------------------
        // Metodo que obtine las subcategorias de cada platillo
        //

        public static List<Model.SubCategorias> SubCategorias()
        {
          
            Model.SubCategorias subcategoria;
            List<Model.SubCategorias> sub = new List<Model.SubCategorias>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/categoria/index.php?op=allSubCategorias");
           
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
         
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    subcategoria = new Model.SubCategorias();
                    //Console.WriteLine(item["nombre"].ToString());
                    subcategoria.id = Int32.Parse(item["id"].ToString());
                    subcategoria.nombre = item["nombre"].ToString();
                    subcategoria.estatus = Int32.Parse(item["estatus"].ToString());
                    subcategoria.idCategoria = Int32.Parse(item["idCategoria"].ToString());
                    sub.Add(subcategoria);
                }
                if (sub.Count == 0)
                {
                    return null;
                }
                else
                {
                    return sub;
                }
            }
            else
            {

                return null;
            }
        }
        //--------------------------------------------------------------------------


        //--------------------------------------------------------------------------
        // Metodo que obtine las platillos
        //

        public static List<Model.Platillos> Platillos(string opc)
        {
         
            Model.Platillos platillo;
            var sub = new List<Model.Platillos>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/Platillo/index.php?op=obtenerPlatillos" + opc);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();

                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    platillo = new Model.Platillos();
                    //Console.WriteLine(urls);
                    /*
                    var byteArray = Convert.FromBase64String(item["url"].ToString().Replace("data:media_type;base64,", ""));

                    Stream stream = new MemoryStream(byteArray);
                    var imageSource = ImageSource.FromStream(() => stream);
                    */
                    //MyImage.Source = imageSource;
                    platillo.id = Int32.Parse(item["id"].ToString());
                    platillo.nombre = item["nombre"].ToString();
                    platillo.descrip = item["descrip"].ToString();
                    platillo.precio = item["precio"].ToString();
                    platillo.url = item["url"].ToString().Remove(0, 23);
                    //platillo.imgurl = imageSource;
                    platillo.estatus = Int32.Parse(item["estatus"].ToString());
                    platillo.categoria = item["categoria"].ToString();
                    platillo.clasificacion = item["clasificacion"].ToString();
                    platillo.subcategoria = item["subcategoria"].ToString();


                    sub.Add(platillo);

                }
                return sub;
            }
            else
            {
                return null;
            }
        }
        public static List<Model.Extras> Extras(int id)
        {
            List<Model.Extras> extras = new List<Model.Extras>();
            Model.Extras extra;
            var sub = new List<Model.Extras>();
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/Platillo/index.php?op=obtenerExtras&id=" + id.ToString());
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    extra = new Model.Extras();

                    extra.platillo_id_platillo = Int32.Parse(item["platillo_id_platillo"].ToString());
                    extra.nombre = item["nombre"].ToString();
                    extra.id_esxtra = Int32.Parse(item["id_esxtra"].ToString());
                    extra.extra_nombre = item["extra_nombre"].ToString();
                    extra.extra_precio = Convert.ToDouble(item["extra_precio"].ToString().Replace(",", "."));

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
    //--------------------------------------------------------------------------

    

}
