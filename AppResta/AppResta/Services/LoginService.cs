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
*: Archivo      :   LoginService.cs
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
*: 24/SEP/2022 Jorge - Se creo Empleados() para obtner los datos de los empleados
*: 24/SEP/2022 Jorge - Se creo Mesas() todas las mesas
*: 24/SEP/2022 Jorge - Se creo OrdenInMesas() para obtner la orden de la mesa
*: 24/SEP/2022 Jorge - Se mejoro OrdenInMesas() se agrego codigos de colores
*:----------------------------------------------------------------------------
*/

using AppResta.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppResta.Services
{
    public class LoginService : iLoginRespository
    {
        //--------------------------------------------------------------------------
        // Metodo que obtine todos los empleados 
        // 

        public static async void Empleados(bool band)
        {
            if (band == true)
            {
                var client = new HttpClient();
                Model.Empleado empl;

                List<Model.Empleado> datos = await App.Database.GetEmpleadoAsync();
                //{"id":1,"nombre":"Joreg M","pin":"1112","puesto":"Cajero","email":"(871) 736-2020","celular":"Jorge@resta.mz","estatus":1}
                client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/empleados/index.php?op=obtenerEmpleados");
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    string json = content.ToString();
                    var jsonArray = JArray.Parse(json.ToString());
                    string[] array = new string[2];
                    if (jsonArray.Count != datos.Count)
                    {
                        foreach (var item in jsonArray)
                        {
                            empl = new Model.Empleado();
                            empl.id = Int32.Parse(item["id"].ToString());
                            empl.nombre = item["nombre"].ToString();
                            empl.pin = item["pin"].ToString();
                            empl.puesto = item["puesto"].ToString();
                            empl.email = item["email"].ToString();
                            empl.celular = item["celular"].ToString();
                            empl.estatus = Int32.Parse(item["estatus"].ToString());
                            await App.Database.SaveEmpleadoAsync(empl);
                        }
                    }

                }
            }

        }


        public List<Empleado> Login(string pins)
        {
            Model.Empleado usuario;

            var empelado = new List<Empleado>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/empleados/index.php?op=obtenerPIN&pin=" + pins);
            //client.BaseAddress = new Uri("http://apprestaurante871.000webhostapp.com/mysql/empleados/index.php?op=obtenerPIN&pin=" + pins);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            /* if (response.IsSuccessStatusCode)
             {
                 string content = response.Content.ReadAsStringAsync().Result;
                 userInfo = JsonConvert.DeserializeObject<List<UserInfo>>(content);

                 return await Task.FromResult(userInfo.FirstOrDefault());

             }
             else { 
                 return null;
             }*/
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();

                var jsonArray = JArray.Parse(json.ToString());

                foreach (var item in jsonArray)
                {
                    usuario = new Model.Empleado();

                    //id":1,"nombre":"Joreg M","pin":"1112","puesto":"Cajero","email":"(871) 736-2020","celular":"Jorge@resta.mz","estatus":1}

                    usuario.id = Int32.Parse(item["id"].ToString());
                    usuario.nombre = item["nombre"].ToString();
                    usuario.pin = item["pin"].ToString();
                    usuario.puesto = item["puesto"].ToString();
                    usuario.email = item["email"].ToString();
                    usuario.celular = item["celular"].ToString();
                    empelado.Add(usuario);
                }

            }
            else
            {
                return null;
            }
            if (empelado.Count > 0)
            {
                return empelado;
            }
            else
            {
                return null;
            }

        }

    }
}
