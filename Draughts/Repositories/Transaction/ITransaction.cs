
using Draughts.Common.Events;
using System;
using System.Collections.Generic;

namespace Draughts.Repositories.Transaction;

public interface ITransaction : IDisposable {
    bool IsOpen { get; }
    bool Succeeded { get; }
    IReadOnlyList<DomainEvent> RaisedEvents { get; }

    event TransactionEventHandler? OnOpened;
    event TransactionEventHandler? OnClosed;

    void Commit();
    void Rollback();
    void Start();
}

public delegate void TransactionEventHandler(ITransaction transaction, TransactionEventArgs e);

public sealed class TransactionEventArgs : EventArgs {
    public TransactionDomain TransactionDomain { get; }
    public TransactionEventArgs(TransactionDomain domain) => TransactionDomain = domain;
}
