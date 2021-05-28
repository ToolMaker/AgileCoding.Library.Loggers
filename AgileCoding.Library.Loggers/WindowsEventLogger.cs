namespace AgileCoding.Library.Loggers
{
    using AgileCoding.Extentions.Exceptions;
    using AgileCoding.Extentions.Linq;
    using AgileCoding.Library.Enums.Logging;
    using AgileCoding.Library.Interfaces.Logging;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Security;

    public class WindowsEventLogger : ILogger
    {
        private EventLog eventLog;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This is a Machine generated object")]
        public WindowsEventLogger(EventLog eventLog, string source)
        {
            this.eventLog = eventLog;
            this.eventLog.Source = source;

            CheckIfSourceExistAndCreateIfNot(eventLog, source);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This is a Machine generated object")]
        public WindowsEventLogger(string logName, string source)
        {
            eventLog = EventLog
                .GetEventLogs()
                .Where(x => x.Log.Equals(logName))
                .Single<EventLog, InvalidOperationException, InvalidOperationException>(
                            noElementsMessage: $"There is no windows event log type called '{logName}'",
                            moreThanOneMatchMessage: $"Wow this is imposible there are more than one windows event log with the name '{logName}'");

            eventLog.Source = source;
            CheckIfSourceExistAndCreateIfNot(eventLog, source);
        }

        private void CheckIfSourceExistAndCreateIfNot(EventLog eventLog, string source)
        {
            if (!EventLog.SourceExists(source))
            {
                try
                {
                    EventLog.CreateEventSource(source, eventLog.LogDisplayName);
                }
                catch (SecurityException se)
                {
                    throw new InvalidOperationException($"I was unable to create the Eventlog source {source}. Please make sure you have elevated rigths. You can try running the application as administrator.", se);
                }
            }
        }

        public void Dispose()
        {
            this.eventLog.Dispose();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "State is the messge to log.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "Because the exception is system generated")]
        public bool WriteCore(LogTypeEnum eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {

            EventLogEntryType typeOfEvent = EventLogEntryType.Information;
            string message = string.Empty;
            string exceptionMessage = exception != null ? exception.ToStringAll() : string.Empty;

            switch (eventType)
            {
                case LogTypeEnum.Critical:
                    typeOfEvent = EventLogEntryType.Error;
                    message = string.Format("Critical Error: {0}{1}{1}Exception Details:{1}{2}", state.ToString(), Environment.NewLine, exceptionMessage);
                    break;
                case LogTypeEnum.Error:
                    typeOfEvent = EventLogEntryType.Error;
                    message = string.Format("{0}{1}{1}Exception Details:{1}{2}", state.ToString(), Environment.NewLine, exceptionMessage);
                    break;
                case LogTypeEnum.Warning:
                    typeOfEvent = EventLogEntryType.Warning;
                    message = exception == null
                        ? string.Format("Warning! : {0}", state.ToString())
                        : string.Format("Warning! : {0}{1}{1}Exception Details:{1}{2}", state.ToString(), Environment.NewLine, exceptionMessage);
                    break;
                case LogTypeEnum.Information:
                    typeOfEvent = EventLogEntryType.Information;
                    message = exception == null
                        ? string.Format("Informational : {0}", state.ToString())
                        : string.Format("Informational : {0}{1}{1}Exception Details:{1}{2}", state.ToString(), Environment.NewLine, exceptionMessage);
                    break;
                case LogTypeEnum.Verbose:
                    typeOfEvent = EventLogEntryType.Information;
                    message = exception == null
                        ? string.Format("Verbose Information : {0}", state.ToString())
                        : string.Format("Verbose Information : {0}{1}{1}Exception Details:{1}{2}", state.ToString(), Environment.NewLine, exceptionMessage);
                    break;
                default:
                    break;
            }
            // TODO : Event Log has a message restriction size of 32766 characters.
            try
            {
                eventLog.WriteEntry(message, typeOfEvent, eventId);
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("There Was a error that was too long", EventLogEntryType.Error, eventId);
            }

            return true;
        }

        private bool TempWorkAroundToOnlyAllowErrorsToBeLogged(LogTypeEnum eventType)
        {
            return (eventType == LogTypeEnum.Error || eventType == LogTypeEnum.Critical || eventType == LogTypeEnum.Warning);
        }
    }
}
