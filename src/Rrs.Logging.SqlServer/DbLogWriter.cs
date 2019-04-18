using Rrs.Data;
using Rrs.Tasks;
using Rrs.Tasks.Pulsable;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Logging.SqlServer
{
    public class DbLogWriter : IDisposable
    {
        private readonly Guid _softwareId;
        private readonly IDbDelegator _db;
        private readonly ILogger _logger;
        private readonly string _logTable;

        private readonly PulseWorker _pulseWorker;
        private readonly LoggerQueries _queries;

        private readonly ConcurrentQueue<IPendingLog> _queue = new ConcurrentQueue<IPendingLog>();

        public DbLogWriter(Guid softwareId, IDbDelegator db, ILogger logger = null, string logTable = "Log", int maxLogEntries = 100_000)
        {
            _softwareId = softwareId;
            _db = db;
            _logTable = logTable;
            _logger = logger;
            _pulseWorker = new PulseWorker(t => FlushLog(t));
            _queries = new LoggerQueries(logTable, maxLogEntries);
            EnsureLogTableAvailable();
        }

        private Exception _lastException;

        private void FlushLog(CancellationToken token)
        {
            try
            {
                while (_queue.TryPeek(out var log))
                {
                    if (token.IsCancellationRequested) return;

                    _db.Execute(_queries.Create, log.CreateLogEntry(_softwareId));
                    _queue.TryDequeue(out var _);
                }
            }
            catch (Exception e)
            {
                if (e.ToString() != _lastException?.ToString())
                {
                    _queue.Enqueue(new PendingLog(RrsLogLevel.Error, e));
                    _logger?.Log(e);
                }
                _lastException = e;
                if (token.IsCancellationRequested) return;
                // prune excess
                while (_queue.Count > 1000 && _queue.TryDequeue(out var byeBye));
                Schedule.In(_pulseWorker.Pulse, TimeSpan.FromMinutes(1));
            }
        }

        private void EnsureLogTableAvailable()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    _db.Execute(_queries.EnsureLogTableExists);
                }
                catch(Exception e)
                {
                    _logger?.Log(e, $"Failed to create log table '{_logTable}'");
                }
            });
        }

        public void Log(IPendingLog log)
        {
            _queue.Enqueue(log);
            _pulseWorker.Pulse();
        }

        public void Dispose()
        {
            _pulseWorker.Dispose();
        }
    }
}
