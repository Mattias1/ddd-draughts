using Draughts.Common.Events;
using Draughts.Repositories.Database;
using System;
using System.Collections.Generic;
using static Draughts.Repositories.Databases.PairTableFunctions;

namespace Draughts.Repositories.Databases {
    public abstract class TransactionDomain {
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

        public class AuthUserTransactionDomain : TransactionDomain {
            public const string KEY = "AuthUser";
            public override string Key => KEY;

            public override List<DomainEvent> TempDomainEventsTable => AuthUserDatabase.TempDomainEventsTable;

            protected override void ApplyForAllTablePairs(IPairTableFunction func) {
                func.Apply(AuthUserDatabase.TempRolesTable, AuthUserDatabase.RolesTable);
                func.Apply(AuthUserDatabase.TempAuthUsersTable, AuthUserDatabase.AuthUsersTable);
                func.Apply(AuthUserDatabase.TempDomainEventsTable, AuthUserDatabase.DomainEventsTable);
            }
        }

        public class UserTransactionDomain : TransactionDomain {
            public const string KEY = "User";
            public override string Key => KEY;

            public override List<DomainEvent> TempDomainEventsTable => UserDatabase.TempDomainEventsTable;

            protected override void ApplyForAllTablePairs(IPairTableFunction func) {
                func.Apply(UserDatabase.TempUsersTable, UserDatabase.UsersTable);
                func.Apply(UserDatabase.TempDomainEventsTable, UserDatabase.DomainEventsTable);
            }
        }

        public class GameTransactionDomain : TransactionDomain {
            public const string KEY = "Game";
            public override string Key => KEY;

            public override List<DomainEvent> TempDomainEventsTable => throw new NotImplementedException();

            protected override void ApplyForAllTablePairs(IPairTableFunction func) {
                throw new NotImplementedException();
            }
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
