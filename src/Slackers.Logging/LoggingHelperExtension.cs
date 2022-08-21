using Microsoft.Extensions.Logging;

namespace Slackers.Logging;

/// <summary>
/// Helper class that extends ILogger to log objects
/// </summary>
public static class LoggingHelperExtension
{
    /// <summary>
    /// Log Information with Event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger"><see cref="ILogger"/></param>
    /// <param name="eventContext">Event scheme</param>
    /// <param name="message">Event Message</param>
    public static void LogEvent<T>(this ILogger logger, string message, T eventContext)
    {
        logger.LogInformation(message + " {@Event}", eventContext);
    }

    /// <summary>
    /// Log Error with Event
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/></param>
    /// <param name="exception"><see cref="Exception"/></param>
    /// <param name="eventContext">Event scheme</param>
    public static void LogEventError<T>(this ILogger logger, Exception exception, T eventContext)
    {
        logger.LogError(exception, "Event {@Event}", eventContext);
    }
}