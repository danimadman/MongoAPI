using System;

namespace MongoAPI.Models.Dto
{
    public class PersonDto
    {
        public string Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string MiddleName { get; set; }
        
        public string LastName { get; set; }

        public string PersonFIO => $"{LastName} {FirstName} {MiddleName}";
        
        public DateTime? BirthDay { get; set; }
        
        public Passport Passport { get; set; }
        
        public string Comment { get; set; }
    }
}