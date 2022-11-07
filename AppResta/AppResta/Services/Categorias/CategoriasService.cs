using AppResta.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppResta.Services.Categorias
{
    internal class CategoriasService : ICategoiaRespository
    {

        public List<Model.Categorias> Categorias2()
        {
            Model.Categorias categoria;
            var userInfo = new List<Model.Categorias>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/categoria/index.php?op=obtenerCategorias");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString() ;
 ;
               var jsonArray = JArray.Parse(json.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray)
                {
                    categoria = new Model.Categorias();
                    int ids = Int32.Parse(item["id"].ToString());
                    string nombre = item["nombre"].ToString();
                    string estado = item["estatus"].ToString();
                   // Console.WriteLine(item["nombre"].ToString());
                    categoria.id = ids;
                    categoria.nombre = nombre;
                    categoria.estatus = estado;
                    userInfo.Add(categoria);
                }
                //userInfo.Add(categoria);
                return userInfo;

            }
            else
            {
                return null;
            }
        }/*
        public async Task<Model.Categorias> Categorias()
        {
            Model.Categorias categoria = new Model.Categorias();
            var userInfo = new List<Model.Categorias>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/categoria/index.php?op=obtenerCategorias");
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                return await Task.FromResult(userInfo.ToArray);

            }
            else
            {
                return null;
            }
            /* if (response.IsSuccessStatusCode)
             {
                 var content = response.Content.ReadAsStringAsync().Result;
                 string json = content.ToString();
                 var jsonObject = JObject.Parse(json);

                 var jsonArray = JArray.Parse(jsonObject.ToString());
                 foreach (var item in jsonArray) {
                     int ids = Int32.Parse(item[""].ToString()); 
                     string nombre = item[""].ToString();
                     string estado = item[""].ToString();

                     categoria.id = ids;
                     categoria.nombre = nombre;
                     categoria.estatus = estado;
                 }

                 return Task.FromResult(categoria);
             }
             else
             {
                 return null;
             }
        }*/
    }
}
