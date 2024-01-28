using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace DynamicMongoRepositoryApi.WebApi.Helpers
{
    public static class ParseHelper
    {
        public static string CheckIfJsonRequestIsValidAndParseRequestBody(JsonElement jsonElement)
        {
            try
            {
                string jsonString = jsonElement.GetRawText();
                BsonDocument.Parse(jsonString);
                JObject.Parse(jsonString);
                return jsonString;
            }
            catch { return null; }
        }
    }
}
