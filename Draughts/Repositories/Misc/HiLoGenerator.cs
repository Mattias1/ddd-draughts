using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.Misc;

/// <summary>
/// A thread safe id generator that doesn't need a database connection most of the time.
/// Should be injected as singleton.
/// </summary>
public class HiLoGenerator {
    private static readonly object _lock = new object();

    public int IntervalSize { get; }
    public string Subject { get; }
    private readonly List<HiLoInterval> _intervals;

    public int Count => _intervals.Sum(i => i.Count);

    public HiLoGenerator(int intervalSize, string subject) {
        IntervalSize = intervalSize;
        Subject = subject;
        _intervals = new List<HiLoInterval>();
    }

    public long Next() {
        lock (_lock) {
            var interval = FirstNonEmptyInterval();
            return interval.Next();
        }
    }

    private HiLoInterval FirstNonEmptyInterval() {
        if (_intervals.Count == 0) {
            throw new InvalidOperationException("No ids left in the pool.");
        }
        var interval = _intervals.First();
        if (interval.IsEmpty()) {
            _intervals.RemoveAt(0);
            return FirstNonEmptyInterval();
        }
        return interval;
    }

    public void ReservePool(int minimumSize) {
        lock (_lock) {
            if (minimumSize > IntervalSize) {
                throw new ArgumentException($"The requested (minimum) number of ids ({minimumSize}) "
                    + $"can be at most the set interval ({IntervalSize}).", nameof(minimumSize));
            }
            if (Count < minimumSize) {
                var newInterval = ReserveNewInterval();
                _intervals.Add(newInterval);
            }
        }
    }

    private HiLoInterval ReserveNewInterval() {
        using (var tranFlavor = DbContext.Get.BeginMiscTransaction()) {
            var idGenerationRow = DbContext.Get.Query(tranFlavor)
                .SelectAllFrom("id_generation")
                .Where("subject").Is(Subject)
                .Single<DbIdGeneration>();
            var lo = idGenerationRow.AvailableId;

            idGenerationRow.AvailableId += IntervalSize;
            DbContext.Get.Query(tranFlavor)
                .Update("id_generation")
                .SetFrom(idGenerationRow)
                .Where("subject").Is(Subject)
                .Execute();

            tranFlavor.Commit();

            return new HiLoInterval(lo, lo + IntervalSize);
        }
    }

    private sealed class HiLoInterval {
        public long Lo { get; private set; } // The lowest available id (inclusive).
        public long Hi { get; } // The maximum reserved id (exclusive).

        public int Count => (int)(Hi - Lo);
        public bool IsEmpty() => Lo >= Hi;

        public long Next() => Lo++;

        public HiLoInterval(long lo, long hi) {
            if (lo >= hi) {
                throw new InvalidOperationException("Lo should be less than hi.");
            }

            Lo = lo;
            Hi = hi;
        }
    }
}
