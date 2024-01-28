namespace DynamicMongoRepositoryApi.WebApi.Models.Settings
{
    public class DynamicMongoRepositoryApiSettings
    {
        public string MongoDbConnectionString { get; set; }
        public string MongoDbDatabaseName { get; set; }
        public string ApiSecretKey { get; set; }
    }
}
