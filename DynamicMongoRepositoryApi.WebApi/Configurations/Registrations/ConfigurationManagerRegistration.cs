namespace DynamicMongoRepositoryApi.WebApi.Configurations.Registrations
{
    public static class ConfigurationManagerRegistration
    {
        public static void RegisterAppsettings(this ConfigurationManager configurationManager)
        {
            configurationManager.AddJsonFile("appsettings.json", optional: false, true)
                                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, true)
                                .AddEnvironmentVariables()
                                .Build();
        }
    }
}
