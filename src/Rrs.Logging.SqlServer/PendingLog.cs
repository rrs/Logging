using System;

namespace Rrs.Logging.SqlServer
{
    public class PendingLog : IPendingLog
    {
        private readonly ILogObjectSerializer _serializer;
        public RrsLogLevel Level { get; }
        public string Message { get; }
        public object Object { get; }

        public PendingLog(ILogObjectSerializer serializer, RrsLogLevel level, string message)
        {
            _serializer = serializer;
            Level = level;
            Message = message;
        }

        public PendingLog(ILogObjectSerializer serializer, RrsLogLevel level, object @object)
        {
            _serializer = serializer;
            Level = level;
            Object = @object;
        }

        public PendingLog(ILogObjectSerializer serializer, RrsLogLevel level, string message, object @object)
        {
            _serializer = serializer;
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
                    Object = _serializer.Serialize(new MessageAndObjectLog(Message, Object)),
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
                    Object = Object == null ? null : _serializer.Serialize(Object),
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
