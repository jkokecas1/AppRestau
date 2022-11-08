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
            else {
                return null;
            }

               
            }
        }
    }
