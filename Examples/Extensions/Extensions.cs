using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Extensions
{
    public class ODataMetadata
    {
        public Dictionary<string, string> Metadata = new Dictionary<string, string>();

        public string GetNextLink()
        {
            Metadata.TryGetValue("nextLink", out string value);
            var idx = value?.IndexOf("/odata/");
            return value?.Substring(idx.Value + 7);
        }

        public string GetRawNextLink()
        {
            Metadata.TryGetValue("nextLink", out string value);
            return value;
        }

        public string GetContent()
        {
            Metadata.TryGetValue("content", out string value);
            return value;
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Get the list of ODataMetadata
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static ODataMetadata GetODataMetadata(this HttpResponseMessage response)
        {
            var result = new ODataMetadata();

            var responseString = response.Content.ReadAsStringAsync().Result;

            while (responseString.Contains("\"@odata"))
            {
                var idx = responseString.IndexOf("\"@odata");
                responseString = responseString.Substring(idx + 8);

                var name = GetODataMentadataName(responseString, out string newResponseString);

                responseString = newResponseString;

                var value = GetODataMentadataValue(responseString, out newResponseString);

                responseString = newResponseString;

                result.Metadata.Add(name, value);
            }

            return result;
        }

        /// <summary>
        /// Get the message response content as the specified type T
        /// </summary>
        /// <typeparam name="T">Type used to deserialize the content of the response</typeparam>
        /// <param name="response">HttpResponseMessage from where the Content is taken</param>
        /// <returns>Content as the type specified</returns>
        public static async Task<T> GetContentAsAsync<T>(this HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Convert collections
            if (typeof(T).IsGenericType &&
                (
                    typeof(T).GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                    typeof(T).GetGenericTypeDefinition() == typeof(ICollection<>) ||
                    typeof(T).GetGenericTypeDefinition() == typeof(List<>)
                ) &&
                typeof(T).GenericTypeArguments.Any())
            {
                if (!responseString.Contains("\"@odata")) return JsonConvert.DeserializeObject<T>(responseString);

                // Get value from the content
                responseString = JObject.Parse(responseString)["value"].ToString();

                var collectionType = typeof(ICollection<>);
                var genericType = collectionType.MakeGenericType(typeof(T).GenericTypeArguments[0]);

                return (T)JsonConvert.DeserializeObject(responseString, genericType);
            }

            // Convert any type not collection
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public static async Task<string> GetContentRawContentAsStringAsync(this HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);                
            return JObject.Parse(responseString)["value"].ToString();
        }

        public static string GetContentRawContentAsString(this HttpResponseMessage response)
        {
            return GetContentRawContentAsStringAsync(response).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sync method to get content deserialised as the specified generic type
        /// </summary>
        /// <typeparam name="T">Type of the expected result, as single or list of objects</typeparam>
        /// <param name="response">HttpResponseMessage response from where the results has to be extracted</param>
        /// <returns>Object or objects requested</returns>
        public static T GetContentAs<T>(this HttpResponseMessage response)
        {
            return GetContentAsAsync<T>(response).GetAwaiter().GetResult();
        }

        private static string GetODataMentadataValue(string responseString, out string newResponseString)
        {
            var idx = responseString.IndexOf("\":\"");
            responseString = responseString.Substring(idx + 3);

            idx = responseString.IndexOf('"');

            var result = responseString.Substring(0, idx);
            newResponseString = responseString.Substring(idx + 1);

            return result;
        }

        private static string GetODataMentadataName(string responseString, out string newResponseString)
        {
            var idx = responseString.IndexOf('"');
            var result = responseString.Substring(0, idx);

            newResponseString = responseString.Substring(idx + 1);

            return result;
        }
    }
}
