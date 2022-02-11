using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoAPI.Models;
using MongoAPI.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoAPI.Services
{
    public class PersonInRoomService
    {
        private readonly PersonInRoomDbSettings _personInRoomDbSettings;
        private readonly MongoClient _mongoClient;
        private IMongoCollection<PersonInRoom> _collection;
        
        public PersonInRoomService(IOptions<PersonInRoomDbSettings> personInRoomDbSettings)
        {
            _personInRoomDbSettings = personInRoomDbSettings.Value;
            _mongoClient = new MongoClient(_personInRoomDbSettings.ConnectionString);
            
            IMongoDatabase db = _mongoClient.GetDatabase(_personInRoomDbSettings.DatabaseName);
            _collection = db.GetCollection<PersonInRoom>(_personInRoomDbSettings.CollectionName);
        }

        public async Task<List<PersonInRoom>> GetAsync() => await _collection.Find(_ => true).ToListAsync();
        
        public async Task<PersonInRoom> GetAsync(string id) => 
            await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        
        public async Task CreateAsync(PersonInRoom personInRoom) => await _collection.InsertOneAsync(personInRoom);

        public async Task UpdateAsync(string id, PersonInRoom personInRoom) =>
            await _collection.ReplaceOneAsync(x => x.Id == id, personInRoom);

        public async Task RemoveAsync(string id) => await _collection.DeleteOneAsync(x => x.Id == id);
    }
}