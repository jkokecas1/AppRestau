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
                    int id = Int32.Parse(item["id"].ToString());
                    string nombre = item["mesa"].ToString();
                    string mesero = item["mesa"].ToString();
                    array = OrdenInMesas(nombre);
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

        //VALIDA SI LA MESA TINE ORDEN ACTIVA
        public static string[] OrdenInMesas(string mesa)
        {
            var client = new HttpClient();
            string[] array = new string[2];
            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrdenMesas&mesa=" + mesa);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                var jsonArray = JArray.Parse(json.ToString());
                foreach (var item in jsonArray)
                {
                    //Console.WriteLine(item["estado"]);
                    if (Int32.Parse(item["cont"].ToString()) == 1)
                    {
                        if (Int32.Parse(item["estado"].ToString()) == 0)
                        {
                            array[0] = "#2C67E6";
                            array[1] = item["id_orden"].ToString();
                        }
                        else if (Int32.Parse(item["estado"].ToString()) == 1)
                        {
                            array[0] = "#3AE62C ";
                            array[1] = item["id_orden"].ToString();
                        }
                        else if (Int32.Parse(item["estado"].ToString()) == 2)
                        {
                            array[0] = "#F7DB2F";
                            array[1] = item["id_orden"].ToString();
                        }
                        else
                        {
                            array[0] = "#E62C2C";
                            array[1] = item["id_orden"].ToString();
                        }

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
