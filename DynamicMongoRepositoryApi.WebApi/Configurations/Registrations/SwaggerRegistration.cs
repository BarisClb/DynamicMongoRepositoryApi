using Microsoft.OpenApi.Models;

namespace DynamicMongoRepositoryApi.WebApi.Configurations.Registrations
{
    public static class SwaggerRegistration
    {
        public static void RegisterSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name?.Split('.')[0], Version = "v1" });
            });
        }

        public static void RegisterSwaggerUI(this WebApplication app, IWebHostEnvironment environment)
        {
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("v1/swagger.json", System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name?.Split('.')[0]);
            });
        }
    }
}
