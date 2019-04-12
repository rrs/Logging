using Rrs.Data;
using System;

namespace Rrs.Logging.SqlServer
{
    public class DbLogger : ILogger, IDisposable
    {
        private readonly DbLogWriter _logWriter;

        public DbLogger(DbLogWriter logWriter)
        {
            _logWriter = logWriter;
        }

        public DbLogger(Guid softwareId, IDbDelegator db, ILogger logger = null) : this(new DbLogWriter(softwareId, db, logger)) { }

        public void Dispose()
        {
            _logWriter.Dispose();
        }

        public void Log(string message)
        {
            _logWriter.Log(new PendingLog(RrsLogLevel.Info, message));
        }

        public void Log(Exception e, string message)
        {
            _logWriter.Log(new PendingLog(RrsLogLevel.Error, message, e));
        }

        public void Log(Exception e)
        {
            _logWriter.Log(new PendingLog(RrsLogLevel.Error, e));
        }

        public void Log(object o)
        {
            _logWriter.Log(new PendingLog(RrsLogLevel.Info, o));
        }

        public void Log(object o, string message)
        {
            _logWriter.Log(new PendingLog(RrsLogLevel.Info, message, o));
        }
    }
}
