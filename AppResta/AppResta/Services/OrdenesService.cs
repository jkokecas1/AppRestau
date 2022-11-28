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

        public static int IDordene()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerIDOrden");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)//(response.IsSuccessStatusCode)
            {
                int id = 0;
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    id = Int32.Parse(item["id"] + "");
                }
                return id;
            }
            else
            {
                return 0;
            }
        }
        public static string[] IdOrden(string mesa)
        {

            var client = new HttpClient();
            string[] orden = new string[2];
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoOrden&mesa=" + mesa);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    orden[0] = item["ordern"].ToString();
                    orden[1] = item["estado"].ToString();
                }

            }
            //Console.WriteLine(ordenID);
            return orden;
        }


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
                        // if(item["fecha_estimada"] != null)
                        //  orden.fecha_estimada = item["fecha_estimada"].ToString().Remove(0, 10);
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


        /****************************************************************************************************
        *                              BAR
        **************************************************************************************************/

        public static List<Model.Ordenes> OrdeneBar()
        {
            Model.Ordenes orden;
            var h = DateTime.Now.ToString("yyyy-MM-dd");
            string[] itemaux = new string[3];
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenBebidas");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                //Console.WriteLine(jsonArray);
                foreach (var item in jsonArray)
                {
                    //Console.WriteLine(item["fecha_orden"].ToString().Remove(10, 9)+"==" +h);
                    orden = new Model.Ordenes();
                    if (item["fecha_orden"].ToString().Remove(10, 9) == h && (item["estado"].ToString() == "1" || item["estado"].ToString() == "2"))
                    {
                        //Console.WriteLine("Estadooooo -- "+item["estado"].ToString());
                        //itemaux = bebidas(item["id"].ToString(), item["mesa"].ToString());
                        // Console.WriteLine(itemaux);
                        //if (itemaux[0] != null)
                        // {
                        //arlist.Add(itemaux[0].ToString());
                        // arlist2.Add(itemaux[3].ToString());
                        orden.id = Int32.Parse(item["id"].ToString());
                        //orden.fecha_orden = itemaux[0]; // Items Bebidaas
                        //orden.fecha_start = itemaux[1]; // Item Mesa
                        orden.mesa = item["mesa"].ToString();
                        orden.mesero = item["mesero"].ToString();
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
                        // }
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


        public static List<Model.Ordenes> OrdeneBarEmpleado(string id)
        {
            Model.Ordenes orden;
            var h = DateTime.Now.ToString("yyyy-MM-dd");
            string[] itemaux = new string[3];
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenEmpleado&empleado=" + id);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                //  Console.WriteLine(jsonArray);
                foreach (var item in jsonArray)
                {

                    orden = new Model.Ordenes();

                    orden.id = Int32.Parse(item["id"].ToString());
                    orden.fecha_orden = itemaux[0]; // Items Bebidaas
                    orden.fecha_start = itemaux[1]; // Item Mesa
                    orden.mesa = item["mesa"].ToString();
                    orden.mesero = item["mesero"].ToString();

                    ordenList.Add(orden);

                }
                return ordenList;
            }
            else
            {
                Console.WriteLine("OrdeneBarEmpleado");
                return null;
            }

        }


        public static string[] bebidas(string id, string mesa)
        {
            string[] ordenList = new string[4];
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 1 + "&fecha=2022-11-22");//110&mesa=MESA-4

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

        /****************************************************************************************************
         *                              CAJERO
         **************************************************************************************************/
        public static List<Model.Ordenes> OrdenCaja()
        {
            Model.Ordenes orden;
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrden");
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
                    orden.fecha_orden = item["fecha_orden"].ToString().Remove(0, 10);
                    if (item["fecha_start"].ToString() != "")
                    {
                        orden.fecha_start = item["fecha_start"].ToString().Remove(0, 10);
                        //orden.fecha_estimada = item["fecha_estimada"].ToString().Remove(0, 10);
                    }
                    //PLATILLOS
                    orden.fecha_cerada = item["fecha_cerada"].ToString();
                    switch (item["estado"].ToString())
                    {
                        case "1": orden.estado = "En espera"; orden.fecha_estimada = "#11111"; break;
                        case "2": orden.estado = "Preparando... "; orden.fecha_estimada = "#11111"; break;
                        case "3": orden.estado = "! Terminado !"; orden.fecha_estimada = "#ff0000"; break;
                    }
                    //BEBIDAS
                    orden.mesero = item["mesero"].ToString();
                    orden.mesa = item["mesa"].ToString();
                    //  orden.totoalExtras = item["totoalExtras"].ToString();
                    orden.total = item["total"].ToString();
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

        public static List<Model.Ordenes> OrdenecajeroEmpleado(string id)
        {
            Model.Ordenes orden;
            var h = DateTime.Now.ToString("yyyy-MM-dd");
            string[] itemaux = new string[3];
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenCajaEmpleado&empleado=" + id);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                //  Console.WriteLine(jsonArray);
                foreach (var item in jsonArray)
                {

                    orden = new Model.Ordenes();

                    orden.id = Int32.Parse(item["id"].ToString());
                    orden.fecha_orden = itemaux[0]; // Items Bebidaas
                    orden.fecha_start = itemaux[1]; // Item Mesa
                    orden.mesa = item["mesa"].ToString();
                    orden.mesero = item["mesero"].ToString();

                    ordenList.Add(orden);

                }
                return ordenList;
            }
            else
            {
                Console.WriteLine("OrdeneBarEmpleado");
                return null;
            }

        }


        /****************************************************************************************************
         *                              CCOCINA
         **************************************************************************************************/

        public static List<Model.Ordenes> OrdeneCocina()
        {
            Model.Ordenes orden;
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();
            string[] aux = new string[3];

            var h = DateTime.Now.ToString("yyyy-MM-dd");
            orden = new Model.Ordenes();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenComidas");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    if (item["fecha_orden"].ToString().Remove(10, 9) == h)
                    {
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
                            /* aux = platillos(item["id"].ToString(), item["mesa"].ToString());

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

                                 }*/
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
        public static List<Model.Ordenes> OrdeneCocinaEmpleado(string id)
        {
            Model.Ordenes orden;
            var h = DateTime.Now.ToString("yyyy-MM-dd");
            string[] itemaux = new string[3];
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenCajaEmpleado&empleado=" + id);
            // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenEmpleado&empleado=" + id);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                 Console.WriteLine(jsonArray);
                foreach (var item in jsonArray)
                {

                    orden = new Model.Ordenes();

                    orden.id = Int32.Parse(item["id"].ToString());
                    orden.fecha_orden = itemaux[0]; // Items Bebidaas
                    orden.fecha_start = itemaux[1]; // Item Mesa
                    orden.mesa = item["mesa"].ToString();
                    orden.mesero = item["mesero"].ToString();

                    ordenList.Add(orden);

                }
                return ordenList;
            }
            else
            {
                Console.WriteLine("OrdeneBarEmpleado");
                return null;
            }

        }
        public static string[] platillos(string id, string mesa)
        {
            string[] ordenList = new string[3];
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerCarritoBebidas&idOrden=" + id + "&mesa=" + mesa + "&opc=" + 2 + "&fecha=2022-11-22");
            // Console.WriteLine();

            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                //Console.WriteLine(json);
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
