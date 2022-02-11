using System.ComponentModel.DataAnnotations;
using MongoAPI.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoAPI.Models
{
    public class HotelRoom
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [Required(ErrorMessage = "Укажите номер отеля")]
        public int? Number { get; set; }
        
        [Required(ErrorMessage = "Укажите количество мест в номере отеля")]
        [Range(0, 999)]
        public int? Seats { get; set; }
        
        [Required(ErrorMessage = "Укажите уровень комфорта")]
        public int ComformLevel { get; set; }
        
        [Required(ErrorMessage = "")]
        [Range(0, 999999)]
        public decimal Cost { get; set; }
    }
}