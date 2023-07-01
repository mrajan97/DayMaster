namespace DayMaster.Models
{
    public class Task
    {
        public int taskId { get; set; }
        public string taskName { get; set; }
        public string taskDescription { get; set; }
        public string taskStatus { get; set; }
        public string priority { get; set; }
        public DateTime date { get; set; }
        public string? username { get; set; }
    }
}
