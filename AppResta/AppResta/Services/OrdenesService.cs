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
*: Archivo      :   OrdenesService.cs
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
*: 24/SEP/2022 Jorge - Se creo IDordene() que obtine id de cada orden
*: 24/SEP/2022 Jorge - Se creo IdOrden() obtine el id de cada orden segun su mesa
*: 24/SEP/2022 Jorge - Se creo Ordene() Obtine todas las ordenes
*: 24/SEP/2022 Jorge - Se creo OrdeneBar() obtine las ordenes de bar
*: 24/SEP/2022 Jorge - Se creo OrdeneBarEmpleado() obtine el historia 
*: 24/SEP/2022 Jorge - Se creo OrdenCaja() obtine las ordenes de caja
*: 24/SEP/2022 Jorge - Se creo OrdenecajeroEmpleado() obtine las el historial de caja
*: 24/SEP/2022 Jorge - Se creo OrdeneCocina() obtien las ordenes de cocina
*: 24/SEP/2022 Jorge - Se creo OrdeneCocinaEmpleado() obtine el histoiral de cocina
*:----------------------------------------------------------------------------
*/
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
        
        //--------------------------------------------------------------------------
        // Metodo que obtine el id de cada orden
        // 

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
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        // Metodo que obtine le ide de cada orden segun su mesa
        // 

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
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        // Metodo que obtine todas las ordenes
        // 

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

        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        // Metodo que obtine ordenes que tiene le bar
        //
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
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        // Metodo que obtine todas las ordenes por empleado y fecha
        // 

        public static List<Model.Ordenes> OrdeneBarEmpleado(string id, string fecha)
        {
            Model.Ordenes orden;
            var h = DateTime.Parse(fecha);

            DateTime h2 = h;
            h2 = h2.AddDays(1);


            string[] itemaux = new string[3];
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenEmpleado&empleado=" + id+"&fecha="+h.ToString("yyyy-MM-dd") + "&fecha2=" + h2.ToString("yyyy-MM-dd"));
           //Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenEmpleado&empleado=" + id + "&fecha=" + h.ToString("yyyy-MM-dd") + "&fecha2=" + h2.ToString("yyyy-MM-dd"));
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                // Console.WriteLine(jsonArray);
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
         *                              CAJERO
         **************************************************************************************************/

        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        // Metodo que obtine todas las ordenes de caja
        // 
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
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        // Metodo que obtine todas las ordenes por fecha y id del empleado
        // 

        public static List<Model.Ordenes> OrdenecajeroEmpleado(string id, string fecha)
        {
            Model.Ordenes orden;
            var h = DateTime.Parse(fecha);

            DateTime h2 = h;
            h2 = h2.AddDays(1);
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenCajaEmpleado&empleado=" + id + "&fecha=" + h.ToString("yyyy-MM-dd") + "&fecha2=" + h2.ToString("yyyy-MM-dd"));
           //Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenCajaEmpleado&empleado=" + id + "&fecha=" + h.ToString("yyyy-MM-dd") + "&fecha2=" + h2.ToString("yyyy-MM-dd"));
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
               // Console.WriteLine(jsonArray);
                foreach (var item in jsonArray)
                {

                    orden = new Model.Ordenes();

                    orden.id = Int32.Parse(item["id"].ToString());
                    orden.fecha_orden = item["fecha_orden"].ToString();//itemaux[0]; // Items Bebidaas
                    orden.fecha_start = item["fecha_start"].ToString(); //itemaux[1]; // Item Mesa
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

        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        // Metodo que obtine todas las ordenes de cocina
        // 

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
                           // orden.fecha_cerada = item["fecha_cerada"].ToString();
                            switch (item["estado"].ToString())
                            {
                                case "1": orden.estado = "En espera"; orden.fecha_cerada = "#49FF00"; break;
                                case "2": orden.estado = "Preparando..."; orden.fecha_cerada = "#FF9F01"; break;
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
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        // Metodo que obtine todas las ordenes de cocina por empleado y fecha
        // 

        public static List<Model.Ordenes> OrdeneCocinaEmpleado(string id, string fecha)
        {
            Model.Ordenes orden;
            var h = DateTime.Parse(fecha);

            DateTime h2 = h;
            h2 = h2.AddDays(1);

            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=OrdeneCocinaEmpleado&empleado=" + id + "&fecha=" + h.ToString("yyyy-MM-dd") + "&fecha2=" + h2.ToString("yyyy-MM-dd"));
           // Console.WriteLine("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=OrdeneCocinaEmpleado&empleado=" + id + "&fecha=" + h.ToString("yyyy-MM-dd") + "&fecha2=" + h2.ToString("yyyy-MM-dd"));
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                // Console.WriteLine(jsonArray);
                foreach (var item in jsonArray)
                {

                    orden = new Model.Ordenes();

                    orden.id = Int32.Parse(item["id"].ToString());
                    orden.fecha_orden = item["fecha_orden"].ToString(); //itemaux[0]; // Items Bebidaas
                    orden.fecha_start = item["fecha_start"].ToString();///itemaux[1]; // Item Mesa
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
