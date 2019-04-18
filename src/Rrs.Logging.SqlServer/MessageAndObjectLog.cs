using System;

namespace Rrs.Logging.SqlServer
{
    public class MessageAndObjectLog
    {
        public string Message { get; set; }
        public object Object { get; set; }
        public Type ObjectType { get; set; }

        public MessageAndObjectLog() { }

        public MessageAndObjectLog(string message, object @object)
        {
            Message = message;
            Object = @object;
            ObjectType = @object.GetType();
        }
    }
}
