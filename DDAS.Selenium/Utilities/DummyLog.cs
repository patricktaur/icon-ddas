using DDAS.Models.Interfaces;
using System;
using DDAS.Models.Repository;

namespace Utilities
{
    public class DummyLog : ILog, IDisposable
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void LogEnd()
        {
            //throw new NotImplementedException();
        }

        public void LogStart()
        {
            //throw new NotImplementedException();
        }

        public void WriteLog(Log log)
        {
            throw new NotImplementedException();
        }

        public void WriteLog(string message)
        {
            //throw new NotImplementedException();
        }

        public void WriteLog(string caption, string message)
        {
            //throw new NotImplementedException();
        }
    }
}
