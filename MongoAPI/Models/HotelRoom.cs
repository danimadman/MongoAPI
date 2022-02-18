using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoAPI.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoAPI.Models
{
    [Serializable]
    public class HotelRoom
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [Required(ErrorMessage = "Недопустимое значение для номера отеля")]
        public int? Number { get; set; }
        
        [Required(ErrorMessage = "Недопустимое значение для количества мест в номере отеля")]
        public int? Seats { get; set; }
        
        [Required(ErrorMessage = "Недопустимое значение для уровня комфорта")]
        [Range(1,99)]
        public int ComfortLevel { get; set; }
        
        [Required(ErrorMessage = "Недопустимое значение для цены номера отеля")]
        [Range(1, 999999)]
        public decimal Cost { get; set; }
    }
}