using System;
using System.Collections.Generic;

namespace Draughts.Repositories.Transaction {
    // Sort of inverted visitor pattern?
    public class PairTableFunctions {
        private static readonly object _lock = new object();

        public interface IPairTableFunction {
            void Apply<T>(List<T> tempTable, List<T> table) where T : IEquatable<T>;
        }

        public struct ClearTempFunction : IPairTableFunction {

            public void Apply<T>(List<T> tempTable, List<T> table) where T : IEquatable<T> {
                lock (_lock) {
                    tempTable.Clear();
                }
            }
        }

        public struct StoreIntoFunction : IPairTableFunction {
            public void Apply<T>(List<T> tempTable, List<T> table) where T : IEquatable<T> {
                lock (_lock) {
                    tempTable.ForEach(entry => TransactionDomain.InMemoryDatabaseUtils.StoreInto(entry, table));
                }
            }
        }
    }
}
