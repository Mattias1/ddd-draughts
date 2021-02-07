using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Repositories.Database;
using Draughts.Repositories.InMemory;
using SqlQueryBuilder.Options;
using System;
using System.Collections.Generic;
using static Draughts.Repositories.Transaction.PairTableFunctions;

namespace Draughts.Repositories.Transaction {
    public abstract class TransactionDomain : IEquatable<TransactionDomain> {
        public static TransactionDomain AuthUser => new AuthUserTransactionDomain();
        public static TransactionDomain User => new UserTransactionDomain();
        public static TransactionDomain Game => new GameTransactionDomain();

        public abstract string Key { get; }
        public abstract List<DomainEvent> TempDomainEventsTable { get; }

        private TransactionDomain() { }

        public virtual void InMemoryFlush() {
            ApplyForAllTablePairs(new StoreIntoFunction());
            InMemoryClear();
        }

        public virtual void InMemoryRollback() => InMemoryClear();

        private void InMemoryClear() => ApplyForAllTablePairs(new ClearTempFunction());

        public void InMemoryStore<T>(T evt, List<T> tempTable) where T : IEquatable<T> {
            var func = new ContainsTempTableFunction(tempTable);
            ApplyForAllTablePairs(func);
            if (!func.Result) {
                throw new InvalidOperationException("Storing into this table is not valid for this transaction domain.");
            }
            InMemoryDatabaseUtils.StoreInto(evt, tempTable);
        }

        protected abstract void ApplyForAllTablePairs(IPairTableFunction func);

        public abstract ISqlTransactionFlavor BeginTransaction();

        public override bool Equals(object? obj) => obj is TransactionDomain tdObj && tdObj.Key.Equals(Key);
        public bool Equals(TransactionDomain? other) => other?.Key == Key;

        public override int GetHashCode() => Key.GetHashCode();

        public static bool operator ==(TransactionDomain? left, TransactionDomain? right) => ComparisonUtils.NullSafeEquals(left, right);
        public static bool operator !=(TransactionDomain? left, TransactionDomain? right) => ComparisonUtils.NullSafeNotEquals(left, right);

        public class AuthUserTransactionDomain : TransactionDomain {
            public const string KEY = "AuthUser";
            public override string Key => KEY;

            public override List<DomainEvent> TempDomainEventsTable => AuthUserDatabase.TempDomainEventsTable;

            protected override void ApplyForAllTablePairs(IPairTableFunction func) {
                func.Apply(AuthUserDatabase.TempRolesTable, AuthUserDatabase.RolesTable);
                func.Apply(AuthUserDatabase.TempAuthUsersTable, AuthUserDatabase.AuthUsersTable);
                func.Apply(AuthUserDatabase.TempDomainEventsTable, AuthUserDatabase.DomainEventsTable);
            }

            public override ISqlTransactionFlavor BeginTransaction() => DbContext.Get.AuthUserTransaction();
        }

        public class UserTransactionDomain : TransactionDomain {
            public const string KEY = "User";
            public override string Key => KEY;

            public override List<DomainEvent> TempDomainEventsTable => UserDatabase.TempDomainEventsTable;

            protected override void ApplyForAllTablePairs(IPairTableFunction func) {
                func.Apply(UserDatabase.TempUsersTable, UserDatabase.UsersTable);
                func.Apply(UserDatabase.TempDomainEventsTable, UserDatabase.DomainEventsTable);
            }

            public override ISqlTransactionFlavor BeginTransaction() => DbContext.Get.UserTransaction();
        }

        public class GameTransactionDomain : TransactionDomain {
            public const string KEY = "Game";
            public override string Key => KEY;

            public override List<DomainEvent> TempDomainEventsTable => GameDatabase.TempDomainEventsTable;

            protected override void ApplyForAllTablePairs(IPairTableFunction func) {
                func.Apply(GameDatabase.TempPlayersTable, GameDatabase.PlayersTable);
                func.Apply(GameDatabase.TempGamesTable, GameDatabase.GamesTable);
                func.Apply(GameDatabase.TempDomainEventsTable, GameDatabase.DomainEventsTable);
            }

            public override ISqlTransactionFlavor BeginTransaction() => DbContext.Get.GameTransaction();
        }

        public static class InMemoryDatabaseUtils {
            public static void StoreInto<T>(T obj, List<T> table) where T : IEquatable<T> {
                int index = table.FindIndex(u => u.Equals(obj));
                if (index == -1) {
                    table.Add(obj);
                }
                else {
                    table[index] = obj;
                }
            }
        }
    }
}
