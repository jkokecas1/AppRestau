using AppResta.Model;
using Newtonsoft.Json;
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
        public async Task<UserInfo> Login(string pin)
        {
            var userInfo = new List<UserInfo>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/empleados/index.php?op=obtenerPIN&pin=" + pin);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                userInfo = JsonConvert.DeserializeObject<List<UserInfo>>(content);
                return await Task.FromResult(userInfo.FirstOrDefault());

            }
            else { 
                return null;
            }
        }
    }
}
