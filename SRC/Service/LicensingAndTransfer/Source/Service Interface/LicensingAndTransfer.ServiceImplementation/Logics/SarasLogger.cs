using System;
using log4net;

namespace LicensingAndTransfer
{
    public class Logger : ISarasLog4NetLogger
    {
        public static log4net.ILog log;
        bool intialized;

        public Logger()
        {
            if (log == null)
                InitializeLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public Logger(Type t)
        {
            InitializeLogger(t);
        }

        // config
        public void LogDebug(string Message)
        {
            SetSessionID();
            if (this.intialized)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(Message);
                }
            }
        }

        public void LogDebug(string EventName, string Message)
        {
            SetSessionID();
            if (this.intialized)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(EventName + " -> " + Message);
                }
            }
        }

        //For Debug
        public void LogDebug(object Message)
        {
            SetSessionID();
            if (this.intialized)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(Message);
                }
            }
        }

        //For Debug
        public void LogDebug(object Message, System.Exception ex)
        {
            SetSessionID();
            if (this.intialized)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(Message, ex);
                }
            }
        }

        public void LogInfo(object Message)
        {
            SetSessionID();
            if (this.intialized)
            {
                if (log.IsInfoEnabled)
                {
                    log.Info(Message);
                }
            }
        }

        public void LogInfo(string Message)
        {
            SetSessionID();
            if (this.intialized)
            {
                if (log.IsInfoEnabled)
                {
                    log.Info(Message);
                }
            }
        }


        //For Info
        public void LogInfo(object Message, System.Exception ex)
        {
            SetSessionID();
            if (this.intialized)
            {
                if (log.IsInfoEnabled)
                {
                    log.Info(Message, ex);
                }
                //Else
                //throwEx()
            }
        }

        //For Warning
        public void LogWarn(object Message)
        {
            SetSessionID();
            if (this.intialized)
            {
                if (log.IsWarnEnabled)
                {
                    log.Warn(Message);
                }
            }
        }
        //For Warning
        public void LogWarn(object Message, System.Exception ex)
        {
            SetSessionID();
            if (this.intialized)
            {
                if (log.IsWarnEnabled)
                {
                    log.Warn(Message, ex);
                }
            }
        }
        //For Error
        public void LogError(object Message)
        {
            SetSessionID();
            if (this.intialized)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(Message);
                }
            }
        }
        //For Error
        public void LogError(object Message, System.Exception ex)
        {
            SetSessionID();
            if (this.intialized)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(Message, ex);
                }
            }
        }

        //For Fatal
        public void LogFatal(object Message)
        {
            SetSessionID();
            if (this.intialized)
            {
                if (log.IsFatalEnabled)
                {
                    log.Fatal(Message);
                }
            }
        }
        //For Fatal
        public void LogFatal(object Message, System.Exception ex)
        {

            SetSessionID();
            if (this.intialized)
            {
                if (log.IsFatalEnabled)
                {
                    log.Fatal(Message, ex);
                }
            }
        }

        private void SetSessionID()
        {
            try
            {
                log4net.LogicalThreadContext.Properties["SessionID"] = System.Diagnostics.Process.GetCurrentProcess().SessionId;
                log4net.LogicalThreadContext.Properties["UserID"] = System.Diagnostics.Process.GetCurrentProcess().Id;
            }
            catch (System.Exception) // need to see why exceptions are coming here. Found exception occuring for "HttpContext.Current.Session"
            {
            }
        }

        public void SetSessionID(string UserID)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["SessionID"] = System.Diagnostics.Process.GetCurrentProcess().SessionId;
                log4net.LogicalThreadContext.Properties["UserID"] = System.Diagnostics.Process.GetCurrentProcess().Id;
            }
            catch (System.Exception) // need to see why exceptions are coming here. Found exception occuring for "HttpContext.Current.Session"
            {
            }
        }

        public void ClearNDC()
        {
            log4net.NDC.Clear();
        }
        public void throwEx()
        {
        }

        public void InitializeLogger(Type Type)
        {
            try
            {
                log = LogManager.GetLogger(Type);
                intialized = true;
            }
            catch (Exception) // SarasLog4Net.SystemFrameworks.Exception.SarasLog4NetException
            {
                throw (new Exception("Log4Net Not Intialized"));
                //Exception.SarasLog4NetException("Log4Net Not Intialized")
            }
        }

        public void LogTrack(string PageName, string EventName)
        {

        }

        public void LogTrack(string Message, string PageName, string EventName)
        {

        }
    }

    public interface ISarasLog4NetLogger
    {
        void LogDebug(string Message);
        void LogDebug(object Message);
        void LogDebug(object Message, System.Exception ex);
        void LogDebug(string EventName, string Message);
        void LogTrack(string Message, string PageName, string EventName);
        void LogTrack(string PageName, string EventName);
        void LogInfo(string Message);
        void LogInfo(object Message);
        void LogInfo(object Message, System.Exception ex);
        void LogWarn(object Message);
        void LogWarn(object Message, System.Exception ex);
        void LogError(object Message);
        void LogError(object Message, System.Exception ex);
        void LogFatal(object Message);
        void LogFatal(object Message, System.Exception ex);
        void ClearNDC();
    }
}
