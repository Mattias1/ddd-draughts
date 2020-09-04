using NodaTime;
using NodaTime.Extensions;

namespace Draughts.Common.Utilities {
    public static class IClockExtensions {
        public static ZonedDateTime UtcNow(this IClock clock) => clock.InUtc().GetCurrentZonedDateTime();
    }
}
