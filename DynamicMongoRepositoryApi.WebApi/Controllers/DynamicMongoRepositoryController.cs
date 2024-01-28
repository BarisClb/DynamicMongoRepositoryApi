using DynamicMongoRepositoryApi.WebApi.Configurations.ActionFilters;
using DynamicMongoRepositoryApi.WebApi.Models.Requests;
using DynamicMongoRepositoryApi.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace DynamicMongoRepositoryApi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(DynamicMongoRepositoryApiSecretKeyHandler))]
    [ApiController]
    public class DynamicMongoRepositoryController : ControllerBase
    {
        private readonly DynamicMongoRepositoryService _dynamicMongoRepositoryService;

        public DynamicMongoRepositoryController(DynamicMongoRepositoryService dynamicMongoRepositoryService)
        {
            _dynamicMongoRepositoryService = dynamicMongoRepositoryService ?? throw new ArgumentNullException(nameof(dynamicMongoRepositoryService));
        }


        [HttpGet("/api/db/{databaseName}/{collectionName}/getAll")]
        public async Task<IActionResult> GetAllCollection([FromHeader(Name = "api-key")][Required] string apiKeyHeader, string databaseName, string collectionName)
        {
            return Ok(await _dynamicMongoRepositoryService.GetMongoCollection(new GetMongoCollectionRequest() { DatabaseName = databaseName, CollectionName = collectionName }));
        }

        [HttpGet("/api/db/{databaseName}/{collectionName}/getById/{id}")]
        public async Task<IActionResult> GetMongoId([FromHeader(Name = "api-key")][Required] string apiKeyHeader, string databaseName, string collectionName, string id)
        {
            return Ok(await _dynamicMongoRepositoryService.GetMongoEntityById(new GetMongoEntityByIdRequest() { DatabaseName = databaseName, CollectionName = collectionName, Id = id }));
        }

        [HttpPost("/api/db/{databaseName}/{collectionName}/getByFields")]
        public async Task<IActionResult> GetByFields([FromHeader(Name = "api-key")][Required] string apiKeyHeader, string databaseName, string collectionName, [FromBody] JsonElement requestBody)
        {
            return Ok(await _dynamicMongoRepositoryService.GetMongoEntitiesByFields(new GetMongoEntitiesByFieldsRequest() { DatabaseName = databaseName, CollectionName = collectionName, RequestBody = requestBody }));
        }

        [HttpPost("/api/db/{databaseName}/{collectionName}/create")]
        public async Task<IActionResult> CreateEntity([FromHeader(Name = "api-key")][Required] string apiKeyHeader, string databaseName, string collectionName, [FromBody] JsonElement requestBody)
        {
            return Ok(await _dynamicMongoRepositoryService.CreateMongoEntity(new CreateMongoEntityRequest() { DatabaseName = databaseName, CollectionName = collectionName, RequestBody = requestBody }));
        }

        [HttpPut("/api/db/{databaseName}/{collectionName}/updateById/{id}")]
        public async Task<IActionResult> UpdateEntityById([FromHeader(Name = "api-key")][Required] string apiKeyHeader, string databaseName, string collectionName, string id, [FromBody] JsonElement requestBody)
        {
            return Ok(await _dynamicMongoRepositoryService.UpdateMongoEntity(new UpdateMongoEntityByIdRequest() { DatabaseName = databaseName, CollectionName = collectionName, Id = id, RequestBody = requestBody }));
        }

        [HttpPut("/api/db/{databaseName}/{collectionName}/updateByFields")]
        public async Task<IActionResult> UpdateEntityByFields([FromHeader(Name = "api-key")][Required] string apiKeyHeader, string databaseName, string collectionName, [FromBody] JsonElement requestBody)
        {
            return Ok(await _dynamicMongoRepositoryService.UpdateMongoEntityByFields(new UpdateMongoEntityByFieldsRequest() { DatabaseName = databaseName, CollectionName = collectionName, RequestBody = requestBody }));
        }

        [HttpDelete("/api/db/{databaseName}/{collectionName}/deleteById/{id}")]
        public async Task<IActionResult> DeleteEntityById([FromHeader(Name = "api-key")][Required] string apiKeyHeader, string databaseName, string collectionName, string id)
        {
            return Ok(await _dynamicMongoRepositoryService.DeleteMongoEntityById(new DeleteMongoEntityByIdRequest() { DatabaseName = databaseName, CollectionName = collectionName, Id = id }));
        }

        [HttpDelete("/api/db/{databaseName}/{collectionName}/deleteByFields")]
        public async Task<IActionResult> DeleteEntityById([FromHeader(Name = "api-key")][Required] string apiKeyHeader, string databaseName, string collectionName, [FromBody] JsonElement requestBody)
        {
            return Ok(await _dynamicMongoRepositoryService.DeleteMongoEntitiesByFields(new DeleteMongoEntitiesByFieldsRequest() { DatabaseName = databaseName, CollectionName = collectionName, RequestBody = requestBody }));
        }
    }
}
