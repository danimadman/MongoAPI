using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json.Serialization;
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
        
        [Required(ErrorMessage = "Имя клиента должно быть заполнено")]
        public string FirstName { get; set; }
        
        public string MiddleName { get; set; }
        
        [Required(ErrorMessage = "Фамилия клиента должна быть заполнена")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Дата рождения должна быть заполнена")]
        public DateTime? BirthDay { get; set; }
        
        [Required(ErrorMessage = "Паспорт должен быть заполнен")]
        public Passport Passport { get; set; }
        
        public string Comment { get; set; }
    }

    public class Passport
    {
        [Required(ErrorMessage = "Серия паспорта должна быть заполнена")]
        public string Series { get; set; }

        [Required(ErrorMessage = "Номер пасопрта должен быть заполнен")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Организация, которая выдала паспорт, должна быть заполнена")]
        public string Issued { get; set; }

        [Required(ErrorMessage = "Дата выдачи паспорта должен быть заполнен")]
        public DateTime? DateIssued { get; set; }

        [Required(ErrorMessage = "Код подразделения должен быть заполнен")]
        public string DivisionCode { get; set; }
    }
}