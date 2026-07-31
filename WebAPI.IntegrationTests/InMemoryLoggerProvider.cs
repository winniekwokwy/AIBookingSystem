using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

public sealed class InMemoryLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, InMemoryLogger> _loggers = new();

    public IReadOnlyCollection<InMemoryLogEntry> Entries =>
        _loggers.Values.SelectMany(l => l.Entries).ToArray();

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new InMemoryLogger(name));

    public void Dispose() { }
}

public sealed class InMemoryLogger : ILogger
{
    private readonly string _category;
    private readonly ConcurrentQueue<InMemoryLogEntry> _entries = new();

    public InMemoryLogger(string category) => _category = category;

    public IReadOnlyCollection<InMemoryLogEntry> Entries => _entries.ToArray();

    IDisposable ILogger.BeginScope<TState>(TState state) => NullScope.Instance;
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        _entries.Enqueue(new InMemoryLogEntry(_category, logLevel, eventId, message, exception?.ToString()));
    }

    private sealed class NullScope : IDisposable { public static readonly NullScope Instance = new(); public void Dispose() { } }
}

public sealed record InMemoryLogEntry(
    string Category,
    LogLevel Level,
    EventId EventId,
    string Message,
    string? ExceptionText);