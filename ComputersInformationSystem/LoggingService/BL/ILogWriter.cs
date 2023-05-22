using Entities;

namespace LoggingService.BL
{
    public interface ILogWriter
    {
        void Write(LogRecord record);
    }
}
