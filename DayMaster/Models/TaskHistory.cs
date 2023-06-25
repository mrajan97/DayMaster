namespace DayMaster.Models
{
    public class TaskHistory
    {
        public int Id { get; set; }
        public int taskId { get; set; }
        public string useId { get; set; }
        public DateTime ActionTime { get; set; }
        public string ActionName { get; set; }

    }
}
