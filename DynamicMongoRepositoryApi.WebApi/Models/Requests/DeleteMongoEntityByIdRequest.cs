namespace DynamicMongoRepositoryApi.WebApi.Models.Requests
{
    public class DeleteMongoEntityByIdRequest
    {
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public string Id { get; set; }
    }
}
