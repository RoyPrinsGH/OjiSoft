namespace OjiSoft.IdentityProvider.Logging;

public class FileLogger : ILogger
{
    private string _logPath;

    private StreamWriter? _writer;

    public FileLogger(string categoryName)
    {
        _logPath = GlobalLogSettings.LogDirectory + "/" + categoryName.Replace('.', '-') + ".log";
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
            _writer = new(_logPath, append: true);
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

public static class GlobalLogSettings
{
    public static string LogDirectory { get; set; } = "logs";

    public static void SetLogDirectory(string logDirectory)
    {
        LogDirectory = logDirectory;
        Directory.CreateDirectory(LogDirectory);
    }
}