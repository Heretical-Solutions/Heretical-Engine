using System;

namespace HereticalSolutions.Logging
{
    public class DefaultLogger
        : ISmartLogger,
          ILoggingConfigurable
    {
        #region ISmartLogger

        public bool Active
        {
            get => false;
        }
        
        public bool WillLog(Type logSource)
        {
            return false;
        }

        public bool ActiveAndWillLog(Type logSource)
        {
            return false;
        }

        public void Log(
            Type logSource,
            string value)
        {
            throw new NotImplementedException();
        }

        public void Log(
            Type logSource,
            string value,
            object[] arguments)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(
            Type logSource,
            string value)
        {
            throw new NotImplementedException();
        }
        
        public void LogWarning(
            Type logSource,
            string value,
            object[] arguments)
        {
            throw new NotImplementedException();
        }
        
        public void LogError(
            Type logSource,
            string value)
        {
            throw new NotImplementedException();
        }
        
        public void LogError(
            Type logSource,
            string value,
            object[] arguments)
        {
            throw new NotImplementedException();
        }

        public string PrepareLog(
            Type logSource,
            string value)
        {
            return value;
        }

        public string Format(
            string value,
            EFormatOptions options)
        {
            return value;
        }

        public void Exception(
            Type logSource,
            string value)
        {
            throw new Exception(
                PrepareLog(
                    logSource,
                    value));
        }
        
        #endregion

        #region ILoggingConfigurable

        public void Allow(Type logSource)
        {
            throw new NotImplementedException();
        }

        public void Deny(Type logSource)
        {
            throw new NotImplementedException();
        }

        public void ToggleActive(bool value)
        {
        }

        #endregion
    }
}