namespace AgileCoding.Library.Loggers
{
    using AgileCoding.Library.Interfaces.Logging;
    using System.Diagnostics;
    using System.Linq;

    public static class WindowsEventLoggerExtentions
    {
        public static ILogger CreateWindowsEventLoggerInstance(this ILogger logger, string LogName, string sourceName)
        {
            var applicatonLog = EventLog.GetEventLogs()
                .Where(x => x.Log.Equals(LogName))
                .Single();

            return new WindowsEventLogger(applicatonLog, sourceName);
        }

        public static ILogger CreateWindowsEventLoggerInstance(this ILogger logger, EventLog eventLog, string sourceName)
        {
            return new WindowsEventLogger(eventLog, sourceName);
        }
    }
}
