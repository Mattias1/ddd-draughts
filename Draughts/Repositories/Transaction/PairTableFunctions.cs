using System;
using System.Collections.Generic;

namespace Draughts.Repositories.Transaction {
    // Sort of inverted visitor pattern?
    public class PairTableFunctions {
        public interface IPairTableFunction {
            void Apply<T>(List<T> tempTable, List<T> table) where T : IEquatable<T>;
        }

        public struct ClearTempFunction : IPairTableFunction {
            public void Apply<T>(List<T> tempTable, List<T> table) where T : IEquatable<T>
                => tempTable.Clear();
        }

        public struct StoreIntoFunction : IPairTableFunction {
            public void Apply<T>(List<T> tempTable, List<T> table) where T : IEquatable<T>
                => tempTable.ForEach(entry => TransactionDomain.InMemoryDatabaseUtils.StoreInto(entry, table));
        }

        public class ContainsTempTableFunction : IPairTableFunction {
            private readonly object _tempTable;
            public bool Result { get; private set; }

            public ContainsTempTableFunction(object tempTable) {
                _tempTable = tempTable;
                Result = false;
            }

            public void Apply<T>(List<T> tempTable, List<T> table) where T : IEquatable<T>
                => Result = ReferenceEquals(_tempTable, tempTable) || Result;
        }
    }
}
