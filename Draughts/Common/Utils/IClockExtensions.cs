using NodaTime;
using NodaTime.Extensions;

namespace Draughts.Common.Utils {
    public static class IClockExtensions {
        public static ZonedDateTime UtcNow(this IClock clock) => clock.InUtc().GetCurrentZonedDateTime();
    }
}
