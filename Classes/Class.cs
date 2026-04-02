using System;
using System.Collections.Generic;

namespace ASPNET.Classes
{
    public class Class
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public string ColorHex { get; set; }
        public bool IsUrgent { get; set; }
        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
        public Tag Tag { get; set; }

        public Lesson AddLesson(string title, string description = null, int? orderIndex = null)
        {
            var lesson = new Lesson
            {
                Title = title,
                Description = description,
                ClassId = this.Id,
                Class = this,
                OrderIndex = orderIndex ?? Lessons.Count
            };
            Lessons.Add(lesson);
            return lesson;
        }

        public void SetTag(string name, Priority? priority = null)
        {
            Tag = new Tag
            {
                Name = name,
                Priority = priority
            };
        }
    }
}
