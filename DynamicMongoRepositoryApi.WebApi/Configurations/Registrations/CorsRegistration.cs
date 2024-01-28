namespace DynamicMongoRepositoryApi.WebApi.Configurations.Registrations
{
    public static class CorsRegistration
    {
        public static void RegisterCorsService(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                        builder =>
                        {
                            builder.AllowAnyOrigin()
                                   .AllowAnyMethod()
                                   .AllowAnyHeader();
                        });
            });
        }

        public static void RegisterCorsApplication(this WebApplication app)
        {
            app.UseCors("AllowAll");
        }
    }
}
