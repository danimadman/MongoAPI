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
        
        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PersonId { get; set; }
        
        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string HotelRoomId { get; set; }
        
        [Required]
        public DateTime SettlementDate { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }
        
        public string Note { get; set; }
    }
}