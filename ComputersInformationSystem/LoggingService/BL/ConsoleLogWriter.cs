using Entities;

namespace LoggingService.BL
{
    public class ConsoleLogWriter : ILogWriter
    {
        public void Write(LogRecord record)
        {
            var originalForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{record.Sender}:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\t{record.Message}");
            Console.ForegroundColor = originalForegroundColor;
        }
    }
}
