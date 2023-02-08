using Microsoft.Extensions.Logging;

namespace Rrs.Logging;

public class MsLoggerAdapter : ILogger
{
    private readonly Microsoft.Extensions.Logging.ILogger _logger;

    public MsLoggerAdapter(Microsoft.Extensions.Logging.ILogger logger)
    {
        _logger = logger;
    }

    public void Log(string message) => _logger.LogInformation("{Message}", message);
    public void Log(Exception e, string message) => _logger.LogError(e, "{Message}", message);
    public void Log(Exception e) => _logger.LogError(e, "An exception has occured at {Source}", e.Source);
    public void Log(object o) => _logger.LogInformation("{@Object}", o);
    public void Log(object o, string message) => _logger.LogInformation("{Message}: {@Object}", message, o);
}
