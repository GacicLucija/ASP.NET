using System;

namespace ASPNET.Classes
{
    public class Reminder
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Message { get; set; }
        public DateTime RemindAt { get; set; }
        public bool IsSent { get; set; }
    }
}
