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
*: Archivo      :   MesasService.cs
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
*: 24/SEP/2022 Jorge - Se creo Mesas() todas las mesas
*: 24/SEP/2022 Jorge - Se creo OrdenInMesas() para obtner la orden de la mesa
*: 24/SEP/2022 Jorge - Se mejoro OrdenInMesas() se agrego codigos de colores
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
    public class MesasService
    {
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        // Metodo que obtine todas las mesas
        // 

        public static List<Model.Mesas> Mesas()
        {
            Model.Mesas mesa;

            var mesas = new List<Model.Mesas>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/empleados/index.php?op=obtenerMesas");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                string[] array = new string[2];
                foreach (var item in jsonArray)
                {
                    mesa = new Model.Mesas();

                    array = OrdenInMesas(item["mesa"].ToString(), DateTime.Now.ToString("yyyy-MM-dd"));
                    mesa.id = Int32.Parse(item["id"].ToString());
                    mesa.mesa = item["mesa"].ToString();
                    if (array[0] != null)
                    {
                        mesa.orden = array[0];
                        mesa.id_orden = array[1];
                    }
                    else
                    {
                        mesa.orden = "0";
                        mesa.id_orden = "0";
                    }
                    mesa.ubicacion = item["ubicacion"].ToString();
                    mesas.Add(mesa);
                }

                return mesas;
            }
            else
            {
                return null;
            }
        }


        public static List<Model.Mesas> ObtenerMesas()
        {
            Model.Mesas mesa;

            var mesas = new List<Model.Mesas>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/empleados/index.php?op=obtenerMesas");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                string[] array = new string[2];
                foreach (var item in jsonArray)
                {
                    mesa = new Model.Mesas();
                    int id = Int32.Parse(item["id"].ToString());
                    string nombre = item["mesa"].ToString();
                    string mesero = item["mesa"].ToString();
                    array = OrdenInMesas(nombre, DateTime.Now.ToString("yyyy-MM-dd"));
                    mesa.id = id;
                    mesa.mesa = nombre;
                    if (array[0] != null)
                    {
                        mesa.orden = array[0];
                        mesa.id_orden = array[1];
                    }
                    else
                    {
                        mesa.orden = "0";
                        mesa.id_orden = "0";
                    }
                    /* if (Int32.Parse(item["estado"].ToString()) == 0)
                    {
                       mesa.orden = "#2C67E6";
                        mesa.id_orden = item["id_orden"].ToString();
                    }
                    else if (Int32.Parse(item["estado"].ToString()) == 1)
                    {
                        mesa.orden = "#3AE62C ";
                        mesa.id_orden = item["id_orden"].ToString();
                    }
                    else if (Int32.Parse(item["estado"].ToString()) == 2)
                    {
                        mesa.orden = "#F7DB2F";
                        mesa.id_orden = item["id_orden"].ToString();
                    }
                    else
                    {
                        mesa.orden = "#E62C2C";
                        mesa.id_orden = item["id_orden"].ToString();
                    }*/
                    mesa.ubicacion = item["ubicacion"].ToString();
                    mesas.Add(mesa);
                }

                return mesas;
            }
            else
            {
                return null;
            }
        }


        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        // Metodo que obtine el estado de cada mesa 
        // 
        public static string[] OrdenInMesas(string mesa,string fecha)
        {
            var client = new HttpClient();
            string[] array = new string[2];
           
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenMesas&mesa=" + mesa+"&fecha="+fecha);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());


                foreach (var item in jsonArray)
                {
                    //Console.WriteLine(item["estado"]);
                    if (jsonArray.Count == 1)
                    {
                        if (Int32.Parse(item["estado"].ToString()) == 0)
                        {
                            array[0] = "#2C67E6";
                        }
                        else if (Int32.Parse(item["estado"].ToString()) == 1)
                        {
                            array[0] = "#3AE62C "; 
                        }
                        else if (Int32.Parse(item["estado"].ToString()) == 2)
                        {
                            array[0] = "#F7DB2F"; 
                        }
                        else
                        {
                            array[0] = "#E62C2C";
                        }
                        array[1] = item["id_orden"].ToString();
                    }
                    /*
                    if(Int32.Parse(item["cont"].ToString()) == 0)
                    {
                        array[0] = "#02B942";
                    }*/

                }

            }
            else
            {
                return null;
            }

            return array;
        }

    }

}
