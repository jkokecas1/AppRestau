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
        public List<UserInfo> Login(string pin)
        {
            Model.UserInfo usuario;
            
            var empelado = new List<UserInfo>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/empleados/index.php?op=obtenerPIN&pin=" + pin);
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
                    usuario = new Model.UserInfo();
                    string pinE = item["pin"].ToString();
                    string nombre = item["nombre"].ToString();
                    
                    // Console.WriteLine(item["nombre"].ToString());
                    usuario.PIN = pinE;
                    usuario.Nombre = nombre;
                    empelado.Add(usuario);
                }

                return empelado;
            }
            else
            {
                return null;
            }
        }
    }
}
