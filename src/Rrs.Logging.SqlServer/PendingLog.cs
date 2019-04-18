using Newtonsoft.Json;
using System;

namespace Rrs.Logging.SqlServer
{
    public class PendingLog : IPendingLog
    {
        public RrsLogLevel Level { get; }
        public string Message { get; }
        public object Object { get; }

        public PendingLog(RrsLogLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        public PendingLog(RrsLogLevel level, object @object)
        {
            Level = level;
            Object = @object;
        }

        public PendingLog(RrsLogLevel level, string message, object @object)
        {
            Level = level;
            Message = message;
            Object = @object;
        }

        public LogEntry CreateLogEntry(Guid softwareId)
        {
            if (Message != null && Object != null)
            {
                return new LogEntry
                {
                    SoftwareId = softwareId,
                    Level = Level,
                    Object = JsonConvert.SerializeObject(new MessageAndObjectLog(Message, Object)),
                    ObjectType = typeof(MessageAndObjectLog).FullName
                };
            }
            if (Message != null)
            {
                return new LogEntry
                {
                    SoftwareId = softwareId,
                    Level = Level,
                    Object = Message,
                    ObjectType = typeof(string).FullName
                };
            }
            if (Object != null)
            {
                return new LogEntry
                {
                    SoftwareId = softwareId,
                    Level = Level,
                    Object = Object == null ? null : JsonConvert.SerializeObject(Object),
                    ObjectType = Object?.GetType().FullName
                };
            }

            return new LogEntry
            {
                SoftwareId = softwareId,
                Level = Level,
                Object = "Empty Log Entry",
                ObjectType = typeof(string).FullName
            };
        }
    }
}
