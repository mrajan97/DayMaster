namespace DayMaster.Models
{
    public class Notification
    {
        public int notificationId { get; set; }
        public string userID { get; set; }
        public DateTime notificationDate { get; set; }
        public bool IsRead { get; set; }

    }
}
