using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

using log4net;

namespace commercetools.Common
{
    /// <summary>
    /// A simple log module using log4net.
    /// </summary>
    public class Logger
    {
        private static bool _configurationAttempted = false;
        private static ILog _log;

        /// <summary>
        /// ILog
        /// </summary>
        private static ILog Log
        {
            get
            {
                if (!_configurationAttempted)
                {
                    _configurationAttempted = true;

                    try
                    {
                        var logRepository = log4net.LogManager.GetRepository(typeof(Logger).GetTypeInfo().Assembly);
                        if (!logRepository.Configured)
                        {
                            log4net.Config.XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
                            _log = LogManager.GetLogger(typeof(Logger));
                        }
                    }
                    catch
                    {
                        _log = null;
                    }
                }

                return _log;
            }
        }

        /// <summary>
        /// Logs information
        /// </summary>
        /// <param name="message">Message</param>
        public static void LogInfo(string message)
        {
            if (Log != null)
            {
                Log.Info(message);
            }
        }

        /// <summary>
        /// Logs a warning
        /// </summary>
        /// <param name="message">Message</param>
        public static void LogWarning(string message)
        {
            if (Log != null)
            {
                Log.Warn(message);
            }
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">Error message</param>
        public static void LogError(string message)
        {
            if (Log != null)
            {
                Log.Error(message);
            }
        }

        private static MethodInfo GetMethodInfoOf<T>(Expression<Func<T>> expression)
        {
            var body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
    }
}
