using Microsoft.Extensions.Logging;
using LoggerLibrary;

namespace UserInteractionBot;

public static class LoggingMethods
{
    /// <summary>
    /// Logs message on specified level.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="message"></param>
    /// <param name="level"></param>
    public static void Log(string fileName, string message, LogLevel level)
    {
        string logFilePath = "../../../../../var/log.txt";
        using (StreamWriter logFileWriter = new StreamWriter(logFilePath, append: true))
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(options => //Adding console output
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                });
                builder.AddProvider(new CustomFileLoggerProvider(logFileWriter)); //Adding a custom log provider
                                                                                  //to write logs to text files
            });
            ILogger logger = loggerFactory.CreateLogger(fileName);
            using (logger.BeginScope("[scope is enabled]")) // Output text on the console
            {
                switch (level)
                {
                    case LogLevel.Information: logger.LogInformation("{time}: {message}", DateTime.Now, message);
                        break;
                    case LogLevel.Error: logger.LogError("{time}: {message}", DateTime.Now, message);
                        break;
                    case LogLevel.Critical: logger.LogCritical("{time}: {message}", DateTime.Now, message);
                        break;
                    case LogLevel.Warning: logger.LogWarning("{time}: {message}", DateTime.Now, message);
                        break;
                    case LogLevel.Debug: logger.LogDebug("{time}: {message}", DateTime.Now, message);
                        break;
                    case LogLevel.Trace: logger.LogTrace("{time}: {message}", DateTime.Now, message);
                        break;
                }
            }
        }
    }
}