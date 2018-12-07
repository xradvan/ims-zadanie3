// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Microsoft.AspNet.WebApi.Client

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CallRequestResponseService
{

    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            InvokeRequestResponseService().Wait();
        }

        static async Task InvokeRequestResponseService()
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable> () { 
                        { 
                            "input1", 
                            new StringTable() 
                            {
                                ColumnNames = new string[] {"Obtiaznost predmetu", "Pocet hodin ucenia"},
                                Values = new string[,] {  { "10", "5" }  }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>() {
}
                };
                const string apiKey = "Wx9Vm/6th8dEt2q4Jy606kRTHW+u5ANryJPkctFojwWcw0gpRkyk70nOIlMEot5OX38FMYpxBujWP5jYyhWAJg=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/b5e84eb6ad36427ba48ac79e73432169/services/79103c08007f42fdaea4d386633dace6/execute?api-version=2.0");
                
               
                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Result: {0}", result);

                    JToken token = JObject.Parse(result);
                    var k = token["Results"]["output1"]["value"]["Values"][0][0];
                    System.Console.WriteLine(k);
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);

                }
            }
        }
    }
}