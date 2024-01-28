using DynamicMongoRepositoryApi.WebApi.Configurations.ActionFilters;
using DynamicMongoRepositoryApi.WebApi.Configurations.Middlewares;
using DynamicMongoRepositoryApi.WebApi.Configurations.Registrations;
using DynamicMongoRepositoryApi.WebApi.Models.Settings;
using DynamicMongoRepositoryApi.WebApi.Repositories;
using DynamicMongoRepositoryApi.WebApi.Services;

namespace DynamicMongoRepositoryApi.WebApi.Configurations
{
    public static class ServiceRegistration
    {
        public static void RegisterConfigurations(this ConfigurationManager configurationManager)
        {
            configurationManager.RegisterAppsettings();
        }

        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddOptions();
            services.Configure<DynamicMongoRepositoryApiSettings>(configuration.GetSection("DynamicMongoRepositoryApiSettings"));
            services.AddScoped<DynamicMongoRepository>();
            services.AddScoped<DynamicMongoRepositoryService>();
            services.AddScoped<DynamicMongoRepositoryApiSecretKeyHandler>();
            services.RegisterSwaggerGen();
            services.RegisterCorsService();
        }

        public static void RegisterApplications(this WebApplication app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.RegisterSwaggerUI(app.Environment);
            app.RegisterCorsApplication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
