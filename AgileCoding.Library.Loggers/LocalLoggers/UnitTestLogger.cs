namespace AgileCoding.Library.Loggers
{
    using System;
    using AgileCoding.Library.Enums.Logging;
    using AgileCoding.Library.Interfaces.Logging;

    public class UnitTestLogger : ILogger
    {
        public void Dispose()
        {
        }

        public bool WriteCore(LogTypeEnum eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            // For unit testing the deployment server has minimal rights.
            // Logging should be tested as a seperate entity on its own and not used during other unit test
            // TODO: Once loging is confirmed what will be used then Unit tests needs to be setup
            return true;
        }
    }
}
