using DDAS.Models.Interfaces;
using System;

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
