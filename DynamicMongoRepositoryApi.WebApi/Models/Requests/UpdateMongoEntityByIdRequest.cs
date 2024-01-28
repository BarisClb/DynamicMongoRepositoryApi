using System.Text.Json;

namespace DynamicMongoRepositoryApi.WebApi.Models.Requests
{
    public class UpdateMongoEntityByIdRequest
    {
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public string Id { get; set; }
        public JsonElement RequestBody { get; set; }
    }
}
