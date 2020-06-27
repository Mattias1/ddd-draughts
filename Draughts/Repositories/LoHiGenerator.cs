using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories {
    /// <summary>
    /// A thread safe id generator that doesn't need database connection most of the time.
    /// Should be injected as singleton.
    /// </summary>
    public class LoHiGenerator : IIdGenerator {
        private readonly int _interval = 10;

        private readonly object _lock = new object();

        private long _lo; // The lowest available id (inclusive).
        private long _hi; // The maximum reserved id (exclusive).

        private readonly Func<List<AvailableId>> _databaseTableFunc;

        public LoHiGenerator(int interval, Func<List<AvailableId>> databaseTableFunc) {
            _interval = interval;
            _databaseTableFunc = databaseTableFunc;
        }

        public long Next() {
            // I can't think of a situation where this deadlocks, under the assumption that this is the only class that locks the MiscDatabase.
            lock (_lock) {
                if (_lo >= _hi) {
                    var availableId = _databaseTableFunc().Single();
                    _lo = availableId.Id;
                    availableId.Id += _interval;
                    _hi = availableId.Id;
                }

                return _lo++;
            }
        }
    }

    public class AvailableId {
        public long Id { get; set; } = 1;
    }
}
