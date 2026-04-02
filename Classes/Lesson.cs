using System;
using System.Collections.Generic;
using System.Linq;

namespace ASPNET.Classes
{
    public class Lesson
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public int OrderIndex { get; set; }
        // Relations
        public Guid? ClassId { get; set; }
        public Class Class { get; set; }
        public List<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
        public Guid? TagId { get; set; }
        public Tag Tag { get; set; }
        public Guid? NoteId { get; set; }
        public Note Note { get; set; }

        public double CompletionPercentage =>
            TodoItems == null || TodoItems.Count == 0
                ? 0.0
                : Math.Round((double)TodoItems.Count(t => t.IsCompleted) / TodoItems.Count * 100.0, 2);
    }
}
