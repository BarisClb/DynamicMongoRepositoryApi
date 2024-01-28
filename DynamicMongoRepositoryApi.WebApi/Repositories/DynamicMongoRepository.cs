using DynamicMongoRepositoryApi.WebApi.Models.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace DynamicMongoRepositoryApi.WebApi.Repositories
{
    public class DynamicMongoRepository
    {
        private IMongoDatabase _database;
        private IMongoCollection<BsonDocument> _collection;
        private readonly string _connectionString;

        public DynamicMongoRepository(IOptions<DynamicMongoRepositoryApiSettings> _dynamicMongoRepositoryApiSettings)
        {
            _connectionString = _dynamicMongoRepositoryApiSettings.Value.MongoDbConnectionString ?? throw new ArgumentNullException("No ConnectionString provided.");
        }


        public async Task<List<object>> GetCollectionAsync(string databaseName, string collectionName)
        {
            await initializeDatabase(databaseName, collectionName);
            var result = (await _collection.FindAsync(new BsonDocument())).ToList();
            if (result is null || result.Count == 0)
                return new();
            List<object> parsedResult = new();
            result.ForEach(document => parsedResult.Add(convertBsonDocumentToSerializableObject(document)));
            return parsedResult;
        }

        public async Task<object?> GetByIdAsync(string databaseName, string collectionName, string id)
        {
            await initializeDatabase(databaseName, collectionName);
            var parsedId = inferIdType(id);
            var result = (await _collection.FindAsync(Builders<BsonDocument>.Filter.Eq("_id", parsedId))).FirstOrDefault();
            return result != null ? convertBsonDocumentToSerializableObject(result) : null;
        }

        public async Task<object> InsertAsync(string databaseName, string collectionName, string jsonString)
        {
            await initializeDatabase(databaseName, collectionName);
            var document = BsonDocument.Parse(jsonString);
            await _collection.InsertOneAsync(document);
            return convertBsonDocumentToSerializableObject(document);
        }

        public async Task<object?> UpdateByIdAsync(string databaseName, string collectionName, string id, string jsonString)
        {
            await initializeDatabase(databaseName, collectionName);

            var document = BsonDocument.Parse(jsonString);
            var parsedId = inferIdType(id);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", parsedId);
            var update = new BsonDocument("$set", document);

            var result = await _collection.UpdateOneAsync(filter, update);
            if (result.ModifiedCount > 0)
                return await GetByIdAsync(databaseName, collectionName, id);
            return null;
        }

        public async Task<object?> UpdateByFieldsAsync(string databaseName, string collectionName, string jsonString)
        {
            await initializeDatabase(databaseName, collectionName);

            var jsonObject = JObject.Parse(jsonString);
            var combinedFilterForUpdateByFields = generateFilters(JObject.Parse(jsonString));
            if (combinedFilterForUpdateByFields is null)
                return null;

            foreach (var property in jsonObject.Properties().ToList())
                if (property.Name.StartsWith("$eq") || property.Name.StartsWith("$gt") || property.Name.StartsWith("$lt"))
                    property.Remove();

            jsonString = jsonObject.ToString();
            var document = BsonDocument.Parse(jsonString);
            var update = new BsonDocument("$set", document);

            var result = await _collection.UpdateManyAsync(combinedFilterForUpdateByFields, update);
            if (result.ModifiedCount > 0)
            {
                JObject modifiedObject = new JObject();
                foreach (var property in jsonObject.Properties())
                    modifiedObject[$"$eq{property.Name}"] = property.Value;
                return await GetByFieldsAsync(databaseName, collectionName, modifiedObject.ToString());
            }
            return new();
        }

        public async Task<int> DeleteByIdAsync(string databaseName, string collectionName, string id)
        {
            await initializeDatabase(databaseName, collectionName);
            var parsedId = inferIdType(id);
            var response = await _collection.DeleteOneAsync(Builders<BsonDocument>.Filter.Eq("_id", parsedId));
            return response.IsAcknowledged ? (int)response.DeletedCount : 0;
        }

        public async Task<List<object>> GetByFieldsAsync(string databaseName, string collectionName, string jsonString)
        {
            await initializeDatabase(databaseName, collectionName);
            var combinedFilter = generateFilters(JObject.Parse(jsonString));
            if (combinedFilter is null)
                return null;
            var result = (await _collection.FindAsync(combinedFilter)).ToList();
            if (result is null || result.Count == 0)
                return new();
            List<object> parsedResult = new();
            result.ForEach(document => parsedResult.Add(convertBsonDocumentToSerializableObject(document)));
            return parsedResult;
        }

        public async Task<int> DeleteByFieldsAsync(string databaseName, string collectionName, string jsonString)
        {
            await initializeDatabase(databaseName, collectionName);
            var combinedFilter = generateFilters(JObject.Parse(jsonString));
            if (combinedFilter is null)
                return -1;
            var response = await _collection.DeleteManyAsync(combinedFilter);
            return response.IsAcknowledged ? (int)response.DeletedCount : 0;
        }


        private async Task initializeDatabase(string databaseName, string collectionName)
        {
            var client = new MongoClient(_connectionString);
            _database = client.GetDatabase(databaseName);
            _collection = _database.GetCollection<BsonDocument>(collectionName);
        }

        private FilterDefinition<BsonDocument> generateFilters(JObject jsonObject)
        {
            var filters = jsonObject.Properties().Select(property =>
            {
                var fieldName = property.Name;
                var fieldValue = inferType(property.Value);
                if (fieldName.Contains("$gt"))
                    return Builders<BsonDocument>.Filter.Gt(fieldName.Substring(3), fieldValue);
                else if (fieldName.Contains("$lt"))
                    return Builders<BsonDocument>.Filter.Lt(fieldName.Substring(3), fieldValue);
                else if (fieldName.Contains("$eq"))
                    return Builders<BsonDocument>.Filter.Eq(fieldName.Substring(3), fieldValue);
                return default;
            }).Where(filter => filter != null);
            if (filters.Count() > 0)
                return Builders<BsonDocument>.Filter.And(filters);
            return null;
        }

        private object inferType(JToken token)
        {
            if (token.Type == JTokenType.Integer)
                return token.Value<int>();
            else if (token.Type == JTokenType.Float)
                return token.Value<double>();
            return token.ToObject<string>();
        }

        private object inferIdType(string value)
        {
            if (Int32.TryParse(value, out int parsedInt))
                return parsedInt;
            else if (ObjectId.TryParse(value, out ObjectId parsedObjectId))
                return parsedObjectId;
            else if (Guid.TryParse(value, out Guid parsedGuid))
                return parsedGuid;
            return value;
        }

        private object convertBsonDocumentToSerializableObject(BsonDocument document)
        {
            var dictionary = new Dictionary<string, object>();

            foreach (var element in document.Elements)
            {
                var value = convertBsonValueToSerializableValue(element.Value);
                dictionary[element.Name] = value;
            }

            return dictionary;
        }

        private object convertBsonValueToSerializableValue(BsonValue bsonValue)
        {
            switch (bsonValue.BsonType)
            {
                case BsonType.Null:
                    return bsonValue.AsBsonNull;
                case BsonType.Decimal128:
                    return bsonValue.AsDecimal;
                case BsonType.Binary:
                    var binaryData = bsonValue.AsBsonBinaryData;
                    if (binaryData.SubType == BsonBinarySubType.UuidLegacy || binaryData.SubType == BsonBinarySubType.UuidStandard)
                        return binaryData.AsGuid;
                    return bsonValue.AsBsonBinaryData.ToString();
                case BsonType.ObjectId:
                    return bsonValue.AsObjectId.ToString();
                case BsonType.DateTime:
                    return bsonValue.AsDateTime.ToString();
                case BsonType.Boolean:
                    return bsonValue.AsBoolean;
                case BsonType.Array:
                    return bsonValue.AsBsonArray;
                case BsonType.Double:
                    return bsonValue.AsDouble;
                case BsonType.Int32:
                    return bsonValue.AsInt32;
                default:
                    return bsonValue.AsString;
            }
        }

        private bool hasFilters(FilterDefinition<BsonDocument> filter)
        {
            if (filter == null)
                return false;
            var renderedFilter = filter.Render(new BsonDocumentSerializer(), new BsonSerializerRegistry());
            return renderedFilter.ElementCount > 0 && !(renderedFilter.ElementCount == 1 && renderedFilter.Names.Contains("$and"));
        }
    }
}
