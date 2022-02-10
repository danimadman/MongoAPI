using MongoAPI.Models.Enums;
using MongoDB.Bson;

namespace MongoAPI.Models
{
    public class HotelNumber
    {
        public ObjectId _id { get; set; }
        public int Number { get; set; }
        public int Seats { get; set; }
        public int ComformLevel { get; set; }
        public decimal Cost { get; set; }
    }
}