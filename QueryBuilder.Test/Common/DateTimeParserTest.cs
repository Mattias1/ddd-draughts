using FluentAssertions;
using NodaTime;
using SqlQueryBuilder.Common;
using SqlQueryBuilder.Exceptions;
using System;
using Xunit;

namespace SqlQueryBuilder.Test.Common;

public sealed class DateTimeParserTest {
    private LocalDateTime TestDateTime => new LocalDateTime(2020, 02, 29, 12, 42, 11);
    private DateTime TestSystemDateTime => new DateTime(2020, 02, 29, 12, 42, 11);
    private const string TestDateString = "2020-02-29";
    private const string TestTimeString = "12:42:11";
    private const string TestDateTimeString = TestDateString + " " + TestTimeString;

    [Fact]
    public void ParseUnknownParameter() {
        bool recognizedType = DateTimeParser.ParseQueryParameter(42.0f, out string? result);
        recognizedType.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void ParseSystemDateTimeParameter() {
        bool recognizedType = DateTimeParser.ParseQueryParameter(TestDateTime.ToDateTimeUnspecified(), out string? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTimeString);
    }

    [Fact]
    public void ParseZonedDateTimeParameter() {
        bool recognizedType = DateTimeParser.ParseQueryParameter(TestDateTime.InUtc(), out string? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTimeString);
    }

    [Fact]
    public void ParseZonedDateTimeNotInUtcParameter() {
        var zone = DateTimeZone.ForOffset(Offset.FromHours(2));
        Action parser = () => DateTimeParser.ParseQueryParameter(TestDateTime.InZoneStrictly(zone), out string? result);
        parser.Should().Throw<SqlQueryBuilderException>();
    }

    [Fact]
    public void ParseLocalDateTimeParameter() {
        bool recognizedType = DateTimeParser.ParseQueryParameter(TestDateTime, out string? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTimeString);
    }

    [Fact]
    public void ParseLocalDateParameter() {
        bool recognizedType = DateTimeParser.ParseQueryParameter(TestDateTime.Date, out string? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateString);
    }

    [Fact]
    public void ParseLocalTimeParameter() {
        bool recognizedType = DateTimeParser.ParseQueryParameter(TestDateTime.TimeOfDay, out string? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestTimeString);
    }

    [Fact]
    public void ParseUnknownPropertyFromString() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(float), TestDateTimeString, out object? result);
        recognizedType.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void ParseSystemDateTimePropertyFromString() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(DateTime), TestDateTimeString, out object? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTime.ToDateTimeUnspecified());
    }

    [Fact]
    public void ParseZonedDateTimePropertyFromString() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(ZonedDateTime), TestDateTimeString, out object? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTime.InUtc());
    }

    [Fact]
    public void ParseLocalDateTimePropertyFromString() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(LocalDateTime), TestDateTimeString, out object? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTime);
    }

    [Fact]
    public void ParseLocalDatePropertyFromString() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(LocalDate), TestDateString, out object? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTime.Date);
    }

    [Fact]
    public void ParseLocalTimePropertyFromString() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(LocalTime), TestTimeString, out object? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTime.TimeOfDay);
    }

    [Fact]
    public void ParseUnknownPropertyFromDateTime() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(float), TestSystemDateTime, out object? result);
        recognizedType.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void ParseSystemDateTimePropertyFromDateTime() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(DateTime), TestSystemDateTime, out object? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTime.ToDateTimeUnspecified());
    }

    [Fact]
    public void ParseZonedDateTimePropertyFromDateTime() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(ZonedDateTime), TestSystemDateTime, out object? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTime.InUtc());
    }

    [Fact]
    public void ParseLocalDateTimePropertyFromDateTime() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(LocalDateTime), TestSystemDateTime, out object? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTime);
    }

    [Fact]
    public void ParseLocalDatePropertyFromDateTime() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(LocalDate), TestSystemDateTime, out object? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTime.Date);
    }

    [Fact]
    public void ParseLocalTimePropertyFromDateTime() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(LocalTime), TestSystemDateTime, out object? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTime.TimeOfDay);
    }

    [Fact]
    public void ParseNullableLocalTimePropertyFromDateTime() {
        bool recognizedType = DateTimeParser.ParseProperty(typeof(LocalTime?), TestSystemDateTime, out object? result);
        recognizedType.Should().BeTrue();
        result.Should().Be(TestDateTime.TimeOfDay);
    }
}
