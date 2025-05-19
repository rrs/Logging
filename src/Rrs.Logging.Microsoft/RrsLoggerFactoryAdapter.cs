using Microsoft.Extensions.Logging;

namespace Rrs.Logging;

internal class RrsLoggerFactoryAdapter : Microsoft.Extensions.Logging.ILoggerFactory
{
    private readonly ILogger _logger;

    public RrsLoggerFactoryAdapter(ILogger logger)
    {
        _logger = logger;
    }

    public void AddProvider(ILoggerProvider provider)
    {
    }

    public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
    {
        return new RrsLoggerAdapter(_logger);
    }

    public void Dispose()
    {
    }
}
