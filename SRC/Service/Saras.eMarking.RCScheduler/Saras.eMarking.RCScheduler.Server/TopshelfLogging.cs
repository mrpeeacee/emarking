using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;
using System.IO;
using log4net;
using log4net.Config;
using System.Diagnostics;
using Topshelf.Logging;
using Topshelf;
using System.Globalization;
using Topshelf.HostConfigurators;

namespace Saras.Scheduler
{
  
    public enum AccountType
    {
        NetworkService = 0,
        //To specify that the service uses the Network Service account
        User = 1,
        // To specify that the service uses the user account  
        LocalSystem = 2,
        // To specify that the service uses the Local System account
        LocalService = 3
        // To specify that the service uses the Local Service account
    }

}

namespace Topshelf
{
    using HostConfigurators;
    using Logging;

    /// <summary>
    ///   Extensions for configuring Logging for log4net
    /// </summary>
    public static class Log4NetConfigurationExtensions
    {
        /// <summary>
        ///   Specify that you want to use the Log4net logging engine.
        /// </summary>
        /// <param name="configurator"> </param>
        public static void UseLog4Net(this HostConfigurator configurator)
        {
            Log4NetLogWriterFactory.Use();
        }

        /// <summary>
        ///   Specify that you want to use the Log4net logging engine.
        /// </summary>
        /// <param name="configurator"> </param>
        /// <param name="configFileName"> The name of the log4net xml configuration file </param>
        public static void UseLog4Net(this HostConfigurator configurator, string configFileName)
        {
            Log4NetLogWriterFactory.Use(configFileName);
        }
    }
}

namespace Topshelf.Logging
{
    using System;
    using System.IO;
    using log4net;
    using log4net.Config;

    public class Log4NetLogWriterFactory :
        LogWriterFactory
    {
        public LogWriter Get(string name)
        {
            return new Log4NetLogWriter(LogManager.GetLogger(name));
        }

        public void Shutdown()
        {
            LogManager.Shutdown();
        }

        public static void Use()
        {
            HostLogger.UseLogger(new Log4NetLoggerConfigurator(null));
        }

        public static void Use(string file)
        {
            HostLogger.UseLogger(new Log4NetLoggerConfigurator(file));
        }

        [Serializable]
        public class Log4NetLoggerConfigurator :
            HostLoggerConfigurator
        {
            readonly string _file;

            public Log4NetLoggerConfigurator(string file)
            {
                _file = file;
            }

            public LogWriterFactory CreateLogWriterFactory()
            {
                if (!string.IsNullOrEmpty(_file))
                {
                    string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _file);
                    var configFile = new FileInfo(file);
                    if (configFile.Exists)
                    {
                        XmlConfigurator.Configure(configFile);
                    }
                }

                return new Log4NetLogWriterFactory();
            }
        }
    }
}

namespace Topshelf.Logging
{
    using System;
    using System.Globalization;
    using Topshelf.Logging;

    public class Log4NetLogWriter :
        LogWriter
    {
        readonly log4net.ILog _log;

        public Log4NetLogWriter(log4net.ILog log)
        {
            _log = log;
        }

        public void Debug(object message)
        {
            _log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        public void Debug(LogWriterOutputProvider messageProvider)
        {
            if (!IsDebugEnabled)
                return;

            _log.Debug(messageProvider());
        }

        public void DebugFormat(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.DebugFormat(provider, format, args);
        }

        public void Info(object message)
        {
            _log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            _log.Info(message, exception);
        }

        public void Info(LogWriterOutputProvider messageProvider)
        {
            if (!IsInfoEnabled)
                return;

            _log.Info(messageProvider());
        }

        public void InfoFormat(string format, params object[] args)
        {
            _log.InfoFormat(format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.InfoFormat(provider, format, args);
        }

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
        }

        public void Warn(LogWriterOutputProvider messageProvider)
        {
            if (!IsWarnEnabled)
                return;

            _log.Warn(messageProvider());
        }

        public void WarnFormat(string format, params object[] args)
        {
            _log.WarnFormat(format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.WarnFormat(provider, format, args);
        }

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            _log.Error(message, exception);
        }

        public void Error(LogWriterOutputProvider messageProvider)
        {
            if (!IsErrorEnabled)
                return;

            _log.Error(messageProvider());
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _log.ErrorFormat(format, args);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.ErrorFormat(provider, format, args);
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _log.Fatal(message, exception);
        }

        public void Fatal(LogWriterOutputProvider messageProvider)
        {
            if (!IsFatalEnabled)
                return;

            _log.Fatal(messageProvider());
        }

        public void FatalFormat(string format, params object[] args)
        {
            _log.FatalFormat(format, args);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.FatalFormat(provider, format, args);
        }

        public bool IsDebugEnabled
        {
            get { return _log.IsDebugEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return _log.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return _log.IsWarnEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return _log.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return _log.IsFatalEnabled; }
        }

        public void Log(LoggingLevel level, object obj)
        {
            if (level == LoggingLevel.Fatal)
                Fatal(obj);
            else if (level == LoggingLevel.Error)
                Error(obj);
            else if (level == LoggingLevel.Warn)
                Warn(obj);
            else if (level == LoggingLevel.Info)
                Info(obj);
            else if (level >= LoggingLevel.Debug)
                Debug(obj);
        }

        public void Log(LoggingLevel level, object obj, Exception exception)
        {
            if (level == LoggingLevel.Fatal)
                Fatal(obj, exception);
            else if (level == LoggingLevel.Error)
                Error(obj, exception);
            else if (level == LoggingLevel.Warn)
                Warn(obj, exception);
            else if (level == LoggingLevel.Info)
                Info(obj, exception);
            else if (level >= LoggingLevel.Debug)
                Debug(obj, exception);
        }

        public void Log(LoggingLevel level, LogWriterOutputProvider messageProvider)
        {
            if (level == LoggingLevel.Fatal)
                Fatal(messageProvider);
            else if (level == LoggingLevel.Error)
                Error(messageProvider);
            else if (level == LoggingLevel.Warn)
                Warn(messageProvider);
            else if (level == LoggingLevel.Info)
                Info(messageProvider);
            else if (level >= LoggingLevel.Debug)
                Debug(messageProvider);
        }

        public void LogFormat(LoggingLevel level, string format, params object[] args)
        {
            if (level == LoggingLevel.Fatal)
                FatalFormat(CultureInfo.InvariantCulture, format, args);
            else if (level == LoggingLevel.Error)
                ErrorFormat(CultureInfo.InvariantCulture, format, args);
            else if (level == LoggingLevel.Warn)
                WarnFormat(CultureInfo.InvariantCulture, format, args);
            else if (level == LoggingLevel.Info)
                InfoFormat(CultureInfo.InvariantCulture, format, args);
            else if (level >= LoggingLevel.Debug)
                DebugFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void LogFormat(LoggingLevel level, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (level == LoggingLevel.Fatal)
                FatalFormat(formatProvider, format, args);
            else if (level == LoggingLevel.Error)
                ErrorFormat(formatProvider, format, args);
            else if (level == LoggingLevel.Warn)
                WarnFormat(formatProvider, format, args);
            else if (level == LoggingLevel.Info)
                InfoFormat(formatProvider, format, args);
            else if (level >= LoggingLevel.Debug)
                DebugFormat(formatProvider, format, args);
        }
    }
}