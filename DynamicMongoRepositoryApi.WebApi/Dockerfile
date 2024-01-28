FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DynamicMongoRepositoryApi.WebApi/DynamicMongoRepositoryApi.WebApi.csproj", "DynamicMongoRepositoryApi.WebApi/"]
RUN dotnet restore "./DynamicMongoRepositoryApi.WebApi/./DynamicMongoRepositoryApi.WebApi.csproj"
COPY . .
WORKDIR "/src/DynamicMongoRepositoryApi.WebApi"
RUN dotnet build "./DynamicMongoRepositoryApi.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DynamicMongoRepositoryApi.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DynamicMongoRepositoryApi.WebApi.dll"]