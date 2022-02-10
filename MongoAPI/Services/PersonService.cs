using Microsoft.Extensions.Options;
using MongoAPI.Models;
using MongoAPI.Options;
using MongoDB.Driver;

namespace MongoAPI.Services
{
    public class PersonService
    {
        private readonly Settings _settings;
        private readonly MongoClient _mongoClient;
        private IMongoCollection<Person> _collection;
        
        public PersonService(IOptions<Settings> settings)
        {
            _settings = settings.Value;
            _mongoClient = new MongoClient(_settings.ConnectionString);
            IMongoDatabase db = _mongoClient.GetDatabase(_settings.Database);
            _collection = db.GetCollection<Person>("person");
        }
    }
}