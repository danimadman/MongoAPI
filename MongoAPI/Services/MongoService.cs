using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoAPI.Models;
using MongoAPI.Models.Dto;
using MongoAPI.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

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
        /// Возвращение списка доступных баз данных
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<DdlDto>> GetDataBases(string connString)
        {
            var client = new MongoClient(connString);
            
            if (client == null)
                throw new Exception("Сессия с MongoDB отсутствует");
            
            var res = new List<DdlDto>();
            
            using (var cursor = await client.ListDatabasesAsync())
            {
                var databaseDocuments = await cursor.ToListAsync();
                foreach (var databaseDocument in databaseDocuments)
                    if (databaseDocument["name"] != null)
                        res.Add(new DdlDto() 
                        {
                            Name = databaseDocument["name"].ToString()
                        });
            }
            
            return res;
        }

        /// <summary>
        /// Возвращение списка коллекции указанной базы данных
        /// </summary>
        /// <param name="dbName">имя базы данных</param>
        /// <returns></returns>
        public async Task<List<DdlDto>> GetCollections(string connString, string dbName)
        {
            var client = new MongoClient(connString);
            
            if (client == null)
                throw new Exception("Сессия с MongoDB отсутствует");
            
            if (string.IsNullOrEmpty(dbName) || string.IsNullOrWhiteSpace(dbName))
                throw new Exception("Не указано название базы данных");
            
            IMongoDatabase database = client.GetDatabase(dbName);

            if (database == null)
                throw new Exception("База данных не найдена");
            
            var collectionsCursor = await database.ListCollectionsAsync();
            List<BsonDocument> collections = await collectionsCursor.ToListAsync();

            if (collections == null)
                return null;
            
            List<DdlDto> res = new List<DdlDto>();
            foreach (var collection in collections)
                res.Add(new DdlDto() 
                {
                    Name = collection["name"].ToString()
                });
            
            return res;
        }
        
        public void CreateDb(string connString, string dbName)
        {
            var client = new MongoClient(connString);
            
            if (client == null)
                throw new Exception("Сессия с MongoDB отсутствует");
            
            if (string.IsNullOrEmpty(dbName) || string.IsNullOrWhiteSpace(dbName))
                throw new Exception("Не указано название базы данных");
        
            client.GetDatabase(dbName);
        }
        
        public void CreateCollection(string connString, string dbName, string collectionName)
        {
            var client = new MongoClient(connString);
            
            if (client == null)
                throw new Exception("Сессия с MongoDB отсутствует");
            
            if (string.IsNullOrEmpty(dbName) || string.IsNullOrWhiteSpace(dbName))
                throw new Exception("Не указано название базы данных");
        
            var database = client.GetDatabase(dbName);
            
            if (database == null)
                throw new Exception("База данных не найдена");
            
            database.CreateCollection(collectionName);
        }

        public async Task<List<HotelRoom>> GetAsync(string connString, string dbName, string collectionName, int comfortLevel)
        {
            var client = new MongoClient(connString);
            
            if (client == null)
                throw new Exception("Сессия с MongoDB отсутствует");
            
            if (string.IsNullOrEmpty(dbName) || string.IsNullOrWhiteSpace(dbName))
                throw new Exception("Не указано название базы данных");
        
            var database = client.GetDatabase(dbName);
            
            if (database == null)
                throw new Exception("База данных не найдена");
            
            IMongoCollection<HotelRoom> collection = database.GetCollection<HotelRoom>(collectionName);
            var filter = Builders<HotelRoom>.Filter.Eq("Cost", comfortLevel);
            return await collection.Find(filter).ToListAsync();
        }
        
        public void CreateDoc(string connString, string dbName, string collectionName, HotelRoom room)
        {
            var client = new MongoClient(connString);
            
            if (client == null)
                throw new Exception("Сессия с MongoDB отсутствует");
            
            if (string.IsNullOrEmpty(dbName) || string.IsNullOrWhiteSpace(dbName))
                throw new Exception("Не указано название базы данных");
        
            var database = client.GetDatabase(dbName);
            
            if (database == null)
                throw new Exception("База данных не найдена");
            
            IMongoCollection<HotelRoom> collection = database.GetCollection<HotelRoom>(collectionName);
            collection.InsertOne(room);
        }
    }
}