using DynamicMongoRepositoryApi.WebApi.Helpers;
using DynamicMongoRepositoryApi.WebApi.Models.Requests;
using DynamicMongoRepositoryApi.WebApi.Models.Responses;
using DynamicMongoRepositoryApi.WebApi.Repositories;
using MongoDB.Driver;
using System.Net;

namespace DynamicMongoRepositoryApi.WebApi.Services
{
    public class DynamicMongoRepositoryService
    {
        private readonly DynamicMongoRepository _mongoRepository;

        public DynamicMongoRepositoryService(DynamicMongoRepository mongoRepository)
        {
            _mongoRepository = mongoRepository ?? throw new ArgumentNullException(nameof(mongoRepository));
        }


        public async Task<ApiResponse<List<object>>> GetMongoCollection(GetMongoCollectionRequest request)
        {
            if (string.IsNullOrEmpty(request.DatabaseName) || string.IsNullOrEmpty(request.CollectionName))
                return ApiResponse<List<object>>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            var result = await _mongoRepository.GetCollectionAsync(request.DatabaseName, request.CollectionName);
            return ApiResponse<List<object>>.Success(result, (int)HttpStatusCode.OK);
        }

        public async Task<ApiResponse<object>> GetMongoEntityById(GetMongoEntityByIdRequest request)
        {
            if (string.IsNullOrEmpty(request.DatabaseName) || string.IsNullOrEmpty(request.CollectionName) || string.IsNullOrEmpty(request.Id))
                return ApiResponse<object>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            var result = await _mongoRepository.GetByIdAsync(request.DatabaseName, request.CollectionName, request.Id);
            if (result is null)
                return ApiResponse<object>.Success(new(), (int)HttpStatusCode.NotFound);
            return ApiResponse<object>.Success(result, (int)HttpStatusCode.OK);
        }

        public async Task<ApiResponse<List<object>>> GetMongoEntitiesByFields(GetMongoEntitiesByFieldsRequest request)
        {
            if (string.IsNullOrEmpty(request.DatabaseName) || string.IsNullOrEmpty(request.CollectionName))
                return ApiResponse<List<object>>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            var requestJson = ParseHelper.CheckIfJsonRequestIsValidAndParseRequestBody(request.RequestBody);
            if (requestJson == null)
                return ApiResponse<List<object>>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            var result = await _mongoRepository.GetByFieldsAsync(request.DatabaseName, request.CollectionName, requestJson);
            if (result is null)
                return ApiResponse<List<object>>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            return ApiResponse<List<object>>.Success(result, (int)HttpStatusCode.OK);
        }

        public async Task<ApiResponse<object>> CreateMongoEntity(CreateMongoEntityRequest request)
        {
            if (string.IsNullOrEmpty(request.DatabaseName) || string.IsNullOrEmpty(request.CollectionName))
                return ApiResponse<object>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            var requestJson = ParseHelper.CheckIfJsonRequestIsValidAndParseRequestBody(request.RequestBody);
            if (requestJson == null)
                return ApiResponse<object>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            try
            {
                var result = await _mongoRepository.InsertAsync(request.DatabaseName, request.CollectionName, requestJson);
                return ApiResponse<object>.Success(result, (int)HttpStatusCode.Created);
            }
            catch (MongoWriteException ex)
            {
                if (ex.Message.IndexOf("duplicatekey", StringComparison.CurrentCultureIgnoreCase) > -1)
                    return ApiResponse<object>.Fail($"A Document with the same Id already exists in '{request.DatabaseName}','{request.CollectionName}'.", (int)HttpStatusCode.BadRequest);
                throw;
            }
        }

        public async Task<ApiResponse<object>> UpdateMongoEntity(UpdateMongoEntityByIdRequest request)
        {
            if (string.IsNullOrEmpty(request.DatabaseName) || string.IsNullOrEmpty(request.CollectionName) || string.IsNullOrEmpty(request.Id))
                return ApiResponse<object>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            var requestJson = ParseHelper.CheckIfJsonRequestIsValidAndParseRequestBody(request.RequestBody);
            if (requestJson == null)
                return ApiResponse<object>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            var result = await _mongoRepository.UpdateByIdAsync(request.DatabaseName, request.CollectionName, request.Id, requestJson);
            if (result is null)
                return ApiResponse<object>.Success(new(), (int)HttpStatusCode.NotFound);
            return ApiResponse<object>.Success(result, (int)HttpStatusCode.OK);
        }

        public async Task<ApiResponse<object>> UpdateMongoEntityByFields(UpdateMongoEntityByFieldsRequest request)
        {
            if (string.IsNullOrEmpty(request.DatabaseName) || string.IsNullOrEmpty(request.CollectionName))
                return ApiResponse<object>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            var requestJson = ParseHelper.CheckIfJsonRequestIsValidAndParseRequestBody(request.RequestBody);
            if (requestJson == null)
                return ApiResponse<object>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            var result = await _mongoRepository.UpdateByFieldsAsync(request.DatabaseName, request.CollectionName, requestJson);
            if (result is null)
                return ApiResponse<object>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            return ApiResponse<object>.Success(result, (int)HttpStatusCode.OK);
        }

        public async Task<ApiResponse<object>> DeleteMongoEntityById(DeleteMongoEntityByIdRequest request)
        {
            if (string.IsNullOrEmpty(request.DatabaseName) || string.IsNullOrEmpty(request.CollectionName) || string.IsNullOrEmpty(request.Id))
                return ApiResponse<object>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            var result = await _mongoRepository.DeleteByIdAsync(request.DatabaseName, request.CollectionName, request.Id);
            return ApiResponse<object>.Success(result, (int)HttpStatusCode.OK);
        }

        public async Task<ApiResponse<object>> DeleteMongoEntitiesByFields(DeleteMongoEntitiesByFieldsRequest request)
        {
            if (string.IsNullOrEmpty(request.DatabaseName) || string.IsNullOrEmpty(request.CollectionName))
                return ApiResponse<object>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            var requestJson = ParseHelper.CheckIfJsonRequestIsValidAndParseRequestBody(request.RequestBody);
            if (requestJson == null)
                return ApiResponse<object>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            var result = await _mongoRepository.DeleteByFieldsAsync(request.DatabaseName, request.CollectionName, requestJson);
            if (result < 0)
                return ApiResponse<object>.Fail(GenericDataHelper.InvalidRequest, (int)HttpStatusCode.BadRequest);
            return ApiResponse<object>.Success(result, (int)HttpStatusCode.OK);
        }
    }
}
