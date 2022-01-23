using NodaTime;
using NodaTime.Text;
using SqlQueryBuilder.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace SqlQueryBuilder.Common;

public static class DateTimeParser {
    private const string DATE_PATTERN = "yyyy-MM-dd";
    private const string TIME_PATTERN = "HH:mm:ss";
    private const string DATETIME_PATTERN = DATE_PATTERN + " " + TIME_PATTERN;

    public static bool ParseQueryParameter(object? parameter, [NotNullWhen(returnValue: true)] out string? parsedParameter) {
        switch (parameter) {
            case DateTime systemDateTime:
                parsedParameter = systemDateTime.ToString(DATETIME_PATTERN);
                return true;
            case ZonedDateTime zonedDateTime:
                if (zonedDateTime.Zone != DateTimeZone.Utc) {
                    string exceptionMessage = "The query builder received a ZonedDateTime which was not in UTC, " +
                        "try passing the parameter with `.WithZone(DateTimeZone.Utc)`.";
                    throw new SqlQueryBuilderException(exceptionMessage);
                }
                parsedParameter = zonedDateTime.ToString(DATETIME_PATTERN, CultureInfo.InvariantCulture);
                return true;
            case LocalDateTime localDateTime:
                parsedParameter = localDateTime.ToString(DATETIME_PATTERN, CultureInfo.InvariantCulture);
                return true;
            case LocalDate date:
                parsedParameter = date.ToString(DATE_PATTERN, CultureInfo.InvariantCulture);
                return true;
            case LocalTime time:
                parsedParameter = time.ToString(TIME_PATTERN, CultureInfo.InvariantCulture);
                return true;
            default:
                parsedParameter = null;
                return false;
        }
    }

    public static bool ParseProperty(Type type, string value, [NotNullWhen(returnValue: true)] out object? parsedValue) {
        if (type == typeof(DateTime) || type == typeof(DateTime?)) {
            parsedValue = DateTime.Parse(value);
            return true;
        }
        if (type == typeof(ZonedDateTime) || type == typeof(ZonedDateTime?)) {
            parsedValue = LocalDateTimePattern.CreateWithInvariantCulture(DATETIME_PATTERN).Parse(value).GetValueOrThrow().InUtc();
            return true;
        }
        if (type == typeof(LocalDateTime) || type == typeof(LocalDateTime?)) {
            parsedValue = LocalDateTimePattern.CreateWithInvariantCulture(DATETIME_PATTERN).Parse(value).GetValueOrThrow();
            return true;
        }
        if (type == typeof(LocalDate) || type == typeof(LocalDate?)) {
            parsedValue = LocalDatePattern.CreateWithInvariantCulture(DATE_PATTERN).Parse(value).GetValueOrThrow();
            return true;
        }
        if (type == typeof(LocalTime) || type == typeof(LocalTime?)) {
            parsedValue = LocalTimePattern.CreateWithInvariantCulture(TIME_PATTERN).Parse(value).GetValueOrThrow();
            return true;
        }
        parsedValue = null;
        return false;
    }

    public static bool ParseProperty(Type type, DateTime value, [NotNullWhen(returnValue: true)] out object? parsedValue) {
        if (type == typeof(DateTime) || type == typeof(DateTime?)) {
            parsedValue = value;
            return true;
        }
        if (type == typeof(ZonedDateTime) || type == typeof(ZonedDateTime?)) {
            parsedValue = LocalDateTime.FromDateTime(value).InUtc();
            return true;
        }
        if (type == typeof(LocalDateTime) || type == typeof(LocalDateTime?)) {
            parsedValue = LocalDateTime.FromDateTime(value);
            return true;
        }
        if (type == typeof(LocalDate) || type == typeof(LocalDate?)) {
            parsedValue = LocalDate.FromDateTime(value);
            return true;
        }
        if (type == typeof(LocalTime) || type == typeof(LocalTime?)) {
            parsedValue = LocalDateTime.FromDateTime(value).TimeOfDay;
            return true;
        }
        parsedValue = null;
        return false;
    }
}
