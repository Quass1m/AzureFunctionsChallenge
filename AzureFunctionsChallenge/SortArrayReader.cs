using System.Net;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace AzureFunctionsChallenge
{
    public static class SortArrayReader
    {
        /// <summary>
        /// Azure Functions Challenge - Sort Array reader
        /// This function will return sorted array of integers saved in Azure Table Storage
        /// Required application settings:
        /// - SortArrayConnection - connection to Azure Table Storage
        /// </summary>
        /// <param name="req">Request</param>
        /// <param name="outTable">Storage Table reference</param>
        /// <param name="log">Logger</param>
        /// <example>
        /// Headers:
        /// Content-Type = application/json
        /// Body:
        /// {
        ///     "key": "1b189de1-5c682222-49ab-aa3b-5a8311111"
        /// }
        /// </example>
        /// <returns>
        /// Key and sorted array of int values
        /// Example reponse:
        /// {
	    ///   "key": "1b189de1-5c682222-49ab-aa3b-5a8311111",
	    ///   "ArrayOfValues": [3,34,45,95,700]
        /// }
        /// </returns>
        /// <see cref="https://functionschallenge.azure.com/functions"/>
        [FunctionName("SortArrayReader")]
        public static async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, [Table("SortArray", Connection = "SortArrayConnection")]IQueryable<DataTable> outTable, TraceWriter log)
        {
            log.Info("C# HTTP trigger SortArrayReader");

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();
            string key = data?.key;

            // Get values
            var values = outTable.Where(x => x.PartitionKey == key).Select(x => x.Value).ToList().OrderBy(x => x);

            // Return
            log.Info($"Key = \"{key}\"");
            var myObj = new { key = key, ArrayOfValues = values };
            var jsonToReturn = JsonConvert.SerializeObject(myObj);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json") 
            };
        }
    }
}
