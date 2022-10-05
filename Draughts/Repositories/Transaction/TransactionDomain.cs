using Draughts.Common.Utilities;
using Draughts.Repositories.Misc;
using SqlQueryBuilder.Options;
using System;

namespace Draughts.Repositories.Transaction;

public abstract class TransactionDomain : IEquatable<TransactionDomain> {
    public static TransactionDomain Auth => new AuthTransactionDomain();
    public static TransactionDomain User => new UserTransactionDomain();
    public static TransactionDomain Game => new GameTransactionDomain();

    public abstract string Key { get; }

    private TransactionDomain() { }

    public abstract ISqlTransactionFlavor BeginTransaction();

    public override bool Equals(object? obj) => obj is TransactionDomain tdObj && tdObj.Key.Equals(Key);
    public bool Equals(TransactionDomain? other) => other?.Key == Key;

    public override int GetHashCode() => Key.GetHashCode();

    public static bool operator ==(TransactionDomain? left, TransactionDomain? right) => ComparisonUtils.NullSafeEquals(left, right);
    public static bool operator !=(TransactionDomain? left, TransactionDomain? right) => ComparisonUtils.NullSafeNotEquals(left, right);

    public sealed class AuthTransactionDomain : TransactionDomain {
        public const string KEY = "Auth";
        public override string Key => KEY;

        public override ISqlTransactionFlavor BeginTransaction() => DbContext.Get.BeginAuthTransaction();
    }

    public sealed class UserTransactionDomain : TransactionDomain {
        public const string KEY = "User";
        public override string Key => KEY;

        public override ISqlTransactionFlavor BeginTransaction() => DbContext.Get.BeginUserTransaction();
    }

    public sealed class GameTransactionDomain : TransactionDomain {
        public const string KEY = "Game";
        public override string Key => KEY;

        public override ISqlTransactionFlavor BeginTransaction() => DbContext.Get.BeginGameTransaction();
    }
}
