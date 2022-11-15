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
        public static async void Mesas(bool band)
        {
            Model.Mesas mesa;
            var client = new HttpClient();
            
            if (band == true)
            {
                List<Model.Mesas> datos = await App.Database.GetMesaAsync();
                client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/empleados/index.php?op=obtenerMesas");
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
                            mesa = new Model.Mesas();
                            int id = Int32.Parse(item["id"].ToString());
                            string nombre = item["mesa"].ToString();
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
                            await App.Database.SaveMesaAsync(mesa);
                        }
                    }



                }
            }
                    
            
        }


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
                    if (Int32.Parse(item["cont"].ToString()) == 1)
                    {
                        if (Int32.Parse(item["estado"].ToString()) == 0)
                        {
                            array[0] = "#2C67E6 ";
                            array[1] = item["id_orden"].ToString();
                        }
                        else
                        {
                            array[0] = "#E62C2C";
                            array[1] = item["id_orden"].ToString();
                        }

                    }
                }

            }
            return array;
        }
    }
       
}
