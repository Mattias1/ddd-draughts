using NodaTime;
using NodaTime.Extensions;
using System;
using System.Globalization;

namespace Draughts.Common.Utilities;

public static class NodaTimeExtensions {
    public static ZonedDateTime UtcNow(this IClock clock) => clock.InUtc().GetCurrentZonedDateTime();
    public static string ToIsoString(this ZonedDateTime? datetime) {
        if (datetime is not null && datetime.Value.Zone != DateTimeZone.Utc) {
            throw new NotImplementedException("Currently only UTC times are supported.");
        }
        return datetime is null ? "" : datetime.Value.ToIsoString();
    }
    public static string ToIsoString(this ZonedDateTime datetime) {
        return datetime.ToString("yyyy-MM-ddTHH:mm:ss'Z'", CultureInfo.InvariantCulture);
    }
}
