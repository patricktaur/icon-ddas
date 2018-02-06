using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Repository;

namespace Utilities
{
    class LogConsole : ILog, IDisposable
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
