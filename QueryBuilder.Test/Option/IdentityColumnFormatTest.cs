using FluentAssertions;
using SqlQueryBuilder.Options;
using Xunit;

namespace SqlQueryBuilder.Test.Option;

public sealed class IdentityColumnFormatTest {
    private readonly IColumnFormat _columnFormat = new IdentityColumnFormat();

    [Theory]
    [InlineData("", "")]
    [InlineData("OneTwoThree123", "OneTwoThree123")]
    [InlineData("one_two_three123", "one_two_three123")]
    [InlineData("Iñtërnâtiônàlizætiøn_𐐒𐐌_あ", "Iñtërnâtiônàlizætiøn_𐐒𐐌_あ")]
    public void ToDatabase(string entityColumn, string expectedDatabaseColumn) {
        _columnFormat.Format(entityColumn).Should().Be(expectedDatabaseColumn);
    }
}
