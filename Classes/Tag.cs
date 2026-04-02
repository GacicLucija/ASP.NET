using System;

namespace ASPNET.Classes
{
    public class Tag
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public Priority? Priority { get; set; }
    }
}
