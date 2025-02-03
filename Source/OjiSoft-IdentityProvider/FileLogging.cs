namespace OjiSoft.IdentityProvider.Logging;

public static class GlobalFileLogData
{
    public static string LogDirectory { get; } = "logs/oji-idp-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
}

public class FileLogger : ILogger
{
    private string _logName;

    private StreamWriter? _writer;

    public FileLogger(string categoryName)
    {
        Directory.CreateDirectory("logs");
        Directory.CreateDirectory(GlobalFileLogData.LogDirectory);
        _logName = categoryName.Replace('.', '-') + ".log";
    }

    public void Dispose()
    {
        Console.WriteLine("Disposing FileLogging instance");
        _writer?.Dispose();
        _writer = null;
    }

    public void EnsureLogWriter()
    {
        if (_writer == null)
        {
            _writer = new(GlobalFileLogData.LogDirectory + "/" + _logName, append: true);
            _writer.AutoFlush = true;
        }
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        EnsureLogWriter();
        _writer?.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + logLevel + ": " + formatter(state, exception));
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;
}

public class FileLoggingProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName);

    public void Dispose() {
        Console.WriteLine("Disposing FileLoggingProvider");
    }
}