using Microsoft.Extensions.Logging;

namespace Rrs.Logging;

public class RrsLoggerAdapter : Microsoft.Extensions.Logging.ILogger
{
    private readonly ILogger _logger;

    public RrsLoggerAdapter(ILogger logger)
    {
        _logger = logger;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return NullScope.Instance;
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        if (exception != null)
        {
            if (message != null)
            {
                _logger.Log(exception, message);
            }
            else
            {
                _logger.Log(exception);
            }
        }
        else
        {
            _logger.Log(message);
        }
    }

    // Dummy scope to fulfill interface contract
    private class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new NullScope();
        public void Dispose() { }
    }
}
