
namespace DDAS.Models.Interfaces
{
    public interface ILog
    {
        void LogStart();
        void WriteLog(string caption, string message);
        void WriteLog( string message);
        void LogEnd();
    }
}
