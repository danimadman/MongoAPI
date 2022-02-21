using System;

namespace MongoAPI.Models.Dto
{
    public class RecordsDto
    {
        public string Id { get; set; }
        public string PersonId { get; set; }
        public string PersonName { get; set; }
        public string HotelRoomId { get; set; }
        public string HotelRoomName { get; set; }
        public DateTime? SettlementDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Note { get; set; }
    }
    
    public class RecordDetailsDto
    {
        public string Id { get; set; }
        public Person Person { get; set; }
        public HotelRoom HotelRoom { get; set; }
        public DateTime? SettlementDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Note { get; set; }
    }
}