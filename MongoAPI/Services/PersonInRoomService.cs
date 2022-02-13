using System;
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
        private HotelService _hotelService;
        
        public PersonInRoomService(IOptions<PersonInRoomDbSettings> personInRoomDbSettings, HotelService hotelService)
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
        }

        public async Task<List<PersonInRoom>> GetAsync() => await _collection.Find(_ => true).ToListAsync();
        
        public async Task<PersonInRoom> GetAsync(string id) => 
            await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

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
            var personsInRoomCount = await _collection.Find(x => 
                    x.HotelRoomId == personInRoom.HotelRoomId
                    && (string.IsNullOrEmpty(personInRoom.Id) || x.Id != personInRoom.Id)
                    && (x.SettlementDate >= personInRoom.SettlementDate && x.SettlementDate <= personInRoom.ReleaseDate 
                         || x.ReleaseDate >= personInRoom.SettlementDate && x.ReleaseDate <= personInRoom.ReleaseDate))
                .CountDocumentsAsync();

            if (personsInRoomCount >= hotelRoom.Seats)
                throw new Exception($"Нет свободных мест в {hotelRoom.Number} номере отеля");
        }
    }
}