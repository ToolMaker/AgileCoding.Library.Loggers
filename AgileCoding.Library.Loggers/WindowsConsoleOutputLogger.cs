namespace AgileCoding.Library.Loggers.Console
{
    using AgileCoding.Extentions.Exceptions;
    using AgileCoding.Library.Enums.Logging;
    using AgileCoding.Library.Interfaces.Logging;
    using System;
    using System.IO;

    public class WindowsConsoleOutputLogger : ILogger
    {
        private StreamWriter standardOutput = null;

        private StreamWriter standardErrorOutput = null;

        public WindowsConsoleOutputLogger(StreamWriter standardOutput, StreamWriter standardErrorOutput)
        {
            this.standardErrorOutput = standardErrorOutput;
            this.standardOutput = standardOutput;
        }

        public void Dispose()
        {
            this.standardOutput.Close();
            this.standardErrorOutput.Close();
            this.standardOutput.Dispose();
            this.standardErrorOutput.Dispose();
        }

        public bool WriteCore(LogTypeEnum eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter = null)
        {
            string exceptionMessage = exception != null ? exception.ToStringAll() : string.Empty;
            string message = string.Empty;

            switch (eventType)
            {
                case LogTypeEnum.Critical:
                    message = string.Format("Critical Error: {0}{1}{1}Exception Details:{1}{2}", state.ToString(), Environment.NewLine, exceptionMessage);
                    break;
                case LogTypeEnum.Error:
                    message = string.Format("{0}{1}{1}Exception Details:{1}{2}", state.ToString(), Environment.NewLine, exceptionMessage);
                    break;
                case LogTypeEnum.Warning:
                    message = exception == null
                        ? string.Format("Warning! : {0}", state.ToString())
                        : string.Format("Warning! : {0}{1}{1}Exception Details:{1}{2}", state.ToString(), Environment.NewLine, exceptionMessage);
                    break;
                case LogTypeEnum.Information:
                    message = exception == null
                        ? string.Format("Informational : {0}", state.ToString())
                        : string.Format("Informational : {0}{1}{1}Exception Details:{1}{2}", state.ToString(), Environment.NewLine, exceptionMessage);
                    break;
                case LogTypeEnum.Verbose:
                    message = exception == null
                        ? string.Format("Verbose Information : {0}", state.ToString())
                        : string.Format("Verbose Information : {0}{1}{1}Exception Details:{1}{2}", state.ToString(), Environment.NewLine, exceptionMessage);
                    break;
                default:
                    break;
            }

            if (UseStandardErrorOutput(eventType))
            {
                this.standardErrorOutput.WriteLine(message);
            }
            else
            {
                this.standardOutput.WriteLine(message);
            }

            return true;
        }

        private bool UseStandardErrorOutput(LogTypeEnum eventType)
        {
            return (eventType == LogTypeEnum.Error || eventType == LogTypeEnum.Critical || eventType == LogTypeEnum.Warning);
        }
    }
}
