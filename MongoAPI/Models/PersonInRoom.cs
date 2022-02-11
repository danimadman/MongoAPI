using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoAPI.Models
{
    public class PersonInRoom
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string PersonId { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string HotelRoomId { get; set; }

        public DateTime SettlementDate { get; set; }

        public DateTime ReleaseDate { get; set; }
        
        public string Note { get; set; }
    }
}