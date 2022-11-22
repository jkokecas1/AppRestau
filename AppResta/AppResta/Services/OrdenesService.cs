using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials;

namespace AppResta.Services
{
    public class OrdenesService
    {


        public static List<Model.Ordenes> Ordene()
        {
            Model.Ordenes orden;
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrden");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)//(response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {

                    orden = new Model.Ordenes();

                    orden.id = Int32.Parse(item["id"].ToString());
                    orden.fecha_orden = item["fecha_orden"].ToString().Remove(0, 10);
                    if (item["fecha_start"].ToString() != "")
                    {
                        orden.fecha_start = item["fecha_start"].ToString().Remove(0, 10);
                        orden.fecha_estimada = item["fecha_estimada"].ToString().Remove(0, 10);
                    }
                    //PLATILLOS
                    // orden.fecha_cerada = ObtenerNumeorDeItems(orden.id, 1);// + "/" + ObtenerNumeorDeItemsPlatillos(orden.id, 1);
                    switch (item["estado"].ToString())
                    {
                        case "1": orden.estado = "En espera"; break;
                        case "2": orden.estado = "Preparando... "; break;
                        case "3": orden.estado = "! Terminado !"; break;
                    }
                    //BEBIDAS
                    //orden.mesero = ObtenerNumeorDeItems(orden.id, 2);//+ "/" + ObtenerNumeorDeItemsPlatillos(orden.id,2);
                    orden.mesa = item["mesa"].ToString();
                    // orden.totoalExtras = obtenerPagoFinal(orden.id)[1].ToString();
                    //orden.total = obtenerPagoFinal(orden.id)[0].ToString() ;// 
                    orden.pago = Int32.Parse(item["pago"].ToString());
                    ordenList.Add(orden);
                }
                return ordenList;
            }
            else
            {
                return null;
            }
        }

        public static List<Model.Ordenes> OrdeneBar()
        {
            Model.Ordenes orden;
            var h = DateTime.Now.ToString("yyyy-MM-dd");
            string[] itemaux = new string[3];
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrden");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                // Console.WriteLine(jsonArray);
                foreach (var item in jsonArray)
                {
                    //Console.WriteLine(item["fecha_orden"].ToString().Remove(10, 9)+"==" +h);
                    orden = new Model.Ordenes();
                    if (item["fecha_orden"].ToString().Remove(10, 9) == h && (item["estado"].ToString() == "1" || item["estado"].ToString() == "2"))
                    {
                        itemaux = bebidas(item["id"].ToString(), item["mesa"].ToString());
                        // Console.WriteLine(itemaux);
                        if (itemaux[0] != null)
                        {
                            //arlist.Add(itemaux[0].ToString());
                            // arlist2.Add(itemaux[3].ToString());
                            orden.id = Int32.Parse(item["id"].ToString());
                            orden.fecha_orden = itemaux[0]; // Items Bebidaas
                            orden.fecha_start = itemaux[1]; // Item Mesa
                            orden.mesa = item["mesa"].ToString();
                            //orden.fecha_estimada = item["fecha_estimada"].ToString();
                            // orden.fecha_cerada = item["fecha_cerada"].ToString();
                            /*switch (item["estado"].ToString())
                            {
                                case "1": orden.estado = "En espera"; break;
                                case "2": orden.estado = "Preparando..."; break;
                                case "3": orden.estado = "! Terminado !"; break;
                            }*/
                            // orden.mesero = item["mesero"].ToString();

                            //orden.total = item["total"].ToString();
                            //orden.pago = Int32.Parse(item["pago"].ToString());


                            ordenList.Add(orden);
                        }
                        //   
                    }

                }
                return ordenList;
            }
            else
            {
                return null;
            }
        }

        public static string[] bebidas(string id, string mesa)
        {
            string[] ordenList = new string[4];
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 1);//110&mesa=MESA-4

            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                //Console.WriteLine(jsonArray);
                foreach (var item in jsonArray)
                {
                    ordenList[0] += item["cantidad"] + " X " + item["nombre"] + "\n       --" + item["comentario"].ToString().Replace("-", " ") + "\n \n";
                    ordenList[1] = item["mesa"].ToString();
                    ordenList[2] += item["idItem"].ToString();
                    ordenList[3] = item["idItem"].ToString();
                }
                return ordenList;
            }
            return ordenList;
        }

        public static List<Model.Ordenes> OrdeneCocina()
        {
            Model.Ordenes orden;
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();
            string[] aux = new string[3];

            var h = DateTime.Now.ToString("yyyy-MM-dd");
            orden = new Model.Ordenes();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrden");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                //Console.WriteLine(jsonArray);
                /*foreach (var item in jsonArray)
                {
                    //if (item["fecha_orden"].ToString().Remove(10, 9) == h)
                   // {
                        aux = bebidas(item["id"].ToString(), item["mesa"].ToString());

                        if (aux[0] != null)
                        {
                            if (true) {
                                orden.id = Int32.Parse(item["id"].ToString());
                                orden.fecha_orden = item["fecha_orden"].ToString();
                                orden.fecha_start = item["fecha_start"].ToString();
                                orden.fecha_estimada = item["fecha_estimada"].ToString();
                                orden.fecha_cerada = item["fecha_cerada"].ToString();
                                switch (item["estado"].ToString())
                                {
                                    case "1": orden.estado = "En espera"; break;
                                    case "2": orden.estado = "Preparando..."; break;
                                    case "3": orden.estado = "! Terminado !"; break;
                                }
                                orden.mesero = item["mesero"].ToString();
                                orden.mesa = item["mesa"].ToString();
                                orden.total = item["total"].ToString();
                                orden.pago = Int32.Parse(item["pago"].ToString());
                                ordenList.Add(orden);

                            }


                           


                        }
                   // }

                }*/
                foreach (var item in jsonArray)
                {
                    if (item["fecha_orden"].ToString().Remove(10, 9) == h)
                    {
                        aux = platillos(item["id"].ToString(), item["mesa"].ToString());

                        if (aux[0] != null)
                        {
                            //Console.WriteLine(item["estado"].ToString());
                            orden = new Model.Ordenes();
                            // Console.WriteLine("Estado ORDEN = " + item["estado"] + " ---" + aux[0]);
                            if (Int32.Parse(item["estado"] + "") == 2 || Int32.Parse(item["estado"] + "") == 1 || Int32.Parse(item["estado"] + "") == 0)
                            {

                                orden.id = Int32.Parse(item["id"].ToString());
                                orden.fecha_orden = item["fecha_orden"].ToString();
                                orden.fecha_start = item["fecha_start"].ToString();
                                orden.fecha_estimada = item["fecha_estimada"].ToString();
                                orden.fecha_cerada = item["fecha_cerada"].ToString();
                                switch (item["estado"].ToString())
                                {
                                    case "1": orden.estado = "En espera"; break;
                                    case "2": orden.estado = "Preparando..."; break;
                                    case "3": orden.estado = "! Terminado !"; break;
                                }
                                orden.mesero = item["mesero"].ToString();
                                orden.mesa = item["mesa"].ToString();
                                orden.total = item["total"].ToString();
                                orden.pago = Int32.Parse(item["pago"].ToString());


                                ordenList.Add(orden);

                            }
                        }
                    }

                }

                return ordenList;
            }
            else
            {
                return null;
            }
        }

        public static string[] platillos(string id, string mesa)
        {
            string[] ordenList = new string[3];
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 2);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                foreach (var item in jsonArray)
                {
                    ordenList[0] += item["cantidad"] + "X" + item["nombre"] + "\n";
                    ordenList[1] = item["mesa"].ToString();
                    ordenList[2] = item["idItem"].ToString();

                }
                return ordenList;
            }
            return ordenList;
        }
    }
}
