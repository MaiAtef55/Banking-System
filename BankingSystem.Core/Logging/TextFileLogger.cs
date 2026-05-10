using BankingSystem.Core.Interfaces;

namespace BankingSystem.Core.Logging;

public sealed class TextFileLogger : ILoggerService
{
    private readonly string _logPath;

    public TextFileLogger(string? logPath = null)
    {
        _logPath = logPath ?? Path.Combine(AppContext.BaseDirectory, "bank-log.txt");
    }

    public void Log(string activity, string details)
    {
        var lines = new[]
        {
            $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}]",
            activity,
            details,
            string.Empty
        };

        File.AppendAllLines(_logPath, lines);
    }
}
