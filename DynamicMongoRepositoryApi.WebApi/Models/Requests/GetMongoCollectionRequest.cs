namespace DynamicMongoRepositoryApi.WebApi.Models.Requests
{
    public class GetMongoCollectionRequest
    {
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}
