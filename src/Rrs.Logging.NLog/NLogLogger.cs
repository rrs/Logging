using NLog;
using System;

namespace Rrs.Logging.NLog
{
    public class NLogLogger : ILogger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ILogObjectSerializer _serializer;

        public NLogLogger(ILogObjectSerializer serializer)
        {
            _serializer = serializer;
        }

        public void Log(string message)
        {
            Logger.Info(message);
        }

        public void Log(Exception e, string message)
        {
            Logger.Error(e, message);
        }

        public void Log(Exception e)
        {
            Logger.Error(e);
        }

        public void Log(object o)
        {
            Logger.Info(_serializer.Serialize(o));
        }

        public void Log(object o, string message)
        {
            Logger.Info(message);
            Logger.Info(_serializer.Serialize(o));
        }
    }
}
