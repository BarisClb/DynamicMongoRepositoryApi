using System.Text.Json;

namespace DynamicMongoRepositoryApi.WebApi.Models.Requests
{
    public class UpdateMongoEntityByFieldsRequest
    {
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public JsonElement RequestBody { get; set; }
    }
}
