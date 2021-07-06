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
        public abstract List<DomainEvent> TempDomainEventsTable(ITransaction transaction);

        private TransactionDomain() { }

        public abstract void ApplyForAllTablePairs(ITransaction tran, IPairTableFunction func);

        public abstract void CreateTempDatabase(ITransaction tran);
        public abstract void RemoveTempDatabase(ITransaction tran);

        public abstract ISqlTransactionFlavor BeginTransaction();

        public override bool Equals(object? obj) => obj is TransactionDomain tdObj && tdObj.Key.Equals(Key);
        public bool Equals(TransactionDomain? other) => other?.Key == Key;

        public override int GetHashCode() => Key.GetHashCode();

        public static bool operator ==(TransactionDomain? left, TransactionDomain? right) => ComparisonUtils.NullSafeEquals(left, right);
        public static bool operator !=(TransactionDomain? left, TransactionDomain? right) => ComparisonUtils.NullSafeNotEquals(left, right);

        public class AuthUserTransactionDomain : TransactionDomain {
            public const string KEY = "AuthUser";
            public override string Key => KEY;

            public override List<DomainEvent> TempDomainEventsTable(ITransaction transaction) {
                return AuthUserDatabase.Temp(transaction).DomainEventsTable;
            }

            public override void ApplyForAllTablePairs(ITransaction tran, IPairTableFunction func) {
                func.Apply(AuthUserDatabase.Temp(tran).RolesTable, AuthUserDatabase.Get.RolesTable);
                func.Apply(AuthUserDatabase.Temp(tran).PermissionRolesTable, AuthUserDatabase.Get.PermissionRolesTable);
                func.Apply(AuthUserDatabase.Temp(tran).AuthUsersTable, AuthUserDatabase.Get.AuthUsersTable);
                func.Apply(AuthUserDatabase.Temp(tran).AuthUserRolesTable, AuthUserDatabase.Get.AuthUserRolesTable);
                func.Apply(AuthUserDatabase.Temp(tran).AdminLogsTable, AuthUserDatabase.Get.AdminLogsTable);
                func.Apply(AuthUserDatabase.Temp(tran).DomainEventsTable, AuthUserDatabase.Get.DomainEventsTable);
            }

            public override void CreateTempDatabase(ITransaction tran) => AuthUserDatabase.CreateTempDatabase(tran);
            public override void RemoveTempDatabase(ITransaction tran) => AuthUserDatabase.RemoveTempDatabase(tran);

            public override ISqlTransactionFlavor BeginTransaction() => DbContext.Get.BeginAuthUserTransaction();
        }

        public class UserTransactionDomain : TransactionDomain {
            public const string KEY = "User";
            public override string Key => KEY;

            public override List<DomainEvent> TempDomainEventsTable(ITransaction transaction) {
                return UserDatabase.Temp(transaction).DomainEventsTable;
            }

            public override void ApplyForAllTablePairs(ITransaction tran, IPairTableFunction func) {
                func.Apply(UserDatabase.Temp(tran).UsersTable, UserDatabase.Get.UsersTable);
                func.Apply(UserDatabase.Temp(tran).DomainEventsTable, UserDatabase.Get.DomainEventsTable);
            }

            public override void CreateTempDatabase(ITransaction tran) => UserDatabase.CreateTempDatabase(tran);
            public override void RemoveTempDatabase(ITransaction tran) => UserDatabase.RemoveTempDatabase(tran);

            public override ISqlTransactionFlavor BeginTransaction() => DbContext.Get.BeginUserTransaction();
        }

        public class GameTransactionDomain : TransactionDomain {
            public const string KEY = "Game";
            public override string Key => KEY;

            public override List<DomainEvent> TempDomainEventsTable(ITransaction transaction) {
                return GameDatabase.Temp(transaction).DomainEventsTable;
            }

            public override void ApplyForAllTablePairs(ITransaction tran, IPairTableFunction func) {
                func.Apply(GameDatabase.Temp(tran).MovesTable, GameDatabase.Get.MovesTable);
                func.Apply(GameDatabase.Temp(tran).GameStatesTable, GameDatabase.Get.GameStatesTable);
                func.Apply(GameDatabase.Temp(tran).PlayersTable, GameDatabase.Get.PlayersTable);
                func.Apply(GameDatabase.Temp(tran).GamesTable, GameDatabase.Get.GamesTable);
                func.Apply(GameDatabase.Temp(tran).DomainEventsTable, GameDatabase.Get.DomainEventsTable);
            }

            public override void CreateTempDatabase(ITransaction tran) => GameDatabase.CreateTempDatabase(tran);
            public override void RemoveTempDatabase(ITransaction tran) => GameDatabase.RemoveTempDatabase(tran);

            public override ISqlTransactionFlavor BeginTransaction() => DbContext.Get.BeginGameTransaction();
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
