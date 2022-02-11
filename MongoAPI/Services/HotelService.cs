using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoAPI.Models;
using MongoAPI.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoAPI.Services
{
    public class HotelService
    {
        private readonly HotelDatabaseSettings _hotelDatabaseSettings;
        private readonly MongoClient _mongoClient;
        private IMongoCollection<HotelRoom> _collection;
        
        public HotelService(IOptions<HotelDatabaseSettings> hotelDatabaseSettings)
        {
            _hotelDatabaseSettings = hotelDatabaseSettings.Value;
            _mongoClient = new MongoClient(_hotelDatabaseSettings.ConnectionString);
            
            IMongoDatabase db = _mongoClient.GetDatabase(_hotelDatabaseSettings.DatabaseName);
            _collection = db.GetCollection<HotelRoom>(_hotelDatabaseSettings.CollectionName);
        }

        public async Task<List<HotelRoom>> GetAsync() => await _collection.Find(_ => true).ToListAsync();
        
        public async Task<HotelRoom> GetAsync(string id) => 
            await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        
        public async Task CreateAsync(HotelRoom hotelNumber) => await _collection.InsertOneAsync(hotelNumber);

        public async Task UpdateAsync(string id, HotelRoom hotelNumber) =>
            await _collection.ReplaceOneAsync(x => x.Id == id, hotelNumber);

        public async Task RemoveAsync(string id) => await _collection.DeleteOneAsync(x => x.Id == id);
    }
}