namespace DayMaster.Models
{
    public class Audit
    {
        public int Id { get; set; }
        public string userID { get; set; }
        public DateTime timeStamp { get; set;}
        public string action { get; set; }
    }
}
