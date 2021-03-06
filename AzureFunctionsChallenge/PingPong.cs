using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace AzureFunctionsChallenge
{
    public static class PingPong
    {
        /// <summary>
        /// Azure Functions Challenge - Sort Array writer
        /// This function will respond to an HTTP request
        /// </summary>
        /// <param name="req">Request</param>
        /// <param name="log">Logger</param>
        /// <example>
        /// {
        ///     "ping": "1b189de1-5c682222-49ab-aa3b-5a8311111"
        /// }
        /// </example>
        /// <returns>Response with request key</returns>
        /// <see cref="https://functionschallenge.azure.com/functions"/>
        [FunctionName("PingPong")]
        public static async System.Threading.Tasks.Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // Parse query parameter
            //string name = req.GetQueryNameValuePairs()
            //    .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
            //    .Value;

            // Get request data
            string requestStr = await req.Content.ReadAsStringAsync();
            RequestData request;

            try
            {
                request = JsonConvert.DeserializeObject<RequestData>(requestStr);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            // Create reponse
            var myObj = new { pong = request.Ping };
            var jsonToReturn = JsonConvert.SerializeObject(myObj);

            // Return
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
            };
        }

        private class RequestData
        {
            public string Ping { get; set; }
        }
    }
}
