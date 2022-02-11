using System;
using System.ComponentModel.DataAnnotations;
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
        
        [Required]
        public string FirstName { get; set; }
        
        public string MiddleName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [Required]
        public Passport Passport { get; set; }
        
        public string Comment { get; set; }
    }

    public class Passport
    {
        [Required]
        public string Series { get; set; }
        
        [Required]
        public string Number { get; set; }
        
        [Required]
        public string Issued { get; set; }
        
        [Required]
        public DateTime DateIssued { get; set; }
        
        [Required]
        public string DivisionCode { get; set; }
    }
}