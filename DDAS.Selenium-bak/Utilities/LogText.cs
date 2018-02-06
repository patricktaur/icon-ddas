using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Repository;

namespace Utilities
{
    public class LogText : ILog, IDisposable
    {
        private StreamWriter _writer;
        private bool _showMessage;
        private string _logFile;
        public LogText(string logFile, bool showMessage = false)
        {
            _logFile = logFile;
            
            _showMessage = showMessage;
        }

        public void LogStart()
        {
            _writer = new StreamWriter(_logFile, true);  //true = append
        }

        public void WriteLog(string message)
        {
            _writer.WriteLine(message);
            if (_showMessage == true)
            {
                Console.WriteLine(message);
            }
        }

        public void WriteLog(string caption, string message)
        {
            WriteLog(caption+ ", " + message);
        }

        public void LogEnd()
        {
            _writer.Close();
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Close();
            }
            
        }

        public void WriteLog(Log log)
        {
            throw new NotImplementedException();
        }
    }
}
