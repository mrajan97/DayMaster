namespace DayMaster.Models
{
    public class Priority
    {
        public int Id { get; set; }
        public enum priorityType
        {
            Low = 0,
            Medium = 1,
            High = 2,
            Urgent=3
        }
    }
}
