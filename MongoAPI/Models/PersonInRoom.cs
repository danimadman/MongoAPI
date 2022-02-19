using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoAPI.Models
{
    public class PersonInRoom
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [Required(ErrorMessage = "Клиент должен быть указан")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PersonId { get; set; }
        
        [Required(ErrorMessage = "Номер отеля должен быть указан")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string HotelRoomId { get; set; }
        
        [Required(ErrorMessage = "Поле 'Дата заселения в номер' должно быть заполнено")]
        public DateTime? SettlementDate { get; set; }

        [Required(ErrorMessage = "Поле 'Дата освобождения' должно быть заполнено")]
        public DateTime? ReleaseDate { get; set; }
        
        public string Note { get; set; }
    }
}