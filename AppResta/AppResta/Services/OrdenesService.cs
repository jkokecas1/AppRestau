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

        public static async void Ordene(bool band)
        {
            Model.Ordenes orden;
            List<Model.Ordenes> ordenList = new List<Model.Ordenes>();
            var client = new HttpClient();

            List<Model.Ordenes> datos = await App.Database.GetOrdenesAsync();
            if (band == true)
            {
                client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=obtenerOrden");
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    string json = content.ToString();
                    var jsonArray = JArray.Parse(json.ToString());
                    // Console.WriteLine(jsonArray);
                    if (jsonArray.Count != datos.Count && datos != null)
                    {
                        foreach (var item in jsonArray)
                        {
                            if (Int32.Parse(item["pago"].ToString()) != 0)
                            {
                                orden = new Model.Ordenes();

                                orden.id = Int32.Parse(item["id"].ToString());
                                orden.fecha_orden = item["fecha_orden"].ToString();
                                orden.fecha_cerada = item["fecha_cerada"].ToString();
                                orden.mesero = item["mesero"].ToString();
                                orden.mesa = item["mesa"].ToString();
                                orden.total = item["total"].ToString();
                                orden.pago = Int32.Parse(item["pago"].ToString());

                                await App.Database.SaveOrdenesAsync(orden);

                            }

                        }
                    }

                }
            }
                   
             
        }


      
    }
}
