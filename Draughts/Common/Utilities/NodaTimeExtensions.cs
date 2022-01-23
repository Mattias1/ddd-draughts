using NodaTime;
using NodaTime.Extensions;
using System.Globalization;

namespace Draughts.Common.Utilities;

public static class NodaTimeExtensions {
    public static ZonedDateTime UtcNow(this IClock clock) => clock.InUtc().GetCurrentZonedDateTime();
    public static string ToIsoString(this ZonedDateTime dateTime)
        => dateTime.ToString("yyyy-MM-ddTHH:mm:sso<g>", CultureInfo.InvariantCulture);
}
