using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace AzureFunctionsChallenge
{
    public static class DecipherText
    {
        /// <summary>
        /// Azure Functions Challenge - Decipher Text
        /// </summary>
        /// <param name="req">Request</param>
        /// <param name="log">Logger</param>
        /// <example>
        /// Headers:
        /// Content-Type = application/json
        /// Body:
        /// {
        ///   "key":"2588afea-027f-4093-a0e0-79e25923d614",
        ///   "msg":"3919158618333218751586927582881532861062862782398627102415",
        ///   "cipher":{
        ///         "a":56,"b":20,"c":24,"d":85,"e":15,"f":92,"g":25,"h":19,"i":10,"j":66,"k":83,"l":75,"m":73,
		/// 				        "n":27,"o":82,"p":18,"q":71,"r":32,"s":62,"t":39,"u":33,"v":64,"w":88,"x":54,"y":95,"z":35," ":86
        ///   }
        /// } 
        /// </example>
        /// <returns>
        /// Request key and decoded message
        /// Example response:
        /// {
        ///   "key": "2588afea-027f-4093-a0e0-79e25923d614",
        ///   "result": "the purple flower is not nice"
        /// }
        /// </returns>
        /// <see cref="https://functionschallenge.azure.com/functions"/>
        [FunctionName("DecipherText")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger DecipherText");

            // Get request data
            dynamic data = await req.Content.ReadAsAsync<object>();
            string key = data?.key;
            string msg = data?.msg;

            // Create Cipher
            Dictionary<string, object> values =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(data?.cipher.ToString());
            Dictionary<string, string> cipher = new Dictionary<string, string>();
            foreach (var element in values)
            {
                cipher.Add(element.Value.ToString(), element.Key);
            }

            // Decode
            int i = 0;
            StringBuilder sb = new StringBuilder();
            while (i + 2 <= msg.Length)
            {
                sb.Append(cipher[msg.Substring(i, 2)]);
                i += 2;
            }

            // Return
            log.Info($"Key = \"{key}\", Message = \"{sb}\"");
            var myObj = new { key = key, result = sb.ToString() };
            var jsonToReturn = JsonConvert.SerializeObject(myObj);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
            };
        }
    }
}
