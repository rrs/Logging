using Hangfire.Logging;

namespace Rrs.Logging.Hangfire;

public class HangfireLoggingAdapter : ILog
{
    private readonly ILogger _logger;

    public HangfireLoggingAdapter(ILogger logger)
    {
        _logger = logger;
    }

    public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
    {
        if (messageFunc == null)
        {
            // Before calling a method with an actual message, LogLib first probes
            // whether the corresponding log level is enabled by passing a `null`
            // messageFunc instance.
            return logLevel > LogLevel.Info;
        }

        var message = messageFunc();
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

        return true;
    }
}

public class HangfireLogProvider : ILogProvider
{
    private readonly ILogger _logger;

    public HangfireLogProvider(ILogger logger)
    {
        _logger = logger;
    }

    public ILog GetLogger(string name)
    {
        return new HangfireLoggingAdapter(_logger);
    }
}