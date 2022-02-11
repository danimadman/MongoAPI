using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoAPI.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoAPI.Services
{
    public class MongoService
    {
        private readonly Settings _settings;
        private readonly MongoClient _mongoClient;
        private IMongoDatabase _database;
        
        public MongoService(IOptions<Settings> settings)
        {
            _settings = settings.Value;
            _mongoClient = new MongoClient(_settings.ConnectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">имя базы данных</param>
        /// <exception cref="Exception"></exception>
        private void SetDatabase(string name)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new Exception("Не указано название базы данных");
            
            _database = _mongoClient.GetDatabase(name);

            if (_database == null)
                throw new Exception("База данных не найдена");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">имя коллекции</param>
        /// <exception cref="Exception"></exception>
        private IMongoCollection<T> GetCollection<T>(string name)
        {
            if (_database == null)
                throw new Exception("База данных не найдена");
            
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new Exception("Не указано название коллекции");

            return _database.GetCollection<T>(name);
        }
        
        /// <summary>
        /// Возвращение списка доступных баз данных
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<string>> GetDataBases()
        {
            if (_mongoClient == null)
                throw new Exception("Сессия с MongoDB отсутствует");
            
            var res = new List<string>();
            
            using (var cursor = await _mongoClient.ListDatabasesAsync())
            {
                var databaseDocuments = await cursor.ToListAsync();
                foreach (var databaseDocument in databaseDocuments)
                    if (databaseDocument["name"] != null)
                        res.Add(databaseDocument["name"].ToString());
            }
            
            return res;
        }

        /// <summary>
        /// Возвращение списка коллекции указанной базы данных
        /// </summary>
        /// <param name="dbName">имя базы данных</param>
        /// <returns></returns>
        public async Task<List<string>> GetCollections(string dbName)
        {
            SetDatabase(dbName);

            var collectionsCursor = await _database.ListCollectionsAsync();
            List<BsonDocument> collections = await collectionsCursor.ToListAsync();

            if (collections == null)
                return null;
            
            List<string> res = new List<string>();
            foreach (var collection in collections)
                res.Add(collection["name"].ToString());
            
            return res;
        }

        public async Task<List<T>> GetAsync<T>(string dbName, string collectionName)
        {
            SetDatabase(dbName);
            IMongoCollection<T> collection = GetCollection<T>(collectionName);

            return await collection.Find(_ => true).ToListAsync();;
        }
        
        public async Task<T> GetOneAsync<T>(string dbName, string collectionName, ObjectId id)
        {
            SetDatabase(dbName);
            IMongoCollection<T> collection = GetCollection<T>(collectionName);

            var res = await collection.Find(new BsonDocument("_id", id)).FirstOrDefaultAsync();
            return res;
        }

        public async Task CreateAsync<T>(string dbName, string collectionName, T data)
        {
            SetDatabase(dbName);
            IMongoCollection<T> collection = GetCollection<T>(collectionName);

            await collection.InsertOneAsync(data);
        }

        public async Task UpdateAsync<T>(string dbName, string collectionName, T data, ObjectId id)
        {
            SetDatabase(dbName);
            IMongoCollection<T> collection = GetCollection<T>(collectionName);

            await collection.ReplaceOneAsync(new BsonDocument("_id", id), data);
        }

        public async Task RemoveAsync<T>(string dbName, string collectionName, ObjectId id)
        {
            SetDatabase(dbName);
            IMongoCollection<T> collection = GetCollection<T>(collectionName);

            await collection.DeleteOneAsync(new BsonDocument("_id", id));
        }
    }
}