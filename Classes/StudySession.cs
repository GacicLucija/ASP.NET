using System;

namespace ASPNET.Classes
{
    public class StudySession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public Guid? ClassId { get; set; }
        public string Notes { get; set; }
    }
}
