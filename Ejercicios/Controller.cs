using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ejercicios
{
    public class Controller
    {

        public async Task LlamaApi(string url, string parametros)
        {
            string responseData = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();

                url += parametros;
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        responseData = await response.Content.ReadAsStringAsync();
                        var JSONModel = JsonConvert.DeserializeObject<Factura>(responseData);
                        decimal SUMATotalPartidas = 0;
                        Console.WriteLine("Ejercicio 1");
                        Console.WriteLine("IDS");
                        foreach (var partida in JSONModel.partidas)
                        {
                            Console.WriteLine("-->" + partida.id);
                            SUMATotalPartidas += partida.Precio == null ? 0 : Convert.ToDecimal(partida.Precio);
                        }

                        Console.WriteLine("\nEjercicio 2");
                        Console.WriteLine("TOTAL DE IMPORTE DE PARTIDAS: $" + Math.Round(SUMATotalPartidas, 2));

                        Console.WriteLine("\nEjercicio 3");
                        Console.WriteLine("Total factura: " + Math.Round(Convert.ToDecimal(JSONModel.total), 2));
                        Console.WriteLine("Total partidas: " + Math.Round(SUMATotalPartidas, 2));
                        decimal diferencia = Convert.ToDecimal(JSONModel.total) - SUMATotalPartidas;
                        decimal limite = Convert.ToDecimal(0.10);
                        if (diferencia < limite)
                        {
                            Console.WriteLine("Discrepancia permitida");
                            Console.WriteLine("Realizando put");
                            
                            ModelPOST modelPOST = new ModelPOST();
                            modelPOST.userID = "oscarskapee@gmail.com";
                            modelPOST.companyID = "123456789";
                            modelPOST.portalID = "oaXh7EU0ExaygAvvZM3y";
                            modelPOST.facturaID = "L90107";
                            modelPOST.notification = "La factura fue adendada correctamente";

                            string JSONPost = JsonConvert.SerializeObject(modelPOST);
                            StringContent content = new StringContent(
                            JSONPost, Encoding.UTF8, "application/json");
                            await LlamaApiPut("https://us-central1-b2b-hub-82515.cloudfunctions.net/app/api/Ej1", content);
                        }
                        else
                        {
                            Console.WriteLine("Discrepancia no permitida");
                        }
                    }
                    else
                    {
                        Console.WriteLine("___________________________________");
                        Console.WriteLine("Response invalid");
                        Console.WriteLine("___________________________________");
                        Console.WriteLine(response.ToString());
                        Console.WriteLine("___________________________________");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("___________________________________");
                Console.WriteLine("Ha ocurrido un error en la peticion");
                Console.WriteLine("___________________________________");
                Console.WriteLine(e.ToString());
                Console.WriteLine("___________________________________");
            }
        }

        public async Task LlamaApiPut(string url, StringContent content)
        {
            string responseData = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();


                using (HttpResponseMessage response = await client.PutAsync(url, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("La factura fue adendada correctamente");
                    }
                    else
                    {
                        Console.WriteLine("___________________________________");
                        Console.WriteLine("Response invalid");
                        Console.WriteLine("___________________________________");
                        Console.WriteLine(response.ToString());
                        Console.WriteLine("___________________________________");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("___________________________________");
                Console.WriteLine("Ha ocurrido un error en la peticion");
                Console.WriteLine("___________________________________");
                Console.WriteLine(e.ToString());
                Console.WriteLine("___________________________________");
            }
        }

    }
}
