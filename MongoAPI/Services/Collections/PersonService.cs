using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoAPI.Models;
using MongoAPI.Models.Dto;
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
            if (_collection == null)
            {
                db.CreateCollection(_personDatabaseSettings.CollectionName);
                _collection = db.GetCollection<Person>(_personDatabaseSettings.CollectionName);
            }
        }

        public IMongoCollection<Person> GetCollection() => _collection;

        public async Task<List<PersonDto>> GetAsync()
        {
            var res = await _collection.Find(_ => true).ToListAsync();
            return res.Select(x => new PersonDto()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                LastName = x.LastName,
                Passport = x.Passport,
                Comment = x.Comment,
                BirthDay = x.BirthDay.HasValue 
                    ? x.BirthDay.Value.ToLocalTime() : (DateTime?)null
            }).ToList();
        }

        public async Task<Person> GetAsync(string id)
        {
            var person = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            person.BirthDay = person.BirthDay.HasValue
                ? person.BirthDay.Value.ToLocalTime()
                : (DateTime?) null;
            return person;
        }
        
        public async Task CreateAsync(Person person) => await _collection.InsertOneAsync(person);

        public async Task UpdateAsync(Person person) =>
            await _collection.ReplaceOneAsync(x => x.Id == person.Id, person);

        public async Task RemoveAsync(string id) => await _collection.DeleteOneAsync(x => x.Id == id);
    }
}