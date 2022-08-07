using System.Linq;
using Broadcast.Configuration;

namespace Broadcast.EventSourcing
{
    public class TaskType : Enumeration
    {
        public static readonly TaskType Simple = new TaskType(nameof(Simple));

        public static readonly TaskType Recurring = new TaskType(nameof(Recurring));

        public static readonly TaskType Scheduled = new TaskType(nameof(Scheduled));

        public static readonly TaskType[] All = new[]
        {
            Simple,
            Recurring,
            Scheduled
        };

        public TaskType(string name) : base(name)
        {
        }

        public static TaskType Parse(string value)
        {
            return All.FirstOrDefault(a => a == value);
        }
    }
}
