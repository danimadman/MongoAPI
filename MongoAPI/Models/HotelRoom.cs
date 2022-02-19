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
        
        [Required(ErrorMessage = "Поле 'Номер отеля' должно быть заполнено")]
        public int? Number { get; set; }
        
        [Required(ErrorMessage = "Поле 'Количество мест в номере отеля' должно быть заполнено")]
        public int? Seats { get; set; }
        
        [Required(ErrorMessage = "Поле 'Уровень комфорта' должно быть заполнено")]
        [Range(1,99, ErrorMessage = "Недопустимое значение для уровня комфорта")]
        public int? ComfortLevel { get; set; }
        
        [Required(ErrorMessage = "Поле 'Цена за номер отеля' должно быть заполнено")]
        [Range(1, 999999, ErrorMessage = "Недопустимое значение для цены номера отеля")]
        public decimal Cost { get; set; }
    }
}