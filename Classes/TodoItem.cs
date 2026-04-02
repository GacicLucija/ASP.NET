using System;

namespace ASPNET.Classes
{
    public class TodoItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public Guid? TagId { get; set; }
        public Tag Tag { get; set; }
        public Guid? NoteId { get; set; }
        public Note Note { get; set; }
        public Guid? LessonId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }

        public void MarkComplete() => IsCompleted = true;
        public void MarkIncomplete() => IsCompleted = false;

        public override string ToString()
        {
            return $"[{(IsCompleted ? "✓" : " ")}] {Title}";
        }
    }
}
