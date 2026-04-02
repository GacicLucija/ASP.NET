using System;

namespace ASPNET.Classes
{
    public enum Priority
    {
        Low,
        Medium,
        High,
        Urgent
    }

    public static class PriorityExtensions
    {
        public static string ToColorHex(this Priority p)
        {
            return p switch
            {
                Priority.Urgent => "#FF0000",   // red
                Priority.High => "#FFA500",     // orange
                Priority.Medium => "#FFFF00",   // yellow
                Priority.Low => "#FFFFFF",      // white
                _ => "#FFFFFF",
            };
        }
    }
}
