using FluentAssertions;
using SqlQueryBuilder.Options;
using Xunit;

namespace SqlQueryBuilder.Test.Option;

public sealed class CamelToSnakeColumnFormatTest {
    private readonly IColumnFormat _columnFormat = new CamelToSnakeColumnFormat();

    [Theory]
    [InlineData("", "")]
    [InlineData("One", "one")]
    [InlineData("OneTwo", "one_two")]
    [InlineData("OneTwoThree", "one_two_three")]
    [InlineData("OneTwoThree123", "one_two_three123")]
    [InlineData("ABC", "a_b_c")]
    [InlineData("one", "one")]
    [InlineData("oneTwo", "one_two")]
    [InlineData("oneTwoThree", "one_two_three")]
    [InlineData("oneTwoThree123", "one_two_three123")]
    [InlineData("aBCD", "a_b_c_d")]
    public void FormatTest(string entityColumn, string expectedDatabaseColumn) {
        _columnFormat.Format(entityColumn).Should().Be(expectedDatabaseColumn);
    }
}
