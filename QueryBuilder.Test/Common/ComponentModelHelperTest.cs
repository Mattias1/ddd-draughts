using FluentAssertions;
using NodaTime;
using SqlQueryBuilder.Common;
using SqlQueryBuilder.Options;
using System.Collections.Generic;
using Xunit;

namespace SqlQueryBuilder.Test.Common;

public sealed class ComponentModelHelperTest {
    private LocalDateTime TestDateTime => new LocalDateTime(2020, 02, 29, 12, 42, 11);
    private string TestDateTimeString => "2020-02-29 12:42:11";

    [Fact]
    public void TestObjectToDictionary() {
        var obj = new TestObject {
            Id = 1,
            Name = "name",
            Number = null,
            Check = false,
            DateTime = TestDateTime
        };

        var dict = ComponentModelHelper.ToDictionary(obj);
        dict.TryGetValue("Id", out var id);
        dict.TryGetValue("Name", out var name);
        dict.TryGetValue("Number", out var number);
        dict.TryGetValue("Check", out var check);
        dict.TryGetValue("DateTime", out var dateTime);

        id.Should().Be(1);
        name.Should().Be("name");
        number.Should().BeNull();
        check.Should().Be(false);
        dateTime.Should().Be(TestDateTime);
    }

    [Fact]
    public void TestDictionaryToObject() {
        var dict = new Dictionary<string, object?> {
                { "Id", 1 },
                { "Name", "name" },
                { "Number", null },
                { "Check", false },
                { "DateTime", TestDateTime }
            };

        var obj = ComponentModelHelper.ToObject<TestObject>(dict);
        obj.Id.Should().Be(1);
        obj.Name.Should().Be("name");
        obj.Number.Should().BeNull();
        obj.Check.Should().BeFalse();
        obj.DateTime.Should().Be(TestDateTime);
    }

    [Fact]
    public void TestDictionaryToObjectWithDateTimeString() {
        var dict = new Dictionary<string, object?> {
                { "Id", 1 },
                { "Name", "name" },
                { "Number", null },
                { "check", false },
                { "DateTime", TestDateTimeString }
            };

        var obj = ComponentModelHelper.ToObject<TestObject>(dict);
        obj.DateTime.Should().Be(TestDateTime);
    }

    [Fact]
    public void TestDictionaryToObjectWithOneAsBoolean() {
        var dict = new Dictionary<string, object?> {
                { "Id", 1 },
                { "Name", "name" },
                { "Number", null },
                { "Check", 1 },
                { "DateTime", TestDateTime }
            };

        var obj = ComponentModelHelper.ToObject<TestObject>(dict);
        obj.Check.Should().BeTrue();
    }

    [Fact]
    public void TestDictionaryToObjectWithZeroAsBoolean() {
        var dict = new Dictionary<string, object?> {
                { "Id", 1 },
                { "Name", "name" },
                { "Number", null },
                { "Check", 0 },
                { "DateTime", TestDateTime }
            };

        var obj = ComponentModelHelper.ToObject<TestObject>(dict);
        obj.Check.Should().BeFalse();
    }

    [Fact]
    public void TestObjectToSnakeCasedDictionary() {
        var obj = new TestObject {
            Id = 1,
            Name = "name",
            Number = null,
            Check = false,
            DateTime = TestDateTime
        };

        var dict = ComponentModelHelper.ToDictionary(obj, new CamelToSnakeColumnFormat());
        dict.TryGetValue("id", out var id);
        dict.TryGetValue("name", out var name);
        dict.TryGetValue("number", out var number);
        dict.TryGetValue("check", out var check);
        dict.TryGetValue("date_time", out var dateTime);

        id.Should().Be(1);
        name.Should().Be("name");
        number.Should().BeNull();
        check.Should().Be(false);
        dateTime.Should().Be(TestDateTime);
    }

    [Fact]
    public void TestSnakeCasedDictionaryToObject() {
        var dict = new Dictionary<string, object?> {
                { "id", 1 },
                { "name", "name" },
                { "number", null },
                { "check", false },
                { "date_time", TestDateTimeString }
            };

        var obj = ComponentModelHelper.ToObject<TestObject>(dict, new CamelToSnakeColumnFormat());
        obj.Id.Should().Be(1);
        obj.Name.Should().Be("name");
        obj.Number.Should().BeNull();
        obj.Check.Should().BeFalse();
        obj.DateTime.Should().Be(TestDateTime);
    }

    public sealed class TestObject {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int? Number { get; set; }
        public bool Check { get; set; }
        public LocalDateTime? DateTime { get; set; }
    }
}
