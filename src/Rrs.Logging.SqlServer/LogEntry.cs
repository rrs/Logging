using System;

namespace Rrs.Logging.SqlServer
{
    public class LogEntry
    {
        public int Id { get; set; }
        public Guid SoftwareId { get; set; }
        public RrsLogLevel Level { get; set; }
        public string Object { get; set; }
        public string ObjectType { get; set; }
        public DateTime Created { get; set; }
    }
}
