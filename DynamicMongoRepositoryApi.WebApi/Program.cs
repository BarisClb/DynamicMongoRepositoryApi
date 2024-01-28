using DynamicMongoRepositoryApi.WebApi.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.RegisterConfigurations();
builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();
app.RegisterApplications();
app.Run();
