using Rrs.Data;
using System;

namespace Rrs.Logging.SqlServer
{
    public class DbLogger : ILogger, IDisposable
    {
        private readonly ILogObjectSerializer _serializer;
        private readonly DbLogWriter _logWriter;

        public DbLogger(ILogObjectSerializer serializer, DbLogWriter logWriter)
        {
            _serializer = serializer;
            _logWriter = logWriter;
        }

        public DbLogger(ILogObjectSerializer serializer, Guid softwareId, IDbDelegator db, ILogger logger = null) : this(serializer, new DbLogWriter(serializer, softwareId, db, logger)) { }

        public void Dispose()
        {
            _logWriter.Dispose();
        }

        public void Log(string message)
        {
            _logWriter.Log(new PendingLog(_serializer, RrsLogLevel.Info, message));
        }

        public void Log(Exception e, string message)
        {
            _logWriter.Log(new PendingLog(_serializer, RrsLogLevel.Error, message, e));
        }

        public void Log(Exception e)
        {
            _logWriter.Log(new PendingLog(_serializer, RrsLogLevel.Error, e));
        }

        public void Log(object o)
        {
            _logWriter.Log(new PendingLog(_serializer, RrsLogLevel.Info, o));
        }

        public void Log(object o, string message)
        {
            _logWriter.Log(new PendingLog(_serializer, RrsLogLevel.Info, message, o));
        }
    }
}
