using System;
using System.Collections.Generic;
using System.Linq;

namespace Rrs.Logging
{
    public class LoggerAggregate : ILogger
    {
        private IEnumerable<ILogger> _loggers;

        public LoggerAggregate(IEnumerable<ILogger> loggers)
        {
            _loggers = loggers;
        }

        public void DetachLoggers()
        {
            _loggers = Enumerable.Empty<ILogger>();
        }

        public void Log(string message)
        {
            foreach (var l in _loggers) l.Log(message);
        }

        public void Log(Exception e, string message)
        {
            foreach (var l in _loggers) l.Log(e, message);
        }

        public void Log(Exception e)
        {
            foreach (var l in _loggers) l.Log(e);
        }

        public void Log(object o)
        {
            foreach (var l in _loggers) l.Log(o);
        }

        public void Log(object o, string message)
        {
            foreach (var l in _loggers) l.Log(o, message);
        }
    }
}
