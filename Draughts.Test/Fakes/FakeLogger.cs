using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Draughts.Test.Fakes;

public sealed class FakeLogger<T> : ILogger<T> {
    // --- Implementation of the fake ---
    private readonly List<Data> _loggedData = new List<Data>();

    public IReadOnlyList<Data> LoggedData => _loggedData.AsReadOnly();

    public record Data(LogLevel LogLevel, string Message, Exception? Exception);

    // --- Implementation of the interface ---
    public IDisposable BeginScope<TState>(TState state) where TState : notnull {
        throw new NotImplementedException("The fake logger doesn't support scopes (yet?).");
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) {
        _loggedData.Add(new Data(logLevel, formatter.Invoke(state, exception), exception));
    }
}
