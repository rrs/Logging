using System;

namespace Rrs.Logging.SqlServer
{
    public interface IPendingLog
    {
        LogEntry CreateLogEntry(Guid softwareId);
    }
}
