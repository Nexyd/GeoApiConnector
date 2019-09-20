using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ArcGIS.Services
{
    public class GeoApiConnector<T>
    {
        private const string URL = "https://apiv1.geoapi.es/";
        private const string apiKey = "4f68c7dd570312e673a6eda9d340af016664b8d74f63a825981bf23f8e8008f9";

        public List<T> GetData(string requestedObject, 
            string requestType, bool isTest, string CPRO = "")
        {
            HttpClient client  = new HttpClient();
            client.BaseAddress = new Uri(URL);

            string urlParameters = requestedObject + 
                "?key=" + apiKey + "&type=" + requestType +
                "&sandbox=" + (isTest? "1" : "0");

            if (requestedObject.Equals("municipios"))
                urlParameters += "&CPRO=" + CPRO;

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            // Blocking call! Program will wait here until a response is received or a timeout occurs.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;

            List<T> jsonObject = new List<T>();
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                // Make sure to install asp-net web client:
                // Install-Package Microsoft.AspNet.WebApi.Client
                jsonObject = GetJsonAs<T>(response).Result;

            } else {
                Console.WriteLine("{0} ({1})", (int)response
                    .StatusCode, response.ReasonPhrase);
            }

            // Make any other calls using HttpClient here.

            // Dispose once all HttpClient calls are complete. 
            // This is not necessary if the containing object will be disposed of, 
            // for example in this case the HttpClient instance will be disposed 
            // automatically when the application terminates 
            // making the following call superfluous.
            client.Dispose();

            return jsonObject;
        }

        public async Task<List<T>> GetJsonAs<T>(HttpResponseMessage response) {
            string data  = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(data);
            JToken value;

            List<T> jsonObject = new List<T>();
            if (json.TryGetValue("data", out value))
                jsonObject = JsonConvert.DeserializeObject
                    <List<T>>(value.ToString());

            return jsonObject;
        }
    }
}