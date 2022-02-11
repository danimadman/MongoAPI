using System;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoAPI.Models
{
    public class Person
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Passport Passport { get; set; }
        public string Comment { get; set; }
    }

    public class Passport
    {
        public string Series { get; set; }
        public string Number { get; set; }
        public string Issued { get; set; }
        public DateTime DateIssued { get; set; }
        public string DivisionCode { get; set; }
    }
}