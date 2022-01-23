using Draughts.Common;
using Draughts.Domain.AuthContext.Models;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.AuthContext;

public class UsernameTest {
    [Fact]
    public void NameCanBeAlphaDash() {
        var name = new Username("Matty-_-1337");
        name.Value.Should().Be("Matty-_-1337");
    }

    [Fact]
    public void NameCanBeJustNumbers() {
        var name = new Username("1771");
        name.Value.Should().Be("1771");
    }

    [Fact]
    public void NameCannotBeNull() => ThrowWhenNameIs(null);

    [Fact]
    public void NameCannotBeWhitespace() => ThrowWhenNameIs(" ");

    [Fact]
    public void NameCannotBeTooLong() => ThrowWhenNameIs(new string('a', Username.MAX_LENGTH + 1));

    [Theory]
    [InlineData("a@z")]
    [InlineData("a$z")]
    [InlineData("a!z")]
    [InlineData("a<z")]
    [InlineData("a>z")]
    [InlineData("a=z")]
    [InlineData("a.z")]
    [InlineData("a,z")]
    [InlineData("a|z")]
    [InlineData("a/z")]
    [InlineData("a\\z")]
    [InlineData("a&z")]
    [InlineData("a;z")]
    [InlineData("a:z")]
    [InlineData("a	z")]
    [InlineData("a z")]
    public void NameCannotContainSpecialCharacters(string name) => ThrowWhenNameIs(name);

    private void ThrowWhenNameIs(string? name) {
        Action newName = () => new Username(name);
        newName.Should().Throw<ManualValidationException>();
    }
}
