using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoAPI.Models;
using MongoAPI.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoAPI.Services
{
    public class PersonService
    {
        private readonly PersonDatabaseSettings _personDatabaseSettings;
        private readonly MongoClient _mongoClient;
        private IMongoCollection<Person> _collection;
        
        public PersonService(IOptions<PersonDatabaseSettings> personDatabaseSettings)
        {
            _personDatabaseSettings = personDatabaseSettings.Value;
            _mongoClient = new MongoClient(_personDatabaseSettings.ConnectionString);
            
            IMongoDatabase db = _mongoClient.GetDatabase(_personDatabaseSettings.DatabaseName);
            _collection = db.GetCollection<Person>(_personDatabaseSettings.CollectionName);
        }

        public async Task<List<Person>> GetAsync() => await _collection.Find(_ => true).ToListAsync();
        
        public async Task<Person> GetAsync(string id) => 
            await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        
        public async Task CreateAsync(Person person) => await _collection.InsertOneAsync(person);

        public async Task UpdateAsync(string id, Person person) =>
            await _collection.ReplaceOneAsync(x => x.Id == id, person);

        public async Task RemoveAsync(string id) => await _collection.DeleteOneAsync(x => x.Id == id);
    }
}