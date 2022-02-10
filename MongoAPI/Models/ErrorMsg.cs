namespace MongoAPI.Models
{
    public class ErrorMsg
    {
        public ErrorMsg() {}
        
        public ErrorMsg(bool status, string message)
        {
            Status = status;
            Message = message;
        }
        
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}