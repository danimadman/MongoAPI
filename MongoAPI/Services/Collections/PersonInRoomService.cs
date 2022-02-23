using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoAPI.Models;
using MongoAPI.Models.Dto;
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
        private HotelService _hotelService;
        private PersonService _personService;
        
        public PersonInRoomService(IOptions<PersonInRoomDbSettings> personInRoomDbSettings, HotelService hotelService, PersonService personService)
        {
            _personInRoomDbSettings = personInRoomDbSettings.Value;
            _mongoClient = new MongoClient(_personInRoomDbSettings.ConnectionString);
            
            IMongoDatabase db = _mongoClient.GetDatabase(_personInRoomDbSettings.DatabaseName);
            _collection = db.GetCollection<PersonInRoom>(_personInRoomDbSettings.CollectionName);
            if (_collection == null)
            {
                db.CreateCollection(_personInRoomDbSettings.CollectionName);
                _collection = db.GetCollection<PersonInRoom>(_personInRoomDbSettings.CollectionName);
            }
            
            _hotelService = hotelService;
            _personService = personService;
        }

        public async Task<List<RecordsDto>> GetAsync()
        {
            var records = await _collection.Find(_ => true).ToListAsync();
            var query = from r in records
                join p in _personService.GetCollection().AsQueryable() on r.PersonId equals p.Id
                join hr in _hotelService.GetCollection().AsQueryable() on r.HotelRoomId equals hr.Id
                select new RecordsDto()
                {
                    Id = r.Id,
                    PersonId = p.Id,
                    PersonName = $"{p.LastName} {p.FirstName} {p.MiddleName}",
                    HotelRoomId = hr.Id,
                    HotelRoomName = hr.Number.ToString(),
                    SettlementDate = r.SettlementDate,
                    ReleaseDate = r.ReleaseDate,
                    Note = r.Note
                };

            return query.ToList();
        }

        public async Task<RecordsDto> GetAsync(string id)
        {
            var record = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (record == null)
                throw new Exception("Запись не найдена");

            var person = await _personService.GetAsync(record.PersonId);
            var hotelRoom = await _hotelService.GetAsync(record.HotelRoomId);

            if (person == null || hotelRoom == null)
                throw new Exception("Данная запись уже неактуальна");
            
            return new RecordsDto()
            {
                Id = record.Id,
                PersonId = person.Id,
                PersonName = $"{person.LastName} {person.FirstName} {person.MiddleName}",
                HotelRoomId = hotelRoom.Id,
                HotelRoomName = hotelRoom.Number.ToString(),
                SettlementDate = record.SettlementDate.HasValue 
                    ? record.SettlementDate.Value.ToLocalTime() : (DateTime?)null,
                ReleaseDate = record.ReleaseDate.HasValue
                    ? record.ReleaseDate.Value.ToLocalTime() : (DateTime?)null,
                Note = record.Note
            };
        }
        
        public async Task<RecordDetailsDto> GetDetailsAsync(string id)
        {
            var record = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (record == null)
                throw new Exception("Запись не найдена");

            var person = await _personService.GetAsync(record.PersonId);
            var hotelRoom = await _hotelService.GetAsync(record.HotelRoomId);

            if (person == null || hotelRoom == null)
                throw new Exception("Данная запись уже неактуальна");
            
            return new RecordDetailsDto()
            {
                Id = record.Id,
                Person = person,
                HotelRoom = hotelRoom,
                SettlementDate = record.SettlementDate,
                ReleaseDate = record.ReleaseDate,
                Note = record.Note
            };
        }

        public async Task CreateAsync(PersonInRoom personInRoom)
        {
            await AllowSave(personInRoom);
            await _collection.InsertOneAsync(personInRoom);
        }

        public async Task UpdateAsync(PersonInRoom personInRoom)
        {
            await AllowSave(personInRoom);
            await _collection.ReplaceOneAsync(x => x.Id == personInRoom.Id, personInRoom);
        }

        public async Task RemoveAsync(string id) => await _collection.DeleteOneAsync(x => x.Id == id);

        private async Task AllowSave(PersonInRoom personInRoom)
        {
            // проверим на наличие свободного места
            var hotelRoom = await _hotelService.GetAsync(personInRoom.HotelRoomId);

            if (hotelRoom == null)
                throw new Exception("Выбранный номер отеля не найден");
            
            // найдем количество забронированных мест, которые пересекаются по времени с текущим клиентом 
            var personsInRoom = await _collection.Find(x => 
                    x.HotelRoomId == personInRoom.HotelRoomId
                    && (x.SettlementDate >= personInRoom.SettlementDate && x.SettlementDate <= personInRoom.ReleaseDate 
                         || x.ReleaseDate >= personInRoom.SettlementDate && x.ReleaseDate <= personInRoom.ReleaseDate)).ToListAsync();

            if (string.IsNullOrEmpty(personInRoom.Id) && personsInRoom.Any(x => x.PersonId == personInRoom.PersonId))
                throw new Exception("Данный клиент уже записан в этом номере отеля");
            
            if (personsInRoom.Count >= hotelRoom.Seats)
                throw new Exception($"Нет свободных мест в {hotelRoom.Number} номере отеля");
        }
    }
}